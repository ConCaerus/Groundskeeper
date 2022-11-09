using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyTreeNodeObj : MonoBehaviour {
    [SerializeField] Button but;
    [SerializeField] GameObject check;
    [SerializeField] TextMeshProUGUI costText;

    private void Start() {
        transform.localScale = Vector3.one;
    }

    public void setColor(Color c) {
        but.GetComponent<Image>().color = c;
    }
    public Color getColor() {
        return but.GetComponent<Image>().color;
    }
    public void setText(string t) {
        but.GetComponentInChildren<TextMeshProUGUI>().text = t;
    }

    public void setCost(float t) {
        costText.text = t.ToString("0.0");
    }

    public void showCheckmark() {
        check.SetActive(true);
        costText.enabled = false;
    }
}
