using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountainInstance : StructureInstance {

    [SerializeField] float healAmtPerSec;

    Collider2D c;

    Coroutine healer = null;

    private void Start() {
        c = GetComponent<Collider2D>();
        mortalInit();
        structureInit();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(canHeal() && (col.gameObject.tag == "Player" || col.gameObject.tag == "Helper")) {
            startHealing(col.gameObject.GetComponent<MortalUnit>(), col.collider);
        }
    }


    void startHealing(MortalUnit unit, Collider2D touchingCol) {
        if(healer == null) {
            healer = StartCoroutine(heal(unit, touchingCol));
            unit.playHealingParticles();
        }
    }

    bool canHeal() {
        return healer == null;
    }

    IEnumerator heal(MortalUnit unit, Collider2D touchingCol) {
        float div = 10f;
        unit.health = Mathf.Clamp((int)(unit.health + healAmtPerSec / div), -10, unit.maxHealth);
        yield return new WaitForSeconds(1f / div);

        //  checks if still healing
        if(c.IsTouching(touchingCol)) {
            healer = StartCoroutine(heal(unit, touchingCol));
        }
        else {
            healer = null;
            unit.stopHealingParticles();
        }
    }

    public override void aoeEffect(GameObject effected) {
        //  does nothing
    }
    public override void aoeLeaveEffect(GameObject effected) {
        //  does nothing 
    }
}
