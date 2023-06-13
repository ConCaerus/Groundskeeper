using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveCanvas : MonoBehaviour {
    [SerializeField] GameObject[] slots;

    [SerializeField] TextMeshProUGUI nameText;

    private void Start() {
        nameText.transform.parent.gameObject.SetActive(false);
    }

    public void showSaveInfo() {
        var curInd = SaveData.getCurrentSaveIndex();
        for(int i = 0; i < slots.Length; i++) {
            //  checks if the slot has info
            if(SaveData.hasSaveDataForSlot(i)) {
                SaveData.setCurrentSaveIndex(i);
                //  slot number and time played
                slots[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Slot: " + (i + 1).ToString() + "\n" + TimeInfo.timeToString();
                //  night count
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Night: " + (GameInfo.getNightCount() + 1).ToString();
            }

            //  if has no info for the slot, tell the player that it is empty
            else {
                slots[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Empty Slot";
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
        SaveData.setCurrentSaveIndex(curInd);
    }

    public void deleteSave(int ind) {
        SaveData.deleteSave(ind);
        showSaveInfo();
    }

    void loadNewGame() {
        SaveData.deleteCurrentSave();
        GameInfo.resetSave(FindObjectOfType<BuyableLibrary>(), FindObjectOfType<PresetLibrary>());
        FindObjectOfType<TransitionCanvas>().loadScene("Intro");
    }

    void loadGame(int i) {
        //  checks if the save is empty, if so, load a new game
        SaveData.setCurrentSaveIndex(i);
        if(SaveData.hasSaveDataForSlot(i))
            FindObjectOfType<TransitionCanvas>().loadScene("Game");
        else
            loadNewGame();
    }
}
