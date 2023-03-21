using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    private void OnTriggerEnter2D(Collider2D col) {
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            var mi = unit.GetComponent<MonsterInstance>();
            //  checks if tag is untargetable
            if(!isTagTargetable(col.gameObject.tag, mi))
                return;

            //  checks if attractive pieces have entered the sight
            if(col.GetComponent<Buyable>() != null && col.GetComponent<Buyable>().isAttractive) {
                mi.followingTransform = col.gameObject.transform;
            }

            //  checks if already has target
            if(mi.followingTransform != null && mi.followingTransform.gameObject != null) {
                return;
            }
            //  if in need of target, give
            mi.followingTransform = col.gameObject.transform;
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
                    if(mi.favoriteTarget == Monster.targetType.People)
                        mi.followingTransform = FindObjectOfType<PlayerInstance>().gameObject.transform;
                    else
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

    bool isTagTargetable(string tag, MonsterInstance mi) {
        //  checks if tag is in the attackable pool
        if(!(tag == "Player" || tag == "Helper" || tag == "Structure" || tag == "House"))
            return false;
        //  if can attack all, return true right away
        if(mi.favoriteTarget == Monster.targetType.All)
            return true;

        //  specifics
        switch(tag) {
            case "Player": 
            case "Helper": return mi.favoriteTarget == Monster.targetType.People;
            case "Structure": return mi.favoriteTarget == Monster.targetType.Structures;
            case "House": return mi.favoriteTarget == Monster.targetType.House;
        }
        return false;
    }
}
