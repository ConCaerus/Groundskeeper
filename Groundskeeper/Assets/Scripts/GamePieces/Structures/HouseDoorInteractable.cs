using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoorInteractable : Interactable {

    public bool isTheEnd = false;
    GameBoard gb;


    private void Start() {
        gb = FindObjectOfType<GameBoard>();
    }


    public override void interact() {
        StartCoroutine(intAnim());

        //  achievement shit
        if(GameInfo.getNightCount() == 0)
            FindObjectOfType<SteamManager>().unlockAchievement(SteamManager.achievements.Live);
        else if(GameInfo.getNightCount() == 9)
            FindObjectOfType<SteamManager>().unlockAchievement(SteamManager.achievements.LiveMore);
    }

    public override void deinteract() {
        //  nothing 
    }
    public override bool canInteract() {
        return isTheEnd;
    }
    public override void anim(bool b) {
    }

    IEnumerator intAnim() {
        FindObjectOfType<CameraMovement>().enabled = false;
        var pt = GameObject.FindGameObjectWithTag("Player").transform.position;
        Camera.main.transform.DOMove(new Vector3(pt.x, pt.y, Camera.main.transform.position.z), .25f);
        DOTween.To(() => Camera.main.orthographicSize, x => Camera.main.orthographicSize = x, Camera.main.orthographicSize / 2f, .25f);
        yield return new WaitForSeconds(.5f);
        FindObjectOfType<TransitionCanvas>().loadScene("House");
    }
}
