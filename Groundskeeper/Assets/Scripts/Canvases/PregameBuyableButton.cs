using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PregameBuyableButton : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject newDot;
    GameBoard gb;

    BuyableLibrary bl;


    private void Awake() {
        newDot.SetActive(false);
        bl = FindObjectOfType<BuyableLibrary>();
        gb = FindObjectOfType<GameBoard>();
    }

    public void manageNewDot() {
        bool hasNew = false;
        foreach(var i in bl.getUnlockedBuyablesOfType(getType(), true)) {
            if(bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == -1 || (bl.getNightBuyableWasSeen(i.GetComponent<Buyable>().title) == GameInfo.getNightCount() && !gb.hasBuyableOnBoard(i.GetComponent<Buyable>().title))) {
                hasNew = true;
                break;
            }
        }

        
        if(hasNew && !newDot.activeInHierarchy)
            StartCoroutine(FindObjectOfType<BuyableButtonSpawner>().setupDot(transform, newDot.transform, null));
        else if(!hasNew && newDot.activeInHierarchy)
            newDot.SetActive(false);
    }

    public Buyable.buyType getType() {
        return text.text == "Helpers" ? Buyable.buyType.Helper : text.text == "Defenses" ? Buyable.buyType.Defense : Buyable.buyType.Structure;
    }
}
