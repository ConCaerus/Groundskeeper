using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperAttackManager : MonoBehaviour {
    GameBoard gb;

    private void Awake() {
        gb = FindObjectOfType<GameBoard>();
    }

    public void addHelper(HelperInstance hel) {
        StartCoroutine(inReachLogic(hel));
    }


    IEnumerator inReachLogic(HelperInstance hel) {
        //  checks if helper is null
        if(hel == null)
            yield break;

        //  waits for monsters to spawn
        while(gb.monsters.Count == 0)
            yield return new WaitForSeconds(2.0f);

        //  checks if helper is null
        if(hel == null)
            yield break;

        //  checks if in reach
        if(hel.inReach) {
            hel.inReachEnterAction(hel.followingTransform.gameObject);
        }

        //  while nothing important is happening
        while(hel != null && hel.gameObject != null && !hel.hasTarget)
            yield return new WaitForSeconds(.25f);

        //  if something important is about to happen
        if(hel != null && hel.gameObject != null && hel.hasTarget)
            yield return new WaitForEndOfFrame();
        StartCoroutine(inReachLogic(hel));
    }
}
