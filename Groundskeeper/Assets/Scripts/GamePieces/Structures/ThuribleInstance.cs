using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThuribleInstance : StructureInstance {
    Coroutine sucker = null;

    //  make this game object because monster instance might not be being attactched after modifying
    List<GameObject> beingSucked = new List<GameObject>();

    private void Start() {
        mortalInit();
        structureInit();
        GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    public override void aoeEffect(GameObject effected) {
        //  adds the monster to the sucking coroutine
        beingSucked.Add(effected.gameObject);
        //  starts the coroutine if it's not started
        if(sucker == null)
            StartCoroutine(suckMonsters());
    }

    public override void aoeLeaveEffect(GameObject effected) {
        //  removes the monster from the sucking coroutine
        beingSucked.Remove(effected.gameObject);
        //  stops the coroutine if there are not more monsters being sucked
        if(beingSucked.Count == 0 && sucker != null) {
            StopCoroutine(sucker);
            sucker = null;
        }
    }
    public void setup() {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
        GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    IEnumerator suckMonsters() {
        for(int i = beingSucked.Count - 1; i >= 0; i--) {
            //  check if the number of monsters has decreased
            if(i > beingSucked.Count - 1)
                continue;
            //  check if the thing exists
            if(beingSucked[i] == null || beingSucked[i].GetComponent<MonsterInstance>() == null) {
                beingSucked.RemoveAt(i);
                continue;
            }
            
            //  does things
            var m = beingSucked[i].GetComponent<MonsterInstance>();
            m.takeDamage(2, 0f, transform.position, false, 0.0f, false);
            GameInfo.addSouls(m.soulsGiven / 10f, false);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(.5f);
        sucker = StartCoroutine(suckMonsters());
    }
}
