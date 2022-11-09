using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PregameCanvas : MonoBehaviour {
    [SerializeField] GameObject upper, lower, lowerGenericButtonHolder;
    [SerializeField] TextMeshProUGUI soulsText;
    [SerializeField] CircularSlider timer;
    float prepTime = 2 * 60; // 3 mins


    private void Start() {
        transform.GetChild(0).gameObject.SetActive(true);
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
        StartCoroutine(startAfterTime());

        //  remove buttons that don't have shit unlocked
        foreach(var i in lowerGenericButtonHolder.GetComponentsInChildren<Button>()) {
            if(i.GetComponentInChildren<TextMeshProUGUI>().text == "Helpers") {
                if(FindObjectOfType<BuyableLibrary>().getHelpers().Count == 0)
                    i.gameObject.SetActive(false);
            }
            else if(i.GetComponentInChildren<TextMeshProUGUI>().text == "Defences") {
                if(FindObjectOfType<BuyableLibrary>().getDefences().Count == 0)
                    i.gameObject.SetActive(false);
            }
            else if(i.GetComponentInChildren<TextMeshProUGUI>().text == "Structures") {
                if(FindObjectOfType<BuyableLibrary>().getStructures().Count == 0)
                    i.gameObject.SetActive(false);
            }
        }
    }

    private void Update() {
        if(EventSystem.current.IsPointerOverGameObject()) {
            FindObjectOfType<PlacementGrid>().clear();
        }
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";

        FindObjectOfType<PlayerWeaponInstance>().canAttack = !(mouseOverUI() || FindObjectOfType<PlacementGrid>().placing);
    }

    public bool mouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public bool updateCoins(int spent) {
        if(GameInfo.getSouls() < spent) {
            soulsText.DOKill();
            soulsText.color = Color.red;
            soulsText.DOColor(Color.white, .5f);
            return false;
        }
        GameInfo.addSouls(-spent);
        return true;
    }

    public bool prepping() {
        return timer.value > 0f;
    }


    void hide() {
        float t = 2f;
        float amt = 500f;
        lower.GetComponent<RectTransform>().DOAnchorPosY(-amt, t);
        upper.GetComponent<RectTransform>().DOAnchorPosY(amt, t);
        Destroy(gameObject, t);
    }

    //  buttons
    public void ready() {
        FindObjectOfType<MonsterSpawner>().spawning = true;
        FindObjectOfType<PlacementGrid>().end();
        FindObjectOfType<PlayerWeaponInstance>().canAttack = true;
        hide();
    }

    public void setPlacementObj(GameObject obj) {
        FindObjectOfType<PlacementGrid>().changePlacing(obj, obj.gameObject == FindObjectOfType<PlacementGrid>().currentObj);
    }

    IEnumerator startAfterTime() {
        float elapsedTime = 0.0f, incTime = .25f;
        timer.setValue(1f);

        while(elapsedTime < prepTime) {
            yield return new WaitForSeconds(incTime);
            elapsedTime += incTime;
            timer.setValue(1 - (elapsedTime / prepTime));
            if(elapsedTime / prepTime > .85f)
                timer.setColor(new Color(elapsedTime / prepTime, 0, 0, 1f));
        }

        yield return new WaitForSeconds(.5f);
        ready();
    }
}
