using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperInstance : Helper {
    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    float accSpeed = .1f, slowSpeed = 18.0f;

    public bool hasTarget = false;
    public bool inReach = false;
    public Vector2 target { get; private set; }
    public Vector2 startingPos;

    private void Awake() {
        stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
        startingPos = transform.position;
    }

    private void OnCollisionStay2D(Collision2D col) {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(spriteObj);
    }

    private void OnTriggerStay2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            GetComponentInChildren<HelperWeaponInstance>().attack();
            inReach = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Monster") {
            inReach = false;
        }
    }

    #region ---   MOVEMENT SHIT   ---
    private void FixedUpdate() {
        //  move the object
        target = FindObjectOfType<TargetFinder>().getTargetForHelper(gameObject);
        if(!inReach)
            moveToPos(target, GetComponent<Rigidbody2D>(), speed);
        //  if player is not moving, decrement the movementInfo
        //  if player is moving, increase the movementInfo to target value
        moveInfo = hasTarget && !inReach ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, Vector2.zero, slowSpeed * 100.0f * Time.fixedDeltaTime);
    }
    public override bool restartWalkAnim() {
        return hasTarget && !inReach;
    }
    public override void updateSprite(Vector2 movingDir) {
        //  not moving, face forward
        if(movingDir == Vector2.zero) {
            spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
            return;
        }

        //  moving more along the y axis, set to a y axis sprite
        else if(Mathf.Abs(movingDir.x) < .5f && Mathf.Abs(movingDir.y) > .5f) {
            if(movingDir.y > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = backSprite;
            else
                spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
        }

        //  set to a x axis sprite
        else {
            if(movingDir.x > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = rightSprite;
            else
                spriteObj.GetComponent<SpriteRenderer>().sprite = leftSprite;
        }
    }
    public override WalkAnimInfo getWalkInfo() {
        return new WalkAnimInfo(.2f, .7f, 25f, 5f);
    }
    #endregion


    #region ---   ATTACKER SHIT   ---
    public override float getAttackCoolDown() {
        return GetComponentInChildren<HelperWeaponInstance>().reference.cooldown;
    }
    public override int getDamage() {
        return GetComponentInChildren<HelperWeaponInstance>().reference.damage;
    }
    public override float getKnockback() {
        return GetComponentInChildren<HelperWeaponInstance>().reference.knockback;
    }
    #endregion


    #region ---   MORTAL SHIT   ---
    public override void die() {
        Destroy(gameObject);
    }
    #endregion
}
