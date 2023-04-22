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

    [SerializeField] float curSpeed = 1000f;

    [SerializeField] float padding = 35f;

    bool prevState;
    const string gamepadscheme = "Gamepad";
    const string keyboardScheme = "Keyboard";
    string prevControls = "";

    /*
    private void Update() {
        if(prevControls != playerInput.currentControlScheme && vMouse != null) {
            onControlsChanged(playerInput);
        }
        prevControls = playerInput.currentControlScheme;
    }*/


    private void OnEnable() {
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
        playerInput.onControlsChanged += onControlsChanged;
    }

    private void OnDisable() {
        if(vMouse != null && vMouse.added)
            InputSystem.RemoveDevice(vMouse);
        InputSystem.onAfterUpdate -= updateMotion;
        playerInput.onControlsChanged -= onControlsChanged;
    }

    void updateMotion() {
        if(vMouse == null || Gamepad.current == null)
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
        if(prevState != clicked) {
            vMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, clicked);
            InputState.Change(vMouse, mouseState);
            prevState = clicked;
        }

        anchorCursor(newPos);
    }

    void anchorCursor(Vector2 pos) {
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pos, canvas.renderMode
            ==  RenderMode.ScreenSpaceOverlay ? null : Camera.main, out anchoredPos);
        cursorTrans.anchoredPosition = anchoredPos;
    }

    void onControlsChanged(PlayerInput pl) {
        if(pl.currentControlScheme == keyboardScheme && prevControls != keyboardScheme && vMouse != null) {
            Cursor.visible = true;
            curMouse.WarpCursorPosition(vMouse.position.ReadValue());
            cursorTrans.gameObject.GetComponent<Image>().enabled = false;
            prevControls = keyboardScheme;
        }
        else if(pl.currentControlScheme == gamepadscheme && prevControls != gamepadscheme) {
            cursorTrans.gameObject.GetComponent<Image>().enabled = true;
            Cursor.visible = false;
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
