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
        public Vector2 pos;
        public Vector2 size;
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
            info.pos = b.unlockedImagePos;
            info.size = b.unlockedImageSize;
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = b.title.ToString() + " Unlocked!";
        icon.sprite = b.mainSprite.sprite;
        icon.SetNativeSize();
        icon.transform.localPosition = b.unlockedImagePos;
        icon.transform.localScale = b.unlockedImageSize;

        hiderWaiter = StartCoroutine(hider());
    }
    public void showForWeapon(Weapon w) {
        if(hiderWaiter != null) {
            var info = new unlockInfo();
            info.title = w.title.ToString();
            info.sprite = w.sprite;
            info.pos = w.unlockedImagePos;
            info.size = w.unlockedImageSize;
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = w.title.ToString() + " Unlocked!";
        icon.sprite = w.sprite;
        icon.SetNativeSize();
        icon.transform.localPosition = w.unlockedImagePos;
        icon.transform.localScale = w.unlockedImageSize;

        hiderWaiter = StartCoroutine(hider());
    }

    void showForInfo(unlockInfo info) {
        if(hiderWaiter != null) {
            queue.Add(info);
            return;
        }
        background.gameObject.SetActive(true);

        text.text = info.title.ToString() + " Unlocked!";
        icon.sprite = info.sprite;
        icon.SetNativeSize();
        icon.transform.localPosition = info.pos;
        icon.transform.localScale = info.size;

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
