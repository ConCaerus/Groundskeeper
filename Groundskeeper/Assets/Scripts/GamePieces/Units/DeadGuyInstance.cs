using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGuyInstance : Interactable {
    [HideInInspector] public int soulsGiven = 250; //  have this number go up as the nights increase
    [SerializeField] public string title;

    GameUICanvas guc;

    private void Start() {
        FindObjectOfType<GameBoard>().deadGuys.Add(this);
        guc = FindObjectOfType<GameUICanvas>();
    }

    public override bool canInteract() {
        return true;
    }

    public override void interact() {
        //  give souls and destory
        GameInfo.addSouls(soulsGiven, false);
        guc.incSouls(soulsGiven);
        Destroy(gameObject);
    }

    public override void deinteract() {
        //  does nothing
    }

    public override void anim(bool b) {
    }
}
