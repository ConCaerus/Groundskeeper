using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Helper : Attacker {
    public enum helperType {
        None, Attack, Repair, Heal
    }

    public float speed;
    public int helpAmount;
    public helperType helpType;
    public Sprite sprite;
    public Vector2 startingPos { get; private set; }
    protected HelperStats hStats;


    protected void helperInit() {
        startingPos = transform.position;
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponents<Collider2D>()[0].isTrigger ? GetComponents<Collider2D>()[1] : GetComponents<Collider2D>()[0], spriteObj.GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        FindObjectOfType<GameBoard>().helpers.Add(this);
        hStats = GameInfo.getHelperStats();

        //  apply health buff
        maxHealth = (int)(maxHealth * hStats.helperWeaponHealthBuff);
        health = maxHealth;
        healthBar.setParent(gameObject);
    }
}
