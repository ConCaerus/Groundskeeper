using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyableLibrary : MonoBehaviour {
    [SerializeField] GameObject[] uniqueBuyables;   //  you designed this to work with house buyable
    [SerializeField] GameObject[] earlyBuyables;
    [SerializeField] GameObject[] midBuyables;
    [SerializeField] GameObject[] lateBuyables;

    //  does not include uniques
    public GameObject[][] buyables() {
        return new GameObject[][] { earlyBuyables, midBuyables, lateBuyables };
    }

    //  only used in GameInfo.lockAllBuyables
    public GameObject[][] buyablesWithUniques() {
        return new GameObject[][] { earlyBuyables, midBuyables, lateBuyables, uniqueBuyables };
    }

    public int getBuyableCost(Buyable.buyType t, unlockTier u) {
        switch(u) {
            case unlockTier.Early:
                switch(t) {
                    case Buyable.buyType.Helper:
                        return 50;
                    case Buyable.buyType.Defence:
                        return 35;
                    case Buyable.buyType.Structure:
                        return 75;
                }
                return 0;
            case unlockTier.Mid:
                switch(t) {
                    case Buyable.buyType.Helper:
                        return 150;
                    case Buyable.buyType.Defence:
                        return 50;
                    case Buyable.buyType.Structure:
                        return 100;
                }
                return 0;
            case unlockTier.Late:
                switch(t) {
                    case Buyable.buyType.Helper:
                        return 200;
                    case Buyable.buyType.Defence:
                        return 100;
                    case Buyable.buyType.Structure:
                        return 200;
                }
                return 0;
        }
        return 0;
    }

    public enum unlockTier {
        All, Early, Mid, Late, Unique
    }

    string seenTag(Buyable.buyableTitle name) {
        return "HasPlayerSeenThisBuyable:" + name.ToString();
    }


    //  only returns the unlocked shit
    public GameObject getHelper(int index) {
        return getUnlockedBuyablesOfType(Buyable.buyType.Helper, false)[index];
    }
    public GameObject getHelper(string name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Helper, includeUniques)) {
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getHelper(Buyable.buyableTitle name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Helper, includeUniques)) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public GameObject getDefence(int index) {
        return getUnlockedBuyablesOfType(Buyable.buyType.Defence, false)[index];
    }
    public GameObject getDefence(string name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Defence, includeUniques)) {
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getDefence(Buyable.buyableTitle name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Defence, includeUniques)) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }

    public GameObject getStructure(int index) {
        return getUnlockedBuyablesOfType(Buyable.buyType.Structure, false)[index];
    }
    public GameObject getStructure(string name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Structure, includeUniques)) {
            if(i.GetComponent<Buyable>().title.ToString() == name)
                return i.gameObject;
        }
        return null;
    }
    public GameObject getStructure(Buyable.buyableTitle name, bool includeUniques) {
        foreach(var i in getUnlockedBuyablesOfType(Buyable.buyType.Structure, includeUniques)) {
            if(i.GetComponent<Buyable>().title == name)
                return i.gameObject;
        }
        return null;
    }


    public bool hasPlayerSeenBuyable(Buyable.buyableTitle title) {
        return SaveData.getInt(seenTag(title)) == 0;
    }
    public void playerSawBuyable(Buyable.buyableTitle title) {
        SaveData.setInt(seenTag(title), 1);
    }


    public List<GameObject> getUnlockedBuyablesOfType(Buyable.buyType t, bool includeUniques) {
        var temp = new List<GameObject>();
        int index = 0;
        for(int u = 0; u < 3; u++) {
            for(int i = 0; i < buyables()[u].Length; i++) {
                if(GameInfo.isBuyableUnlocked(buyables()[u][i].GetComponent<Buyable>().title) && buyables()[u][i].GetComponent<Buyable>().bType == t)
                    temp.Add(buyables()[u][i]);
                index++;
            }
        }
        if(includeUniques) {
            foreach(var i in uniqueBuyables) {
                if(i.GetComponent<Buyable>().bType == t && GameInfo.isBuyableUnlocked(i.GetComponent<Buyable>().title))
                    temp.Add(i.gameObject);
            }
        }
        return temp;
    }

    //  not public cause scarry OwO
    List<GameObject> getLockedBuyablesOfType(Buyable.buyType t, unlockTier u) {
        var temp = new List<GameObject>();
        if(u == unlockTier.All) {
            for(int i = 0; i < 3; i++) {
                foreach(var l in getLockedBuyablesOfType(t, (unlockTier)(i + 1)))
                    temp.Add(l);
            }
        }
        else {
            for(int i = 0; i < buyables()[(int)u - 1].Length; i++) {
                if(!GameInfo.isBuyableUnlocked(buyables()[(int)u - 1][i].GetComponent<Buyable>().title) && buyables()[(int)u - 1][i].GetComponent<Buyable>().bType == t)
                    temp.Add(buyables()[(int)u - 1][i]);
            }
        }
        return temp;
    }
    public int getNumberOfLockedBuyables(Buyable.buyType t) {
        int count = 0;
        for(int u = 0; u < 3; u++) {
            for(int i = 0; i < buyables()[u].Length; i++) {
                if(!GameInfo.isBuyableUnlocked(buyables()[u][i].GetComponent<Buyable>().title) && buyables()[u][i].GetComponent<Buyable>().bType == t)
                    count++;
            }
        }
        return count;
    }
    public int getNumberOfUnlockedBuyables(Buyable.buyType t, bool includeUniques) {
        if(!includeUniques)
            return getUnlockedBuyablesOfType(t, includeUniques).Count;
        int count = getUnlockedBuyablesOfType(t, includeUniques).Count;
        foreach(var i in uniqueBuyables) {
            if(i.GetComponent<Buyable>().bType == t && GameInfo.isBuyableUnlocked(i.GetComponent<Buyable>().title))
                count++;
        }
        return count;
    }
    public int getTotalNumberOfBuyables(Buyable.buyType t) {
        return getNumberOfUnlockedBuyables(t, false) + getNumberOfLockedBuyables(t);
    }
    public bool hasUnlockedBuyables(bool includeUniques) {
        bool h = getNumberOfUnlockedBuyables(Buyable.buyType.Helper, includeUniques) > 0;
        bool s = getNumberOfUnlockedBuyables(Buyable.buyType.Structure, includeUniques) > 0;
        bool d = getNumberOfUnlockedBuyables(Buyable.buyType.Defence, includeUniques) > 0;
        return h || s || d;
    }

    public bool unlockRandomBuyableOfType(Buyable.buyType t, unlockTier u) {
        if(getNumberOfLockedBuyables(t) == 0)
            return false;
        GameInfo.unlockBuyable(getLockedBuyablesOfType(t, u)[Random.Range(0, getLockedBuyablesOfType(t, u).Count)].GetComponent<Buyable>().title);
        return true;
    }
    public void unlockAll() {
        for(int u = 0; u < 3; u++) {
            for(int i = 0; i < buyables()[u].Length; i++)
                GameInfo.unlockBuyable(buyables()[u][i].GetComponent<Buyable>().title);
        }
    }

    public unlockTier getRelevantUnlockTierForBuyableType(Buyable.buyType t) {
        if(getLockedBuyablesOfType(t, unlockTier.Early).Count > 0)
            return unlockTier.Early;
        if(getLockedBuyablesOfType(t, unlockTier.Mid).Count > 0)
            return unlockTier.Mid;
        return unlockTier.Late;
    }


    private void Awake() {
        SaveData.setInt(GameInfo.buyableCount, earlyBuyables.Length + midBuyables.Length + lateBuyables.Length);
    }
}
