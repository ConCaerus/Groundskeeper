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


    private void Start() {
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
        image.enabled = showCursor || freeRoam();
    }

    void aimCursor(Vector2 dir) {
        //  player is not placing a buyable, so have the cursor orbit the player
        if(!freeRoam()) {
            var worldTarget = (Vector2)player.transform.position + dir * rangeMod;
            transform.position = Camera.main.WorldToScreenPoint(worldTarget);
        }

        //  player is placing a buyable, so haver the cursor move freely
        else {
            var worldTarget = getMousePosInWorld() + dir * rangeMod / 10f;
            var target = Camera.main.WorldToScreenPoint(worldTarget);
            target.x = Mathf.Clamp(target.x, 0.0f, Screen.width);
            target.y = Mathf.Clamp(target.y, 0.0f, Screen.height);
            transform.position = target;
        }
    }

    bool freeRoam() {
        return pg != null && pg.placing;
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
