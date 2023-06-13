using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DefenseInstance : Defense {
    [SerializeField] GameObject bloodParticles;
    TickDamager td;
    defenseStats dStats;

    public void dealDamage(GameObject triggerer) {
        if(triggerer == null)
            return;
        //  apply buffs to the damage
        int realDmg = (int)(dmgAmt * dStats.defenseDamageBuff);

        //  does any special actions
        specialTickAction(triggerer);

        //  removes the obj from the ticking pool if the unit is going to die this time
        if(triggerer.GetComponent<Mortal>().health <= realDmg)
            td.removeTick(triggerer);

        triggerer.GetComponent<Mortal>().takeDamage(realDmg, 0, transform.position, false, 0.0f, true);
    }

    public abstract void specialTickAction(GameObject triggerer);

    private void Start() {
        dStats = GameInfo.getdefenseStats();
        FindObjectOfType<GameBoard>().defenses.Add(this);
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
        td = FindObjectOfType<TickDamager>();
    }

    public override void hitLogic(float knockback, Vector2 origin, float stunTime) {
    }

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        Destroy(gameObject);
    }
}
