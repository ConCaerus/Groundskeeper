using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuildingInstance : Building {
    [SerializeField] GameObject bloodParticles;

    protected void buildingInit() {
        var sStats = GameInfo.getStructureStats();
        FindObjectOfType<GameBoard>().structures.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        //  apply health buff
        maxHealth = (int)(maxHealth * sStats.structureHealthBuff);
        health = (int)(health * sStats.structureHealthBuff); //  i don't know if buildings start each night with full health

        //  AOE shit
        if(usesAreaOfEffect) {
            aoeObject.GetComponent<CircleCollider2D>().radius = aoeRadius;
            aoeObject.GetComponent<StructureAreaOfEffect>().aoeEffect = aoeEffect;
            aoeObject.GetComponent<StructureAreaOfEffect>().amt = aoeEffectAmount;
            foreach(var i in aoeEffectedGamePieces)
                aoeObject.GetComponent<StructureAreaOfEffect>().effectedPieces.Add(i);
        }
    }

    public abstract void aoeEffect(GameObject effected, float amount);
    public abstract void aoeLeaveEffect(GameObject effected, float amount);

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
