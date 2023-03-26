using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StructureAreaOfEffect : MonoBehaviour {
    public delegate void effect(GameObject effected);
    public effect aoeEffect, aoeLeaveEffect;
    float maxRad;
    [HideInInspector] public List<GameInfo.GamePiece> effectedPieces = new List<GameInfo.GamePiece>();

    CircleCollider2D col;

    private void Awake() {
        col = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(effectedPieces.Contains(GameInfo.tagToGamePiece(col.gameObject.tag))) {
            aoeEffect(col.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  check if found a dying unit
        if(col.gameObject.GetComponent<Mortal>() != null && col.gameObject.GetComponent<Mortal>().health <= 0)
            return;

        if(effectedPieces.Contains(GameInfo.tagToGamePiece(col.gameObject.tag))) {
            aoeLeaveEffect(col.gameObject);
        }
    }


    public IEnumerator areaStartExpansion(float radius) {
        //  shrinks the area of effect at the start of the game
        col.radius = 0f;
        maxRad = radius;

        //  waits until the player leaves pregame
        var pc = FindObjectOfType<PregameCanvas>().gameObject;
        while(pc != null) {
            yield return new WaitForSeconds(.1f);
        }

        //  then expands the area of effect to trigger all collisions
        DOTween.To(() => col.radius, x => col.radius = x, maxRad, .15f);
    }

    public void shrinkArea() {
        DOTween.To(() => col.radius, x => col.radius = x, 0f, .15f);
    }
    public void expandArea() {
        DOTween.To(() => col.radius, x => col.radius = x, maxRad, .15f);
    }
}
