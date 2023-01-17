using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthInteractable : Interactable {
    public override bool canInteract() {
        return true;
    }

    void manageMouth(bool b) {
        //  opens the mouth when the player gets close
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
