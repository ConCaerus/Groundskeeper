using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UITest : MonoBehaviour {
    int UILayer;

    MouseManager mm;
    FreeGamepadCursor fgc;
    GameGamepadCursor ggc;

    private void Start() {
        UILayer = LayerMask.NameToLayer("UI");
        mm = FindObjectOfType<MouseManager>();
        fgc = FindObjectOfType<FreeGamepadCursor>();
        ggc = FindObjectOfType<GameGamepadCursor>();
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    public GameObject getMousedOverObject() {
        return getMousedOverObject(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private GameObject getMousedOverObject(List<RaycastResult> eventSystemRaysastResults) {
        for(int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if(curRaysastResult.gameObject.layer == UILayer)
                return curRaysastResult.gameObject;
        }
        return null;
    }


    //Gets all event system raycast results of current mouse or touch position.
    List<RaycastResult> GetEventSystemRaycastResults() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        if(mm.usingKeyboard())
            eventData.position = Input.mousePosition;
        else if(fgc != null)
            eventData.position = fgc.getScreenCursorPos();
        else if(ggc != null)
            eventData.position = ggc.getMousePosInScreen();
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

}