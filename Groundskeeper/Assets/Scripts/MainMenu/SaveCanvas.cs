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
            SaveData.setCurrentSaveIndex(i);
            var name = SaveData.getCurrentSaveName();
            slots[i].GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString() + ") " + (name == "" ? "Empty Slot" : name) + "\n" + TimeInfo.timeToString();
        }
        SaveData.setCurrentSaveIndex(curInd);
    }

    public void deleteSave(int ind) {
        SaveData.deleteSave(ind);
        showSaveInfo();
    }


    public void updateNameText(string name) {
        nameText.text = name;
    }
    public void setNameAndLoadGame(string name) {
        nameText.text = name;
        SaveData.deleteCurrentSave();
        SaveData.setSaveName(name);
        GameInfo.resetSave(FindObjectOfType<BuyableLibrary>(), FindObjectOfType<PresetLibrary>());
        FindObjectOfType<TransitionCanvas>().loadScene("Game");
    }

    public void loadGame(int i) {
        SaveData.setCurrentSaveIndex(i);
        //  create a new save
        if(!SaveData.hasSaveDataForSlot(i)) {
            //  unselected the current button so that when the play hits enter, they don't press it again
            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            //  name shit
            nameText.transform.parent.gameObject.SetActive(true);
            FindObjectOfType<TextInputReader>().startReading(16, "No Name", updateNameText, setNameAndLoadGame);
        }

        //  load the save
        else
            FindObjectOfType<TransitionCanvas>().loadScene("Game");
    }
}
