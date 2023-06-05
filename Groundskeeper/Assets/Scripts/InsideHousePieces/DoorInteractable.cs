using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable {

    public override bool canInteract() {
        return FindObjectOfType<BedInteractable>().hasSlept;
    }

    public override void interact() {
        StartCoroutine(intAnim());
    }
    public override void deinteract() {
    }
    public override void anim(bool b) {
    }

    IEnumerator intAnim() {
        FindObjectOfType<CameraMovement>().enabled = false;
        var pt = GameObject.FindGameObjectWithTag("Player").transform.position;
        Camera.main.transform.DOMove(new Vector3(pt.x, pt.y, Camera.main.transform.position.z), .25f);
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, Camera.main.orthographicSize / 2f, .25f);
        yield return new WaitForSeconds(.5f);
        GameInfo.addNights(1);
        FindObjectOfType<TransitionCanvas>().loadScene("Game");
    }
}
