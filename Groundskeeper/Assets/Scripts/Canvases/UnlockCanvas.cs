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

    List<unlockInfo> queue = new List<unlockInfo>();

    struct unlockInfo {
        public string title;
        public Sprite sprite;
    }

    private void Start() {
        DOTween.Init();
        moveBy = background.rect.height * 1.5f;
        background.gameObject.SetActive(false);
    }

    public void showForBuyable(Buyable b) {
        if(hiderWaiter != null) {
            var info = new unlockInfo();
            info.title = b.title.ToString();
            info.sprite = b.mainSprite.sprite;
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = b.title.ToString() + " Unlocked";
        icon.sprite = b.mainSprite.sprite;
        icon.SetNativeSize();

        hiderWaiter = StartCoroutine(hider());
    }
    public void showForWeapon(Weapon w) {
        if(hiderWaiter != null) {
            var info = new unlockInfo();
            info.title = w.title.ToString();
            info.sprite = w.sprite;
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = w.title.ToString() + " Unlocked";
        icon.sprite = w.sprite;
        icon.SetNativeSize();

        hiderWaiter = StartCoroutine(hider());
    }

    void showForInfo(unlockInfo info) {
        if(hiderWaiter != null) {
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = info.title.ToString() + " Unlocked";
        icon.sprite = info.sprite;
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
            showForInfo(b);
        }
    }
}
