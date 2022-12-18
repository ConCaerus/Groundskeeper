using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PregameCanvas : MonoBehaviour {
    [SerializeField] GameObject upper, lower, lowerGenericButtonHolder;
    [SerializeField] public TextMeshProUGUI soulsText;  //  referenced in PlacementGrid place()
    [SerializeField] CircularSlider timer;
    float prepTime = 2 * 60; // 2 mins

    [SerializeField] AudioClip gameMusic;


    private void Start() {
        FindObjectOfType<GameUICanvas>().hide();
        transform.GetChild(0).gameObject.SetActive(true);
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
        StartCoroutine(startAfterTime());

        //  player doesn't have anything unlocked, so remove the lower bar
        if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Helper) == 0 && FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Defence) == 0 && FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Structure) == 0) {
            lower.SetActive(false);
        }
        else {
            //  remove buttons that don't have shit unlocked
            foreach(var i in lowerGenericButtonHolder.GetComponentsInChildren<Button>()) {
                if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Helper) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Helper) == 0)
                        i.gameObject.SetActive(false);
                }
                else if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Defence) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Defence) == 0)
                        i.gameObject.SetActive(false);
                }
                else if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Structure) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Structure) == 0)
                        i.gameObject.SetActive(false);
                }
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

    public bool prepping() {
        return timer.value > 0f;
    }


    void hide() {
        float t = 2f;
        float amt = 500f;
        if(lower != null)
            lower.GetComponent<RectTransform>().DOAnchorPosY(-amt, t);
        upper.GetComponent<RectTransform>().DOAnchorPosY(amt, t);
        Destroy(gameObject, t);
    }

    //  buttons
    public void ready() {
        FindObjectOfType<GameBoard>().saveBoard();
        FindObjectOfType<GameUICanvas>().show();
        FindObjectOfType<MonsterSpawner>().spawning = true;
        FindObjectOfType<PlacementGrid>().end();
        FindObjectOfType<PlayerWeaponInstance>().canAttack = true;
        FindObjectOfType<AudioManager>().playMusic(gameMusic, true);
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
