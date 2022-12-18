using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CircularSlider : MonoBehaviour {
    [SerializeField] GameObject fill;
    [SerializeField] TextMeshProUGUI text;
    Color startColor;

    public float value { get; private set; } = 0.0f;

    private void Start() {
        fill.GetComponent<Image>().fillAmount = value;
        startColor = fill.GetComponent<Image>().color;
    }

    public void setValue(float f) {
        value = Mathf.Clamp(f, 0.0f, 1.0f);
        fill.GetComponent<Image>().fillAmount = value;
    }
    public void doValue(float f, float dur) {
        var tempValue = value;
        value = Mathf.Clamp(f, 0.0f, 1.0f); //  sets the real value to the desired value

        //  uses a temp variable to animate the slider
        DOTween.To(() => tempValue, x => tempValue = x, f, dur).OnUpdate(() => {
            fill.GetComponent<Image>().fillAmount = tempValue;
        });
    }

    public void setText(string t) {
        text.text = t;
    }

    public void setColor(Color c) {
        fill.GetComponent<Image>().color = c;
    }
    public void doColor(Color c, float dur) {
        fill.GetComponent<Image>().DOColor(c, dur);
    }
    public void resetColor() {
        fill.GetComponent<Image>().DOKill();
        setColor(startColor);
    }
}
