using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BuyTreeNode : MonoBehaviour {
    [SerializeField] CircularSlider slider;
    [SerializeField] TextMeshProUGUI title, costText, tierText;
    [SerializeField] public InfoableImage info;

    public int cost { get; private set; }
    public int tier { get; private set; } = 0;  //  gets one of these every time it reachs a max tick
    public int tick { get; private set; } = 0;
    [HideInInspector] public int maxTier;
    [HideInInspector] public int maxTicks;
    public Buyable.buyType mainType = Buyable.buyType.None;
    public BuyTreeCanvas.subType subType = BuyTreeCanvas.subType.None;

    public void setTitle(string name) {
        title.text = name;
    }
    public void setCost(int c) {
        cost = c;
        if(c > 0)
            costText.text = c.ToString() + "s";
        else
            costText.text = "";
    }
    public CircularSlider getSlider() {
        return slider;
    }

    public void animateClick() {
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOPunchScale(new Vector3(.5f, .5f), .15f);
    }

    public void showAnimation(bool sub) {
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).transform.localScale = Vector3.zero;
        float endScale = 1f;
        if(sub)
            endScale = .5f;
        transform.GetChild(0).transform.DOScale(endScale, .1f);
    }

    public void setTier(int t) {
        if(t < maxTier && t > -1) {
            tier = t;
            tierText.text = tierToString(tier);
        }
        else if(t == -1)
            tierText.text = "";
        else if(t >= maxTier) {
            tier = 100;
            tierText.text = tierToString(maxTier - 1);
        }
    }
    string tierToString(int t) {
        if(t == 0)
            return "I";
        else if(t == 1)
            return "II";
        else if(t == 2)
            return "III";
        else if(t == 3)
            return "IV";
        else if(t == 4)
            return "V";
        else if(t == 5)
            return "VI";
        else if(t == 6)
            return "VII";
        else if(t == 7)
            return "VIII";
        else if(t == 8)
            return "IX";
        else if(t == 9)
            return "X";
        return "";
    }
    public bool setTick(int t) {
        tick = t;
        bool temp = false;

        //  checks if the slider is complete
        if(tier != -1 && tier == maxTier && tick == maxTicks) {
            slider.doColor(Color.white, .1f);
        }

        //  incs the tier if tick is full
        if(tick >= maxTicks) {
            setTier(tier + 1);
            temp = true;
            if(tier < maxTier) {
                tick = 0;
            }
        }

        //  updates slider
        slider.DOKill();
        slider.doValue((float)tick / maxTicks, .1f, false);
        slider.setText(tick.ToString());
        return temp;
    }
    public bool incTick() {
        return setTick(tick + 1);
    }

    public bool canIncrease() {
        return !(tier >= maxTier && tick >= maxTicks);
    }
}
