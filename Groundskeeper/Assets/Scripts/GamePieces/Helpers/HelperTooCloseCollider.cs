using DG.Tweening;
using FunkyCode.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTooCloseCollider : MonoBehaviour {
    [SerializeField] GameObject unit;

    [SerializeField] List<GameInfo.GamePiece> scaries = new List<GameInfo.GamePiece>();

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
        //  if the entered thing is scary
        if(scaries.Contains(GameInfo.tagToGamePiece(col.gameObject.tag))) {
            hi.tooClose = true;
            hi.followingTransform = col.gameObject.transform;   //  sets the scary thing to be the following transform so the helper will move directly away from it
        }
    }

    void inReachExitLogic(GameObject col) {
        //  checks if the exited collider was the thing that the helper was following
        if(hi.hasTarget && hi.followingTransform == col.transform) {
            hi.inReach = false;
            resetCollider();
        }
    }

    //  resets the collider to check and see if there are still any relevant collisions
    public void resetCollider() {
        hi.tooClose = false;
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
