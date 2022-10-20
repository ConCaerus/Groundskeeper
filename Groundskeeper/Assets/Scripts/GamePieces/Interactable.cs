using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    InputMaster controls;

    bool touching = false, interacting = false;

    [SerializeField] bool toggleMovement = true;
    [SerializeField] float helpOffset = 1.0f;

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            touching = true;
            if(canInteract()) {
                var pos = transform.position;
                pos.y += helpOffset;
                FindObjectOfType<InteractHelperCanvas>().show(pos);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            touching = false;
            FindObjectOfType<InteractHelperCanvas>().hide();
        }
    }


    private void Awake() {
        controls = new InputMaster();
        controls.Player.Interact.performed += ctx => toggleInteract();
    }


    void toggleInteract() {
        if(interacting)
            tryDeinteract();
        else
            tryInteract();
    }

    protected void tryInteract() {
        if(canInteract() && touching) {
            interacting = true;
            interact();
            if(toggleMovement) {
                if(FindObjectOfType<PlayerInstance>() != null)
                    FindObjectOfType<PlayerInstance>().setCanMove(false);
                if(FindObjectOfType<PlayerHouseInstance>() != null)
                    FindObjectOfType<PlayerHouseInstance>().setCanMove(false);
            }
            FindObjectOfType<InteractHelperCanvas>().hardHide();
        }
    }

    protected void tryDeinteract() {
        if(canInteract()) {
            interacting = false;
            deinteract();
            if(toggleMovement) {
                if(FindObjectOfType<PlayerInstance>() != null)
                    FindObjectOfType<PlayerInstance>().setCanMove(true);
                if(FindObjectOfType<PlayerHouseInstance>() != null)
                    FindObjectOfType<PlayerHouseInstance>().setCanMove(true);
            }
        }
    }


    public abstract void interact();
    public abstract void deinteract();
    public abstract bool canInteract();




    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
}
