using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text, otherText;
    [SerializeField] GameObject box, hidePos;
    [SerializeField] GameObject lEye, rEye;
    [SerializeField] List<eyeExpressionPos> eyePoses;
    [SerializeField] GameObject faceObj;
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

    [SerializeField] AudioClip devilVoice;

    [System.Serializable]
    struct eyeExpressionPos {
        public Vector2 lPos, rPos;
    }


    private void Start() {
        DOTween.Init();
        controls = new InputMaster();
        controls.Enable();
        controls.Dialog.Advance.performed += ctx => advance();
        controls.Dialog.HardSkip.performed += ctx => {
            if(!showingText)
                return;
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
            hardSkipSlider.setValue(0.0f);
        };
        hardHide();
        otherText.gameObject.SetActive(false);
        hardSkipSlider.setValue(0.0f);
    }




    void advance() {
        if(!showingText)
            return;
        FindObjectOfType<AudioManager>().playSound(devilVoice, Camera.main.transform.position);
        faceObj.transform.DOComplete();
        faceObj.transform.DOPunchPosition(new Vector3(0.0f, 10.0f), .25f);
        if(!skip && anim != null)
            skip = true;
        if(anim == null && showingText) {
            currentTexts.dialogs.RemoveAt(0);
            currentTexts.expressions.RemoveAt(0);
            if(currentTexts.dialogs.Count <= 0) {
                hide();
                if(f != null)
                    f();
            }
            else
                showText(currentTexts.dialogs[0], currentTexts.expressions[0]);
        }
    }

    IEnumerator hardSkip() {
        yield return new WaitForSeconds(.5f);

        hardSkipSlider.doValue(1.0f, .5f, true, delegate {
            hide();
            if(f != null)
                f();
        });
        hardSkiper = null;
    }


    public void showText(string s, DialogText.facialExpression e) {
        otherText.gameObject.SetActive(false);
        if(anim != null)
            StopCoroutine(anim);
        text.text = "";
        show();
        anim = StartCoroutine(textAnimator(s));

        //  eye stuff
        rEye.transform.DOKill();
        lEye.transform.DOKill();
        rEye.transform.DOLocalMove(eyePoses[(int)e].rPos, .15f);
        lEye.transform.DOLocalMove(eyePoses[(int)e].lPos, .15f);
    }
    public void loadDialogText(DialogText t, func runOnDone) {
        FindObjectOfType<AudioManager>().playSound(devilVoice, Camera.main.transform.position);
        currentTexts = t;
        showText(currentTexts.dialogs[0], currentTexts.expressions[0]);
        f = runOnDone;
    }


    IEnumerator textAnimator(string s) {
        yield return new WaitForFixedUpdate();
        skip = false;
        showingText = true;
        bool inTextMod = false;
        foreach(var i in s) {
            if(skip) {
                text.text = s;
                break;
            }
            if(i == ' ') {
                text.text = text.text + i;
                continue;
            }

            //  skip over text modifiers like color changers
            if(i == '<')
                inTextMod = true;

            text.text = text.text + i;
            //  play sound
            if(!inTextMod)
                yield return new WaitForSeconds(secondsBtwLetters);
            else if(inTextMod && i == '>')
                inTextMod = false;
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


    private void OnEnable() {
        FindObjectOfType<MouseManager>().addOnInputChangeFunc(switchInputType);
    }
    private void OnDisable() {
        controls.Disable();
    }


    public void switchInputType(bool usingKeyboard) {
        hardSkipSlider.setText(usingKeyboard ? "Q" : "B");
    }
}

public class DialogText {
    public enum facialExpression {
        normal, dismissive, thinking, happy
    }

    public List<string> dialogs = new List<string>();
    public List<facialExpression> expressions = new List<facialExpression>();

    public DialogText(List<string> s, List<facialExpression> e) {
        dialogs = s;
        expressions = e;
    }
}
