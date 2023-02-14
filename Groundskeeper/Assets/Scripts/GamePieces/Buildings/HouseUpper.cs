using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HouseUpper : MonoBehaviour {
    SpriteRenderer sr;

    bool populated = false;
    float yPos;
    Collider2D col;

    Coroutine checker = null;

    private void Awake() {
        sr = FindObjectOfType<HouseInstance>().GetComponent<SpriteRenderer>();
        yPos = transform.localPosition.y;
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        //  let the house be clear
        if(!populated && (col.gameObject.tag == "Player" || col.gameObject.tag == "Monster")) {
            populated = true;
            var c = sr.color;
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, 0.5f), .15f);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  let the house not be clear anymore
        if(populated && (col.gameObject.tag == "Player" || col.gameObject.tag == "Monster")) {
            populated = false;
            if(checker != null)
                StopCoroutine(checker);
            checker = StartCoroutine(checkForMoreUnits());
        }
    }

    IEnumerator clearer() {
        yield return new WaitForFixedUpdate();
        var c = sr.color;
        if(!populated) {
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, 1f), .15f);
        }
        else {
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, .5f), .15f);
        }
    }

    IEnumerator checkForMoreUnits() {
        col.enabled = false;
        transform.localPosition = Vector3.zero;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        col.enabled = true;
        transform.DOLocalMoveY(yPos, .05f);
        yield return new WaitForSeconds(.05f);
        StartCoroutine(clearer());
    }
}
