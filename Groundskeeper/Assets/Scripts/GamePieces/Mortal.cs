using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Mortal : MonoBehaviour {
    [SerializeField] public int maxHealth;
    [SerializeField] int unitHealth;
    [SerializeField] AudioClip hurtSound;
    public int health {
        get {
            return unitHealth;
        }

        set {
            unitHealth = value;
            if(healthBar != null)
                healthBar.updateBar();
        }
    }
    protected bool invincible = false;

    public HealthBar healthBar = null;

    public abstract void die();

    public abstract void customHitLogic(float knockback, Vector2 origin, bool stun = true);

    public void takeDamage(int dmg, float knockback, Vector2 origin, bool activateInvinc = true, bool stun = true) {
        if(invincible)
            return;
        if(activateInvinc)
            StartCoroutine(invincTimer());
        if(hurtSound != null)
            FindObjectOfType<AudioManager>().playSound(hurtSound, transform.position);

        health -= dmg;
        //  check for death
        if(health <= 0) {
            die();
            return;
        }

        customHitLogic(knockback, origin, stun);
    }


    //  mainly so that things don't get hit twice per frame
    //  also used so that the player can't get hit by two enemies at the same time.
    IEnumerator invincTimer() {
        invincible = true;
        yield return new WaitForSeconds(.1f);
        invincible = false;
    }
}
