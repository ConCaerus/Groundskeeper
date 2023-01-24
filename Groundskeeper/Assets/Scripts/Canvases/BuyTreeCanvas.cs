using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class BuyTreeCanvas : MenuCanvas {
    int subMaxTicks = 4, subMaxTier = 3;
    float sliderFillSpeed = .1f;

    //  end of tier 1 - 1.4, end of tier 2 - 2.0, end of teir 3 - 3.0
    float[] helperDamageInc = { .1f, .15f, .25f }, helperHealthInc = { .1f, .15f, .25f };
    float[] defenceDamageInc = { .1f, .15f, .25f };
    float[] structureHealthInc = { .1f, .15f, .25f };
    float[] weaponDamageInc = { .1f, .15f, .25f }, weaponSpeedInc = { .1f, .15f, .25f };

    [SerializeField] GameObject node;

    //  ordered: Helper, Defences, Structure, Weapon
    [SerializeField] GameObject[] mainCircles;
    [SerializeField] GameObject[] subCircles;

    [SerializeField] TextMeshProUGUI soulsText;

    GameObject[] queuedBuyables = new GameObject[3];

    enum subType {
        Damage, Health, Speed
    }


    private void Start() {
        transform.localScale = Vector3.zero;
        createTree();
        updateSoulsText();
    }

    void createTree() {
        //  queue up some random buyables to be saved
        var bl = FindObjectOfType<BuyableLibrary>();
        queuedBuyables[0] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Helper, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Helper));
        queuedBuyables[1] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Defence, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Defence));
        queuedBuyables[2] = bl.getRandomUnlockableBuyableOfType(Buyable.buyType.Structure, bl.getRelevantUnlockTierForBuyableType(Buyable.buyType.Structure));

        //  main shits
        for(int i = 0; i < 3; i++)
            createMainNode(i);

        //  weapon
        var w = Instantiate(node.gameObject, mainCircles[3].transform);
        setupSlider(w.GetComponent<BuyTreeNode>().getSlider(), false, 1.0f);
        w.GetComponent<BuyTreeNode>().setTitle("Weapons");
        w.GetComponent<BuyTreeNode>().setTier(-1, subMaxTier);
        w.GetComponent<BuyTreeNode>().info.info = "Weapon";


        //  side shits
        //  weapons
        createSubCirclesForType(Buyable.buyType.Helper);
        createSubCirclesForType(Buyable.buyType.Defence);
        createSubCirclesForType(Buyable.buyType.Structure);
        createSubNode(subCircles[3], subType.Damage);
        createSubNode(subCircles[3], subType.Speed);
    }

    //  not used to create the weapon node
    GameObject createMainNode(int index) {
        var bl = FindObjectOfType<BuyableLibrary>();
        var h = Instantiate(node.gameObject, mainCircles[index].transform);
        var t = (Buyable.buyType)(index + 1);
        h.GetComponent<BuyTreeNode>().mainType = t;
        h.GetComponent<BuyTreeNode>().getSlider().setText(bl.getNumberOfUnlockedBuyables(t, false).ToString());
        h.GetComponent<BuyTreeNode>().setTitle(t == Buyable.buyType.Helper ? "Helpers" : t == Buyable.buyType.Defence ? "Defences" : t == Buyable.buyType.Structure ? "Structures" : "?");
        h.GetComponent<BuyTreeNode>().setCost(getUpdatedCost(h.GetComponent<BuyTreeNode>()));
        h.GetComponent<BuyTreeNode>().setTier(-1, subMaxTier); //  hides the tierText
        var qb = queuedBuyables[index];
        h.GetComponent<BuyTreeNode>().info.info = (qb == null || qb.GetComponent<Buyable>() == null) ? "Completed" : qb.GetComponent<Buyable>().title.ToString();

        setupSlider(h.GetComponent<BuyTreeNode>().getSlider(), false,
            (float)bl.getNumberOfUnlockedBuyables((Buyable.buyType)(index + 1), false) / bl.getTotalNumberOfBuyables(t),
            delegate {
                var c = bl.getBuyableUnlockCost(t, bl.getRelevantUnlockTierForBuyableType(t));
                //  checks money
                if(FindObjectOfType<SoulTransactionHandler>().tryTransaction(c, soulsText, true)) {
                    //  checks if there are any more locked buyables of that type
                    if(bl.unlockBuyable(qb)) {
                        //  transaction
                        h.GetComponent<BuyTreeNode>().setCost(getUpdatedCost(h.GetComponent<BuyTreeNode>()));

                        //  flair
                        h.GetComponent<BuyTreeNode>().getSlider().doValue((float)bl.getNumberOfUnlockedBuyables(t, false) / bl.getTotalNumberOfBuyables(t), sliderFillSpeed);
                        h.GetComponent<BuyTreeNode>().getSlider().setText(bl.getNumberOfUnlockedBuyables(t, false).ToString());
                        h.GetComponent<BuyTreeNode>().animateClick();
                        updateSoulsText();

                        //  unlock new sub circles
                        //  show the first sub circles
                        if(bl.getNumberOfUnlockedBuyables(t, false) == 1) {
                            createSubCirclesForType(t);
                        }

                        //  update buyables in queue
                        queuedBuyables[index] = bl.getRandomUnlockableBuyableOfType(t, bl.getRelevantUnlockTierForBuyableType(t));
                        qb = queuedBuyables[index];
                        h.GetComponent<BuyTreeNode>().info.info = (qb == null || qb.GetComponent<Buyable>() == null) ? "Completed" : qb.GetComponent<Buyable>().title.ToString();
                        FindObjectOfType<InfoBox>().updateInfo(h.GetComponent<BuyTreeNode>().info.info);
                    }
                }
            });
        return h;
    }

    void createSubCirclesForType(Buyable.buyType t) {
        //  helpers
        if(t == Buyable.buyType.Helper && FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Helper, false) > 0) {
            createSubNode(subCircles[0], subType.Damage);
            createSubNode(subCircles[0], subType.Health);
        }
        //  defences
        if(t == Buyable.buyType.Defence && FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Defence, false) > 0)
            createSubNode(subCircles[1], subType.Damage);
        //  structures
        if(t == Buyable.buyType.Structure && FindObjectOfType<BuyableLibrary>().getNumberOfUnlockedBuyables(Buyable.buyType.Structure, false) > 0)
            createSubNode(subCircles[2], subType.Health);
    }
    GameObject createSubNode(GameObject sub, subType s) {
        var o = Instantiate(node.gameObject, sub.transform);
        int sInd = sub == subCircles[0] ? 0 : sub == subCircles[1] ? 1 : sub == subCircles[2] ? 2 : 3;
        o.GetComponent<BuyTreeNode>().setTier(0, subMaxTier);


        setupSlider(o.GetComponent<BuyTreeNode>().getSlider(), true, 0.0f, delegate {
            //  checks if the player is allowed to buy this upgrade
            if(o.GetComponent<BuyTreeNode>().canIncrease(subMaxTier)) {
                if(FindObjectOfType<SoulTransactionHandler>().tryTransaction(o.GetComponent<BuyTreeNode>().cost, soulsText, true)) {
                    subLogic(sInd, s, o.GetComponent<BuyTreeNode>().tier);

                    updateSubSlider(sInd, s, o.GetComponent<BuyTreeNode>());
                    o.GetComponent<BuyTreeNode>().animateClick();
                    updateSoulsText();
                }
            }
        });

        updateSubSlider(sInd, s, o.GetComponent<BuyTreeNode>());
        o.GetComponent<BuyTreeNode>().setTitle(s.ToString());
        return o;
    }

    void subLogic(int index, subType s, int tier) {
        switch(index) {
            //  helpers
            case 0:
                if(s == subType.Damage) {
                    GameInfo.incHelperDamageBuff(helperDamageInc[tier]);
                }
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
                if(s == subType.Speed)
                    GameInfo.incPWeaponSpeedBuff(weaponSpeedInc[tier]);
                break;
        }
    }


    void updateSubSlider(int index, subType s, BuyTreeNode node) {
        switch(index) {
            //  helpers
            case 0:
                if(s == subType.Damage) {
                    //  set the tier of the node
                    var temp = GameInfo.getHelperDamageBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= helperDamageInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += helperDamageInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getHelperDamageBuff() - 1f - not) / (helperDamageInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }
                else if(s == subType.Health) {
                    //  set the tier of the node
                    var temp = GameInfo.getHelperHealthBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= helperHealthInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += helperHealthInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getHelperHealthBuff() - 1f - not) / (helperHealthInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }

                node.setCost(200);
                break;
            //  defences
            case 1:
                if(s == subType.Damage) {
                    //  set the tier of the node
                    var temp = GameInfo.getDefenceDamageBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= defenceDamageInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += defenceDamageInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getDefenceDamageBuff() - 1f - not) / (defenceDamageInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }

                node.setCost(75);
                break;
            //  structures
            case 2:
                if(s == subType.Health) {
                    //  set the tier of the node
                    var temp = GameInfo.getStructureHealthBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= structureHealthInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += structureHealthInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getStructureHealthBuff() - 1f - not) / (structureHealthInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }

                node.setCost(200);
                break;
            //  weapons
            case 3:
                if(s == subType.Damage) {
                    //  set the tier of the node
                    var temp = GameInfo.getPWeaponDamageBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= weaponDamageInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += weaponDamageInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getPWeaponDamageBuff() - 1f - not) / (weaponDamageInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }
                else if(s == subType.Speed) {
                    //  set the tier of the node
                    var temp = GameInfo.getPWeaponSpeedBuff() - 1f;
                    for(int i = 0; i < subMaxTier; i++) {
                        temp -= weaponSpeedInc[i] * subMaxTicks;
                        if(temp < .05f) {
                            node.setTier(i, subMaxTier);
                            break;
                        }
                    }

                    float not = 0.0f;
                    for(int i = 0; i < node.tier; i++)
                        not += weaponSpeedInc[i] * subMaxTicks;
                    node.getSlider().doValue((GameInfo.getPWeaponSpeedBuff() - 1f - not) / (weaponSpeedInc[node.tier] * subMaxTicks), sliderFillSpeed);
                }

                node.setCost(150);
                break;
        }

        //  slider is full, increase tier and reset value (if you want to do an animation later, put a wait for seconds before this
        if(node.getSlider().value > .97f) {
            node.setTier(node.tier + 1, subMaxTier);
            if(node.canIncrease(subMaxTier)) {
                node.getSlider().doValue(0f, sliderFillSpeed);
            }
            //  player has maxed out this node
            else
                node.getSlider().doColor(Color.white, .15f);
        }
        node.getSlider().setText(((int)(node.getSlider().value * subMaxTicks)).ToString());
    }

    void setupSlider(CircularSlider c, bool sub, float startingVal, UnityEngine.Events.UnityAction clickEvent = null) {
        c.setValue(0.0f);
        c.doValue(startingVal, sliderFillSpeed);
        var co = Random.ColorHSV() + new Color(.2f, .2f, .2f);
        co = new Color(co.r, co.g, co.b, 1.0f);
        c.setColor(co);
        if(clickEvent != null)
            c.GetComponentInChildren<Button>().onClick.AddListener(clickEvent);

        c.GetComponentInParent<BuyTreeNode>().showAnimation(sub);
    }

    int getUpdatedCost(BuyTreeNode node) {
        if(node.mainType == Buyable.buyType.None)
            return node.cost + 50;

        return FindObjectOfType<BuyableLibrary>().getBuyableUnlockCost(node.mainType, FindObjectOfType<BuyableLibrary>().getRelevantUnlockTierForBuyableType(node.mainType));
    }


    void updateSoulsText() {
        soulsText.text = GameInfo.getSouls().ToString("0.0") + "s";
    }


    protected override void show() {
        transform.DOKill();
        transform.DOScale(1.0f, .15f);
    }

    protected override void close() {
        transform.DOKill();
        transform.DOScale(0.0f, .25f);
        FindObjectOfType<MouthInteractable>().tryDeinteract();
    }
}
