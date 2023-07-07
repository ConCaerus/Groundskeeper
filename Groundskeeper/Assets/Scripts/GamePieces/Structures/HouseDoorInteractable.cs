using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseDoorInteractable : Interactable {

    public bool isTheEnd = false;
    bool startedEnding = false;
    GameBoard gb;


    private void Start() {
        gb = FindObjectOfType<GameBoard>();
    }


    public override void interact() {
        if(startedEnding)
            return;
        startedEnding = true;
        StartCoroutine(intAnim());

        //  achievement shit
        if(GameInfo.getNightCount() == 0 && FindObjectOfType<SteamHandler>() != null)
            FindObjectOfType<SteamHandler>().unlockAchievement(SteamHandler.achievements.Live);
        else if(GameInfo.getNightCount() == 9 && FindObjectOfType<SteamHandler>() != null)
            FindObjectOfType<SteamHandler>().unlockAchievement(SteamHandler.achievements.LiveMore);

        //  saves the state of the game
        FindObjectOfType<GameBoard>().saveBoard();

        //  sets the current scene to house
        GameInfo.setCurrentScene(GameInfo.SceneType.House);
        //  saves the house's health as what it is
        var stats = GameInfo.getHouseStats();
        stats.houseHealth = FindObjectOfType<HouseInstance>().health;
        GameInfo.setHouseStats(stats);
        GameInfo.healHousePerNight();   //  heals the house before starting the next night
    }

    public override void deinteract() {
        //  nothing 
    }
    public override bool canInteract() {
        return isTheEnd && !startedEnding;
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
