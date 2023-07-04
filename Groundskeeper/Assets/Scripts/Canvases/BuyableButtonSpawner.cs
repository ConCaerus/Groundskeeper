using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Reflection;
using UnityEngine.EventSystems;

public class BuyableButtonSpawner : MonoBehaviour {
    [SerializeField] GameObject buyableButton;
    [SerializeField] Transform holder;

    List<GameObject> buttons = new List<GameObject>();

    List<Coroutine> buttonCoroutines = new List<Coroutine>();

    [SerializeField] GameObject readyButton;

    int prevGenre = -1;
    BuyableLibrary bl;
    PlacementGrid pg;
    GameBoard gb;

    MouseManager mm;

    private void Awake() {
        bl = FindObjectOfType<BuyableLibrary>();
        pg = FindObjectOfType<PlacementGrid>();
        mm = FindObjectOfType<MouseManager>();
        gb = FindObjectOfType<GameBoard>();
    }

    public void switchGenre(int index) {
        if(prevGenre == index)
            return;
        foreach(var i in buttonCoroutines)
            StopCoroutine(i);
        buttonCoroutines.Clear();
        prevGenre = index;
        pg.placing = false;
        List<GameObject> buyables = index == 0 ? bl.getUnlockedBuyablesOfType(Buyable.buyType.Helper, true) :
            index == 1 ? bl.getUnlockedBuyablesOfType(Buyable.buyType.Defense, true) :
            bl.getUnlockedBuyablesOfType(Buyable.buyType.Structure, true);

        foreach(var i in buttons)
            Destroy(i.gameObject);
        buttons.Clear();

        foreach(var i in buyables) {
            var obj = Instantiate(buyableButton.gameObject, holder);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().titleToText();
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().cost.ToString("0.0") + "s";
            obj.GetComponent<InfoableImage>().info = i.GetComponent<Buyable>().title.ToString() + ":\n" + i.GetComponent<Buyable>().description;
            if(bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == -1 || i.GetComponent<Buyable>().cost == 0f || (bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == GameInfo.getNightCount() && !gb.hasBuyableOnBoard(i.GetComponent<Buyable>().title))) {
                StartCoroutine(setupDot(obj.transform, obj.transform.GetChild(2).transform, i.GetComponent<Buyable>()));
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Free!";
            }
            else
                obj.transform.GetChild(2).gameObject.SetActive(false);
            obj.GetComponent<Button>().onClick.AddListener(delegate { 
                pg.changePlacing(i.gameObject, i.gameObject == pg.currentObj);
            });
            buttons.Add(obj.gameObject);
        }
    }

    public void updateBuyableButtons() {
        StartCoroutine(waitToUpdateDot());
    }

    public IEnumerator setupDot(Transform parent, Transform dot, Buyable b) {
        dot.gameObject.SetActive(true);
        dot.GetComponent<RectTransform>().localScale = Vector3.zero;
        while(parent.GetComponent<RectTransform>().rect.height == 0)
            yield return new WaitForEndOfFrame();

        var x = parent.GetComponent<RectTransform>().rect.width / 2.0f;
        var y = parent.GetComponent<RectTransform>().rect.height / 2.0f;
        dot.GetComponent<RectTransform>().localPosition = new Vector3(x, y);

        //  animate showing the dot
        dot.DOScale(1.0f, .15f);
        yield return new WaitForSeconds(.15f);
        var animCo = StartCoroutine(animateDot(dot.gameObject));
        if(dot.GetComponentInParent<PregameBuyableButton>() == null)
            buttonCoroutines.Add(animCo);
        dot.GetComponentInParent<Button>().onClick.AddListener(delegate {
            if(b != null) {
                dot.gameObject.SetActive(false);
            }
            if(dot.GetComponentInParent<PregameBuyableButton>() == null) {
                foreach(var i in FindObjectsOfType<PregameBuyableButton>()) {
                    if(i.getType() == b.bType) {
                        i.manageNewDot();
                        break;
                    }
                }
            }
        });
    }

    public IEnumerator animateDot(GameObject dot) {
        float animTime = .25f;
        //  grow
        if(dot != null)
            dot.transform.DOScale(1.25f, animTime);
        yield return new WaitForSeconds(animTime);

        //  shrink
        if(dot != null)
            dot.transform.DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);
        StartCoroutine(animateDot(dot));
    }

    IEnumerator waitToUpdateDot() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        List<GameObject> buyables = prevGenre == 0 ? bl.getUnlockedBuyablesOfType(Buyable.buyType.Helper, true) :
            prevGenre == 1 ? bl.getUnlockedBuyablesOfType(Buyable.buyType.Defense, true) :
            bl.getUnlockedBuyablesOfType(Buyable.buyType.Structure, true);

        foreach(var i in buttons)
            Destroy(i.gameObject);
        buttons.Clear();

        foreach(var i in buyables) {
            var obj = Instantiate(buyableButton.gameObject, holder);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().title.ToString();
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().cost.ToString("0.0") + "s";
            obj.GetComponent<InfoableImage>().info = i.GetComponent<Buyable>().title.ToString() + ":\n" + i.GetComponent<Buyable>().description;
            if(bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == -1 || i.GetComponent<Buyable>().cost == 0f || (bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == GameInfo.getNightCount() && !gb.hasBuyableOnBoard(i.GetComponent<Buyable>().title))) {
                StartCoroutine(setupDot(obj.transform, obj.transform.GetChild(2).transform, i.GetComponent<Buyable>()));
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Free!";
            }
            else
                obj.transform.GetChild(2).gameObject.SetActive(false);
            obj.GetComponent<Button>().onClick.AddListener(delegate { pg.changePlacing(i.gameObject, i.gameObject == pg.currentObj); });
            buttons.Add(obj.gameObject);
        }
    }
}
