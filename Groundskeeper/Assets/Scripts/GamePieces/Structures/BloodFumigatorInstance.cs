using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFumigatorInstance : StructureInstance {

    private void Start() {
        mortalInit();
        buildingInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  rolls a chance to see if the monster gets confused (20% chance)
        if(Random.Range(0, 5) == 0) {
            effected.GetComponent<MonsterInstance>().setConfused(true);
        }
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  does nothing
    }
}
