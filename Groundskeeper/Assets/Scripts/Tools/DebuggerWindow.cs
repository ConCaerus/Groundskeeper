using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class DebuggerWindow : EditorWindow {

    [MenuItem("Window/Debugger")]
    public static void showWindow() {
        GetWindow<DebuggerWindow>("Debugger");
    }


    private void OnGUI() {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("+100s")) 
            GameInfo.addSouls(100, true);
        if(GUILayout.Button("+1000s"))
            GameInfo.addSouls(1000, true);
        if(GUILayout.Button("+100000s"))
            GameInfo.addSouls(100000, true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save Board"))
            FindObjectOfType<GameBoard>().saveBoard();
        if(GUILayout.Button("Clear Board Save"))
            GameInfo.clearBoard();
        if(GUILayout.Button("Clear Save")) {
            SaveData.deleteCurrentSave();
            GameInfo.resetSave();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Info");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Night Count"))
            Debug.Log("Night: " + GameInfo.getNightCount());
        if(GUILayout.Button("Inc Night"))
            GameInfo.addNights(1);
        if(GUILayout.Button("Coin Count"))
            Debug.Log("Night: " + GameInfo.getSouls());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Unlock Everything"))
            FindObjectOfType<BuyableLibrary>().unlockAll();
        GUILayout.EndHorizontal();

        GUILayout.Label("Weapon");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Give Axe"))
            GameInfo.setPlayerWeaponIndex(FindObjectOfType<PresetLibrary>().getWeaponIndex(Weapon.weaponName.Axe));
        if(GUILayout.Button("Give Shotgun"))
            GameInfo.setPlayerWeaponIndex(FindObjectOfType<PresetLibrary>().getWeaponIndex(Weapon.weaponName.Shotgun));
        if(GUILayout.Button("Give Rifle"))
            GameInfo.setPlayerWeaponIndex(FindObjectOfType<PresetLibrary>().getWeaponIndex(Weapon.weaponName.Rifle));
        GUILayout.EndHorizontal();
    }
}

#endif