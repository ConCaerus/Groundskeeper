using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public abstract class Movement : MortalUnit {
    public Sprite forwardSprite, backSprite, leftSprite, rightSprite;
    protected Coroutine anim = null;
    [SerializeField] public GameObject spriteObj;
    Coroutine moveWaiter = null;
    protected bool canMove = true;

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

        //  play walk sound

        if(jumpDir)
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(getWalkInfo().minRot, getWalkInfo().maxRot)), getWalkInfo().time);
        else
            spriteObj.transform.DOPunchRotation(new Vector3(0.0f, 0.0f, Random.Range(-getWalkInfo().maxRot, -getWalkInfo().minRot)), getWalkInfo().time);

        FindObjectOfType<LayerSorter>().requestNewSortingLayer(spriteObj);

        yield return new WaitForSeconds(getWalkInfo().time);

        anim = restartWalkAnim() ? StartCoroutine(walkAnim(!jumpDir)) : null;
    }


    public void stopMovingForATime(float time) {
        if(moveWaiter != null)
            StopCoroutine(moveWaiter);
        moveWaiter = StartCoroutine(moveTimedWaiter(time));
    }

    IEnumerator moveTimedWaiter(float time) {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
        moveWaiter = null;
    }

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
