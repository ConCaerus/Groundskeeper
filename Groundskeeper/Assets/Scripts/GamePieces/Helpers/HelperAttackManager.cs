using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperAttackManager : MonoBehaviour {

    public void addHelper(LumberjackInstance hel) {
        StartCoroutine(attackLogic(hel));
    }


    IEnumerator attackLogic(LumberjackInstance hel) {
        if(hel == null)
            yield break;
        while(FindObjectOfType<GameBoard>().monsters.Count == 0)
            yield return new WaitForSeconds(2.0f);

        if(hel == null)
            yield break;
        /*
        var d = 0f;
        if(hel.followingTransform != null) {
            d = Vector2.Distance(hel.transform.position, hel.followingTransform.position);
        }
        else {
            var mon = FindObjectOfType<GameBoard>().monsters.FindClosest(hel.transform.position);
            d = Vector2.Distance(hel.transform.position, mon.transform.position);
        }
        hel.inReach = d <= inReachDist;

        if(!hel.inReach)
            yield return new WaitForSeconds(Mathf.Clamp((d - inReachDist) / 12.0f, .05f, 2.0f));  //  wait to check if in reach again based on the distance to the nearest fucker
        */
        if(hel.inReach) {
            hel.GetComponentInChildren<LumberjackWeaponInstance>().attack();   //  attack the fucker
            yield return new WaitForSeconds(hel.getAttackCoolDown());  //  already set to inReach, wait to check if not inreach
        }

        //  while nothing important is happening
        while(hel != null && hel.gameObject != null && !hel.hasTarget)
            yield return new WaitForSeconds(.25f);

        //  if something important is about to happen
        if(hel != null && hel.gameObject != null && hel.hasTarget)
            yield return new WaitForEndOfFrame();
        StartCoroutine(attackLogic(hel));
    }
}
