using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRifleInstance : PlayerWeaponVariant {
    GameTutorialCanvas gtc;
    [SerializeField] public ParticleSystem gunFireParticles;

    public override void variantSetup() {
        gtc = FindObjectOfType<GameTutorialCanvas>();
        //  destroys all gunfire particles that aren't being used
        foreach(var i in gunFireParticles.transform.parent.GetComponentsInChildren<ParticleSystem>()) {
            if(i != gunFireParticles)
                Destroy(i.gameObject);
        }
    }

    public override void performOnAttack() {
        if(gtc != null) {
            gtc.hasAttacked();
            if(pi.weaponAttackMod > 1.01f)
                gtc.hasChargedAttacked();
        }
        //  flair
        gunFireParticles.Play();
        gunLight.size = 20f;
        DOTween.To(() => gunLight.size, x => gunLight.size = x, 0.0f, .15f);

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
