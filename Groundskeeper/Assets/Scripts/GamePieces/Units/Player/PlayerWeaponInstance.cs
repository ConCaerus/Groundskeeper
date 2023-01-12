using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWeaponInstance : WeaponInstance {
    [HideInInspector]
    [SerializeField] public bool canAttack = true;

    Coroutine charger = null;

    public float chargeMod { get; private set; } = 1.0f;

    private void Awake() {
        canAttack = true;
    }

    public override void movementLogic() {
        if(canAttack && transform.lossyScale.x > 0f) {
            //  player is attacking normally
            if(Input.GetMouseButton(0) && charger == null) {
                charger = StartCoroutine(chargeTimer());
            }
            //  player released their attack, whether it's charged or not
            else if(!Input.GetMouseButton(0) && charger != null) {
                if(FindObjectOfType<GameTutorialCanvas>() != null)
                    FindObjectOfType<GameTutorialCanvas>().hasAttacked();
                StopCoroutine(charger);
                charger = null;
                FindObjectOfType<PlayerUICanvas>().updateChargeSlider(0.0f, 1.0f);
                attack();
            }
        }
        lookAtMouse();
    }

    IEnumerator chargeTimer() {
        float maxCharge = 2.0f;
        float tickTime = .5f, origTickTime = .5f;
        float ticksToComplete = 5;  //  zero doens't count, so it'll seem like it'll take this +1 ticks to complete
        float target = 0f;
        bool firstTime = true;

        for(int i = 0; i <= ticksToComplete; i++) {
            //  not charging anymore
            if(charger == null)
                yield return 0;



            var s = FindObjectOfType<PlayerInstance>().isSprinting();
            //  the player is sprinting
            if(s) {
                tickTime = .25f;
            }

            yield return new WaitForSeconds(tickTime * .85f);  //  tick time

            if(FindObjectOfType<PlayerInstance>().isSprinting()) {
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
                    s = FindObjectOfType<PlayerInstance>().isSprinting();
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
                    FindObjectOfType<PlayerUICanvas>().updateChargeSlider(chargeMod - 1f, maxCharge - 1f);
                else
                    FindObjectOfType<PlayerUICanvas>().updateChargeSlider(0f, maxCharge);
            });

            if(s)
                i--;

            tickTime = origTickTime;
        }
    }
}