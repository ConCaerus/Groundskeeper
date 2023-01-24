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
    public int tier { get; private set; } = 0;
    public Buyable.buyType mainType = Buyable.buyType.None;

    public void setTitle(string name) {
        title.text = name;
    }
    public void setCost(int c) {
        cost = c;
        if(c > 0)
            costText.text = c.ToString() + "s";
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
        float endScale = 1.0f;
        if(sub)
            endScale = .5f + .15f * tier;
        transform.GetChild(0).transform.DOScale(endScale, .1f);
    }

    public void setTier(int t, int maxTier) {
        if(t < maxTier && t > -1) {
            tier = t;
            if(tier == 0)
                tierText.text = "I";
            else if(tier == 1)
                tierText.text = "II";
            else if(tier == 2)
                tierText.text = "III";
            else if(tier == 3)
                tierText.text = "IV";
            else if(tier == 4)
                tierText.text = "V";
            else if(tier == 5)
                tierText.text = "VI";
            else if(tier == 6)
                tierText.text = "VII";
            else if(tier == 7)
                tierText.text = "VIII";
            else if(tier == 8)
                tierText.text = "IX";
            else if(tier == 9)
                tierText.text = "X";
        }
        else if(t == -1)
            tierText.text = "";
        else if(t >= maxTier)
            tier = 100;  //  just to make sure that the code knows that tier is not usable
    }

    public bool canIncrease(int maxTier) {
        return !(tier >= maxTier && slider.value >= 1f);
    }
}
