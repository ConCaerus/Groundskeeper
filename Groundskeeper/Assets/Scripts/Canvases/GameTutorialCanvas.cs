using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameTutorialCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI movementText, sprintingText, attackingText, chargeAttackText;
    bool[] has = new bool[4] { false, false, false, false };

    bool shown = false;

    private void Start() {
        gameObject.SetActive(GameInfo.getNightCount() == 0);
        movementText.color = Color.clear;
        attackingText.color = Color.clear;
        sprintingText.color = Color.clear;
        chargeAttackText.color = Color.clear;
    }

    public void show() {
        movementText.color = Color.white;
        attackingText.color = Color.white;
        sprintingText.color = Color.white;
        chargeAttackText.color = Color.white;
        shown = true;
    }

    public void hasMoved() {
        if(movementText.color.a < 1f || !shown)
            return;
        movementText.DOColor(Color.clear, .5f);
        has[0] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasSprinted() {
        if(sprintingText.color.a < 1f || !shown)
            return;
        sprintingText.DOColor(Color.clear, .5f);
        has[1] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasAttacked() {
        if(attackingText.color.a < 1f || !shown)
            return;
        attackingText.DOColor(Color.clear, .5f);
        has[2] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasChargedAttacked() {
        if(chargeAttackText.color.a < 1f || !shown)
            return;
        chargeAttackText.DOColor(Color.clear, .5f);
        has[3] = true;
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
