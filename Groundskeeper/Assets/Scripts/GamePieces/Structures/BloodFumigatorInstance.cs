using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFumigatorInstance : StructureInstance {

    private void Start() {
        mortalInit();
        structureInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  rolls a chance to see if the monster gets confused (25% chance)
        if(Random.Range(0, 4) == 0) {
            effected.GetComponent<MonsterInstance>().setConfused(true, transform.position);
        }
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  does nothing
    }
}
