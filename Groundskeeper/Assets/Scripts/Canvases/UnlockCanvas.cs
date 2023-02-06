using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UnlockCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image icon;
    [SerializeField] RectTransform background;
    Coroutine hiderWaiter = null;

    private void Start() {
        DOTween.Init();
        background.position = new Vector3(background.position.x, 1500f);
    }

    public void showForBuyable(Buyable b) {
        if(hiderWaiter != null)
            StopCoroutine(hiderWaiter);

        text.text = b.title.ToString() + " Unlocked";
        icon.sprite = b.mainSprite.sprite;

        hiderWaiter = StartCoroutine(hider());
    }

    IEnumerator hider() {
        background.DOKill();
        background.DOMoveY(1100f, .15f);
        yield return new WaitForSeconds(2.0f);
        background.DOMoveY(1500f, .25f);
        hiderWaiter = null;
    }
}
