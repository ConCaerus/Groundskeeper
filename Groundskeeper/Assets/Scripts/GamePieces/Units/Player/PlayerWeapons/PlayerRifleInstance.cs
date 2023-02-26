using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRifleInstance : PlayerWeaponVariant {
    GameTutorialCanvas gtc;

    public override void setup() {
        gtc = FindObjectOfType<GameTutorialCanvas>();
    }

    public override void performOnAttack() {
        if(gtc != null) {
            gtc.hasAttacked();
            if(pi.weaponAttackMod > 1.01f)
                gtc.hasChargedAttacked();
        }
        attack(GameInfo.mousePos(), pi.weaponAttackMod);
        pi.weaponAttackMod = 1.0f;
    }

    public override void performOnAttackEnd() {
    }

    //  currently shoots from the position of the gun sprite
    //  might want to change it to the player's position later idk
    public override void shootMonster() {
        LayerMask monsterLayer = LayerMask.GetMask("Monster");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, GameInfo.mousePos() - (Vector2)transform.position, reference.range, monsterLayer);
        if(hit.collider != null) {
            var o = hit.collider.gameObject.GetComponentInParent<MonsterInstance>().gameObject;
            a.attack(o.gameObject, true);   //  also starts the cooldown
            if(pi != null) {
                cm.shake(pi.getDamage());
            }
            o.GetComponentInChildren<SlashEffect>().slash(user.transform.position, rotObj.transform.GetChild(0).localRotation.x != 0f);
        }
        Debug.DrawRay(transform.position, GameInfo.mousePos() - (Vector2)transform.position);
    }
}
