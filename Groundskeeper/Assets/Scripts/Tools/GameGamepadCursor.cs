using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGamepadCursor : MonoBehaviour {
    [SerializeField] GameObject player;

    [SerializeField] float rangeMod = 10f;

    InputMaster controls;

    Image image;

    PlacementGrid pg;


    private void Awake() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Aim.performed += ctx => aimCursor(ctx.ReadValue<Vector2>());
        controls.Player.Aim.started += ctx => cursorChange(true);
        controls.Player.Aim.canceled += ctx => cursorChange(false);
        image = GetComponent<Image>();
        pg = FindObjectOfType<PlacementGrid>();

        FindObjectOfType<MouseManager>().addOnInputChangeFunc(changeCursor);
        changeCursor(FindObjectOfType<MouseManager>().usingKeyboard());
        changeCursor(true);
    }

    public void changeCursor(bool usingKeyboard) {
        image.enabled = !usingKeyboard;
    }

    void cursorChange(bool showCursor) {
        image.enabled = showCursor;
    }

    void aimCursor(Vector2 dir) {
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
        controls.Disable();
    }
}
