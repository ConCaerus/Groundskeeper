using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UnlockBuyableCanvas : MonoBehaviour {
    [SerializeField] GameObject background;


    private void Start() {
        hide();
    }

    public void show() {
        background.transform.DOKill();
        background.transform.DOScale(1.0f, .15f);
    }

    public void hide() {
        background.transform.DOKill();
        background.transform.DOScale(0.0f, .25f);
    }
}
