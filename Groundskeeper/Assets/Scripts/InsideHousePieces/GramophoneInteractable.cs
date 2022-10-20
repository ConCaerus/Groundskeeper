using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GramophoneInteractable : Interactable {
    [SerializeField] AudioClip song;

    public override bool canInteract() {
        return true;
    }

    public override void interact() {
        GetComponent<AudioSource>().PlayOneShot(song);
    }

    public override void deinteract() {
        GetComponent<AudioSource>().Stop();
    }
}
