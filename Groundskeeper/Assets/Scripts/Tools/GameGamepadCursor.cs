using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGamepadCursor : MonoBehaviour {
    [SerializeField] GameObject player;

    [SerializeField] float rangeMod = 10f;

    InputMaster controls;

    Vector2 lastTarget = Vector2.zero;
    Vector2 curDir = Vector2.zero;

    [SerializeField] Image image;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
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
        if(image == null)
            return;
        image.enabled = !usingKeyboard && Time.timeScale != 0f && !pastCutoff();
    }

    void cursorChange(bool showCursor) {
        if(image == null)
            return;
        image.enabled = showCursor && Time.timeScale != 0f && !pastCutoff();
    }

    void aimCursor(Vector2 dir) {
        if(image == null)
            return;
        if(Time.timeScale == 0f || !image.gameObject.activeInHierarchy)
            return;
        curDir = dir;
        if(pastCutoff())
            return;
        else if(!image.enabled)
            image.enabled = true;

        //  checks if the position is too close to the last pos, if so, do nothing
        var worldTarget = (Vector2)player.transform.position + dir * rangeMod;
        float minDist = .2f;
        if(Vector2.Distance(lastTarget, worldTarget) > minDist) {
            transform.position = Camera.main.WorldToScreenPoint(worldTarget);
            lastTarget = worldTarget;
        }
    }

    public Vector2 getMousePosInWorld() {
        var pos = transform.position;
        return Camera.main.ScreenToWorldPoint(pos);
    }
    public Vector2 getMousePosInScreen() {
        return transform.position;
    }

    public bool changingDir() {
        return image.enabled && !pastCutoff();
    }
    bool pastCutoff() {
        return Vector2.Distance(Vector2.zero, curDir) < .35f;
    }


    private void OnDisable() {
        if(controls != null)
            controls.Disable();
    }
}
