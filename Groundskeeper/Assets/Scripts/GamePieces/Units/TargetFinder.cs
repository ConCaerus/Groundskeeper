using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {
    //  max dist that an enemy will care about whether or not they are close to a thing
    //  if they are out of range from everything, they will walk towards the house
    float monsterDistToCare = 15.0f;
    float monsterInfatuatedDist = 30.0f;
    float helperDistToCare = 25.0f;

    public Vector2 getTargetForMonster(GameObject monster) {
        if(monster.GetComponent<MonsterInstance>() == null)
            return monster.transform.position;
        var m = monster.GetComponent<MonsterInstance>();
        m.followingTransform = null;    //  resets the transform that the monster is currently following

        //  gets all of the collisions within the relevant radius
        LayerMask relevantCols = new LayerMask();
        if(m.favoriteTarget == Monster.targetType.People || m.favoriteTarget == Monster.targetType.All) {
            relevantCols += LayerMask.GetMask("Player");
            relevantCols += LayerMask.GetMask("Helper");
        }
        if(m.favoriteTarget == Monster.targetType.Buildings || m.favoriteTarget == Monster.targetType.All) {
            relevantCols += LayerMask.GetMask("Building");
            if(Vector2.Distance(monster.transform.position, Vector2.zero) < monsterInfatuatedDist) {
                m.infatuated = true;
                return Vector2.zero;
            }
        }
        if(m.favoriteTarget == Monster.targetType.House || m.favoriteTarget == Monster.targetType.All) {
            relevantCols += LayerMask.GetMask("House");
            if(Vector2.Distance(monster.transform.position, Vector2.zero) < monsterInfatuatedDist) {
                m.infatuated = true;
                return Vector2.zero;
            }
        }

        var cols = Physics2D.OverlapCircleAll(monster.transform.position, monsterDistToCare, relevantCols);

        //  if no collisions, have the monster move closer to the house
        if(cols.Length == 0)
            return Vector2.zero;

        KdTree<Transform> rel = new KdTree<Transform>();
        foreach(var i in cols) {
            rel.Add(i.gameObject.transform);
        }

        var closest = rel.FindClosest(monster.transform.position);
        if(closest.gameObject.tag == "Player" || closest.gameObject.tag == "Helper")
            m.followingTransform = closest.gameObject.transform;
        return closest.transform.position;
    }


    public Vector2 getTargetForHelper(GameObject helper) {
        if(helper.GetComponent<HelperInstance>() == null)
            return helper.transform.position;

        var h = helper.GetComponent<HelperInstance>();
        h.followingTransform = null;
        h.hasTarget = (Vector2)helper.transform.position != h.startingPos;

        LayerMask relevantCols = new LayerMask();
        if(h.helpType == Helper.helperType.Attack || h.helpType == Helper.helperType.All)
            relevantCols += LayerMask.GetMask("Monster");
        if(h.helpType == Helper.helperType.Repair || h.helpType == Helper.helperType.All) {
            relevantCols += LayerMask.GetMask("Building");
            relevantCols += LayerMask.GetMask("House");
        }
        if(h.helpType == Helper.helperType.Heal || h.helpType == Helper.helperType.All) {
            relevantCols += LayerMask.GetMask("Player");
            relevantCols += LayerMask.GetMask("Helper");
        }

        var cols = Physics2D.OverlapCircleAll(helper.transform.position, helperDistToCare, relevantCols);

        //  if no collisions, have the helper go back to the starting pos
        if(cols.Length == 0) {
            h.inReach = false;
            return h.startingPos;
        }

        KdTree<Transform> rel = new KdTree<Transform>();
        foreach(var i in cols) {
            if(i.gameObject != helper.gameObject)
                rel.Add(i.gameObject.transform);
        }

        if(rel.Count == 0) {
            h.inReach = false;
            return h.startingPos;
        }
        h.hasTarget = true;

        var closest = rel.FindClosest(helper.transform.position);
        if(closest.gameObject.tag == "Monster" || closest.gameObject.tag == "Helper" || closest.gameObject.tag == "Player")
            h.followingTransform = closest.gameObject.transform;
        return closest.transform.position;
    }
}
