using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthInteractable : Interactable {
    public override bool canInteract() {
        return true;
    }


    public override void interact() {
        FindObjectOfType<BuyTreeCanvas>().show();
    }
    public override void deinteract() {
        FindObjectOfType<BuyTreeCanvas>().hide();
    }
    public override void anim(bool b) {
        GetComponent<Animator>().SetBool("isOpen", b);
    }
}
