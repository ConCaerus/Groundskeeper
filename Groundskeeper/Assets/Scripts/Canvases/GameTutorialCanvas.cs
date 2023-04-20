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

        if(GameInfo.getNightCount() == 0)
            FindObjectOfType<MouseManager>().addOnInputChangeFunc(changeTextType);
    }

    public void show() {
        movementText.DOColor(Color.white, .15f);
        attackingText.DOColor(Color.white, .15f);
        sprintingText.DOColor(Color.white, .15f);
        chargeAttackText.DOColor(Color.white, .15f);
        shown = true;
    }

    public void hasMoved() {
        if(movementText.color.a < 1f || !shown || FindObjectOfType<SetupSequenceManager>().isActiveAndEnabled)
            return;
        movementText.DOColor(Color.clear, .5f);
        has[0] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasSprinted() {
        if(sprintingText.color.a < 1f || !shown || FindObjectOfType<SetupSequenceManager>().isActiveAndEnabled)
            return;
        sprintingText.DOColor(Color.clear, .5f);
        has[1] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasAttacked() {
        if(attackingText.color.a < 1f || !shown || FindObjectOfType<SetupSequenceManager>().isActiveAndEnabled)
            return;
        attackingText.DOColor(Color.clear, .5f);
        has[2] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }
    public void hasChargedAttacked() {
        if(chargeAttackText.color.a < 1f || !shown || FindObjectOfType<SetupSequenceManager>().isActiveAndEnabled)
            return;
        chargeAttackText.DOColor(Color.clear, .5f);
        has[3] = true;
        StopAllCoroutines();
        StartCoroutine(finish());
    }

    public void changeTextType(bool usingKeyboard) {
        if(usingKeyboard) {
            movementText.text = "Move:\nW\nA S D";
            attackingText.text = "Attack:\nM1 (tap)";
            sprintingText.text = "Sprint:\nL-Shift";
            chargeAttackText.text = "Charge Attack:\nM1 (hold)";
        } else {
            movementText.text = "Move:\nL-Stick";
            attackingText.text = "Attack:\nR-Trigger";
            sprintingText.text = "Sprint:\nL-Bumper";
            chargeAttackText.text = "Charge Attack:\nR-Trigger\n(hold)";
        }
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
