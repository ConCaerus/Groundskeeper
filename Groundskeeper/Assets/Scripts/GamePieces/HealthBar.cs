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
    Mortal pm;

    Vector2 startSize;

    Coroutine shower = null;

    private void Awake() {
        startSize = slider.transform.localScale;
    }


    public void updatePos() {
        if(!moveWithParent)
            return;
        if(parent == null || pm.health <= 0.0f) {
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
        slider.maxValue = pm.maxHealth;
        slider.DOKill();
        slider.DOValue(pm.health, .15f);
        if(!slider.isActiveAndEnabled && pm.health < pm.maxHealth)
            slider.gameObject.SetActive(true);
        else if(slider.isActiveAndEnabled && pm.health >= pm.maxHealth) {
            slider.DOKill();
            slider.value = pm.maxHealth;
            slider.gameObject.SetActive(false);
            return;
        }
    }

    public void setParent(GameObject obj) {
        parent = obj;
        pm = parent.GetComponent<Mortal>();
        slider.maxValue = pm.maxHealth;
        slider.value = pm.health;
        pm.healthBar = this;
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
