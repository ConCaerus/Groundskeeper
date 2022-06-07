using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour {
    //  max dist that an enemy will care about whether or not they are close to a thing
    //  if they are out of range from everything, they will walk towards the house
    float monsterDistToCare = 15.0f;
    float helperDistToCare = 25.0f;

    public Vector2 getTargetForMonster(GameObject monster) {
        if(monster.GetComponent<MonsterInstance>() == null)
            return monster.transform.position;

        var m = monster.GetComponent<MonsterInstance>();
        var target = monster.transform.position;

        //  check for hard set targets
        if(m.favoriteTarget == Monster.targetType.People)
            target = getClosestPerson(monster.transform.position);
        else if(m.favoriteTarget == Monster.targetType.Buildings)
            target = getClosestBuilding(monster.transform.position);
        else if(m.favoriteTarget == Monster.targetType.House || monster.GetComponent<MonsterInstance>().infatuated && GameObject.FindGameObjectWithTag("House") != null)
            target = GameObject.FindGameObjectWithTag("House").transform.position;
        else
            target = getClosestPersonOrBuilding(monster.transform.position);

        if(target != monster.transform.position && Vector2.Distance(target, monster.transform.position) < monsterDistToCare)
            return target;
        return GameObject.FindGameObjectWithTag("House") != null ? (Vector2)GameObject.FindGameObjectWithTag("House").transform.position : getClosestPerson(monster.transform.position);
    }

    public Vector2 getClosestPerson(Vector2 origin) {
        float closest = Vector2.Distance(origin, GameObject.FindGameObjectWithTag("Player").transform.position);
        Vector2 temp = GameObject.FindGameObjectWithTag("Player").transform.position;
        foreach(var i in GameObject.FindGameObjectsWithTag("Person")) {
            if(Vector2.Distance(origin, i.transform.position) < closest) {
                closest = Vector2.Distance(origin, i.transform.position);
                temp = i.transform.position;
            }
        }
        return temp;
    }
    public Vector2 getClosestBuilding(Vector2 origin) {
        float closest = -1f;
        Vector2 temp = origin;
        foreach(var i in GameObject.FindGameObjectsWithTag("Building")) {
            if(closest == -1f || Vector2.Distance(origin, i.transform.position) < closest) {
                closest = Vector2.Distance(origin, i.transform.position);
                temp = i.transform.position;
            }
        }
        return temp;
    }
    public Vector2 getClosestPersonOrBuilding(Vector2 origin) {
        Vector2 closestPerson = getClosestPerson(origin);
        Vector2 closestBuilding = getClosestBuilding(origin);
        if(closestBuilding == origin)
            return closestPerson;
        return Vector2.Distance(closestBuilding, origin) < Vector2.Distance(closestPerson, origin) ? closestBuilding : closestPerson;
    }


    public Vector2 getTargetForHelper(GameObject helper) {
        if(helper.GetComponent<HelperInstance>() == null)
            return helper.transform.position;

        var h = helper.GetComponent<HelperInstance>();

        if(GameObject.FindGameObjectsWithTag("Monster").Length > 0) {
            var target = getClosestMonster(helper.transform.position);
            if(Vector2.Distance(target, helper.transform.position) < helperDistToCare) {
                h.hasTarget = true;
                return target;
            }
            else
                h.inReach = false;
        }
        if(Vector2.Distance(helper.transform.position, h.startingPos) > .01f) {
            h.hasTarget = true;
            return h.startingPos;
        }
        h.hasTarget = false;
        return helper.transform.position;
    }

    public Vector2 getClosestMonster(Vector2 origin) {
        float closest = -1f;
        Vector2 temp = origin;
        foreach(var i in GameObject.FindGameObjectsWithTag("Monster")) {
            if(closest == -1f || Vector2.Distance(origin, i.transform.position) < closest) {
                closest = Vector2.Distance(origin, i.transform.position);
                temp = i.transform.position;
            }
        }
        return temp;
    }
}
