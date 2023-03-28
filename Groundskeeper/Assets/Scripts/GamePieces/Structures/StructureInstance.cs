using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureInstance : Structure {
    [SerializeField] GameObject bloodParticles;
    protected StructureAreaOfEffect saoe;

    protected void buildingInit() {
        var sStats = GameInfo.getStructureStats();
        //FindObjectOfType<GameBoard>().structures.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        //  apply health buff
        maxHealth = (int)(maxHealth * sStats.structureHealthBuff);
        health = (int)(health * sStats.structureHealthBuff); //  i don't know if buildings start each night with full health

        //  AOE shit
        if(usesAreaOfEffect) {
            saoe = aoeObject.GetComponent<StructureAreaOfEffect>();
            StartCoroutine(saoe.areaStartExpansion(aoeRadius));
            saoe.aoeEffect = aoeEffect;
            saoe.aoeLeaveEffect = aoeLeaveEffect;
            foreach(var i in aoeEffectedGamePieces)
                saoe.effectedPieces.Add(i);
        }
    }

    public abstract void aoeEffect(GameObject effected);
    public abstract void aoeLeaveEffect(GameObject effected);

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        Destroy(gameObject);
    }
}
