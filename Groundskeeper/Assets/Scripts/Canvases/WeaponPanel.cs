using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WeaponPanel : MonoBehaviour {
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] SlotMenu shopSlot, invSlot;

    private void Start() {
        hardHide();
    }


    public void show() {
        transform.DOComplete();
        transform.DOScale(1.0f, .15f);
        populateSlots();
    }
    public void hide() {
        transform.DOComplete();
        transform.DOScale(0.0f, .25f);
    }
    public void hardHide() {
        transform.DOComplete();
        transform.localScale = Vector3.zero;
    }


    void populateSlots() {
        coinText.text = GameInfo.getSouls().ToString() + "c";
        int shopInd = 0, invInd = 0;
        for(int i = 0; i < FindObjectOfType<PresetLibrary>().getWeapons().Length; i++) {
            if(!Inventory.hasWeapon(i)) {
                var obj = shopSlot.createSlot(shopInd++, new Color(i / 2f, i / 2f, i / 2f));
                obj.GetComponent<SlotObject>().setText(0, i.ToString() + "c");
                obj.GetComponent<SlotObject>().emptyIndex = i;
                obj.GetComponent<SlotObject>().button.onClick.AddListener(delegate {
                    //  check if player has enought money
                    Inventory.addWeapon(obj.GetComponent<SlotObject>().emptyIndex);
                    populateSlots();
                    //  pay for the weapon
                });
            }
            else {
                var obj = invSlot.createSlot(invInd++, new Color(i / 2f, i / 2f, i / 2f));
                obj.GetComponent<SlotObject>().emptyIndex = i;
                obj.GetComponent<SlotObject>().button.onClick.AddListener(delegate { 
                    GameInfo.setPlayerWeaponIndex(obj.GetComponent<SlotObject>().emptyIndex);
                    FindObjectOfType<PlayerWeaponInstance>().updateReference(obj.GetComponent<SlotObject>().emptyIndex);
                });
            }
        }

        shopSlot.deleteSlotsAfterIndex(shopInd);
        invSlot.deleteSlotsAfterIndex(invInd);
    }

    public void buy() {
        //  buy shit here
    }
}
