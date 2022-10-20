using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuyableButtonSpawner : MonoBehaviour {
    [SerializeField] GameObject buyableButton;
    [SerializeField] Transform holder;

    List<GameObject> buttons = new List<GameObject>();

    public void switchGenre(int index) {
        FindObjectOfType<PlacementGrid>().placing = false;
        List<GameObject> buyables = null;
        switch(index) {
            case 0: buyables = FindObjectOfType<BuyableLibrary>().getHelpers(); break;
            case 1: buyables = FindObjectOfType<BuyableLibrary>().getDefences(); break;
            case 2: buyables = FindObjectOfType<BuyableLibrary>().getMiscBuyables(); break;
        }

        foreach(var i in buttons)
            Destroy(i.gameObject);
        buttons.Clear();

        foreach(var i in buyables) {
            var obj = Instantiate(buyableButton.gameObject, holder);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().title;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = i.GetComponent<Buyable>().cost.ToString("0.0") + "s";
            obj.GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<PregameCanvas>().setPlacementObj(i.gameObject); });
            buttons.Add(obj.gameObject);
        }
    }
}
