using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoorInteractable : Interactable {

    public bool isTheEnd = false;

    public override void interact() {
        FindObjectOfType<TransitionCanvas>().loadScene("House");
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
            foreach(var i in FindObjectsOfType<MonsterInstance>())
                i.die();
    }

    public override void deinteract() {
        //  nothing 
    }

    public override bool canInteract() {
        return isTheEnd;
    }
}
