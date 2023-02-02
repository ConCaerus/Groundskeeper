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
    Transform pt;
    RectTransform ssrt, csrt;
    CircularSlider sscs, cscs;
    PlayerInstance pi;

    private void Start() {
        pt = FindObjectOfType<PlayerInstance>().transform;
        stamSlider.GetComponent<RectTransform>().transform.position = (Vector2)pt.position + stamOffset;
        chargeSlider.GetComponent<RectTransform>().transform.position = (Vector2)pt.position + chargeOffset;
        ssrt = stamSlider.GetComponent<RectTransform>();
        csrt = chargeSlider.GetComponent<RectTransform>();
        sscs = stamSlider.GetComponent<CircularSlider>();
        cscs = chargeSlider.GetComponent<CircularSlider>();
        pi = FindObjectOfType<PlayerInstance>();
    }

    private void LateUpdate() {
        if(pi == null)
            return;

        var stamTarget = (Vector2)pt.position + stamOffset;
        var charTarget = (Vector2)pt.position + chargeOffset;
        ssrt.transform.position = Vector2.Lerp(ssrt.transform.position, stamTarget, sliderMoveSpeed * Time.deltaTime);
        csrt.transform.position = Vector2.Lerp(csrt.transform.position, charTarget, sliderMoveSpeed * Time.deltaTime);
    }

    public void updateStamSlider(float val, float maxVal) {
        //  player is in the darkess O.O
        if(pi.gameObject.transform.GetChild(0).transform.lossyScale.x < 1.0f) {
            ssrt.DOKill();
            ssrt.DOScale(0f, .075f);
            stamDis = null;
            return;
        }
        if(val < maxVal) {
            ssrt.DOKill();
            ssrt.DOScale(.01f, .15f);
            sscs.setValue(val / maxVal);
        }
        if(val == maxVal && stamDis == null) {
            stamDis = StartCoroutine(stamWaitToDisappear());
            sscs.setValue(maxVal);
        }
        else if(val < maxVal && stamDis != null) {
            StopCoroutine(stamDis);
            stamDis = null;
        }
    }

    public void updateChargeSlider(float val, float maxVal) {
        //  player is in the darkess O.O
        if(pi.gameObject.transform.GetChild(0).transform.lossyScale.x < 1.0f) {
            csrt.DOKill();
            csrt.DOScale(0f, .075f);
            charDis = null;
            return;
        }

        if(val > 0f) {
            csrt.DOKill();
            csrt.DOScale(.01f, .15f);
            cscs.setValue(val / maxVal);
            cscs.setValue(val / maxVal);
            if(val == maxVal)
                cscs.doColor(Color.white, .15f);
            else
                cscs.resetColor();
        }
        if(charDis != null)
            StopCoroutine(charDis);
        if(val == 0f)
            charDis = StartCoroutine(charWaitToDisappear());
    }

    IEnumerator stamWaitToDisappear() {
        yield return new WaitForSeconds(.5f);
        ssrt.DOKill();
        ssrt.DOScale(0f, .25f);
        stamDis = null;
    }

    IEnumerator charWaitToDisappear() {
        float i = cscs.value;
        DOTween.To(() => i, x => i = x, 0f, .2f).OnUpdate(() => {
            cscs.setValue(i / 1.0f);
        });

        yield return new WaitForSeconds(.25f);
        csrt.DOKill();
        csrt.DOScale(0f, .25f);
        charDis = null;
    }
}
