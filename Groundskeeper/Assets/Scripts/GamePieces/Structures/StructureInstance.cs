using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureInstance : Structure {
    protected StructureAreaOfEffect saoe;

    protected void structureInit() {
        var sStats = GameInfo.getStructureStats();
        FindObjectOfType<GameBoard>().structures.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        //  apply health buff
        maxHealth = (int)(maxHealth * sStats.structureHealthBuff);
        health = maxHealth;
        healthBar.setParent(gameObject);

        //  AOE shit
        if(usesAreaOfEffect) {
            saoe = aoeObject.GetComponent<StructureAreaOfEffect>();
            StartCoroutine(saoe.areaStartExpansion(GetComponent<Buyable>().effectRadius));
            saoe.aoeEffect = aoeEffect;
            saoe.aoeLeaveEffect = aoeLeaveEffect;
            foreach(var i in aoeEffectedGamePieces)
                saoe.effectedPieces.Add(i);
        }
    }

    public abstract void aoeEffect(GameObject effected);
    public abstract void aoeLeaveEffect(GameObject effected);

    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        Destroy(gameObject);
    }
}
