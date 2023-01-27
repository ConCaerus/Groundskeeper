using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HouseUpper : MonoBehaviour {

    List<MortalUnit> inTopUnits = new List<MortalUnit>();   //  fuckers that want the house to hide it's top
    SpriteRenderer sr;

    bool populated = false;

    private void Awake() {
        sr = FindObjectOfType<HouseInstance>().GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        //  let the house be clear
        if(!populated && (col.gameObject.tag == "Player" || col.gameObject.tag == "Monster")) {
            populated = true;
            var c = sr.color;
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, 0.5f), .15f);
            inTopUnits.Add(col.gameObject.GetComponent<MortalUnit>());
        }
    }

    private void OnTriggerStay2D(Collider2D col) {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Monster") {
            populated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  let the house not be clear anymore
        if(populated && (col.gameObject.tag == "Player" || col.gameObject.tag == "Monster")) {
            populated = false;
            //removeUnitFromInTopUnits(col.gameObject.GetComponent<MortalUnit>());
            StartCoroutine(checker());
        }
    }

    void removeUnitFromInTopUnits(MortalUnit unit) {
        inTopUnits.Remove(unit);
        for(int i = inTopUnits.Count - 1; i >= 0; i--) {
            if(inTopUnits[i] == null || inTopUnits[i].health <= 0f)
                inTopUnits.RemoveAt(i);
        }
        if(inTopUnits.Count == 0) {
            var c = sr.color;
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, 1f), .15f);
        }
    }

    IEnumerator checker() {
        yield return new WaitForFixedUpdate();
        var c = sr.color;
        if(!populated) {
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, 1f), .15f);
        }
        else {
            sr.DOBlendableColor(new Color(c.r, c.g, c.b, .5f), .15f);
        }
    }
}
