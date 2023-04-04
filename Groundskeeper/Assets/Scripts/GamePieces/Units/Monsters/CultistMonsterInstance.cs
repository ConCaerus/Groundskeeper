using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistMonsterInstance : MonsterInstance {

    public override void sightEnterEffect(GameObject other) {
        other.GetComponent<MonsterInstance>().hasCultistBuff = true;
    }

    public override void sightExitEffect(GameObject other) {
        other.GetComponent<MonsterInstance>().hasCultistBuff = false;
    }
}
