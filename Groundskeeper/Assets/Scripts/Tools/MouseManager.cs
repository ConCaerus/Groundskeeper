using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour {
    InputMaster controls;

    public delegate void func(bool usingKeyboard);

    List<func> changeFuncs = new List<func>();


    private void Awake() {
        controls = new InputMaster();
        controls.InputSwitch.Gamepad.performed += ctx => switchToGamepad();
        controls.InputSwitch.GamepadStick.performed += ctx => switchToGamepad();
        controls.InputSwitch.Keyboard.performed += ctx => switchToKeyboard();
        controls.InputSwitch.Mouse.performed += ctx => switchToKeyboard();

        //  checks if the player is currently using a controller or not
        bool hasController = false;
        foreach(var i in Input.GetJoystickNames()) {
            if(!string.IsNullOrEmpty(i)) {
                hasController = true;
                break;
            }
        }
        Cursor.visible = !hasController;
    }

    void switchToGamepad() {
        if(Cursor.visible) {
            Cursor.visible = false;
            foreach(var i in changeFuncs)
                i(usingKeyboard());
        }
    }

    void switchToKeyboard() {
        if(!Cursor.visible) {
            Cursor.visible = true;
            foreach(var i in changeFuncs)
                i(usingKeyboard());
        }
    }

    public bool usingKeyboard() {
        return Cursor.visible;
    }


    public void addOnInputChangeFunc(func f) {
        changeFuncs.Add(f);
    }
    public void removeOnInputChangeFunc(func f) {
        changeFuncs.Remove(f);
    }

    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
}
