using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class FreeGamepadHouseCursor : MonoBehaviour {
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

    MouseManager mm;
    BuyTreeCanvas btc;

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
        btc = FindObjectOfType<BuyTreeCanvas>();
        showCursor(false, false);
    }


    private void OnEnable() {
        mm = FindObjectOfType<MouseManager>();
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
        if(vMouse == null || Gamepad.current == null || mm.usingKeyboard() || cursorImage == null)
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
        //onControlsChanged(playerInput);
        cursorImage.enabled = !usingKeyboard && btc.isOpen();
    }

    //  when hiding (and moveCursor is true) if b is true, the cursor will move out of the way of all UI and move to (0, 0)
    //  when showing (and moveCursor is true) if b is false, the cursor will move to the previously stored position (stored when hiding)
    public void showCursor(bool b, bool moveCursor) {
        if(cursorImage != null) {
            cursorImage.enabled = b && btc.isOpen();
            if(moveCursor) {
                if(!b)
                    prevPos = vMouse.position.ReadValue();
                InputState.Change(vMouse.position, !b ? Vector2.zero : prevPos);
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
            ==  RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPos);
        cursorTrans.anchoredPosition = anchoredPos;
    }

    public Vector2 getScreenCursorPos() {
        return cursorTrans.transform.position;
    }
    public Vector2 getWorldCursorPos() {
        return Camera.main.ScreenToWorldPoint(cursorTrans.transform.position);
    }
}
