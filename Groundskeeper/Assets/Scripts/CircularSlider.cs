using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircularSlider : MonoBehaviour {
    [SerializeField] GameObject fill;
    [SerializeField] TextMeshProUGUI text;


    public float value { get; private set; } = 0.0f;

    private void Start() {
        fill.GetComponent<Image>().fillAmount = value;
    }

    public void setValue(float f) {
        value = Mathf.Clamp(f, 0.0f, 1.0f);
        fill.GetComponent<Image>().fillAmount = value;
    }

    public void setText(string t) {
        text.text = t;
    }

    public void setColor(Color c) {
        fill.GetComponent<Image>().color = c;
    }
}
