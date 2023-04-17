using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {
    InputMaster controls;

    public delegate void func(bool usingKeyboard);

    List<func> changeFuncs = new List<func>();


    private void Awake() {
        controls = new InputMaster();
        controls.Enable();
        controls.InputSwitch.Mouse.started += ctx => switchToKeyboard();
        controls.InputSwitch.Gamepad.started += ctx => switchToGamepad();
        controls.UI.Click.canceled += ctx => checkUIOptions();
        controls.UI.Submit.canceled += ctx => checkUIOptions();
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
            foreach(var i in changeFuncs)
                i(usingKeyboard());
            if(EventSystem.current.currentSelectedGameObject == null && FindObjectOfType<Selectable>() != null)
                FindObjectOfType<Selectable>().Select();
        }
    }

    void switchToKeyboard() {
        if(!Cursor.visible) {
            Cursor.visible = true;
            foreach(var i in changeFuncs)
                i(usingKeyboard());
            if(EventSystem.current.currentSelectedGameObject == null && FindObjectOfType<Selectable>() != null)
                FindObjectOfType<Selectable>().Select();
        }
    }

    void checkUIOptions() {
        if(!usingKeyboard() && EventSystem.current.currentSelectedGameObject == null && FindObjectOfType<Selectable>() != null) {
            //FindObjectOfType<Selectable>().Select();
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

    private void OnDisable() {
        controls.Disable();
    }
}
