using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LobbedInstance : MonoBehaviour {
    CircleCollider2D c;
    float maxRad;

    TickDamager td;

    [SerializeField] Weapon reference;
    [SerializeField] Buyable.buyableTitle damageDealer;
    [SerializeField] float tickBtwTime;
    [SerializeField] float tickSlowAmt;
    [SerializeField] int tickEndAfterTicks;
    [SerializeField] GameObject effect;

    private void OnTriggerEnter2D(Collider2D col) {
        //  add tick damage
        if(col.gameObject.tag == "Monster") {
            td.addTick(col.gameObject, dealDamage, damageDealer, tickBtwTime, tickSlowAmt, null);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  remove tick damage
        if(col.gameObject.tag == "Monster") {
            td.removeTick(col.gameObject);
        }
    }

    public void dealDamage(GameObject triggerer) {
        if(triggerer == null || triggerer.GetComponent<Mortal>() == null || reference == null)
            return;
        //  apply buffs to the damage
        int realDmg = (int)(reference.damage * GameInfo.getHelperStats().helperWeaponDamageBuff);

        //  removes the obj from the ticking pool if the unit is going to die this time
        if(triggerer.GetComponent<Mortal>().health <= realDmg)
            td.removeTick(triggerer);

        if(triggerer == null || triggerer.GetComponent<Mortal>() == null || reference == null)
            return;
        triggerer.GetComponent<Mortal>().takeDamage(realDmg, 0, transform.position, false, 0.0f, false);
    }

    public void lob(float airTime) {
        //  sets up variables
        c = GetComponent<CircleCollider2D>();
        maxRad = c.radius;
        td = FindObjectOfType<TickDamager>();

        //  starts effect
        StartCoroutine(lobEffect(airTime));
    }


    IEnumerator lobEffect(float airTime) {
        c.radius = 0f;

        //  wait until the object lands
        yield return new WaitForSeconds(airTime);

        FindObjectOfType<LayerSorter>().requestNewSortingLayer(c, GetComponent<SpriteRenderer>());
        GetComponentInChildren<ParticleSystemRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        //  deal damage
        expandArea();

        //  start playing effect
        effect.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(reference.groundedTime);
        //  end effect / cleanup
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(c, GetComponent<SpriteRenderer>());
        shrinkArea();
        effect.GetComponent<ParticleSystem>().Stop();
        transform.DOScale(0f, .15f);
        yield return new WaitForSeconds(tickEndAfterTicks * tickBtwTime);
        yield return new WaitForSeconds(.1f);
        Destroy(gameObject);
    }

    void expandArea() {
        DOTween.To(() => c.radius, x => c.radius = x, maxRad, .15f);
    }
    void shrinkArea() {
        DOTween.To(() => c.radius, x => c.radius = x, 0, .15f);
    }
}
