using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : BuildingMortal {
    public string title;
    public Sprite sprite;

    [Header("Area of Effect")]
    public bool usesAreaOfEffect;
    public List<GameInfo.GamePiece> aoeEffectedGamePieces;
    public float aoeRadius;
    public GameObject aoeObject;
}
