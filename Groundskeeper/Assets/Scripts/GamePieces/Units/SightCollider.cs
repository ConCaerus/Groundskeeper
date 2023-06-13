using DG.Tweening;
using FunkyCode.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditorInternal;

public class SightCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    [SerializeField] bool dynamicChecking = false;

    CircleCollider2D c;
    float maxRad;

    MonsterInstance mi = null;
    HelperInstance hi = null;
    HelperTooCloseCollider htcc = null;
    GameBoard gb;

    Coroutine pauseCheckingWaiter = null;

    List<string> relevantTags = new List<string>() {
        "Player", "Helper", "Structure", "House", "Monster"
    };

    private void OnTriggerEnter2D(Collider2D col) {
        if(pauseCheckingWaiter != null)
            return;
        //  monster shit
        if(mi != null) {
            //  effects
            if(mi.sightEffectedPieces.Count > 0) {
                foreach(var i in mi.sightEffectedPieces) {
                    if(GameInfo.gamePieceToTag(i) == col.gameObject.tag) {
                        mi.sightEnterEffect(col.gameObject);
                        break;
                    }
                }
            }

            //  movement
            //  checks if tag is untargetable
            //  doesn't really need to do this, but just to make sure
            if(!isTagTargetable(col.gameObject.tag, mi))
                return;

            //  checks if already going after an attractive target
            if(mi.hasTarget && mi.hasAttractiveTarget)
                return;
            //  checks if attractive pieces have entered the sight
            else if(col.GetComponent<Buyable>() != null && col.GetComponent<Buyable>().isAttractive) {
                mi.followingTransform = col.gameObject.transform;
                mi.hasTarget = true;
                mi.hasAttractiveTarget = true;
                return;
            }

            //  if in need of target, give
            if(!mi.hasTarget) {
                mi.hasTarget = true;
                mi.followingTransform = col.gameObject.transform;
                mi.hasAttractiveTarget = col.GetComponent<Buyable>() != null && col.GetComponent<Buyable>().isAttractive;
            }
        }

        //  lumberjack / helper shit
        else if(hi != null) {
            //  checks if tag is targetable
            //  NOTE: uses the structure instance script (even though it checks for more than just structures) to check the health of them
            if(hi.hasTarget || !isTagTargetable(col.gameObject.tag, hi, col.gameObject.GetComponent<StructureInstance>()))
                return;

            //  checks if the target has too many monsters around it
            //Debug.Log((htcc == null) + " " + hi.helpType.ToString());
            bool scaredOfDestination = false;
            if(htcc != null) {
                if(Vector2.Distance(gb.monsters.FindClosest(col.gameObject.transform.position).transform.position, unit.transform.position) < htcc.maxRad) {
                    Debug.Log("here");
                    scaredOfDestination = true;
                }
            }

            if(!scaredOfDestination) {
                hi.hasTarget = true;
                hi.followingTransform = col.gameObject.transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(pauseCheckingWaiter != null)
            return;
        //  monster shit
        if(unit.GetComponent<MonsterInstance>() != null) {
            var mi = unit.GetComponent<MonsterInstance>();

            //  movement
            if(col.gameObject.transform == mi.followingTransform) {
                if(mi.favoriteTarget == Monster.targetType.People)
                    mi.followingTransform = FindObjectOfType<PlayerInstance>().gameObject.transform;
                else
                    mi.followingTransform = FindObjectOfType<HouseInstance>().gameObject.transform;
                resetCollider(mi);
            }

            //  effects
            if(mi.sightEffectedPieces.Count > 0) {
                foreach(var i in mi.sightEffectedPieces) {
                    if(GameInfo.gamePieceToTag(i) == col.gameObject.tag) {
                        mi.sightExitEffect(col.gameObject);
                        break;
                    }
                }
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
        for(int i = 0; i < 20; i++) {
            var tag = LayerMask.LayerToName(i);
            if(!relevantTags.Contains(tag)) {
                Physics2D.IgnoreLayerCollision(gameObject.layer, i);
            }
        }
        c = GetComponent<CircleCollider2D>();
        gb = FindObjectOfType<GameBoard>();
        StartCoroutine(areaStartExpansion(c.radius));
        if(dynamicChecking)
            StartCoroutine(dynamicCheck(unit.GetComponent<HelperInstance>(), unit.GetComponent<MonsterInstance>()));

        if(unit.GetComponent<MonsterInstance>() != null)
            mi = unit.GetComponent<MonsterInstance>();
        else if(unit.GetComponent<HelperInstance>() != null) {
            hi = unit.GetComponent<HelperInstance>();
            htcc = unit.GetComponentInChildren<HelperTooCloseCollider>();
        }
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
        if(!relevantTags.Contains(tag))
            return false;

        var ft = mi.favoriteTarget;

        //  specifics
        switch(tag) {
            case "Player": return ft == Monster.targetType.People || ft == Monster.targetType.All;
            case "Helper": return ft == Monster.targetType.People || ft == Monster.targetType.All;
            case "Structure": return ft == Monster.targetType.Structures || ft == Monster.targetType.All;
            case "House": return ft == Monster.targetType.House || ft == Monster.targetType.All;
            case "Monster": return false;
        }
        return false;
    }
    bool isTagTargetable(string tag, HelperInstance mi, StructureInstance si) {
        if(!relevantTags.Contains(tag))
            return false;

        //  specifics
        switch(tag) {
            case "Player": return false;
            case "Helper": return mi.helpType == Helper.helperType.Heal;
            case "Structure": return mi.helpType == Helper.helperType.Repair && si.health < si.maxHealth;
            case "House": return false;
            case "Monster": return mi.helpType == Helper.helperType.Attack;
        }
        return false;
    }


    IEnumerator dynamicCheck(HelperInstance hi, MonsterInstance mi) {
        yield return new WaitForSeconds(1f);

        //  if doesn't have target, check for new target
        //  Helpers
        if(hi != null) {
            if(!hi.hasTarget || hi.helpType == Helper.helperType.Repair) {
                shrinkArea();
                yield return new WaitForSeconds(.15f);
                expandArea();
                yield return new WaitForSeconds(.15f);
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
        if(pauseCheckingWaiter != null)
            StopCoroutine(pauseCheckingWaiter);
        pauseCheckingWaiter = StartCoroutine(toggleChecking(.15f));
        DOTween.To(() => c.radius, x => c.radius = x, 0f, .15f);
    }
    public void expandArea() {
        if(pauseCheckingWaiter != null)
            StopCoroutine(pauseCheckingWaiter);
        //pauseCheckingWaiter = StartCoroutine(toggleChecking(.15f));
        DOTween.To(() => c.radius, x => c.radius = x, maxRad, .15f);
    }

    IEnumerator toggleChecking(float dur) {
        yield return new WaitForSeconds(dur);
        pauseCheckingWaiter = null;
    }
}
