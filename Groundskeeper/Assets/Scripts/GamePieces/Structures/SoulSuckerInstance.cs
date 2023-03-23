using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSuckerInstance : BuildingInstance {
    Coroutine sucker = null;

    //  make this game object because monster instance might not be being attactched after modifying
    List<MonsterInstance> beingSucked = new List<MonsterInstance>();

    private void Start() {
        mortalInit();
        buildingInit();
    }

    public override void aoeEffect(GameObject effected, float amount) {
        //  adds the monster to the sucking coroutine
        beingSucked.Add(effected.GetComponent<MonsterInstance>());
        //  starts the coroutine if it's not started
        if(sucker == null)
            StartCoroutine(suckMonster());
        Debug.Log("here");
    }

    public override void aoeLeaveEffect(GameObject effected, float amount) {
        //  removes the monster from the sucking coroutine
        beingSucked.Remove(effected.GetComponent<MonsterInstance>());
        //  stops the coroutine if there are not more monsters being sucked
        if(beingSucked.Count == 0)
            StopCoroutine(sucker);
        sucker = null;
    }

    IEnumerator suckMonster() {
        foreach(var i in beingSucked) {
            i.health -= 1;
            i.checkForDeath();
            GameInfo.addSouls(i.soulsGiven / 10f, false);
        }
        yield return new WaitForSeconds(.25f);
    }
}
