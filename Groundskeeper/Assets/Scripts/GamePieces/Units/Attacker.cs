using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacker : Movement {
    Coroutine cooldown;
    protected bool canAttack = true;


    public abstract int getDamage();
    public abstract float getAttackCoolDown();
    public abstract float getKnockback();

    public void attack(GameObject target, bool cooldown) {
        if(canAttack && target.gameObject.GetComponent<Mortal>() != null) {
            target.gameObject.GetComponent<Mortal>().takeDamage(getDamage(), getKnockback(), transform.position, true);
            if(cooldown)
                startCooldown();
        }
    }

    public void startCooldown() {
        if(cooldown != null)
            cooldown = null;
        cooldown = StartCoroutine(cooldownTimer());
    }

    IEnumerator cooldownTimer() {
        canAttack = false;
        yield return new WaitForSeconds(getAttackCoolDown());
        canAttack = true;
        cooldown = null;
    }
}
