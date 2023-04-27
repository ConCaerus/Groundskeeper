using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InfoBox : MonoBehaviour {
    [SerializeField] RectTransform box;
    [SerializeField] TextMeshProUGUI text;

    float padding = 15f;

    Coroutine shower = null;
    GameObject currentObj = null;
    bool shown = false;

    UITest uit;

    MouseManager mm;
    FreeGamepadCursor fgc;
    FreeGamepadHouseCursor fghc;

    private void Start() {
        uit = FindObjectOfType<UITest>();
        mm = FindObjectOfType<MouseManager>();
        fgc = FindObjectOfType<FreeGamepadCursor>();
        fghc = FindObjectOfType<FreeGamepadHouseCursor>();
        hide();
    }

    private void FixedUpdate() {
        //  checks for new relevant image
        var obj = uit.getMousedOverObject();
        if(obj == null || obj.GetComponent<InfoableImage>() == null) {
            if(shown)
                hide();
            return;
        }

        //  checks if obj is not the current shown obj
        if(obj != currentObj)
            show(obj, obj.GetComponent<InfoableImage>().info);

        //  nothing new, just update pos
        else if(obj == currentObj)
            updatePos();
    }


    public void updatePos() {
        if(!box.gameObject.activeInHierarchy)
            return;
        var mousePos = Vector2.zero;
        if(mm.usingKeyboard())
            mousePos = Input.mousePosition;
        else if(fgc != null)
            mousePos = fgc.getScreenCursorPos();
        else
            mousePos = fghc.getScreenCursorPos();
        box.transform.position = new Vector3(mousePos.x, mousePos.y);
    }

    public void show(GameObject thing, string info) {
        currentObj = thing;
        if(shower != null)
            StopCoroutine(shower);
        shower = StartCoroutine(waitToShow(info));
    }
    public void updateInfo(string info) {
        text.text = info;
        box.sizeDelta = new Vector2(text.preferredWidth + padding > 200 ? 200 : text.preferredWidth + padding, text.preferredHeight + padding);
    }
    public void hide() {
        text.text = "";
        box.gameObject.SetActive(false);
        if(shower != null)
            StopCoroutine(shower);
        currentObj = null;
        shown = false;
    }

    IEnumerator waitToShow(string info) {
        yield return new WaitForSeconds(.25f);
        box.gameObject.SetActive(true);
        text.text = info;

        var mousePos = Vector2.zero;
        if(mm.usingKeyboard())
            mousePos = Input.mousePosition;
        else if(fgc != null)
            mousePos = fgc.getScreenCursorPos();
        else
            mousePos = fghc.getScreenCursorPos();
        box.transform.position = new Vector3(mousePos.x, mousePos.y);
        box.sizeDelta = new Vector2(text.preferredWidth + padding > 200 ? 200 : text.preferredWidth + padding, text.preferredHeight + padding);
        shower = null;
        shown = true;
    }
}
