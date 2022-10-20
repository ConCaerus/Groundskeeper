using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableLibrary : MonoBehaviour {
    [SerializeField] GameObject[] buyables;


    //  only returns the unlocked shit
    public List<GameObject> getHelpers() {
        var temp = new List<GameObject>();
        for(int i = 0; i < buyables.Length; i++) {
            if(GameInfo.isBuyableUnlocked(i) && buyables[i].GetComponent<Buyable>().bType == Buyable.buyType.Helper)
                temp.Add(buyables[i]);
        }
        return temp;
    }
    public GameObject getHelper(int index) {
        return getHelpers()[index];
    }
    public GameObject getHelper(string name) {
        foreach(var i in getHelpers()) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public List<GameObject> getDefences() {
        var temp = new List<GameObject>();
        for(int i = 0; i < buyables.Length; i++) {
            if(GameInfo.isBuyableUnlocked(i) && buyables[i].GetComponent<Buyable>().bType == Buyable.buyType.Defence)
                temp.Add(buyables[i]);
        }
        return temp;
    }
    public GameObject getDefence(int index) {
        return getDefences()[index];
    }
    public GameObject getDefence(string name) {
        foreach(var i in getDefences()) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public List<GameObject> getMiscBuyables() {
        var temp = new List<GameObject>();
        for(int i = 0; i < buyables.Length; i++) {
            if(GameInfo.isBuyableUnlocked(i) && buyables[i].GetComponent<Buyable>().bType == Buyable.buyType.Misc)
                temp.Add(buyables[i]);
        }
        return temp;
    }
    public GameObject getMiscBuyable(int index) {
        return getMiscBuyables()[index];
    }
    public GameObject getMiscBuyable(string name) {
        foreach(var i in getMiscBuyables()) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }


    public void unlockBuyable(Buyable b) {
        for(int i = 0; i < buyables.Length; i++) {
            if(buyables[i].GetComponent<Buyable>().title == b.title) {
                GameInfo.unlockBuyable(i);
                Debug.Log("unlocked: " + b.title);
                break;
            }
        }
    }


    private void Awake() {
        SaveData.setInt(GameInfo.buyableCount, buyables.Length);
    }
}
