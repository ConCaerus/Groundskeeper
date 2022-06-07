using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class NightOverCanvas : MonoBehaviour {
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI coinText, killedText;

    private void Awake() {
        background.SetActive(false);
    }


    public void show() {
        coinText.text = "Pay: " + GameInfo.calcCoins().ToString() + "g";
        killedText.text = "Monsters Killed: " + GameInfo.monstersKilled.ToString();

        background.transform.localScale = Vector3.zero;
        background.SetActive(true);
        background.transform.DOScale(1.0f, .5f);
    }


    //  buttons
    public void nextNight() {
        GameInfo.night++;
        FindObjectOfType<TransitionCanvas>().loadScene("game");
    }
}
