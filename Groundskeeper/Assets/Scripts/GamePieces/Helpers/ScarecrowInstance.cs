using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScarecrowInstance : Helper {
    //  functions that don't do squat but i'm too lazy to make the helper class not need these
    public override void hitLogic(float knockback, Vector2 origin, float stunTime) {

    }
    public override Weapon.weaponTitle getWeapon() {
        return Weapon.weaponTitle.None;
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
    public override void updateSprite(Vector2 movingDir, bool opposite) {
        
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

    public override Color getStartingColor() {
        return Color.white;
    }


    private void Start() {
        mortalInit();
        helperInit();
        spriteObj.GetComponent<SpriteRenderer>().sprite = sprite;
    }


    //  functions that do do squat
    public override void die() {
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}
