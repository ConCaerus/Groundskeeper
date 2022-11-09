using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarecrowInstance : Helper {

    //  functions that don't do squat but i'm too lazy to make the helper class not need these
    public override void customHitLogic(float knockback, Vector2 origin, bool stunMonster = true) {

    }
    public override float getAttackCoolDown() {
        return 100.0f;
    }
    public override float getKnockback() {
        return 100.0f;
    }
    public override int getDamage() {
        return 0;
    }
    public override WalkAnimInfo getWalkInfo() {
        return null;
    }
    public override void updateSprite(Vector2 movingDir) {
        
    }
    public override bool restartWalkAnim() {
        return false;
    }
    public override Vector2 getShadowOriginalScale() {
        return shadowObj.transform.localScale;
    }
    public override Vector2 getSpriteOriginalScale() {
        return spriteObj.transform.localScale;
    }
    public override void specialEffectOnAttack() {
    }


    private void Start() {
        spriteObj.GetComponent<SpriteRenderer>().sprite = sprite;
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), spriteObj.GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
    }


    //  functions that do do squat
    public override void die() {
        FindObjectOfType<GameBoard>().aHelpers.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}
