using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamepadSliderManipulator : MonoBehaviour {
    MouseManager mm;
    Slider slider;
    InputMaster controls;

    int tickStep = 20;

    float changeAmt = 0f;
    bool changing = false;

    private void Start() {
        mm = FindObjectOfType<MouseManager>();
        controls = new InputMaster();
        controls.Enable();
        //controls.Player.Aim.performed += ctx => updateChangeAmt(ctx.ReadValue<Vector2>());
        //controls.Player.Aim.canceled += ctx => stopChanging();
        slider = GetComponent<Slider>();
    }


    private void Update() {
        if(changing && changeAmt != 0f)
            slider.value += changeAmt;
    }

    private void OnDisable() {
        controls.Disable();
    }

    void updateChangeAmt(Vector2 dir) {
        changing = true;
        changeAmt = dir.x / tickStep;
    }

    void stopChanging() {
        changing = false;
        changeAmt = 0f;
    }
}
