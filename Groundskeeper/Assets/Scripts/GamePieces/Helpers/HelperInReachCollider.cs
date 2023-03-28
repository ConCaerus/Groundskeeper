using DG.Tweening;
using FunkyCode.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperInReachCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    HelperInstance hi;
    CircleCollider2D c;

    float maxRad;

    private void Awake() {
        hi = unit.GetComponent<HelperInstance>();
        c = GetComponent<CircleCollider2D>();
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("SightCollider"));
        StartCoroutine(areaStartExpansion(c.radius));
    }

    private void OnTriggerEnter2D(Collider2D col) {
        inReachEnterLogic(col.gameObject);
    }

    private void OnTriggerExit2D(Collider2D col) {
        inReachExitLogic(col.gameObject);
    }

    void inReachEnterLogic(GameObject col) {
        //  checks if the entered unit is relevant to the helper
        bool relevantTarget = false;
        switch(col.gameObject.tag) {
            //  MONSTERS
            case "Monster":
                switch(hi.helpType) {
                    case Helper.helperType.Attack:
                        relevantTarget = true;
                        break;
                }
                break;

            //  STRUCTURES
            case "Structure":
                switch(hi.helpType) {
                    case Helper.helperType.Repair:
                        relevantTarget = true;
                        break;
                }
                break;

            //  PEOPLE
            case "Helper":
                switch(hi.helpType) {
                    case Helper.helperType.Heal:
                        relevantTarget = true;
                        break;
                }
                break;
            case "Player":
                switch(hi.helpType) {
                    case Helper.helperType.Heal:
                        relevantTarget = true;
                        break;
                }
                break;
        }

        if(relevantTarget) {
            hi.inReach = true;
            hi.inReachEnterAction(col.gameObject);
        }
    }

    void inReachExitLogic(GameObject col) {
        //  checks if the exited collider was the thing that the helper was following
        if(hi.hasTarget && hi.followingTransform == col.transform) {
            hi.inReach = false;
            hi.inReachExitAction(col.gameObject);
            resetCollider();
        }
    }

    //  resets the collider to check and see if there are still any relevant collisions
    public void resetCollider() {
        c.radius = 0f;
        expandArea();
    }

    public IEnumerator areaStartExpansion(float radius) {
        //  shrinks the area of effect at the start of the game
        c.radius = 0f;
        maxRad = radius;

        //  waits until the player leaves pregame
        var pc = FindObjectOfType<PregameCanvas>().gameObject;
        while(pc != null) {
            yield return new WaitForSeconds(.1f);
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
