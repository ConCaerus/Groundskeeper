using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Interactable : MonoBehaviour {
    InputMaster controls;

    [SerializeField] protected bool touching = false, interacting = false;

    [SerializeField] bool toggleMovement = true;
    [SerializeField] public GameObject targetOffsetPosition;

    [SerializeField] AudioClip interactSound, deinteractSound;
    AudioManager am;


    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            touching = true;
            if(canInteract()) {
                if(targetOffsetPosition != null)
                    FindObjectOfType<InteractHelperCanvas>().show(targetOffsetPosition.transform.position);
                anim(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            touching = false;
            FindObjectOfType<InteractHelperCanvas>().hide();
            anim(false);
        }
    }


    private void Awake() {
        controls = new InputMaster();
        controls.Player.Interact.canceled += ctx => toggleInteract();
        am = FindObjectOfType<AudioManager>();
    }


    void toggleInteract() {
        if(interacting)
            tryDeinteract();
        else
            tryInteract();
    }

    public void tryInteract() {
        if(canInteract() && touching) {
            interacting = true;
            am.playSound(interactSound, transform.position);
            interact();

            if(toggleMovement && targetOffsetPosition != null) {
                if(FindObjectOfType<PlayerInstance>() != null)
                    FindObjectOfType<PlayerInstance>().setCanMove(false);
                if(FindObjectOfType<PlayerHouseInstance>() != null)
                    FindObjectOfType<PlayerHouseInstance>().setCanMove(false);
            }
            FindObjectOfType<InteractHelperCanvas>().hardHide();
        }
    }

    public void tryDeinteract() {
        if(canInteract()) {
            interacting = false;
            am.playSound(deinteractSound, transform.position);
            deinteract();

            if(toggleMovement && targetOffsetPosition != null) {
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
    public abstract void anim(bool b);  //  b = true: player is starting the can interact animation. b = false: player is leaving



    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
}
