using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text, otherText;
    [SerializeField] GameObject box, hidePos;
    float secondsBtwLetters = 0.01f;
    bool skip = false;
    bool showingText = false;

    DialogText currentTexts;

    Coroutine anim = null;

    public delegate void func();
    func f = null;
    InputMaster controls;
    [SerializeField] CircularSlider hardSkipSlider;
    Coroutine hardSkiper = null;


    private void Start() {
        DOTween.Init();
        controls = new InputMaster();
        controls.Enable();
        controls.Dialog.Advance.performed += ctx => advance();
        controls.Dialog.HardSkip.performed += ctx => {
            if(hardSkiper != null) {
                StopCoroutine(hardSkiper);
                hardSkipSlider.setValue(0.0f);
            }
            hardSkiper = StartCoroutine(hardSkip());
        };
        controls.Dialog.HardSkip.canceled += ctx => {
            if(hardSkiper != null)
                StopCoroutine(hardSkiper);
            hardSkipSlider.doValueKill();
            //hardSkipSlider.setValue(0.0f);
        };
        hardHide();
        otherText.gameObject.SetActive(false);
        hardSkipSlider.setValue(0.0f);
    }


    private void OnDisable() {
        controls.Disable();
    }


    void advance() {
        if(!skip && anim != null)
            skip = true;
        if(anim == null && showingText) {
            currentTexts.dialogs.RemoveAt(0);
            if(currentTexts.dialogs.Count <= 0) {
                hide();
                if(f != null)
                    f();
            }
            else
                showText(currentTexts.dialogs[0]);
        }
    }

    IEnumerator hardSkip() {
        yield return new WaitForSeconds(.5f);

        hardSkipSlider.doValue(1.0f, 1.0f, delegate {
            hide();
            if(f != null)
                f();
        });
        hardSkiper = null;
    }


    public void showText(string s) {
        otherText.gameObject.SetActive(false);
        if(anim != null)
            StopCoroutine(anim);
        text.text = "";
        show();
        anim = StartCoroutine(textAnimator(s));
    }
    public void loadDialogText(DialogText t, func runOnDone) {
        currentTexts = t;
        showText(currentTexts.dialogs[0]);
        f = runOnDone;
    }


    IEnumerator textAnimator(string s) {
        yield return new WaitForFixedUpdate();
        skip = false;
        showingText = true;
        foreach(var i in s) {
            if(skip) {
                text.text = s;
                break;
            }
            if(i == ' ') {
                text.text = text.text + i;
                continue;
            }
            text.text = text.text + i;
            //  play sound
            yield return new WaitForSeconds(secondsBtwLetters);
        }

        anim = null;
        StartCoroutine(otherTextAnimator());
    }

    IEnumerator otherTextAnimator() {
        otherText.gameObject.SetActive(true);
        otherText.transform.localScale = Vector3.one;
        while(true) {
            otherText.transform.DOScale(.8f, .25f);
            yield return new WaitForSeconds(.5f);
            otherText.transform.DOScale(1f, .25f);
            yield return new WaitForSeconds(.5f);
        }
    }


    public void show() {
        box.transform.DOKill();
        box.transform.DOMoveY(0.0f, .15f);
    }
    public void hide() {
        box.transform.DOKill();
        box.transform.DOMoveY(hidePos.transform.position.y, .25f);
        showingText = false;
    }
    public void hardHide() {
        box.transform.DOKill();
        box.transform.position = new Vector3(box.transform.position.x, hidePos.transform.position.y);
        showingText = false;
    }
}

public class DialogText {
    public List<string> dialogs = new List<string>();

    public DialogText(List<string> s) {
        dialogs = s;
    }
}
