using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountainInstance : BuildingInstance {

    [SerializeField] float healAmtPerSec;

    Coroutine healer = null;
    StructureStats sStats;


    private void Start() {
        sStats = GameInfo.getStructureStats();
        FindObjectOfType<GameBoard>().structures.Add(this);
        //  apply health buff
        maxHealth = (int)(maxHealth * sStats.structureHealthBuff);
        health = (int)(health * sStats.structureHealthBuff); //  i don't know if buildings start each night with full health
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(canHeal() && (col.gameObject.tag == "Player" || col.gameObject.tag == "Helper")) {
            startHealing(col.gameObject.GetComponent<MortalUnit>(), col.collider);
        }
    }


    void startHealing(MortalUnit unit, Collider2D touchingCol) {
        if(healer == null) 
            healer = StartCoroutine(heal(unit, touchingCol));
    }

    bool canHeal() {
        return healer == null;
    }

    IEnumerator heal(MortalUnit unit, Collider2D touchingCol) {
        float div = 10f;
        unit.health = Mathf.Clamp((int)(unit.health + healAmtPerSec / div), -10, unit.maxHealth);
        yield return new WaitForSeconds(1f / div);
        
        healer = GetComponent<Collider2D>().IsTouching(touchingCol) ? StartCoroutine(heal(unit, touchingCol)) : null;
    }
}
