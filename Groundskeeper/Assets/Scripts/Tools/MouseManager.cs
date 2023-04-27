using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {
    InputMaster controls;

    public delegate void func(bool usingKeyboard);

    List<func> changeFuncs = new List<func>();

    FreeGamepadCursor fgc;
    FreeGamepadHouseCursor fghc;


    private void Awake() {
        fgc = FindObjectOfType<FreeGamepadCursor>();
        fghc = FindObjectOfType<FreeGamepadHouseCursor>();
        changeFuncs.Clear();
        controls = new InputMaster();
        controls.Enable();
        controls.InputSwitch.Mouse.started += ctx => switchToKeyboard();
        controls.InputSwitch.Keyboard.started += ctx => switchToKeyboard();
        controls.InputSwitch.Gamepad.started += ctx => switchToGamepad();
        bool hasController = false;
        foreach(var i in Input.GetJoystickNames()) {
            if(!string.IsNullOrEmpty(i)) {
                hasController = true;
                break;
            }
        }
        if(hasController)
            switchToKeyboard();
        else
            switchToGamepad();
    }

    void switchToGamepad() {
        if(Cursor.visible) {
            Cursor.visible = false;
            if(fgc != null && fgc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else if(fghc != null && fghc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else
                Cursor.lockState = CursorLockMode.Locked;
            foreach(var i in changeFuncs)
                i(false);
        }
    }

    void switchToKeyboard() {
        if(!Cursor.visible) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            foreach(var i in changeFuncs)
                i(true);
        }
    }

    public bool usingKeyboard() {
        return Cursor.visible;
    }


    public void addOnInputChangeFunc(func f) {
        changeFuncs.Add(f);
        //  run it right away
        f(usingKeyboard());
    }
    public void removeOnInputChangeFunc(func f) {
        changeFuncs.Remove(f);
    }

    private void OnDisable() {
        controls.Disable();
    }
}
