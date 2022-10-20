using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SlotObject : MonoBehaviour {
    public Image[] images;
    public TextMeshProUGUI[] texts;

    public Button button;

    [HideInInspector] public int emptyIndex = -1; //  not used unless specified


    public void setImage(int index, Sprite sp) {
        images[index].sprite = sp;
    }
    public void setImageColor(int index, Color color) {
        images[index].color = color;
    }

    public void setText(int index, string t) {
        texts[index].text = t;
    }

    public void setInteractibility(bool canBe) {
        if(canBe) {
            foreach(var i in images)
                i.color = new Color(i.color.r, i.color.g, i.color.b, 1.0f);
        }
        else { 
            foreach(var i in images)
                i.color = new Color(i.color.r, i.color.g, i.color.b, .5f);
        }
    }


    public void grow(float time) {
        var prevScale = transform.localScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(prevScale, time);
    }
}
