using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxeInstance : PlayerWeaponVariant {
    Coroutine charger = null;


    GameTutorialCanvas gtc;
    PlayerUICanvas puc;
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
            attack(GameInfo.mousePos(), Vector2.zero, pi.weaponAttackMod);
            pi.weaponAttackMod = 1.0f;
        }
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }

    IEnumerator chargeTimer() {
        float maxCharge = 2.0f;
        float tickTime = .3f, origTickTime = .3f;
        float ticksToComplete = 5;  //  zero doens't count, so it'll seem like it'll take this +1 ticks to complete
        float target = 0f;
        bool firstTime = true;

        for(int i = 0; i <= ticksToComplete; i++) {
            //  not charging anymore
            if(charger == null)
                yield return 0;



            var s = pi.isSprinting();
            //  the player is sprinting
            if(s) {
                tickTime = .25f;
            }

            yield return new WaitForSeconds(tickTime * .85f);  //  tick time

            if(pi.isSprinting()) {
                i--;
                if(i < 0)
                    i = 0;
                s = true;
            }

            //  charge is empty
            if(i == 0) {
                pi.weaponAttackMod = 1.0f;
                target = 1.01f;
                //  initial wait time that only exists for before ticks are counted
                if(firstTime) {
                    //yield return new WaitForSeconds(tickTime);   //  - ticktime because ticktime gets waited for at the end
                    s = pi.isSprinting();
                    firstTime = false;
                }
            }
            //  charge is full
            else if(i == ticksToComplete) {
                target = maxCharge;
                if(!s)
                    yield return new WaitForSeconds(tickTime * .15f);
                i -= 1;
            }
            else if(i > 0 && i < ticksToComplete) {
                if(!s)
                    yield return new WaitForSeconds(tickTime * .15f);
                target = ((i * (maxCharge - 1f)) / ticksToComplete) + 1f;
            }
            else if(!s)
                yield return new WaitForSeconds(tickTime * .15f);

            DOTween.To(() => pi.weaponAttackMod, x => pi.weaponAttackMod = x, target, .1f).OnUpdate(() => {
                if(charger != null)
                    puc.updateChargeSlider(pi.weaponAttackMod - 1f, maxCharge - 1f);
                else
                    puc.updateChargeSlider(0f, maxCharge);
            });

            if(s)
                i--;

            tickTime = origTickTime;
        }
    }
}
