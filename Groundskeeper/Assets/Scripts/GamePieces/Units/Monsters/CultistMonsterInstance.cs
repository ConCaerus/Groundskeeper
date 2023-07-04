using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CultistMonsterInstance : MonsterInstance {
    [SerializeField] GameObject buffedParticles;


    public override void sightEnterEffect(GameObject other) {
        other.GetComponent<MonsterInstance>().hasCultistBuff = true;
        var part = Instantiate(buffedParticles.gameObject, other.transform);
        part.transform.localPosition = Vector3.zero;
    }

    public override void sightExitEffect(GameObject other) {
        other.GetComponent<MonsterInstance>().hasCultistBuff = false;
        foreach(var i in other.GetComponentsInChildren<ParticleSystem>()) {
            if(i == buffedParticles.GetComponent<ParticleSystem>())
                i.Stop();
        }
    }
}
