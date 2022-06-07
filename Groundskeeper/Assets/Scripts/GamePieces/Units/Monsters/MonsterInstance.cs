using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterInstance : Monster {
    Vector2 moveTarget;

    public bool infatuated { get; private set; } = false;

    public float affectedMoveAmt = 0f;

    private void Start() {
        stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
    }

    private void OnCollisionStay2D(Collision2D col) {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(spriteObj);

        //  attack
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Person" || col.gameObject.tag == "Building" || col.gameObject.tag == "House")
            attack(col.gameObject, true);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        //  hit the collider on the house that tells the monster to ingore everything else and just go for the house
        if(col.gameObject.tag == "House")
            infatuated = true;
    }

    #region ---   MOVEMENT SHIT   ---

    private void FixedUpdate() {
        moveTarget = FindObjectOfType<TargetFinder>().getTargetForMonster(gameObject);
        moveToPos(moveTarget, GetComponent<Rigidbody2D>(), speed - affectedMoveAmt);
    }
    public override bool restartWalkAnim() {
        return canMove;
    }
    public override WalkAnimInfo getWalkInfo() {
        return new WalkAnimInfo(.25f, .35f, 15f, 5f);
    }
    public override void updateSprite(Vector2 movingDir) {
        Vector2 offset = moveTarget - (Vector2)transform.position;
        if(offset == Vector2.zero) {
            spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
            return;
        }
        else if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            spriteObj.GetComponent<SpriteRenderer>().sprite = offset.x > 0.0f ? rightSprite : leftSprite;
        else
            spriteObj.GetComponent<SpriteRenderer>().sprite = offset.y > 0.0f ? backSprite : forwardSprite;
    }
    #endregion


    #region ---   ATTACKER SHIT   ---

    public override float getAttackCoolDown() {
        return .5f; //  default value for all enemies
    }
    public override int getDamage() {
        return attackDamage;
    }
    public override float getKnockback() {
        return 0f;
    }
    #endregion


    #region ---   MORTAL SHIT   ---

    public override void die() {
        GameInfo.monstersKilled++;
        GameInfo.weightedMonstersKilled += diff;
        transform.DOScale(0f, .25f);
        Destroy(gameObject, .26f);
        enabled = false;
    }
    #endregion
}
