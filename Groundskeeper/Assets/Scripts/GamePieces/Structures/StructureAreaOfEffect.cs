using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureAreaOfEffect : MonoBehaviour {
    public delegate void effect(GameObject effected, float amt);
    public effect aoeEffect;
    [HideInInspector] public float amt;
    [HideInInspector] public List<GameInfo.GamePiece> effectedPieces = new List<GameInfo.GamePiece>();

    private void OnTriggerEnter2D(Collider2D col) {
        if(effectedPieces.Contains(GameInfo.tagToGamePiece(col.gameObject.tag))) {
            aoeEffect(col.gameObject, amt);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  check if found a dying unit
        if(col.gameObject.GetComponent<Mortal>() != null && col.gameObject.GetComponent<Mortal>().health <= 0)
            return;

        if(effectedPieces.Contains(GameInfo.tagToGamePiece(col.gameObject.tag))) {
            aoeEffect(col.gameObject, amt);
        }
    }
}
