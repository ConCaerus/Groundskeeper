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

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Monsters By Diff")) {
            int c = FindObjectOfType<PresetLibrary>().getMonsters().Length;
            int l = 0;
            int max = 0;

            //  finds the max diff out of all of the monsters
            for(int i = 0; i < c; i++) {
                var m = FindObjectOfType<PresetLibrary>().getMonster(i).GetComponent<MonsterInstance>();
                if(m.diff > max)
                    max = m.diff;
            }
            while(l <= max) {
                for(int j = 0; j < c; j++) {
                    var m = FindObjectOfType<PresetLibrary>().getMonster(j).GetComponent<MonsterInstance>();
                    if(m.diff == l)
                        Debug.Log("Monster: " + m.title + " | diff : " + m.diff);
                }
                l++;
            }
        }
        if(GUILayout.Button("Monsters By Night")) {
            int c = FindObjectOfType<PresetLibrary>().getMonsters().Length;
            int l = 0;
            int max = 0;

            //  finds the max diff out of all of the monsters
            for(int i = 0; i < c; i++) {
                var m = FindObjectOfType<PresetLibrary>().getMonster(i).GetComponent<MonsterInstance>();
                if(m.earliestNight > max)
                    max = m.diff;
            }
            while(l <= max) {
                for(int j = 0; j < c; j++) {
                    var m = FindObjectOfType<PresetLibrary>().getMonster(j).GetComponent<MonsterInstance>();
                    if(m.earliestNight == l)
                        Debug.Log("Monster: " + m.title + " | night : " + m.earliestNight);
                }
                l++;
            }
        }
        if(GUILayout.Button("Monsters By Souls")) {
            int c = FindObjectOfType<PresetLibrary>().getMonsters().Length;
            float l = 0.00f;
            float max = 0f;
            List<Monster.monsterTitle> used = new List<Monster.monsterTitle>();
            int catcher = 0;

            //  finds the max diff out of all of the monsters
            for(int i = 0; i < c; i++) {
                var m = FindObjectOfType<PresetLibrary>().getMonster(i).GetComponent<MonsterInstance>();
                if(m.originalSoulsGiven > max)
                    max = m.originalSoulsGiven;
            }
            while(l <= max && catcher < FindObjectOfType<PresetLibrary>().getMonsterCount() * 2f) {
                MonsterInstance next = null;
                l = max;
                for(int j = 0; j < c; j++) {
                    var m = FindObjectOfType<PresetLibrary>().getMonster(j).GetComponent<MonsterInstance>();
                    if(m.originalSoulsGiven <= l && !used.Contains(m.title)) {
                        l = m.originalSoulsGiven;
                        next = m;
                    }
                }
                if(next == null)
                    break;
                catcher++;
                Debug.Log("Monster: " + next.title + " | souls : " + next.originalSoulsGiven.ToString("0.00"));
                used.Add(next.title);
            }
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