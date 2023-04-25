using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountainInstance : StructureInstance {

    [SerializeField] float healAmtPerSec;
    [SerializeField] GameObject healingParticle;

    Collider2D c;

    Coroutine healer = null;

    private void Start() {
        c = GetComponent<Collider2D>();
        mortalInit();
        structureInit();
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(canHeal() && (col.gameObject.tag == "Player" || col.gameObject.tag == "Helper")) {
            var effect = Instantiate(healingParticle.gameObject, col.gameObject.transform);
            effect.transform.localPosition = Vector3.zero;
            startHealing(col.gameObject.GetComponent<MortalUnit>(), col.collider, effect);
        }
    }


    void startHealing(MortalUnit unit, Collider2D touchingCol, GameObject effect) {
        if(healer == null) 
            healer = StartCoroutine(heal(unit, touchingCol, effect));
    }

    bool canHeal() {
        return healer == null;
    }

    IEnumerator heal(MortalUnit unit, Collider2D touchingCol, GameObject effect) {
        float div = 10f;
        unit.health = Mathf.Clamp((int)(unit.health + healAmtPerSec / div), -10, unit.maxHealth);
        yield return new WaitForSeconds(1f / div);
        
        //  checks if still healing
        if(c.IsTouching(touchingCol)) {
            healer = StartCoroutine(heal(unit, touchingCol, effect));
        }
        else {
            healer = null;
            effect.GetComponent<ParticleSystem>().Stop();
        }
    }

    public override void aoeEffect(GameObject effected) {
        //  does nothing
    }
    public override void aoeLeaveEffect(GameObject effected) {
        //  does nothing 
    }
}
