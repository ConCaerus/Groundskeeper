using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CutsceneSkipCanvas : MonoBehaviour {
    [SerializeField] CircularSlider slider;

    InputMaster controls;

    Coroutine hider = null;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Dialog.HardSkip.performed += ctx => {
            slider.gameObject.SetActive(true);
            slider.doValueKill();
            slider.doValue(1.0f, 1f - slider.value, false, delegate { FindObjectOfType<TransitionCanvas>().loadScene("Game"); });
        };
        controls.Dialog.HardSkip.canceled += ctx => {
            if(slider.value >= 1.0f)
                return;
            slider.doValueKill();
            slider.doValue(0f, slider.value * 1f, false, delegate { slider.gameObject.SetActive(false); });
        };
        slider.gameObject.SetActive(false);

        FindObjectOfType<MouseManager>().addOnInputChangeFunc(changeText);
    }

    void changeText(bool usingKeyboard) {
        slider.setText(usingKeyboard ? "Q" : "B");
    }

    private void Update() {
        if(Input.anyKeyDown && hider == null) {
            hider = StartCoroutine(hideAfterShowing());
        }
    }

    private void OnDisable() {
        controls.Disable();
    }

    IEnumerator hideAfterShowing() {
        slider.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        if(slider.value == 0f)
            slider.gameObject.SetActive(false);
        hider = null;
    }
}
