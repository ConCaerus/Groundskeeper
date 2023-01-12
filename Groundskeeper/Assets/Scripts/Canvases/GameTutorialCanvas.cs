using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameTutorialCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI movementText, sprintingText, attackingText;
    bool[] has = new bool[3] { false, false, false };


    private void Start() {
        gameObject.SetActive(GameInfo.getNightCount() == 0);
    }

    public void hasMoved() {
        if(movementText.color.a < 1f)
            return;
        movementText.DOColor(Color.clear, .5f);
        has[0] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasSprinted() {
        if(sprintingText.color.a < 1f)
            return;
        sprintingText.DOColor(Color.clear, .5f);
        has[1] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasAttacked() {
        if(attackingText.color.a < 1f)
            return;
        attackingText.DOColor(Color.clear, .5f);
        has[2] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }

    IEnumerator finish() {
        foreach(var i in has) {
            if(!i)
                yield break;
        }
        yield return new WaitForSeconds(.5f);
        gameObject.SetActive(false);
    }
}
