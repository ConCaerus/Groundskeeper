using DG.Tweening;
using FunkyCode.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SightCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    [SerializeField] bool dynamicChecking = false;

    CircleCollider2D c;
    float maxRad;

    private void OnTriggerEnter2D(Collider2D col) {
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            var mi = unit.GetComponent<MonsterInstance>();
            //  checks if tag is untargetable
            if(!isTagTargetable(col.gameObject.tag, mi))
                return;

            //  checks if already going after an attractive target
            if(mi.hasTarget && mi.followingTransform.GetComponent<Buyable>() != null && mi.followingTransform.GetComponent<Buyable>().isAttractive) {
                return;
            }
            //  checks if attractive pieces have entered the sight
            else if(col.GetComponent<Buyable>() != null && col.GetComponent<Buyable>().isAttractive) {
                mi.followingTransform = col.gameObject.transform;
                return;
            }

            //  if in need of target, give
            if(!mi.hasTarget) {
                mi.hasTarget = true;
                mi.followingTransform = col.gameObject.transform;
            }
        }

        //  lumberjack / helper shit
        else if(unit.GetComponent<HelperInstance>() != null) {
            var li = unit.GetComponent<HelperInstance>();
            //  checks if tag is targetable
            if(li.hasTarget || !isTagTargetable(col.gameObject.tag, li, col.gameObject.GetComponent<StructureInstance>()))
                return;

            li.hasTarget = true;
            li.followingTransform = col.gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            var mi = unit.GetComponent<MonsterInstance>();

            if(col.gameObject.transform == mi.followingTransform) {
                if(mi.favoriteTarget == Monster.targetType.People)
                    mi.followingTransform = FindObjectOfType<PlayerInstance>().gameObject.transform;
                else
                    mi.followingTransform = FindObjectOfType<HouseInstance>().gameObject.transform;
                resetCollider(mi);
            }
        }

        //  helper / lumberjack shit
        else if(unit.GetComponent<HelperInstance>() != null) {
            //  an attackable unit entered sights
            var li = unit.GetComponent<HelperInstance>();

            //  checks if the exiter is the same as the current following transform
            if(col.gameObject.transform == li.followingTransform) {
                li.hasTarget = false;
                li.followingTransform = null;
                resetCollider(li);
            }
        }
    }

    private void Awake() {
        c = GetComponent<CircleCollider2D>();
        StartCoroutine(areaStartExpansion(c.radius));
        if(dynamicChecking)
            StartCoroutine(dynamicCheck(unit.GetComponent<HelperInstance>(), unit.GetComponent<MonsterInstance>()));
    }

    //  resets the collider to check and see if there are still any relevant collisions
    public void resetCollider(HelperInstance li) {
        li.hasTarget = false;
        li.followingTransform = null;
        c.radius = 0f;
        expandArea();
    }
    public void resetCollider(MonsterInstance li) {
        li.hasTarget = false;
        li.followingTransform = null;
        c.radius = 0f;
        expandArea();
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
    bool isTagTargetable(string tag, HelperInstance mi, StructureInstance si) {
        //  checks if tag is in the attackable pool
        if(!(tag == "Player" || tag == "Helper" || tag == "Structure" || tag == "Monster"))
            return false;

        //  specifics
        switch(tag) {
            case "Player":
            case "Helper": return mi.helpType == Helper.helperType.Heal;
            case "Structure": return mi.helpType == Helper.helperType.Repair && si.health < si.maxHealth;
            case "Monster": return mi.helpType == Helper.helperType.Attack;
        }
        return false;
    }


    IEnumerator dynamicCheck(HelperInstance hi, MonsterInstance mi) {
        yield return new WaitForSeconds(.25f);

        //  if doesn't have target, check for new target
        //  Helpers
        if(hi != null) {
            if(!hi.hasTarget) {
                c.radius = 0f;
                expandArea();
            }
        }

        StartCoroutine(dynamicCheck(hi, mi));
    }

    public IEnumerator areaStartExpansion(float radius) {
        //  shrinks the area of effect at the start of the game
        c.radius = 0f;
        maxRad = radius;
        if(FindObjectOfType<PregameCanvas>() != null) {
            //  waits until the player leaves pregame
            var pc = FindObjectOfType<PregameCanvas>().gameObject;
            while(pc != null) {
                yield return new WaitForSeconds(.1f);
            }
        }

        //  then expands the area of effect to trigger all collisions
        expandArea();
    }

    public void shrinkArea() {
        DOTween.To(() => c.radius, x => c.radius = x, 0f, .15f);
    }
    public void expandArea() {
        DOTween.To(() => c.radius, x => c.radius = x, maxRad, .15f);
    }
}
