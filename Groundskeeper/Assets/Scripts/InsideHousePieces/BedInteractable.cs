using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedInteractable : Interactable {
    bool sleeping = false;
    public bool hasSlept { get; private set; } = false;

    public override bool canInteract() {
        return !sleeping && !hasSlept;
    }

    public override void interact() {
        sleeping = true;
        FindObjectOfType<SleepingCanvas>().goToSleep();
    }
    public override void deinteract() {
    }
    public override void anim(bool b) {
    }


    public void finshedSleeping() {
        sleeping = false;
        tryDeinteract();
        hasSlept = true;
    }
}
