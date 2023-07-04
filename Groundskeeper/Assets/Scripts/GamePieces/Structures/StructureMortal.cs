using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class StructureMortal : Mortal {
    [HideInInspector] public List<RepairmanInstance> eligibleRepairmen = new List<RepairmanInstance>();
    Coroutine checker = null;

    public override void hitLogic(float knockback, Vector2 origin, float stunTime) {
        float takeTime = .5f;

        //  show that this got hurt
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GetComponent<SpriteRenderer>().color.a);
        GetComponent<SpriteRenderer>().DOBlendableColor(new Color(1.0f, 1.0f, 1.0f, GetComponent<SpriteRenderer>().color.a), takeTime);

        if(gameObject.tag == "House")
            FindObjectOfType<CameraMovement>().shake(10f);

        tellGuy();
    }

    public void seesRepairman(RepairmanInstance guy) {
        //  adds the guy to the thing
        eligibleRepairmen.Add(guy);

        //  decides if needs guy right away
        if(health < maxHealth)
            tellGuy();
    }

    void tellGuy() {
        if(eligibleRepairmen.Count > 0) {
            RepairmanInstance bestGuy = null;
            //  function goes through all repairment and checks if they are busy
            for(int i = eligibleRepairmen.Count - 1; i >= 0; i--) {
                //  checks if shit's fucked
                if(eligibleRepairmen[i] == null || eligibleRepairmen[i].isDead) {
                    eligibleRepairmen.RemoveAt(i);
                    continue;
                }

                //  if they aren't busy, use immediately
                if(!eligibleRepairmen[i].isBusy()) {
                    eligibleRepairmen[i].repairStructure((StructureInstance)this);
                    bestGuy = eligibleRepairmen[i];
                    break;
                }

                //  if they are busy, check if they're better than the last busy guy
                else {
                    if(bestGuy == null || eligibleRepairmen[i].queuedTargets.Count < bestGuy.queuedTargets.Count)
                        bestGuy = eligibleRepairmen[i];
                }
            }
            bestGuy.repairStructure((StructureInstance)this);

            if(checker != null)
                StopCoroutine(checker);
            checker = StartCoroutine(followupWithGuy(bestGuy));
        }
    }

    IEnumerator followupWithGuy(RepairmanInstance guy) {
        while(health < maxHealth) {
            //  waits for things to settle
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //  checks if guy got got
            if(guy == null || guy.isDead || guy.health <= 0f) {
                yield return new WaitForEndOfFrame();
                checker = null;
                tellGuy();
                yield break;
            }

            //  check if guy heard
            if(!guy.queuedTargets.Contains((StructureInstance)this)) {
                checker = null;
                tellGuy();
                yield break;
            }
            yield return new WaitForSeconds(.1f);
        }
        checker = null;
    }
}