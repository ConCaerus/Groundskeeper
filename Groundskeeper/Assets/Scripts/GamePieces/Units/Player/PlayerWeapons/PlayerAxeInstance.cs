using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxeInstance : PlayerWeaponVariant {


    GameTutorialCanvas gtc;
    TransitionCanvas tc;

    public override void variantSetup() {
        gtc = FindObjectOfType<GameTutorialCanvas>();
        puc = FindObjectOfType<PlayerUICanvas>();
        tc = FindObjectOfType<TransitionCanvas>();
    }

    public override void performOnAttack() {
        //  player is attacking normally
        if(charger != null)
            StopCoroutine(charger);
        charger = StartCoroutine(chargeTimer());
        wAnimator.SetTrigger("windup");
    }

    public override void performOnAttackEnd() {
        if(charger != null && tc.finishedLoading) {
            if(gtc != null) {
                gtc.hasAttacked();
                if(pi.weaponAttackMod > 1.01f)
                    gtc.hasChargedAttacked();
            }
            StopCoroutine(charger);
            charger = null;
            puc.updateChargeSlider(0.0f, 1.0f);
            attack(GameInfo.mousePos(), Vector2.zero, pi, pi.weaponAttackMod);
        }
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }
}
