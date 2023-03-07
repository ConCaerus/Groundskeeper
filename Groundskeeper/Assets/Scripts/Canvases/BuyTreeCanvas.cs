using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class BuyTreeCanvas : MenuCanvas {
    float sliderFillSpeed = .1f;

    //  end of tier 1 - 1.4, end of tier 2 - 2.0, end of teir 3 - 3.0
    float[] helperDamageInc = { .1f, .15f, .25f }, helperHealthInc = { .1f, .15f, .25f };
    float[] defenceDamageInc = { .1f, .15f, .25f };
    float[] structureHealthInc = { .1f, .15f, .25f };
    float[] weaponDamageInc = { .1f, .15f, .25f }, weaponSpeedInc = { .1f, .15f, .25f };

    [SerializeField] GameObject node;

    //  ordered: Helper, Defences, Structure, Weapon, House
    [SerializeField] GameObject[] mainCircles;
    [SerializeField] GameObject[] subCircles;
    [SerializeField] GameObject holder;

    [SerializeField] TextMeshProUGUI soulsText;

    GameObject[] queuedBuyables = new GameObject[3];
    Weapon queuedWeapon;
    BuyableLibrary bl;
    PresetLibrary pl;

    public enum subType {
        None, Damage, Health, Speed, Light, Repair
    }


    private void Start() {
        holder.SetActive(true);
        bl = FindObjectOfType<BuyableLibrary>();
        pl = FindObjectOfType<PresetLibrary>();
        transform.localScale = Vector3.zero;
        createTree();
        updateSoulsText();
    }

    void createTree() {
        //  queue up some random buyables to be saved
        queuedBuyables[0] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Helper, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Helper));
        queuedBuyables[1] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Defence, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Defence));
        queuedBuyables[2] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Structure, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Structure));
        queuedWeapon = pl.getRandomLockedWeapon();

        //  main shits
        for(int i = 0; i < 3; i++)
            createMainNode(i);

        //  weapon
        /*
        var w = Instantiate(node.gameObject, mainCircles[3].transform);
        setupSlider(w.GetComponent<BuyTreeNode>().getSlider(), false, 1.0f);
        w.GetComponent<BuyTreeNode>().setTitle("Weapons");
        w.GetComponent<BuyTreeNode>().setTier(-1, subMaxTier);
        w.GetComponent<BuyTreeNode>().info.info = "Weapon";
        */


        //  side shits
        createSubCirclesForType(Buyable.buyType.Helper);
        createSubCirclesForType(Buyable.buyType.Defence);
        createSubCirclesForType(Buyable.buyType.Structure);

        //  weapon
        createWeaponNode();
        createSubNode(subCircles[3], subType.Damage, 4, 3);
        createSubNode(subCircles[3], subType.Speed, 4, 3);

        //  house
        createHouseNode();
        createSubNode(subCircles[4], subType.Light, 5, 1);
        createSubNode(subCircles[4], subType.Repair, 10, 10);
    }

    GameObject createWeaponNode() {
        int index = 3;
        var h = Instantiate(node.gameObject, mainCircles[index].transform);
        var hbtn = h.GetComponent<BuyTreeNode>();
        hbtn.getSlider().setText(pl.getUnlockedWeapons().Count.ToString());
        hbtn.setTitle("Weapon");
        var c = 200;    //  cost of the fucker
        hbtn.setCost(c);
        hbtn.setTier(-1); //  hides the tierText
        var qw = queuedWeapon;
        hbtn.info.info = (qw == null) ? "Completed" : qw.title.ToString();


        //  LOGIC FOR ACTUALLY UNLOCKING THINGS
        setupSlider(hbtn.getSlider(), false,
            (float)pl.getUnlockedWeapons().Count / pl.getWeapons().Length,
            delegate {
                //  checks money
                if(FindObjectOfType<SoulTransactionHandler>().tryTransaction(c, soulsText, true)) {
                    //  checks if there are any more locked buyables of that type
                    if(qw != null && pl.unlockWeapon(qw.title)) {
                        //  transaction
                        hbtn.setCost(c);

                        //  flair
                        hbtn.getSlider().doValue((float)pl.getUnlockedWeapons().Count / pl.getWeapons().Length, sliderFillSpeed, true);
                        hbtn.getSlider().setText(pl.getUnlockedWeapons().Count.ToString());
                        hbtn.animateClick();
                        updateSoulsText();

                        //  update buyables in queue
                        FindObjectOfType<UnlockCanvas>().showForWeapon(qw);
                        queuedWeapon = pl.getRandomLockedWeapon();
                        qw = queuedWeapon;
                        hbtn.info.info = (qw == null) ? "Completed" : qw.title.ToString();
                        FindObjectOfType<InfoBox>().updateInfo(hbtn.info.info);
                    }
                }
            });
        hbtn.getSlider().doValueKill();
        hbtn.maxTicks = pl.getWeapons().Length;
        hbtn.setTick(pl.getUnlockedWeapons().Count);
        return h;
    }
    GameObject createHouseNode() {
        int index = 4;
        var h = Instantiate(node.gameObject, mainCircles[index].transform);
        var hbtn = h.GetComponent<BuyTreeNode>();
        hbtn.getSlider().setText("0");
        hbtn.setTitle("House");
        var c = 200;    //  cost of the fucker
        hbtn.setCost(c);
        hbtn.setTier(-1); //  hides the tierText
        //var qw = queuedWeapon;
        //hbtn.info.info = (qw == null) ? "Completed" : qw.title.ToString();


        //  LOGIC FOR ACTUALLY UNLOCKING THINGS
        setupSlider(hbtn.getSlider(), false,
            (float)pl.getUnlockedWeapons().Count / pl.getWeapons().Length,
            delegate {
                Debug.Log("Bought House Upgrade");
            });
        hbtn.getSlider().doValueKill();
        hbtn.getSlider().setValue(1.0f);
        return h;
    }
    //  not used to create the weapon node
    GameObject createMainNode(int index) {
        var h = Instantiate(node.gameObject, mainCircles[index].transform);
        var hbtn = h.GetComponent<BuyTreeNode>();
        var t = indexToMainType(index);
        hbtn.mainType = t;
        hbtn.getSlider().setText(bl.getNumberOfUnlockedBuyables(t, false).ToString());
        hbtn.setTitle(t.ToString() + "s");
        hbtn.setCost(getUpdatedCost(hbtn));
        hbtn.setTier(-1); //  hides the tierText
        var qb = queuedBuyables[index];
        hbtn.info.info = (qb == null || qb.GetComponent<Buyable>() == null) ? "Completed" : qb.GetComponent<Buyable>().title.ToString();


        //  LOGIC FOR ACTUALLY UNLOCKING THINGS
        setupSlider(hbtn.getSlider(), false,
            (float)bl.getNumberOfUnlockedBuyables((Buyable.buyType)(index + 1), false) / bl.getTotalNumberOfBuyables(t),
            delegate {
                var c = bl.getBuyableUnlockCost(t, bl.getRelevantUnlockTierForBuyableType(t));
                //  checks money
                if(FindObjectOfType<SoulTransactionHandler>().tryTransaction(c, soulsText, true)) {
                    //  checks if there are any more locked buyables of that type
                    if(bl.unlockBuyable(qb)) {
                        //  transaction
                        hbtn.setCost(getUpdatedCost(hbtn));

                        //  flair
                        hbtn.getSlider().doValue((float)bl.getNumberOfUnlockedBuyables(t, false) / bl.getTotalNumberOfBuyables(t), sliderFillSpeed, true);
                        hbtn.getSlider().setText(bl.getNumberOfUnlockedBuyables(t, false).ToString());
                        hbtn.animateClick();
                        updateSoulsText();

                        //  unlock new sub circles
                        //  show the first sub circles
                        if(bl.getNumberOfUnlockedBuyables(t, false) == 1) {
                            createSubCirclesForType(t);
                        }

                        //  update buyables in queue
                        FindObjectOfType<UnlockCanvas>().showForBuyable(qb.GetComponent<Buyable>());
                        queuedBuyables[index] = bl.getRandomUnlockableBuyableOfType(t, bl.getRelevantUnlockTierForBuyableType(t));
                        qb = queuedBuyables[index];
                        hbtn.info.info = (qb == null || qb.GetComponent<Buyable>() == null) ? "Completed" : qb.GetComponent<Buyable>().title.ToString();
                        FindObjectOfType<InfoBox>().updateInfo(hbtn.info.info);
                    }
                }
            });
        if(index == 0)
            h.GetComponentInChildren<Button>().Select();
        hbtn.getSlider().doValueKill();
        hbtn.getSlider().setValue((float)bl.getNumberOfUnlockedBuyables((Buyable.buyType)(index + 1), false) / bl.getTotalNumberOfBuyables(t));
        return h;
    }

    void createSubCirclesForType(Buyable.buyType t) {
        //  helpers
        if(t == Buyable.buyType.Helper && bl.getNumberOfUnlockedBuyables(Buyable.buyType.Helper, false) > 0) {
            createSubNode(subCircles[0], subType.Damage, 4, 3);
            createSubNode(subCircles[0], subType.Health, 4, 3);
        }
        //  defences
        if(t == Buyable.buyType.Defence && bl.getNumberOfUnlockedBuyables(Buyable.buyType.Defence, false) > 0)
            createSubNode(subCircles[1], subType.Damage, 4, 3);
        //  structures
        if(t == Buyable.buyType.Structure && bl.getNumberOfUnlockedBuyables(Buyable.buyType.Structure, false) > 0)
            createSubNode(subCircles[2], subType.Health, 4, 3);
    }
    GameObject createSubNode(GameObject sub, subType s, int maxTick, int maxTier) {
        var o = Instantiate(node.gameObject, sub.transform);
        int sInd = sub == subCircles[0] ? 0 : sub == subCircles[1] ? 1 : sub == subCircles[2] ? 2 : 3;
        var obtn = o.GetComponent<BuyTreeNode>();
        obtn.subType = s;
        obtn.mainType = indexToMainType(sInd);
        obtn.maxTier = maxTier;
        obtn.maxTicks = maxTick;
        obtn.info.info = s.ToString();

        obtn.setTier(GameInfo.getBuyTreeSubNodeTier(indexToMainType(sInd), s));
        obtn.setTick(GameInfo.getBuyTreeSubNodeTick(indexToMainType(sInd), s));


        setupSlider(obtn.getSlider(), true, 0.0f, delegate {
            //  checks if the player is allowed to buy this upgrade
            if(obtn.canIncrease()) {
                if(FindObjectOfType<SoulTransactionHandler>().tryTransaction(obtn.cost, soulsText, true)) {
                    subLogic(sInd, s, obtn.tier);   //  does the thing that the sub slider is supposed to do

                    updateSubSlider(obtn);
                    obtn.animateClick();
                    updateSoulsText();
                }
            }
        });
        obtn.setTitle(s.ToString());
        return o;
    }
    Buyable.buyType indexToMainType(int ind) {
        switch(ind) {
            case 0: return Buyable.buyType.Helper;
            case 1: return Buyable.buyType.Defence;
            case 2: return Buyable.buyType.Structure;
            case 3: return Buyable.buyType.Weapon;
            case 4: return Buyable.buyType.House;
        }
        return Buyable.buyType.None;
    }

    void subLogic(int index, subType s, int tier) {
        switch(index) {
            //  helpers
            case 0:
                if(s == subType.Damage) 
                    GameInfo.incHelperDamageBuff(helperDamageInc[tier]);
                else if(s == subType.Health)
                    GameInfo.incHelperHealthBuff(helperHealthInc[tier]);
                break;
            //  defences
            case 1:
                if(s == subType.Damage)
                    GameInfo.incDefenceDamageBuff(defenceDamageInc[tier]);
                break;
            //  structures
            case 2:
                if(s == subType.Health)
                    GameInfo.incStructureHealthBuff(structureHealthInc[tier]);
                break;
            //  weapons
            case 3:
                if(s == subType.Damage)
                    GameInfo.incPWeaponDamageBuff(weaponDamageInc[tier]);
                else if(s == subType.Speed)
                    GameInfo.incPWeaponSpeedBuff(weaponSpeedInc[tier]);
                break;
            //  house
            case 4:
                if(s == subType.Light)
                    GameInfo.setHouseLightRad(GameInfo.getHouseLightRad() + 10f);
                else if(s == subType.Repair)
                    GameInfo.setHouseHealth(GameInfo.getHouseHealth() + 25);
                break;
        }
    }


    //  this function could be cleaned up a lot
    void updateSubSlider(BuyTreeNode node) {
        node.incTick();

        //  saves the new values
        GameInfo.setBuyTreeSubNodeTick(node.mainType, node.subType, node.tick);
        GameInfo.setBuyTreeSubNodeTier(node.mainType, node.subType, node.tier);
    }

    void setupSlider(CircularSlider c, bool sub, float startingVal, UnityEngine.Events.UnityAction clickEvent = null) {
        var co = Random.ColorHSV() + new Color(.2f, .2f, .2f);
        co = new Color(co.r, co.g, co.b, 1.0f);
        c.setColor(co);
        if(clickEvent != null)
            c.GetComponentInChildren<Button>().onClick.AddListener(clickEvent);

        c.GetComponentInParent<BuyTreeNode>().showAnimation(sub);
    }

    int getUpdatedCost(BuyTreeNode node) {
        Buyable.buyType bt = node.mainType;

        var c = FindObjectOfType<BuyableLibrary>().getBuyableUnlockCost(bt, FindObjectOfType<BuyableLibrary>().getRelevantUnlockTierForBuyableType(bt));
        if(c == 0)
            c = node.cost + 50;

        return c;
    }


    void updateSoulsText() {
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
    }


    protected override void show() {
        transform.DOKill();
        transform.DOScale(1.0f, .15f);
        //createTree();
    }

    protected override void close() {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        FindObjectOfType<MouthInteractable>().tryDeinteract();
    }
}
