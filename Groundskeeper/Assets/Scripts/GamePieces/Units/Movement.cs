using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public abstract class Movement : MortalUnit {
    public Sprite forwardSprite, backSprite, leftSprite, rightSprite;
    protected Coroutine anim = null;
    [SerializeField] public GameObject spriteObj, shadowObj;
    Coroutine moveWaiter = null;
    protected bool canMove = true;
    bool shown = true;

    //  abstract because monsters change their sprites differently to everyone else
    public abstract void updateSprite(Vector2 movingDir);

    protected void moveWithDir(Vector2 info, Rigidbody2D rb, float speed) {
        if(!canMove)
            return;
        rb.velocity = info * speed * 100.0f * Time.fixedDeltaTime;
        if(info.x != 0.0f || info.y != 0.0f) {
            if(anim == null)
                anim = StartCoroutine(walkAnim());
        }

        updateSprite(info);
    }

    protected void moveToPos(Vector2 pos, Rigidbody2D rb, float speed) {
        if(!canMove)
            return;
        if(pos != (Vector2)rb.gameObject.transform.position) {
            if(anim == null)
                anim = StartCoroutine(walkAnim());
        }
        rb.MovePosition(Vector2.MoveTowards(rb.gameObject.transform.position, pos, speed * 10.0f * Time.fixedDeltaTime));

        updateSprite(pos - (Vector2)transform.position);
    }


    public abstract WalkAnimInfo getWalkInfo();

    protected IEnumerator walkAnim(bool jumpDir = true) {
        spriteObj.transform.DOPunchPosition(new Vector3(0.0f, 0.7f, 0.0f), getWalkInfo().time);
        shadowObj.transform.DOComplete();
        Vector2 shadOriginal = shadowObj.transform.localScale;

        //  play walk sound

        if(jumpDir)
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(getWalkInfo().minRot, getWalkInfo().maxRot)), getWalkInfo().time);
        else
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(-getWalkInfo().maxRot, -getWalkInfo().minRot)), getWalkInfo().time);

        if(FindObjectOfType<LayerSorter>() != null) {
            foreach(var i in GetComponents<Collider2D>()) {
                if(!i.isTrigger) {
                    FindObjectOfType<LayerSorter>().requestNewSortingLayer(i, spriteObj.GetComponent<SpriteRenderer>());
                    break;
                }
            }
        }

        shadowObj.transform.DOScale(new Vector2(shadOriginal.x / 2.0f, shadOriginal.y / 1.5f), getWalkInfo().time / 2.0f);
        yield return new WaitForSeconds(getWalkInfo().time / 2.0f);
        shadowObj.transform.DOComplete();
        shadowObj.transform.DOScale(shadOriginal, getWalkInfo().time / 2.0f);
        yield return new WaitForSeconds(getWalkInfo().time / 2.0f);

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
        spriteObj.GetComponent<SpriteRenderer>().enabled = false;
        shadowObj.GetComponent<SpriteRenderer>().enabled = false;
        /*
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(0.0f, .25f);
        */
        GetComponent<Collider2D>().isTrigger = true;
        if(healthBar != null)
            healthBar.hideBar();
    }
    public void show() {
        if(shown)
            return;
        shown = true;
        spriteObj.GetComponent<SpriteRenderer>().enabled = true;
        shadowObj.GetComponent<SpriteRenderer>().enabled = true;
        /*
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(1.0f, .15f);
        */
        GetComponent<Collider2D>().isTrigger = false;
        if(healthBar != null)
            healthBar.showBar();
    }

    public abstract Vector2 getSpriteOriginalScale();
    public abstract Vector2 getShadowOriginalScale();

    public abstract bool restartWalkAnim();
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
