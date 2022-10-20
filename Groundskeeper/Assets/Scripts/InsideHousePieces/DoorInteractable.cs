using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable {

    public override bool canInteract() {
        return FindObjectOfType<BedInteractable>().hasSlept;
    }

    public override void interact() {
        GameInfo.addNights(1);
        FindObjectOfType<TransitionCanvas>().loadScene("Game");
    }

    public override void deinteract() {
    }
}
