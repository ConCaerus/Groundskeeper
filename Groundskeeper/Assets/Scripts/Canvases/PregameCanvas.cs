using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PregameCanvas : MonoBehaviour {
    [SerializeField] GameObject upper, lower, lowerGenericButtonHolder;
    [SerializeField] public GameObject startButton;
    [SerializeField] public TextMeshProUGUI soulsText;  //  referenced in PlacementGrid place()
    [SerializeField] CircularSlider timer;
    float prepTime = 2 * 60; // 2 mins

    [SerializeField] AudioClip gameMusic;


    private void Start() {
        setup();

        if(GameInfo.getNightCount() > 0)
            startTimer();
    }

    public void startTimer() {
        timer.setValue(1.0f);
        timer.doValue(0.0f, prepTime, delegate { ready(); });
        timer.setColor(Color.white);
        timer.doColor(Color.red, prepTime);
    }
    public void setup() {
        FindObjectOfType<GameUICanvas>().hide();
        transform.GetChild(0).gameObject.SetActive(true);
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
        lower.SetActive(true);
        foreach(var i in lowerGenericButtonHolder.GetComponentsInChildren<Button>()) {
            i.gameObject.SetActive(true);
        }


        //  player doesn't have anything unlocked, so remove the lower bar
        if(!FindObjectOfType<BuyableLibrary>().hasUnlockedBuyables(true)) {
            lower.SetActive(false);
        }
        else {
            //  remove buttons that don't have shit unlocked
            foreach(var i in lowerGenericButtonHolder.GetComponentsInChildren<Button>()) {
                if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Helper) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Helper, true) == 0)
                        i.gameObject.SetActive(false);
                }
                if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Defence) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Defence, true) == 0)
                        i.gameObject.SetActive(false);
                }
                if(i.GetComponent<PregameBuyableButton>().getType() == Buyable.buyType.Structure) {
                    i.GetComponent<PregameBuyableButton>().manageNewDot();
                    if(FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Structure, true) == 0)
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


    public void hide() {
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
        FindObjectOfType<MonsterSpawner>().startNewWave();
        FindObjectOfType<PlacementGrid>().end();
        FindObjectOfType<PlayerWeaponInstance>().canAttack = true;
        FindObjectOfType<AudioManager>().playMusic(gameMusic, true);
        hide();
    }

    public void setPlacementObj(GameObject obj) {
        FindObjectOfType<PlacementGrid>().changePlacing(obj, obj.gameObject == FindObjectOfType<PlacementGrid>().currentObj);
    }
}
