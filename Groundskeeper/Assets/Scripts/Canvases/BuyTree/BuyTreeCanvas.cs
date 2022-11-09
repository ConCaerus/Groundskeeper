using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

//  works like the shit from cult of the lamb.
public class BuyTreeCanvas : MonoBehaviour {
    [SerializeField] BuyTree tree;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] List<GameObject> depths;
    List<GameObject> spawnedNodes = new List<GameObject>();
    [SerializeField] TextMeshProUGUI soulsText;
    [SerializeField] GameObject window;

    private void Start() {
        DOTween.Init();
        loadTree();
        foreach(var i in tree.nodes) {
            if(i.pathParentName == "")
                spawnNodesInPath(i, 0);
        }
        window.GetComponent<RectTransform>().localPosition = Vector3.zero;
        hardHide();
    }


    void saveTree() {
        var data = JsonUtility.ToJson(tree);
        SaveData.setString(GameInfo.buyTreeTag, data);
    }

    void loadTree() {
        var data = SaveData.getString(GameInfo.buyTreeTag);
        if(!string.IsNullOrEmpty(data))
            tree = JsonUtility.FromJson<BuyTree>(data);
    }

    public void resetTree() {
        foreach(var i in tree.nodes)
            i.unlocked = false;
        saveTree();
    }

    public void show() {
        transform.DOKill();
        transform.DOScale(1.0f, .15f);
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";

    }
    public void hide() {
        transform.DOKill();
        transform.DOScale(0.0f, .25f);
    }
    public void hardHide() {
        transform.localScale = new Vector3(0.0f, 0.0f);
    }

    public void saveListAndReset(BuyTreeNode boughtNode) {
        //  saves the tree
        saveTree();

        //  makes the next nodes in the list interactable
        foreach(var i in tree.nodes) {
            if(i.pathParentName == boughtNode.name) {
                foreach(var j in spawnedNodes) {
                    if(j.GetComponentInChildren<TextMeshProUGUI>().text == i.name) {
                        j.GetComponentInChildren<Button>().interactable = true;
                        j.transform.DOPunchScale(new Vector3(.5f, .5f), .15f);
                    }
                }
            }
        }
    }

    void spawnNodesInPath(BuyTreeNode n, int depth, GameObject prev = null, bool prevUnlocked = true) {
        //  spawns the given node
        var o = Instantiate(nodePrefab);
        o.name = n.name;
        o.GetComponent<BuyTreeNodeObj>().setText(n.name);

        //  checks if needs to spawn more depths
        while(depth > depths.Count - 1) {
            var d = Instantiate(depths[0].gameObject, depths[0].transform.parent);
            foreach(var i in d.GetComponentsInChildren<Button>())
                Destroy(i.transform.parent.gameObject);
            depths.Add(d.gameObject);
        }

        o.transform.parent = depths[depth].transform;
        if(prev != null && n.c.a == 0f) {
            o.GetComponent<BuyTreeNodeObj>().setColor(prev.GetComponent<BuyTreeNodeObj>().getColor());
        }
        else
            o.GetComponent<BuyTreeNodeObj>().setColor(n.c);

        o.GetComponent<BuyTreeNodeObj>().setCost(n.cost);

        o.GetComponentInChildren<Button>().interactable = prevUnlocked;
        if(n.unlocked)
            o.GetComponent<BuyTreeNodeObj>().showCheckmark();

        spawnedNodes.Add(o.gameObject);

        //  adds the unlock event to the buttons onclick event
        if(n.unlock != null && !n.unlock.Equals(new UnityEvent()) && !n.unlocked) {
            o.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                //  buy the fucker
                if(GameInfo.getSouls() >= n.cost) {
                    GameInfo.addSouls(-n.cost);
                    n.unlock.Invoke();  //  calls the unlock function
                    n.unlocked = true;    //  sets the node to unlocked
                    o.GetComponent<BuyTreeNodeObj>().showCheckmark();
                    saveListAndReset(n);
                    soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
                }
            });
        }

        //  spawns the next nodes in the tree
        foreach(var i in tree.nodes) {
            if(i.pathParentName == n.name)
                spawnNodesInPath(i, depth + 1, o.gameObject, n.unlocked);
        }
    }


    /*      ---     BUTTONS IN THE TREE     ---     */
    
    public void unlockHelperDamage(int tier) {
        float buff = .25f * tier;  //  +25% damage per rank
        //  adds 1 to the number so when I get the number, I can immediately multiply it to the fucker
        buff += 1.0f;
        GameInfo.setHelperDamageBuff(buff);
    }
    //  so far the two weapon unlock functions just do the unlock for the current player weapon
    //  easy to adjust if you want to add more weapons, but also works with just one
    public void unlockWeaponDamage(int tier) {
        float buff = .25f * tier;
        buff += 1.0f;
        GameInfo.setPlayerWeaponDamageBuff(GameInfo.getPlayerWeaponIndex(), buff);
    }
    public void unlockWeaponSpeed(int tier) {
        float buff = tier * .2f;
        buff = 1.0f - buff;
        GameInfo.setPlayerWeaponSpeedBuff(GameInfo.getPlayerWeaponIndex(), buff);
    }
}

/*  nodes that the player will be able to buy in the buy tree
        these could be unlockables, such as unlocking a new helper
        or these could be upgrades for already unlocked things, like damage boost for helpers
*/
[System.Serializable]
public class BuyTreeNode {
    public string name;
    public string pathParentName;
    public Color c;
    public UnityEvent unlock = null;
    public bool unlocked = false;

    //  cost to unlock
    public float cost;
}

[System.Serializable]
public class BuyTree {
    public List<BuyTreeNode> nodes = new List<BuyTreeNode>();
}
