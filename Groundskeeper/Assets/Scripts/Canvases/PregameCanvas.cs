using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class PregameCanvas : MonoBehaviour {
    [SerializeField] GameObject upper, lower;
    [SerializeField] TextMeshProUGUI coinText;


    private void Start() {
        GameInfo.addCoins(100);
        coinText.text = GameInfo.coins.ToString() + "c";
    }

    private void Update() {
        if(EventSystem.current.IsPointerOverGameObject()) {
            FindObjectOfType<PlacementGrid>().clear();
        }
    }

    public bool mouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public bool updateCoins(int spent) {
        if(GameInfo.coins < spent) {
            coinText.DOKill();
            coinText.color = Color.red;
            coinText.DOColor(Color.white, .5f);
            return false;
        }
        GameInfo.addCoins(-spent);
        coinText.text = GameInfo.coins.ToString() + "c";
        return true;
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

    public void helper() {
        //  switch to setting helpers
        FindObjectOfType<PlacementGrid>().state = FindObjectOfType<PlacementGrid>().state == 1 ? 0 : 1;
    }

    public void defence() {
        //  switch to setting defences
        FindObjectOfType<PlacementGrid>().state = FindObjectOfType<PlacementGrid>().state == 2 ? 0 : 2;
    }
}
