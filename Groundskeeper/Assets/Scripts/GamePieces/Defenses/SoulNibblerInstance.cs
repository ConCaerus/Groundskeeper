using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulNibblerInstance : DefenseInstance {

    //  steals some souls
    public override void specialTickAction(GameObject triggerer) {
        if(triggerer != null) {
            GameInfo.addSouls(triggerer.GetComponent<MonsterInstance>().soulsGiven * soulStealMod, false);
            Debug.Log(GameInfo.souls);
        }
    }
}
