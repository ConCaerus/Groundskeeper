using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Movement : MortalUnit {
    public Sprite forwardSprite, backSprite, leftSprite, rightSprite;
    protected Coroutine anim = null;
    [SerializeField] public GameObject spriteObj, shadowObj;
    Coroutine moveWaiter = null;
    public bool canMove = true;
    bool shown = true;
    bool beingAttackedByMonster = false;
    Coroutine beingAttackedWaiter = null;
    float inhibitMod = .35f;
    [SerializeField] ParticleSystem movementParticles = null;   //  should already be an object parented to the mover's feet

    //  storage
    protected SetupSequenceManager ssm;
    protected LayerSorter ls;
    protected SpriteRenderer sr;
    Collider2D srCol;
    protected MouseManager mm;
    protected GameGamepadCursor ggc;
    protected FreeGamepadCursor fgc;

    Transform playerTrans;

    bool playWalkSound;
    [SerializeField] AudioClip walkSound;
    AudioSource aSource;

    //  abstract because monsters change their sprites differently to everyone else
    public abstract void updateSprite(Vector2 movingDir, bool opposite);
    protected void movementInit(SetupSequenceManager s, LayerSorter l, bool audibleWalking) {
        ssm = s;
        ls = l;
        sr = spriteObj.GetComponent<SpriteRenderer>();
        mm = FindObjectOfType<MouseManager>();
        ggc = FindObjectOfType<GameGamepadCursor>();
        fgc = FindObjectOfType<FreeGamepadCursor>();
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        playWalkSound = audibleWalking;
        aSource = GetComponent<AudioSource>();
        foreach(var i in GetComponents<Collider2D>()) {
            if(!i.isTrigger) {
                srCol = i;
                break;
            }
        }
    }

    protected void moveWithDir(Vector2 info, Rigidbody2D rb, float speed) {
        if(!canMove || (ssm != null && ssm.isActiveAndEnabled) || playerTrans == null)
            return;
        var s = speed * 100.0f * (beingAttackedByMonster ? inhibitMod : 1.0f);
        rb.velocity = info * s * Time.fixedDeltaTime;
        if(info.x != 0.0f || info.y != 0.0f) {
            if(anim == null)
                anim = StartCoroutine(walkAnim());
        }

        updateSprite(info, speed < 0f);
    }

    protected void moveToPos(Vector2 pos, Rigidbody2D rb, float speed) {
        if(!canMove || (ssm != null && ssm.isActiveAndEnabled))    //  unit cannot move
            return;
        if(pos != (Vector2)rb.gameObject.transform.position) {
            if(anim == null)
                anim = StartCoroutine(walkAnim());
        }
        var s = speed * 10.0f * (beingAttackedByMonster ? inhibitMod : 1.0f);
        rb.MovePosition(Vector2.MoveTowards(rb.gameObject.transform.position, pos, s * Time.fixedDeltaTime));

        updateSprite(pos - (Vector2)transform.position, speed < 0f);
    }

    public void lookAtPos(Vector2 target) {
        var x = Mathf.Abs(transform.position.x - target.x);
        var y = Mathf.Abs(transform.position.y - target.y);
        if(x > y)
            sr.sprite = target.x < transform.position.x ? leftSprite : rightSprite;
        else
            sr.sprite = target.y < transform.position.y ? forwardSprite : backSprite;
    }


    public abstract WalkAnimInfo getWalkInfo();

    protected IEnumerator walkAnim(bool jumpDir = true) {
        spriteObj.transform.DOPunchPosition(new Vector3(0.0f, 0.7f, 0.0f), getWalkInfo().time);
        float tehe = 0.15f;
        spriteObj.transform.DOPunchScale(new Vector3(-tehe, tehe), getWalkInfo().time);
        if(shadowObj != null)
            shadowObj.transform.DOComplete();
        Vector2 shadOriginal = Vector2.zero;
        if(shadowObj != null)
            shadOriginal = shadowObj.transform.localScale;

        if(movementParticles != null)
            movementParticles.Play();

        if(jumpDir)
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(getWalkInfo().minRot, getWalkInfo().maxRot)), getWalkInfo().time);
        else
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(-getWalkInfo().maxRot, -getWalkInfo().minRot)), getWalkInfo().time);

        if(ls != null) {
            ls.requestNewSortingLayer(srCol, sr);
        }

        if(shadowObj != null) {
            shadowObj.transform.DOScale(new Vector2(shadOriginal.x / 2.0f, shadOriginal.y / 1.5f), getWalkInfo().time / 2.0f);
            yield return new WaitForSeconds(getWalkInfo().time / 2.0f);
            if(playWalkSound && aSource != null) {
                aSource.pitch = Random.Range(0.6f, 1.25f);
                aSource.PlayOneShot(walkSound);
            }
            shadowObj.transform.DOComplete();
            shadowObj.transform.DOScale(shadOriginal, getWalkInfo().time / 2.0f);
            yield return new WaitForSeconds(getWalkInfo().time / 2.0f);
        }
        else {
            yield return new WaitForSeconds(getWalkInfo().time / 2.0f);

            if(playWalkSound && aSource != null) {
                aSource.pitch = Random.Range(0.6f, 1.25f);
                aSource.PlayOneShot(walkSound);
            }
            yield return new WaitForSeconds(getWalkInfo().time / 2.0f);
        }

        anim = restartWalkAnim() ? StartCoroutine(walkAnim(!jumpDir)) : null;
    }


    public void stopMovingForATime(float time) {
        if(moveWaiter != null)
            StopCoroutine(moveWaiter);
        moveWaiter = StartCoroutine(moveTimedWaiter(time));
    }

    public void setCanMove(bool b) {
        canMove = b;
    }

    IEnumerator moveTimedWaiter(float time) {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        moveWaiter = null;
    }

    public void hide() {
        if(!shown)
            return;
        shown = false;
        sr.enabled = false;
        shadowObj.GetComponent<SpriteRenderer>().enabled = false;
        /*
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(0.0f, .25f);
        */
        srCol.isTrigger = true;
        if(healthBar != null)
            healthBar.hideBar();
    }
    public void show() {
        if(shown)
            return;
        shown = true;
        sr.enabled = true;
        shadowObj.GetComponent<SpriteRenderer>().enabled = true;
        /*
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(1.0f, .15f);
        */
        srCol.isTrigger = false;
        if(healthBar != null)
            healthBar.showBar();
    }

    public abstract Vector2 getSpriteOriginalScale();
    public abstract Vector2 getShadowOriginalScale();

    public abstract bool restartWalkAnim();

    public void inhibitMovementCauseBeingAttacked() {
        beingAttackedByMonster = true;
        if(beingAttackedWaiter != null)
            StopCoroutine(beingAttackedWaiter);
        beingAttackedWaiter = StartCoroutine(resetBeingAttackedByMonster());
    }
    protected void stopInhibitingMovement() {
        beingAttackedByMonster = false;
        if(beingAttackedWaiter != null)
            StopCoroutine(beingAttackedWaiter);
        StartCoroutine(waitForNextFrameToStopInhibitation());
    }
    IEnumerator waitForNextFrameToStopInhibitation() {
        yield return new WaitForEndOfFrame();
        beingAttackedByMonster = false;
    }

    IEnumerator resetBeingAttackedByMonster() {
        yield return new WaitForSeconds(.35f);
        beingAttackedByMonster = false;
        beingAttackedWaiter = null;
    }
}


[System.Serializable]
public class WalkAnimInfo {
    public float time;
    public float jumpHeight;
    public float maxRot, minRot;

    public WalkAnimInfo(float t, float j, float maxR, float minR) {
        time = t;
        jumpHeight = j;
        maxRot = maxR;
        minRot = minR;
    }
}
