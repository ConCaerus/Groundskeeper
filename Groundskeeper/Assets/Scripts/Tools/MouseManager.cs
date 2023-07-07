using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MouseManager : MonoBehaviour {
    static InputMaster controls;

    public delegate void func(bool usingKeyboard);

    static List<func> changeFuncs = new List<func>();

    FreeGamepadCursor fgc;
    FreeGamepadHouseCursor fghc;
    GameGamepadCursor ggc;

    [SerializeField] bool debugging = false;

    static bool initted = false;

    //  false - controller, true - keyboard
    static bool state = false;
    [SerializeField] bool seenState;    //  only used to see what state it's in in the inspector, not actually used in any code


    private void Awake() {
        //  if 100% not that guy, check if there is a guy present
        //  if not, become that guy
        //  is that guy
        if(GameInfo.curMouseManager == null)
            GameInfo.curMouseManager = this;
        //  not that guy, pal
        else if(GameInfo.curMouseManager != this) {
            Destroy(gameObject);
        }

        fgc = FindObjectOfType<FreeGamepadCursor>();
        fghc = FindObjectOfType<FreeGamepadHouseCursor>();
        ggc = FindObjectOfType<GameGamepadCursor>();

        //  if not setup yet, setup
        if(!initted) {
            StartCoroutine(checkForInputDeviceAfterTransition());
            initted = true;
        }

        DontDestroyOnLoad(gameObject);
        seenState = state;
    }

    void switchToGamepad() {
        if(debugging)
            Debug.Log("Tried to Gamepad");
        if(state || Cursor.visible) {
            Cursor.visible = false;
            if(debugging)
                Debug.Log("Gamepad");
            /*
            if(fgc != null && fgc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else if(fghc != null && fghc.isActiveAndEnabled)
                Cursor.lockState = CursorLockMode.Confined;
            else
                Cursor.lockState = CursorLockMode.Locked;*/

            //  selects a random selectable
            if((SceneManager.GetActiveScene().name == "MainMenu" || (FindObjectOfType<PauseMenu>() != null && FindObjectOfType<PauseMenu>().isOpen()) || Time.timeScale == 0f) && FindObjectsOfType<Selectable>().Length > 0)
                EventSystem.current.SetSelectedGameObject(FindObjectOfType<Selectable>().gameObject);

            foreach(var i in changeFuncs) {
                i(false);
            }
            state = false;
            seenState = state;
        }
    }

    void switchToKeyboard() {
        if(debugging)
            Debug.Log("Tried to Keyboard");
        if(!state || !Cursor.visible) {
            Cursor.visible = true;
            if(debugging)
                Debug.Log("Keyboard");
            Cursor.lockState = CursorLockMode.Confined;

            //  unselects any selected selecatable
            EventSystem.current.SetSelectedGameObject(null);

            foreach(var i in changeFuncs) {
                i(true);
            }
            state = true;
            seenState = state;
            if(SceneManager.GetActiveScene().name == "Intro")
                Cursor.visible = false;
        }
    }

    public bool usingKeyboard() {
        return state;
    }


    public void addOnInputChangeFunc(func f) {
        changeFuncs.Add(f);
        //  run it right away
        f(usingKeyboard());
    }
    public void removeOnInputChangeFunc(func f) {
        changeFuncs.Remove(f);
    }

    private void OnEnable() {
        controls = new InputMaster();
        controls.Enable();
        controls.InputSwitch.Mouse.performed += ctx => switchToKeyboard();
        controls.InputSwitch.Keyboard.performed += ctx => switchToKeyboard();
        controls.InputSwitch.Gamepad.performed += ctx => switchToGamepad();
    }
    private void OnDisable() {
        //controls.Disable();
        changeFuncs.Clear();
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
        seenState = state;
    }
}
