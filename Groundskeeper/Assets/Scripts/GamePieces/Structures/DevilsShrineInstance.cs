using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilsShrineInstance : BuildingInstance {
    Coroutine healer = null;

    //  make this game object because monster instance might not be being attactched after modifying
    List<GameObject> beingHealed = new List<GameObject>();

    private void Start() {
        mortalInit();
        buildingInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  adds the monster to the sucking coroutine
        beingHealed.Add(effected.gameObject);
        //  starts the coroutine if it's not started
        if(healer == null)
            StartCoroutine(healUnits());
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  removes the monster from the sucking coroutine
        beingHealed.Remove(effected.gameObject);
        //  stops the coroutine if there are not more monsters being sucked
        if(beingHealed.Count == 0 && healer != null) {
            StopCoroutine(healer);
            healer = null;
        }
    }

    IEnumerator healUnits() {
        for(int i = beingHealed.Count - 1; i >= 0; i--) {
            //  check if the thing exists
            if(beingHealed[i] == null || beingHealed[i].GetComponent<Mortal>() == null) {
                beingHealed.RemoveAt(i);
                continue;
            }

            //  does things
            beingHealed[i].GetComponent<Mortal>().heal(1);
        }
        yield return new WaitForSeconds(1.5f);
        healer = StartCoroutine(healUnits());
    }
}
