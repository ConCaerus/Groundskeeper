using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class PregameCanvas : MonoBehaviour {
    [SerializeField] GameObject upper, lower, lowerGenericButtonHolder;
    [SerializeField] public GameObject startButton;
    [SerializeField] public TextMeshProUGUI soulsText;  //  referenced in PlacementGrid place()
    [SerializeField] CircularSlider timer;
    [SerializeField] GameObject helpText;
    float prepTime = 2 * 60f; // 2 mins

    [SerializeField] AudioClip gameMusic;

    [SerializeField] GameObject freeGamepadCursor, gameGamepadCursor;

    PlacementGrid pg;
    PlayerWeaponInstance pwi;


    private void Start() {
        pg = FindObjectOfType<PlacementGrid>();
        pwi = FindObjectOfType<PlayerWeaponInstance>();
        setup();

        if(GameInfo.getNightCount() > 0)
            startTimer();
        else
            helpText.SetActive(false);

        FindObjectOfType<MouseManager>().addOnInputChangeFunc(changeHelpText);
    }

    public void startTimer() {
        timer.setValue(1.0f);
        timer.doValue(0.0f, prepTime, true, delegate { ready(); });

        timer.setColor(Color.white);
        timer.doColor(Color.red, prepTime);
    }
    public void setup() {
        gameGamepadCursor.SetActive(false);
        freeGamepadCursor.SetActive(true);
        FindObjectOfType<GameUICanvas>().hide();
        transform.GetChild(0).gameObject.SetActive(true);
        soulsText.text = GameInfo.getSouls(false).ToString("0.0") + "s";
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
            pg.clear();
        }
        soulsText.text = GameInfo.getSouls(false).ToString("0.0") + "s";

        pwi.canAttackG = !(mouseOverUI() || pg.placing);
    }

    void changeHelpText(bool usingKeyboard) {
        if(usingKeyboard)
            helpText.GetComponent<TextMeshProUGUI>().text = "M1-Place\nM2-Sell";
        else
            helpText.GetComponent<TextMeshProUGUI>().text = "Y-Place\nB-Sell";
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
        startButton.GetComponent<Button>().interactable = false;
        FindObjectOfType<GameBoard>().saveBoard();
        FindObjectOfType<GameUICanvas>().show();
        FindObjectOfType<MonsterSpawner>().startNewWave();
        pg.end();
        pwi.canAttackG = true;
        FindObjectOfType<PlayerInstance>().setCanAttack(true);
        if(gameMusic != null)
            FindObjectOfType<AudioManager>().playMusic(gameMusic, true);
        FindObjectOfType<InfoBox>().gameObject.SetActive(false);

        //  switches controller canvases
        freeGamepadCursor.SetActive(false);
        gameGamepadCursor.SetActive(true);

        hide();
    }

    private void OnDisable() {
        if(FindObjectOfType<MouseManager>() != null)
            FindObjectOfType<MouseManager>().removeOnInputChangeFunc(changeHelpText);
    }
}
