using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractHelperCanvas : MonoBehaviour {

    private void Start() {
        transform.GetChild(0).transform.localScale = Vector3.zero;
    }

    public void show(Vector2 pos) {
        transform.GetChild(0).position = pos;
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(0.25f, .15f);
    }
    public void hide() {
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).DOScale(0.0f, .25f);
    }
    public void hardHide() {
        transform.GetChild(0).DOComplete();
        transform.GetChild(0).transform.localScale = Vector3.zero;
    }
}
