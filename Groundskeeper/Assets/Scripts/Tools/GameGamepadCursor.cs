using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGamepadCursor : MonoBehaviour {
    [SerializeField] GameObject player;

    [SerializeField] float rangeMod = 10f;

    InputMaster controls;

    Image image;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        image = GetComponent<Image>();
        changeCursor(true);
    }

    public void setup() {
        controls.Player.Aim.performed += ctx => aimCursor(ctx.ReadValue<Vector2>());
        controls.Player.Aim.started += ctx => cursorChange(true);
        controls.Player.Aim.canceled += ctx => cursorChange(false);

        FindObjectOfType<MouseManager>().addOnInputChangeFunc(changeCursor);
        changeCursor(FindObjectOfType<MouseManager>().usingKeyboard());
    }

    public void changeCursor(bool usingKeyboard) {
        image.enabled = !usingKeyboard && Time.timeScale != 0f;
    }

    void cursorChange(bool showCursor) {
        image.enabled = showCursor && Time.timeScale != 0f;
    }

    void aimCursor(Vector2 dir) {
        if(Time.timeScale == 0f)
            return;
        var worldTarget = (Vector2)player.transform.position + dir * rangeMod;
        transform.position = Camera.main.WorldToScreenPoint(worldTarget);
    }

    public Vector2 getMousePosInWorld() {
        var pos = transform.position;
        return Camera.main.ScreenToWorldPoint(pos);
    }
    public Vector2 getMousePosInScreen() {
        return transform.position;
    }


    private void OnDisable() {
        if(controls != null)
            controls.Disable();
    }
}
