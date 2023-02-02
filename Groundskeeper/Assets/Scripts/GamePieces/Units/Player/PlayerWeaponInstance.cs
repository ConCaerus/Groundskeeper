using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerWeaponInstance : WeaponInstance {
    bool canAttack;
    public bool canAttackG { 
        get { return canAttack; } 
        set {
            canAttack = value || gb.monsters.Count > 0;
        } 
    }

    Coroutine charger = null;
    PlayerInstance pi;
    GameTutorialCanvas gtc;
    PlayerUICanvas puc;
    SetupSequenceManager ssm;
    GameBoard gb;

    public float chargeMod { get; private set; } = 1.0f;

    private void Awake() {
        pi = FindObjectOfType<PlayerInstance>();
        gtc = FindObjectOfType<GameTutorialCanvas>();
        puc = FindObjectOfType<PlayerUICanvas>();
        ssm = FindObjectOfType<SetupSequenceManager>();
        gb = FindObjectOfType<GameBoard>();
        canAttackG = GameInfo.getNightCount() > 0;
    }

    public override void movementLogic() {
        if(canAttackG && transform.lossyScale.x > 0f && !ssm.isActiveAndEnabled) {
            //  player is attacking normally
            if(Input.GetMouseButton(0) && charger == null) {
                charger = StartCoroutine(chargeTimer());
            }
            //  player released their attack, whether it's charged or not
            else if(!Input.GetMouseButton(0) && charger != null) {
                if(gtc != null) {
                    gtc.hasAttacked();
                    if(chargeMod > 1.01f)
                        gtc.hasChargedAttacked();
                }
                StopCoroutine(charger);
                charger = null;
                puc.updateChargeSlider(0.0f, 1.0f);
                attack(chargeMod);
                chargeMod = 1.0f;
            }
        }
        lookAtMouse();
    }

    IEnumerator chargeTimer() {
        float maxCharge = 2.0f;
        float tickTime = .35f, origTickTime = .35f;
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
                chargeMod = 1.0f;
                target = 1.01f;
                //  initial wait time that only exists for before ticks are counted
                if(firstTime) {
                    yield return new WaitForSeconds(.75f - tickTime);   //  - ticktime because ticktime gets waited for at the end
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

            DOTween.To(() => chargeMod, x => chargeMod = x, target, .1f).OnUpdate(() => {
                if(charger != null)
                    puc.updateChargeSlider(chargeMod - 1f, maxCharge - 1f);
                else
                    puc.updateChargeSlider(0f, maxCharge);
            });

            if(s)
                i--;

            tickTime = origTickTime;
        }
    }
}