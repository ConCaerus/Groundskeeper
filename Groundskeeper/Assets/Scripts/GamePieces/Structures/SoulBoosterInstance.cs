using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBoosterInstance : StructureInstance {
    [SerializeField] GameObject soulEffect;
    List<ParticleSystem> activeEffects = new List<ParticleSystem>();

    private void Start() {
        mortalInit();
        structureInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  increases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven * 2.0f;
        var eff = Instantiate(soulEffect.gameObject, effected.transform);
        eff.transform.localPosition = Vector3.zero;
        activeEffects.Add(eff.GetComponent<ParticleSystem>());
    }
    public override void aoeLeaveEffect(GameObject effected) {
        //  decreases the number of souls that the monster will drop
        effected.GetComponent<MonsterInstance>().soulsGiven = effected.GetComponent<MonsterInstance>().originalSoulsGiven;
        foreach(var i in effected.GetComponentsInChildren<ParticleSystem>()) {
            if(i == soulEffect.GetComponent<ParticleSystem>()) {
                i.Stop();
                break;
            }
        }
    }
}
