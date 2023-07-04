using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StructureAreaOfEffect;

public class DevilsShrineInstance : StructureInstance {
    Coroutine healer = null;

    //  make this game object because monster instance might not be being attactched after modifying
    [SerializeField] GameObject healingEffect;
    List<GameObject> beingHealed = new List<GameObject>();

    private void Start() {
        mortalInit();
        structureInit();
    }

    public override void aoeEffect(GameObject effected) {
        //  if the guy already is being effected by a devils shrine, remove it from the other list and add it to this one
        if(effected.GetComponent<Mortal>().dsi != null && effected.GetComponent<Mortal>().dsi != this)
            effected.GetComponent<Mortal>().dsi.aoeLeaveEffect(effected.gameObject);
        effected.GetComponent<Mortal>().dsi = this;
        beingHealed.Add(effected.gameObject);
        //  starts the coroutine if it's not started
        if(healer == null)
            StartCoroutine(healUnits());
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  removes the unit from the healing coroutine
        beingHealed.Remove(effected.gameObject);
        effected.GetComponent<Mortal>().stopHealingParticles();
        effected.GetComponent<Mortal>().dsi = null;
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
                if(beingHealed[i].GetComponent<Mortal>() != null)
                    beingHealed[i].GetComponent<Mortal>().stopHealingParticles();
                continue;
            }
            var m = beingHealed[i].GetComponent<Mortal>();

            //  checks if thing needs healing
            if(Mathf.Abs(m.health - m.maxHealth) < .1f) {
                m.stopHealingParticles();
            }
            //  does things
            else {
                m.heal(2);
                m.playHealingParticles();
            }
        }
        yield return new WaitForSeconds(3f);
        healer = StartCoroutine(healUnits());
    }
}
