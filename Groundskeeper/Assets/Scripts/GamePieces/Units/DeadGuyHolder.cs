using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGuyHolder : Interactable {
    GameBoard gb;
    Transform player;
    CompositeCollider2D cc;

    private void Start() {
        gb = FindObjectOfType<GameBoard>();
        player = FindObjectOfType<PlayerInstance>().transform;
        cc = GetComponent<CompositeCollider2D>();
    }

    public override bool canInteract() {
        targetOffsetPosition = gb.deadGuys.FindClosest(player.position).gameObject.transform.GetChild(0).gameObject;
        return true;
    }

    public override void deinteract() {
        //  does nothing
    }

    public override void interact() {
        gb.deadGuys.FindClosest(player.position).GetComponent<DeadGuyInstance>().interactedWith();
    }

    public void updateCollider() {
        cc.GenerateGeometry();
    }

    public override void anim(bool b) {
    }
}
