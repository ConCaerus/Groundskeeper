using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Mortal : MonoBehaviour {
    [SerializeField] public int maxHealth;
    [SerializeField] int unitHealth;
    [SerializeField] AudioClip hurtSound;
    GameObject bloodParticle = null;
    [SerializeField] GameObject bloodStain;
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
    public abstract GameObject getBloodParticles();
    public abstract Color getStartingColor();

    public void takeDamage(int dmg, float knockback, Vector2 origin, bool activateInvinc = true, bool stun = true) {
        if(invincible || dmg <= 0.0f)
            return;
        if(activateInvinc)
            StartCoroutine(invincTimer());

        //  flair
        if(hurtSound != null)
            FindObjectOfType<AudioManager>().playSound(hurtSound, transform.position);
        if(getBloodParticles() != null) {
            if(bloodParticle == null)
                bloodParticle = Instantiate(getBloodParticles().gameObject, transform.position, Quaternion.identity, null);
            bloodParticle.transform.position = transform.position;
        }
        if(bloodStain != null)
            Instantiate(bloodStain.gameObject, transform.position, Quaternion.identity, null);

        health -= dmg;
        //  check for death
        if(health <= 0) {
            if(bloodParticle != null) {
                //  change the blood particles to emmit double the normal amount of particles
                var c = Random.Range(bloodParticle.GetComponent<ParticleSystem>().emission.GetBurst(0).minCount,
                        bloodParticle.GetComponent<ParticleSystem>().emission.GetBurst(0).maxCount);
                var t = bloodParticle.GetComponent<ParticleSystem>().emission.GetBurst(0).time;
                var em = bloodParticle.GetComponent<ParticleSystem>().emission;
                em.SetBurst(0, new ParticleSystem.Burst(t, c * 2));
                bloodParticle.GetComponent<ParticleSystem>().Play();
                Destroy(bloodParticle.gameObject, bloodParticle.GetComponent<ParticleSystem>().main.duration + .1f);
            }

            die();  //  this probably has a enabled = false; in there
        }

        if(bloodParticle != null)
            bloodParticle.GetComponent<ParticleSystem>().Play();

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
