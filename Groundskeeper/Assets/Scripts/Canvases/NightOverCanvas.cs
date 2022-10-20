using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NightOverCanvas : MonoBehaviour {
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI killedText;

    private void Awake() {
        background.SetActive(false);
    }


    public void show() {
        killedText.text = "Monsters Killed: " + GameInfo.monstersKilled.ToString();

        background.transform.localScale = Vector3.zero;
        background.SetActive(true);
        background.transform.DOScale(1.0f, .5f);

        FindObjectOfType<GameBoard>().saveBoard();
    }

    public void hide() {
        background.transform.DOScale(0.0f, .15f);
        FindObjectOfType<HouseDoorInteractable>().isTheEnd = true;
    }


    //  buttons
    public void nextNight() {
        GameInfo.addNights(1);
        TimeInfo.saveTime();
        FindObjectOfType<TransitionCanvas>().loadScene("game");
    }
}
