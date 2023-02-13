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
    float moveBy;

    List<Buyable> queue = new List<Buyable>();

    private void Start() {
        DOTween.Init();
        moveBy = background.rect.height * 1.5f;
        background.gameObject.SetActive(false);
    }

    public void showForBuyable(Buyable b) {
        if(hiderWaiter != null) {
            queue.Add(b);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = b.title.ToString() + " Unlocked";
        icon.sprite = b.mainSprite.sprite;
        icon.SetNativeSize();

        hiderWaiter = StartCoroutine(hider());
    }

    IEnumerator hider() {
        background.DOComplete();
        background.DOLocalMoveY(background.transform.localPosition.y - moveBy, .15f);
        yield return new WaitForSeconds(2.0f);
        background.DOLocalMoveY(background.transform.localPosition.y + moveBy, .25f);
        yield return new WaitForSeconds(.26f);
        background.gameObject.SetActive(false);
        hiderWaiter = null;
        if(queue.Count > 0) {
            var b = queue[0];
            queue.RemoveAt(0);
            showForBuyable(b);
        }
    }
}
