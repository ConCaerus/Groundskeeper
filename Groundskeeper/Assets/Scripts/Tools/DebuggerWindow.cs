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
        if(GUILayout.Button("Clear Souls"))
            GameInfo.addSouls(-GameInfo.getSouls(true), true);
        GUILayout.EndHorizontal();
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
        if(GUILayout.Button("Soul Count"))
            Debug.Log("Souls: " + GameInfo.getSouls(true));
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
        if(GUILayout.Button("Give Dagger")) {
            var stats = GameInfo.getPlayerStats();
            GameInfo.setPlayerStats(new PlayerStats(Weapon.weaponTitle.Dagger, stats.playerWeaponDamageBuff, stats.playerWeaponSpeedBuff));
            Debug.Log(GameInfo.getPlayerStats().getWeaponTitle(FindObjectOfType<PresetLibrary>()));
        }
        if(GUILayout.Button("Give Sledger")) {
            var stats = GameInfo.getPlayerStats();
            GameInfo.setPlayerStats(new PlayerStats(Weapon.weaponTitle.Sledgehammer, stats.playerWeaponDamageBuff, stats.playerWeaponSpeedBuff));
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("House");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Max Health?"))
            Debug.Log(GameInfo.getHouseStats().houseMaxHealth);
        if(GUILayout.Button("Health?"))
            Debug.Log(GameInfo.getHouseStats().houseHealth);
        if(GUILayout.Button("Light Rad?"))
            Debug.Log(GameInfo.getHouseStats().houseLightRad);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Set House Health To Full")) {
            var s = GameInfo.getHouseStats();
            s.houseHealth = s.houseMaxHealth;
            GameInfo.setHouseStats(s);
        }
        if(GUILayout.Button("Set House Health To Half")) {
            var s = GameInfo.getHouseStats();
            s.houseHealth = s.houseMaxHealth / 2;
            GameInfo.setHouseStats(s);
        }
        if(GUILayout.Button("Set House Health To 1")) {
            var s = GameInfo.getHouseStats();
            s.houseHealth = 1;
            GameInfo.setHouseStats(s);
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Steam");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear Achievements"))
            FindObjectOfType<SteamHandler>().resetAchievements();
        if(GUILayout.Button("Unlock Achievement")) 
            unlockAchievement();
        GUILayout.EndHorizontal();
    }

    void unlockAchievement() {
        int i = 0;
        bool found = false;
        while((SteamHandler.achievements)i != SteamHandler.achievements.None && !found) {
            if(FindObjectOfType<SteamHandler>().isAchievementUnlocked((SteamHandler.achievements)i))
                i++;
            else {
                found = true;
                FindObjectOfType<SteamHandler>().unlockAchievement((SteamHandler.achievements)i);
            }
        }
        if(!found) {
            FindObjectOfType<SteamHandler>().resetAchievements();
            unlockAchievement();
        }
    }
}

#endif