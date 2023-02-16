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

    Coroutine funcWaiter = null;

    Image fillImg;

    public delegate void func();

    public float value { get; private set; } = 0.0f;

    private void Awake() {
        fillImg = fill.GetComponent<Image>();
        fillImg.fillAmount = value;
        startColor = fillImg.color;
    }

    public void setValue(float f) {
        value = Mathf.Clamp(f, 0.0f, 1.0f);
        fillImg.fillAmount = value;
    }
    public void doValue(float f, float dur, func runOnDone = null) {
        if(funcWaiter != null)
            StopCoroutine(funcWaiter);
        funcWaiter = StartCoroutine(runWhenDone(runOnDone, dur, f, value));
        /*
        //  uses a temp variable to animate the slider
        var i = DOTween.To(() => tempValue, x => tempValue = x, f, dur).OnUpdate(() => {
            fill.GetComponent<Image>().fillAmount = tempValue;
        });
        */
    }
    public void doValueKill() {
        if(funcWaiter != null)
            StopCoroutine(funcWaiter);
    }

    public void setText(string t) {
        text.text = t;
    }

    public void setColor(Color c) {
        fillImg.color = c;
    }
    public void doColor(Color c, float dur) {
        fillImg.DOColor(c, dur);
    }
    public void resetColor() {
        fillImg.DOKill();
        setColor(startColor);
    }

    IEnumerator runWhenDone(func f, float dur, float endVal, float startVal) {
        float elapsedTime = 0.0f;
        float incTime = 0.01f;

        while(elapsedTime < dur) {
            yield return new WaitForSeconds(incTime);
            elapsedTime += incTime;
            value = ((elapsedTime / dur) * (startVal == 0.0f ? 1.0f : (endVal - startVal))) + startVal;
            fillImg.fillAmount = value;
        }

        yield return new WaitForSeconds(dur);
        if(f != null)
            f();
        funcWaiter = null;
    }
}
