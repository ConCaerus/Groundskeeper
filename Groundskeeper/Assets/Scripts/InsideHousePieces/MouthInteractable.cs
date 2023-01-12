using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthInteractable : Interactable {
    float distToOpen = 3f;

    public override bool canInteract() {
        return true;
    }


    private void Update() {
        manageMouth();
    }


    void manageMouth() {
        //  opens the mouth when the player gets close
        GetComponent<Animator>().SetBool("isOpen", Vector2.Distance(FindObjectOfType<PlayerHouseInstance>().transform.position, transform.position) < distToOpen);
    }

    public override void interact() {
        FindObjectOfType<BuyTreeCanvas>().show();
    }
    public override void deinteract() {
        FindObjectOfType<BuyTreeCanvas>().hide();
    }
}
