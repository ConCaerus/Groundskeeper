using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class PregameBuyableButton : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject newDot;


    private void Awake() {
        newDot.SetActive(false);
    }

    public void manageNewDot() {
        bool hasNew = false;
        foreach(var i in FindObjectOfType<BuyableLibrary>().getUnlockedBuyablesOfType(getType(), true)) {
            if(FindObjectOfType<BuyableLibrary>().hasPlayerSeenBuyable(i.GetComponent<Buyable>().title)) {
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
        return text.text == "Helpers" ? Buyable.buyType.Helper : text.text == "Defences" ? Buyable.buyType.Defence : Buyable.buyType.Structure;
    }
}
