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
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getHelper(Buyable.buyableTitle name) {
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
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getDefence(Buyable.buyableTitle name) {
        foreach(var i in getDefences()) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public List<GameObject> getStructures() {
        var temp = new List<GameObject>();
        for(int i = 0; i < buyables.Length; i++) {
            if(GameInfo.isBuyableUnlocked(i) && buyables[i].GetComponent<Buyable>().bType == Buyable.buyType.Structure)
                temp.Add(buyables[i]);
        }
        return temp;
    }
    public GameObject getStructure(int index) {
        return getStructures()[index];
    }
    public GameObject getStructure(string name) {
        foreach(var i in getStructures()) {
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getStructure(Buyable.buyableTitle name) {
        foreach(var i in getStructures()) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public void unlockBuyable(Buyable.buyableTitle name) {
        for(int i = 0; i < buyables.Length; i++) {
            if(buyables[i].GetComponent<Buyable>().title == name) {
                GameInfo.unlockBuyable(i);
                Debug.Log("unlocked: " + name.ToString());
                break;
            }
        }
    }
    //  for the buytree buttons
    public void unlockBuyable(int indexInBuyableLibrary) {
        unlockBuyable(buyables[indexInBuyableLibrary].GetComponent<Buyable>().title);
    }


    private void Awake() {
        SaveData.setInt(GameInfo.buyableCount, buyables.Length);
    }
}
