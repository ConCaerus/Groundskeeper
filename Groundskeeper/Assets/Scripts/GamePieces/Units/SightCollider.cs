using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    private void OnTriggerEnter2D(Collider2D col) {
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            //  an attackable unit entered sights
            if(col.gameObject.tag == "Player" || col.gameObject.tag == "Helper") {
                var mi = unit.GetComponent<MonsterInstance>();
                if(mi.favoriteTarget == Monster.targetType.People || mi.favoriteTarget == Monster.targetType.All) {
                    mi.followingTransform = col.gameObject.transform;
                }
            }
        }

        //  lumberjack / helper shit
        else if(unit.GetComponent<LumberjackInstance>() != null) {
            //  an attackable unit entered sights
            if(col.gameObject.tag == "Monster") {
                var li = unit.GetComponent<LumberjackInstance>();

                li.hasTarget = true;
                li.followingTransform = col.gameObject.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            //  an attackable unit entered sights
            if(col.gameObject.tag == "Player" || col.gameObject.tag == "Helper") {
                var mi = unit.GetComponent<MonsterInstance>();
                if(mi.favoriteTarget == Monster.targetType.People || mi.favoriteTarget == Monster.targetType.All) {
                    mi.followingTransform = FindObjectOfType<HouseInstance>().gameObject.transform;
                    StartCoroutine(resetCollider());
                }
            }
        }

        //  helper / lumberjack shit
        else if(unit.GetComponent<LumberjackInstance>() != null) {
            //  an attackable unit entered sights
            if(col.gameObject.tag == "Monster") {
                var li = unit.GetComponent<LumberjackInstance>();

                li.hasTarget = false;
                li.followingTransform = null;
                StartCoroutine(resetCollider());
            }
        }
    }

    //  resets the collider to check and see if there are still any relevant collisions
    IEnumerator resetCollider() {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForEndOfFrame();
        GetComponent<Collider2D>().enabled = true;
    }
}
