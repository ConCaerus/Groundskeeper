using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SleepingCanvas : MonoBehaviour {
    [SerializeField] GameObject background;

    private void Start() {
        background.transform.localScale = Vector3.zero;
    }


    public void goToSleep() {
        StartCoroutine(sleepAnim());
    }


    IEnumerator sleepAnim() {
        background.transform.DOScale(1.0f, .25f);

        yield return new WaitForSeconds(1.0f);

        background.transform.DOScale(0.0f, .15f);

        yield return new WaitForSeconds(.5f);
        FindObjectOfType<BedInteractable>().finshedSleeping();
    }
}
