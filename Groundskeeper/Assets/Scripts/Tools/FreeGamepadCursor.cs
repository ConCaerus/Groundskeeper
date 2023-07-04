using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class FreeGamepadCursor : MonoBehaviour {
    Mouse vMouse;
    Mouse curMouse;
    [SerializeField] RectTransform canvasRect;
    [SerializeField] Canvas canvas;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] RectTransform cursorTrans;
    [SerializeField] Image cursorImage;

    [SerializeField] float curSpeed = 1000f;

    [SerializeField] float padding = 35f;

    bool lPrevState, rPrevState;
    const string gamepadscheme = "Gamepad";
    const string keyboardScheme = "Keyboard";
    string prevControls = "";

    MouseManager mm;

    Vector2 prevPos;

    /*
    private void Update() {
        if(prevControls != playerInput.currentControlScheme && vMouse != null) {
            onControlsChanged(playerInput);
        }
        prevControls = playerInput.currentControlScheme;
    }*/


    private void Start() {
        mm = FindObjectOfType<MouseManager>();
        mm.addOnInputChangeFunc(changeCursor);
    }


    private void OnEnable() {
        mm = FindObjectOfType<MouseManager>();
        prevControls = keyboardScheme;
        if(vMouse == null) {
            vMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if(vMouse.added) {
            InputSystem.AddDevice(vMouse);
        }
        curMouse = Mouse.current;

        InputUser.PerformPairingWithDevice(vMouse, playerInput.user);

        if(cursorTrans != null) {
            Vector2 pos = cursorTrans.anchoredPosition;
            InputState.Change(vMouse.position, pos);
        }

        InputSystem.onAfterUpdate += updateMotion;
    }

    private void OnDisable() {
        if(vMouse != null && vMouse.added)
            InputSystem.RemoveDevice(vMouse);
        InputSystem.onAfterUpdate -= updateMotion;
    }

    void updateMotion() {
        if(vMouse == null || Gamepad.current == null || mm.usingKeyboard() || cursorImage == null || Time.timeScale == 0f)
            return;
        Vector2 stickVal = Gamepad.current.rightStick.ReadValue();
        stickVal *= curSpeed * Time.deltaTime;

        Vector2 curPos = vMouse.position.ReadValue();
        Vector2 newPos = curPos + stickVal;

        newPos.x = Mathf.Clamp(newPos.x, padding, Screen.width - padding);
        newPos.y = Mathf.Clamp(newPos.y, padding, Screen.height - padding);

        InputState.Change(vMouse.position, newPos);
        InputState.Change(vMouse.delta, stickVal);

        bool clicked = Gamepad.current.aButton.IsPressed();
        bool rightClicked = Gamepad.current.xButton.IsPressed();
        if(lPrevState != clicked) {
            vMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, clicked);
            InputState.Change(vMouse, mouseState);
            lPrevState = clicked;
        }
        else if(rPrevState != rightClicked) {
            vMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Right, rightClicked);
            InputState.Change(vMouse, mouseState);
            rPrevState = rightClicked;
        }

        anchorCursor(newPos);
    }
    void changeCursor(bool usingKeyboard) {
        if(cursorImage == null)
            return;

        if(!usingKeyboard) {
            if(vMouse == null) {
                vMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            }
            else if(vMouse.added) {
                InputSystem.AddDevice(vMouse);
            }
            curMouse = Mouse.current;

            InputUser.PerformPairingWithDevice(vMouse, playerInput.user);

            if(cursorTrans != null) {
                Vector2 pos = cursorTrans.anchoredPosition;
                InputState.Change(vMouse.position, pos);
            }
        }

        onControlsChanged(playerInput);
        cursorImage.enabled = !usingKeyboard;
        cursorTrans.gameObject.GetComponent<Image>().enabled = !usingKeyboard;
    }

    //  when hiding (and moveCursor is true) if b is true, the cursor will move out of the way of all UI and move to (0, 0)
    //  when showing (and moveCursor is true) if b is false, the cursor will move to the previously stored position (stored when hiding)
    public void showCursor(bool b, bool moveCursor) {
        if(cursorImage != null && mm != null && !mm.usingKeyboard()) {
            cursorImage.enabled = b;
            if(moveCursor) {
                if(!b && vMouse != null) {
                    prevPos = vMouse.position.ReadValue();
                    InputState.Change(vMouse.position, !b ? Vector2.zero : prevPos);
                }
            }
        }
    }
    public void destroyCursor() {
        Destroy(cursorImage);
    }

    void anchorCursor(Vector2 pos) {
        if(cursorImage == null)
            return;
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pos, canvas.renderMode
            == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPos);
        cursorTrans.anchoredPosition = anchoredPos;
    }

    void onControlsChanged(PlayerInput pl) {
        if(cursorImage == null || mm.usingKeyboard())
            return;
        cursorTrans.gameObject.GetComponent<Image>().enabled = false;
        if(pl.currentControlScheme == keyboardScheme && prevControls != keyboardScheme && vMouse != null) {
            curMouse.WarpCursorPosition(vMouse.position.ReadValue());
            prevControls = keyboardScheme;
        }
        else if(pl.currentControlScheme == gamepadscheme && prevControls != gamepadscheme && vMouse != null) {
            cursorTrans.gameObject.GetComponent<Image>().enabled = true;
            InputState.Change(vMouse.position, curMouse.position.ReadValue());
            anchorCursor(curMouse.position.ReadValue());
            prevControls = gamepadscheme;
        }
    }

    public Vector2 getScreenCursorPos() {
        return cursorTrans.transform.position;
    }
    public Vector2 getWorldCursorPos() {
        return Camera.main.ScreenToWorldPoint(cursorTrans.transform.position);
    }
}
