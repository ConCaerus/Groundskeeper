using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceInstance : Defence {
    [SerializeField] GameObject bloodParticles;
    TickDamager td;
    DefenceStats dStats;

    public void dealDamage(GameObject obj) {
        if(obj == null)
            return;
        //  apply buffs to the damage
        int realDmg = (int)(dmgAmt * dStats.defenceDamageBuff);

        //  removes the obj from the ticking pool if the unit is going to die this time
        if(obj.GetComponent<Mortal>().health <= realDmg)
            td.removeTick(obj);

        obj.GetComponent<Mortal>().takeDamage(realDmg, 0, transform.position, false, false);
    }

    private void Start() {
        dStats = GameInfo.getDefenceStats();
        FindObjectOfType<GameBoard>().defences.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
        td = FindObjectOfType<TickDamager>();
    }

    public override void customHitLogic(float knockback, Vector2 origin, bool stun) {
    }

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
