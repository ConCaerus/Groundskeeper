using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoorInteractable : Interactable {

    public bool isTheEnd = false;

    public override void interact() {
        FindObjectOfType<TransitionCanvas>().loadScene("House");
    }

    public override void deinteract() {
        //  nothing 
    }
    public override bool canInteract() {
        return isTheEnd;
    }
    public override void anim(bool b) {
    }
}
