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
        if(GUILayout.Button("Wipe")) {
            SaveData.wipe();
        }
        if(GUILayout.Button("Clear All Saves")) {
            for(int i = 0; i < 3; i++)
                SaveData.deleteSave(i);
        }
        if(GUILayout.Button("Clear Save")) {
            SaveData.deleteCurrentSave();
            GameInfo.resetSave(FindObjectOfType<BuyableLibrary>(), FindObjectOfType<PresetLibrary>());
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
        if(GUILayout.Button("Unlock Everything")) {
            FindObjectOfType<BuyableLibrary>().unlockAll();
            FindObjectOfType<PresetLibrary>().unlockAllWeapons();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Weapon");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Give Axe")) {
            var stats = GameInfo.getPlayerStats();
            GameInfo.setPlayerStats(new PlayerStats(Weapon.weaponTitle.Axe, stats.playerWeaponDamageBuff, stats.playerWeaponSpeedBuff));
        }
        if(GUILayout.Button("Give Shotgun")) {
            var stats = GameInfo.getPlayerStats();
            GameInfo.setPlayerStats(new PlayerStats(Weapon.weaponTitle.Shotgun, stats.playerWeaponDamageBuff, stats.playerWeaponSpeedBuff));
        }
        if(GUILayout.Button("Give Rifle")) {
            var stats = GameInfo.getPlayerStats();
            GameInfo.setPlayerStats(new PlayerStats(Weapon.weaponTitle.Rifle, stats.playerWeaponDamageBuff, stats.playerWeaponSpeedBuff));
        }
        GUILayout.EndHorizontal();
    }
}

#endif