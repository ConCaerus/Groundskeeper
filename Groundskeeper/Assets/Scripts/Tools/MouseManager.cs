using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour {
    static InputMaster controls;

    public delegate void func(bool usingKeyboard);

    List<func> changeFuncs = new List<func>();

    FreeGamepadCursor fgc;
    FreeGamepadHouseCursor fghc;
    GameGamepadCursor ggc;

    [SerializeField] bool debug = false;

    static bool initted = false;

    //  false - controller, true - keyboard
    bool state = false;


    private void Awake() {
        //  if 100% not that guy, check if there is a guy present
        //  if not, become that guy
        if(!initted) {
            //  checks for other instances of this script
            //  this is nice because it has the duds destory themselves
            foreach(var i in FindObjectsOfType<MouseManager>()) {
                //  found that guy
                if(i != this && i.isInitted()) {
                    Destroy(gameObject);    //  you're not that guy pal
                }
            }
        }

        fgc = FindObjectOfType<FreeGamepadCursor>();
        fghc = FindObjectOfType<FreeGamepadHouseCursor>();
        ggc = FindObjectOfType<GameGamepadCursor>();
        changeFuncs.Clear();

        //  if not setup yet, setup
        if(!initted) {
            controls = new InputMaster();
            controls.Enable();
            controls.InputSwitch.Mouse.performed += ctx => switchToKeyboard();
            controls.InputSwitch.Keyboard.performed += ctx => switchToKeyboard();
            controls.InputSwitch.Gamepad.performed += ctx => switchToGamepad();

            StartCoroutine(checkForInputDeviceAfterTransition());
            initted = true;
        }

        DontDestroyOnLoad(gameObject);
    }

    void switchToGamepad() {
        if(debug)
            Debug.Log("Tried to Gamepad");
        if(state) {
            Cursor.visible = false;
            if(debug)
                Debug.Log("Gamepad");
            if(fgc != null && fgc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else if(fghc != null && fghc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else
                Cursor.lockState = CursorLockMode.Locked;
            foreach(var i in changeFuncs)
                i(false);
        }
        state = false;
    }

    void switchToKeyboard() {
        if(debug)
            Debug.Log("Tried to Keyboard");
        if(!state) {
            Cursor.visible = true;
            if(debug)
                Debug.Log("Keyboard");
            Cursor.lockState = CursorLockMode.Confined;
            foreach(var i in changeFuncs)
                i(true);
        }
        state = true;
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
        initted = false;
    }

    public bool isInitted() {
        return initted;
    }

    IEnumerator checkForInputDeviceAfterTransition() {
        //  hides all means of mouse movement
        Cursor.visible = false;
        if(fghc != null)
            fghc.showCursor(false, true);
        if(fgc != null)
            fgc.showCursor(false, true);

        //  waits for the game to load
        var tc = FindObjectOfType<TransitionCanvas>();
        while(!tc.finishedLoading)
            yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        //  check which device is being used
        bool hasController = false;
        foreach(var i in Input.GetJoystickNames()) {
            if(!string.IsNullOrEmpty(i)) {
                hasController = true;
                break;
            }
        }
        if(!hasController) {
            state = false;
            switchToKeyboard();
        }
        else {
            state = true;
            switchToGamepad();
        }
    }
}
