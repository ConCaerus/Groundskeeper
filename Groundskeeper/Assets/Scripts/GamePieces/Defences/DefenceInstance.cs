using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenceInstance : Defence {
    [SerializeField] GameObject bloodParticles;
    TickDamager td;
    DefenceStats dStats;

    public void dealDamage(GameObject triggerer) {
        if(triggerer == null)
            return;
        //  apply buffs to the damage
        int realDmg = (int)(dmgAmt * dStats.defenceDamageBuff);

        //  does any special actions
        specialTickAction(triggerer);

        //  removes the obj from the ticking pool if the unit is going to die this time
        if(triggerer.GetComponent<Mortal>().health <= realDmg)
            td.removeTick(triggerer);

        triggerer.GetComponent<Mortal>().takeDamage(realDmg, 0, transform.position, false, false);
    }

    public abstract void specialTickAction(GameObject triggerer);

    private void Start() {
        dStats = GameInfo.getDefenceStats();
        FindObjectOfType<GameBoard>().defences.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
        td = FindObjectOfType<TickDamager>();
    }

    public override void hitLogic(float knockback, Vector2 origin, bool stun) {
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
