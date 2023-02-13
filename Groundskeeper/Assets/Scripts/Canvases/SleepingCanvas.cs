using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SleepingCanvas : MonoBehaviour {
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI sleepyText;
    [TextArea]
    [SerializeField] string sleepText;

    private void Start() {
        background.SetActive(true);
        background.transform.localScale = Vector3.zero;
    }


    public void goToSleep() {
        StartCoroutine(sleepAnim());
    }


    IEnumerator sleepAnim() {
        sleepyText.text = "";
        background.transform.DOScale(1.0f, .25f);

        yield return new WaitForSeconds(1.0f);
        
        foreach(var i in sleepText) {
            sleepyText.text += i;
            yield return new WaitForSeconds(i == 'Z' ? .25f : i == '.' ? .15f : .25f);
        }
        yield return new WaitForSeconds(.5f);

        background.transform.DOScale(0.0f, .15f);

        yield return new WaitForSeconds(.5f);
        FindObjectOfType<BedInteractable>().finshedSleeping();
    }
}
