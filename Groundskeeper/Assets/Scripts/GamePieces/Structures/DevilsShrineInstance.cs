using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilsShrineInstance : StructureInstance {
    Coroutine healer = null;

    //  make this game object because monster instance might not be being attactched after modifying
    [SerializeField] GameObject healingEffect;
    List<GameObject> beingHealed = new List<GameObject>();
    List<GameObject> activeEffects = new List<GameObject>();

    private void Start() {
        mortalInit();
        structureInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  adds the unit to the healing coroutine
        beingHealed.Add(effected.gameObject);
        var e = Instantiate(healingEffect, effected.gameObject.transform);
        e.transform.localPosition = Vector3.zero;
        activeEffects.Add(e.gameObject);
        //  starts the coroutine if it's not started
        if(healer == null)
            StartCoroutine(healUnits());
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  removes the unit from the healing coroutine
        int index = beingHealed.IndexOf(effected.gameObject);
        beingHealed.RemoveAt(index);
        activeEffects[index].GetComponent<ParticleSystem>().Stop();
        activeEffects.RemoveAt(index);
        //  stops the coroutine if there are not more units being healed
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
                activeEffects[i].GetComponent<ParticleSystem>().Stop();
                activeEffects.RemoveAt(i);
                continue;
            }

            //  does things
            beingHealed[i].GetComponent<Mortal>().heal(20);
        }
        yield return new WaitForSeconds(1.5f);
        healer = StartCoroutine(healUnits());
    }
}
