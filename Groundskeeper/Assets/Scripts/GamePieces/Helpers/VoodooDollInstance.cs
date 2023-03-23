using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoodooDollInstance : Helper {
    [SerializeField] GameObject bloodParticles;

    //  functions that don't do squat but i'm too lazy to make the helper class not need these
    public override void hitLogic(float knockback, Vector2 origin, bool stunMonster = true) {

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
    public override void specialEffectOnAttack(GameObject defender) {
    }

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }


    private void Start() {
        mortalInit();
        spriteObj.GetComponent<SpriteRenderer>().sprite = sprite;
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), spriteObj.GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        FindObjectOfType<GameBoard>().helpers.Add(this);
    }


    //  functions that do do squat
    public override void die() {
        FindObjectOfType<GameBoard>().helpers.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }

    //  Damage that monsters receive when attacking this
    //  NOTE: This number is a multiplier that gets applied to the monsters attack
    public float getMirroredDamageMod() {
        return .5f;
    }
}
