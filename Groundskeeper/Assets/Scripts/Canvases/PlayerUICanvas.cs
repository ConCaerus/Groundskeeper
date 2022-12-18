using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerUICanvas : MonoBehaviour {
    Vector2 stamOffset = new Vector2(-1.9f, 1.6f);
    Vector2 chargeOffset = new Vector2(1.9f, 1.6f);
    float sliderMoveSpeed = 23.0f;
    [SerializeField] GameObject stamSlider, chargeSlider;

    Coroutine stamDis = null, charDis = null;

    public void updateStamSlider(float val, float maxVal) {
        //  player is in the darkess O.O
        if(FindObjectOfType<PlayerInstance>().gameObject.transform.GetChild(0).transform.lossyScale.x < 1.0f) {
            stamSlider.GetComponent<RectTransform>().DOKill();
            stamSlider.GetComponent<RectTransform>().DOScale(0f, .075f);
            stamDis = null;
            return;
        }
        if(val < maxVal) {
            stamSlider.GetComponent<RectTransform>().DOKill();
            stamSlider.GetComponent<RectTransform>().DOScale(.01f, .15f);
            stamSlider.GetComponent<CircularSlider>().setValue(val / maxVal);
        }
        if(val == maxVal && stamDis == null) {
            stamDis = StartCoroutine(stamWaitToDisappear());
            stamSlider.GetComponent<CircularSlider>().setValue(maxVal);
        }
        else if(val < maxVal && stamDis != null) {
            StopCoroutine(stamDis);
            stamDis = null;
        }
    }

    public void updateChargeSlider(float val, float maxVal) {
        //  player is in the darkess O.O
        if(FindObjectOfType<PlayerInstance>().gameObject.transform.GetChild(0).transform.lossyScale.x < 1.0f) {
            chargeSlider.GetComponent<RectTransform>().DOKill();
            chargeSlider.GetComponent<RectTransform>().DOScale(0f, .075f);
            charDis = null;
            return;
        }

        if(val > 0f) {
            chargeSlider.GetComponent<RectTransform>().DOKill();
            chargeSlider.GetComponent<RectTransform>().DOScale(.01f, .15f);
            chargeSlider.GetComponent<CircularSlider>().setValue(val / maxVal);
            chargeSlider.GetComponent<CircularSlider>().setValue(val / maxVal);
            if(val == maxVal)
                chargeSlider.GetComponent<CircularSlider>().doColor(Color.white, .15f);
            else
                chargeSlider.GetComponent<CircularSlider>().resetColor();
        }
        if(charDis != null)
            StopCoroutine(charDis);
        if(val == 0f)
            charDis = StartCoroutine(charWaitToDisappear());
    }

    private void Start() {
        stamSlider.GetComponent<RectTransform>().transform.position = (Vector2)FindObjectOfType<PlayerInstance>().transform.position + stamOffset;
        chargeSlider.GetComponent<RectTransform>().transform.position = (Vector2)FindObjectOfType<PlayerInstance>().transform.position + chargeOffset;
    }

    private void LateUpdate() {
        if(FindObjectOfType<PlayerInstance>() == null)
            return;

        var stamTarget = (Vector2)FindObjectOfType<PlayerInstance>().transform.position + stamOffset;
        var charTarget = (Vector2)FindObjectOfType<PlayerInstance>().transform.position + chargeOffset;
        stamSlider.GetComponent<RectTransform>().transform.position = Vector2.Lerp(stamSlider.GetComponent<RectTransform>().transform.position, stamTarget, sliderMoveSpeed * Time.deltaTime);
        chargeSlider.GetComponent<RectTransform>().transform.position = Vector2.Lerp(chargeSlider.GetComponent<RectTransform>().transform.position, charTarget, sliderMoveSpeed * Time.deltaTime);
    }

    IEnumerator stamWaitToDisappear() {
        yield return new WaitForSeconds(.5f);
        stamSlider.GetComponent<RectTransform>().DOKill();
        stamSlider.GetComponent<RectTransform>().DOScale(0f, .25f);
        stamDis = null;
    }

    IEnumerator charWaitToDisappear() {
        float i = chargeSlider.GetComponent<CircularSlider>().value;
        DOTween.To(() => i, x => i = x, 0f, .2f).OnUpdate(() => {
            chargeSlider.GetComponent<CircularSlider>().setValue(i / 1.0f);
        });

        yield return new WaitForSeconds(.25f);
        chargeSlider.GetComponent<RectTransform>().DOKill();
        chargeSlider.GetComponent<RectTransform>().DOScale(0f, .25f);
        charDis = null;
    }
}
