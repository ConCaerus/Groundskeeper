using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionInteractable : Interactable {

    public override bool canInteract() {
        return true;
    }

    public override void interact() {
        FindObjectOfType<WeaponSelectionCanvas>().tryShow();
    }

    public override void deinteract() {
        FindObjectOfType<WeaponSelectionCanvas>().tryClose();
    }

    public override void anim(bool b) {
    }
}
