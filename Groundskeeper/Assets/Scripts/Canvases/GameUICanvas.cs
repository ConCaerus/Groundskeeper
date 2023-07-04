using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameUICanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI waveCount, soulsText, nightCountText;
    float soulsRecieved = 0;
    [SerializeField] GameObject houseHealthBar;
    bool shownB4 = false;
    [SerializeField] GameObject soulsChanger;

    Coroutine houseHealthShower = null, soulTextCor = null;

    public bool ended = false;


    private void Start() {
        soulsChanger.transform.position = new Vector3(GameInfo.getSouls(true), 0.0f);
        soulsRecieved = GameInfo.getSouls(true);
    }


    public void show() {
        transform.GetChild(0).gameObject.SetActive(true);
        if(!shownB4) {
            houseHealthBar.GetComponentInChildren<HealthBar>().setParent(FindObjectOfType<HouseInstance>().gameObject);
            nightCountText.text = "Night: " + (GameInfo.getNightCount() + 1).ToString();
            soulsText.color = Color.clear;
            shownB4 = true;
        }
    }
    public void hide() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void updateCount() {
        if(GameInfo.wave > GameInfo.wavesPerNight())
            return;
        waveCount.text = GameInfo.wave.ToString() + "/" + GameInfo.wavesPerNight().ToString();
    }
    public void incSouls(float s) {
        if(soulTextCor != null)
            StopCoroutine(soulTextCor);
        soulTextCor = StartCoroutine(soulTextShower(s));
    }

    public void endGame() {
        ended = true;
        waveCount.text = "Completed";
        FindObjectOfType<WaveWarnerRose>().hide();
    }

    public void showHouseHealth() {
        if(houseHealthShower != null) {
            StopCoroutine(houseHealthShower);
            houseHealthShower = StartCoroutine(houseHealthHider(false));
        }
        else
            houseHealthShower = StartCoroutine(houseHealthHider(true));
    }

    IEnumerator houseHealthHider(bool show) {
        if(show) {
            houseHealthBar.GetComponent<RectTransform>().DOComplete();
            houseHealthBar.GetComponent<RectTransform>().DOLocalMoveY(houseHealthBar.GetComponent<RectTransform>().localPosition.y - 70f, .25f);
        }
        yield return new WaitForSeconds(3f);
        houseHealthBar.GetComponent<RectTransform>().DOLocalMoveY(houseHealthBar.GetComponent<RectTransform>().localPosition.y + 70f, .25f);
        houseHealthShower = null;
    }

    IEnumerator soulTextShower(float souls) {
        soulsText.DOKill();
        soulsText.DOColor(Color.white, .15f);
        soulsRecieved = GameInfo.getSouls(false);

        soulsChanger.transform.DOKill();

        soulsChanger.transform.DOMoveX(GameInfo.getSouls(false), .35f).OnUpdate(() => {
            soulsText.text = soulsChanger.transform.position.x.ToString("0.0") + "s";
        });

        yield return new WaitForSeconds(2f);

        soulsText.DOKill();
        soulsText.DOColor(Color.clear, .5f);
        soulTextCor = null;
    }
}
