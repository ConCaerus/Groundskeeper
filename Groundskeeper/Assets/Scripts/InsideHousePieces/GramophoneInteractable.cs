using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GramophoneInteractable : Interactable {
    [SerializeField] AudioClip song;
    float initVol;

    private void Start() {
        initVol = GetComponent<AudioSource>().volume;
    }

    public override bool canInteract() {
        return touching;
    }

    public override void interact() {
        GetComponent<AudioSource>().volume = initVol;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(song);
    }
    public override void deinteract() {
        DOTween.To(() => GetComponent<AudioSource>().volume, x => GetComponent<AudioSource>().volume = x, 0f, .15f);
    }
    public override void anim(bool b) {
    }
}
