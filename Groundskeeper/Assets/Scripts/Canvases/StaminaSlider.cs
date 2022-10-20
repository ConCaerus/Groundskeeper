using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StaminaSlider : MonoBehaviour {
    Vector2 offset = new Vector2(-1.9f, 1.6f);

    Coroutine disapearer = null;

    public void updateSlider(float val, float maxVal) {
        //  player is in the darkess O.O
        if(FindObjectOfType<PlayerInstance>().gameObject.transform.GetChild(0).transform.lossyScale.x < 1.0f) {
            transform.GetChild(0).GetComponent<RectTransform>().DOKill();
            transform.GetChild(0).GetComponent<RectTransform>().DOScale(0f, .075f);
            disapearer = null;
            return;
        }
        if(val < maxVal) {
            transform.GetChild(0).GetComponent<RectTransform>().DOKill();
            transform.GetChild(0).GetComponent<RectTransform>().DOScale(.01f, .15f);
            transform.GetChild(0).GetComponent<CircularSlider>().setValue(val / maxVal);
        }
        if(val == maxVal && disapearer == null)
            disapearer = StartCoroutine(waitToDisapear());
        else if(val < maxVal && disapearer != null) {
            StopCoroutine(disapearer);
            disapearer = null;
        }
    }

    private void LateUpdate() {
        if(FindObjectOfType<PlayerInstance>() == null)
            return;
        transform.GetChild(0).GetComponent<RectTransform>().transform.position = (Vector2)FindObjectOfType<PlayerInstance>().transform.position + offset;
    }

    IEnumerator waitToDisapear() {
        yield return new WaitForSeconds(.5f);
        transform.GetChild(0).GetComponent<RectTransform>().DOKill();
        transform.GetChild(0).GetComponent<RectTransform>().DOScale(0f, .25f);
        disapearer = null;
    }
}
