using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacker : Movement {
    Coroutine cooldown;
    protected bool canAttack = true;


    public abstract int getDamage();
    public abstract float getAttackCoolDown();
    public abstract float getKnockback();
    public abstract void specialEffectOnAttack(GameObject defender);
    //  NOTE: function getBloodParticles moved over to Mortal class

    public void attack(GameObject target, bool cooldown) {
        if(canAttack && target.gameObject.GetComponent<Mortal>() != null) {
            if(target == FindObjectOfType<HouseInstance>().gameObject)
                FindObjectOfType<GameUICanvas>().showHouseHealth();
            target.gameObject.GetComponent<Mortal>().takeDamage(getDamage(), getKnockback(), transform.position, true);
            specialEffectOnAttack(target);
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
