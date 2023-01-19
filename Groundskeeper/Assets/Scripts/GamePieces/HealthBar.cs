using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBar : MonoBehaviour {
    Vector2 offset = new Vector2(0.0f, .2f);
    [SerializeField] bool moveWithParent = true;
    [SerializeField] Slider slider;
    GameObject parent;

    Vector2 startSize;

    Coroutine shower = null;

    private void Awake() {
        startSize = slider.transform.localScale;
    }


    public void updatePos() {
        if(!moveWithParent)
            return;
        if(parent == null || parent.GetComponent<Mortal>().health <= 0.0f) {
            Destroy(transform.parent.gameObject);
            enabled = false;
            return;
        }
        transform.position = (Vector2)parent.transform.position + offset;
    }

    public void updateBar() {
        if(shower != null)
            StopCoroutine(shower);
        shower = StartCoroutine(showTimeWaiter());
        if(!slider.isActiveAndEnabled && parent.GetComponent<Mortal>().health < parent.GetComponent<Mortal>().maxHealth)
            slider.gameObject.SetActive(true);
        else if(slider.isActiveAndEnabled && parent.GetComponent<Mortal>().health >= parent.GetComponent<Mortal>().maxHealth) {
            slider.gameObject.SetActive(false);
            return;
        }
        slider.DOKill();
        slider.DOValue(parent.GetComponent<Mortal>().health, .15f);
    }

    public void setParent(GameObject obj) {
        parent = obj;
        slider.maxValue = parent.GetComponent<Mortal>().maxHealth;
        slider.value = parent.GetComponent<Mortal>().health;
        obj.GetComponent<Mortal>().healthBar = this;
        slider.gameObject.SetActive(false);
        updateBar();
    }

    public void hideBar() {
        slider.transform.DOKill();
        slider.transform.DOScale(0.0f, .25f);
    }

    public void showBar() {
        slider.transform.DOKill();
        slider.transform.DOScale(startSize, .15f);
    }

    IEnumerator showTimeWaiter() {
        showBar();

        yield return new WaitForSeconds(5f);

        hideBar();
        shower = null;
    }
}