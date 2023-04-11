using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeadGuyInstance : Interactable {
    [HideInInspector] public float soulsGiven = 250f; //  have this number go up as the nights increase
    [SerializeField] public string title;

    GameUICanvas guc;

    private void Start() {
        FindObjectOfType<GameBoard>().deadGuys.Add(this);
        guc = FindObjectOfType<GameUICanvas>();

        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
    }

    public override bool canInteract() {
        return true;
    }

    public override void interact() {
        //  give souls and destory
        GameInfo.addSouls(soulsGiven, false);
        guc.incSouls(soulsGiven);

        //  dying animation
        transform.DOScale(0.0f, .15f);
        Destroy(gameObject, .151f);
    }

    public override void deinteract() {
        //  does nothing
    }

    public override void anim(bool b) {
    }
}
