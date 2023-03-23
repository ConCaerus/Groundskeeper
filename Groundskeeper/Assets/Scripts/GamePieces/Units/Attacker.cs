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
        if(target.gameObject.GetComponent<Mortal>() != null) {
            //  perform special tasks based on what got attacked
            //  House
            if(target == FindObjectOfType<HouseInstance>().gameObject)
                guc.showHouseHealth();
            //  Voodoo Doll
            else if(target.gameObject.GetComponent<VoodooDollInstance>() != null && getDamage() > 0.0f) {
                var mod = target.gameObject.GetComponent<VoodooDollInstance>().getMirroredDamageMod();
                takeDamage((int)Mathf.Clamp(getDamage() * mod, 1, getDamage()), 0.0f, target.transform.position, false, false);
                specialEffectOnAttack(gameObject);

                //  checks if this fucker died to the doll
                if(checkForDeath())
                    return; //  probably not needed but still (cause check for death calls die() with probably has enabled = false
            }

            //  deal damage
            if(getDamage() > 0.0f) {
                target.gameObject.GetComponent<Mortal>().takeDamage(getDamage(), getKnockback(), transform.position, true);
                specialEffectOnAttack(target);
            }
            //  start cooldown
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

    public bool getCanAttack() {
        return canAttack;
    }

    public void setCanAttack(bool b) {
        canAttack = b;
    }
}
