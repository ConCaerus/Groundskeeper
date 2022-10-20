using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {
    public static bool playing { get; set; } = true;
    public static int wave { get; set; } = 0;
    public static int wavesPerNight() { return 2; }

    public static int monstersKilled { get; set; } = 0;

    //  nights / waves / enemies shit
    
    static string soulTag = "souls";
    static string nightTag = "nights";
    static string lastSeenEnemyTag = "LastSeenEnemyTag";

    //  player shit

    static string pWeaponIndex = "PlayerWeaponInex";

    //  Game Board shit

    public static string helperTag = "helperTag";
    public static string lastSavedHelperCount = "LastSavedHelperCount";

    public static string defenceTag = "defenceTag";
    public static string lastSavedDefenceCount = "LastSavedDefenceCount";

    public static string miscTag = "miscTag";
    public static string lastSavedMiscCount = "LastSavedMiscCount";

    public static string envTag = "EnvTag";
    public static string envCount = "envCount";

    //  unlocked shit

    public static string buyableCount = "numOfBuyables";
    public static string unlockedBuyableTag(int index) {
        return "UnlockedBuyable" + index.ToString();
    }


    public enum MonsterType {
        Both, Physical, Spiritual
    }


    public static void resetVars() {
        playing = true;
        wave = 0;
        monstersKilled = 0;
        monstersKilled = 0;
    }
    public static void resetSave() {
        setSouls(100);
        resetNights();
        resetLastSeenEnemy();
        lockAllBuyables();
        clearBoard();
        Inventory.clear();
        Inventory.addWeapon(0);
        setPlayerWeaponIndex(0);
    }
    public static void clearBoard() {
        //      clears all of the shit before saving new shit
        for(int i = 0; i < SaveData.getInt(lastSavedHelperCount) + 1; i++)
            SaveData.deleteKey(helperTag + i.ToString());
        for(int i = 0; i < SaveData.getInt(lastSavedDefenceCount) + 1; i++)
            SaveData.deleteKey(defenceTag + i.ToString());
        for(int i = 0; i < SaveData.getInt(lastSavedMiscCount) + 1; i++)
            SaveData.deleteKey(miscTag + i.ToString());
        for(int i = 0; i < SaveData.getInt(envCount) + 1; i++)
            SaveData.deleteKey(envTag + i.ToString());

        SaveData.setInt(lastSavedHelperCount, 0);
        SaveData.setInt(lastSavedDefenceCount, 0);
        SaveData.setInt(lastSavedMiscCount, 0);
        SaveData.setInt(envCount, 0);
    }

    public static Vector2 mousePos() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static int calcPartygoerCount() {
        return 5 + getNightCount() * 3;
    }


    //  saves
    public static void addSouls(float c) {
        SaveData.setFloat(soulTag, SaveData.getFloat(soulTag) + c);
    }
    static void setSouls(float c) {
        SaveData.setFloat(soulTag, c);
    }
    public static float getSouls() {
        return SaveData.getFloat(soulTag);
    }

    public static void addNights(int n) {
        SaveData.setInt(nightTag, SaveData.getInt(nightTag) + n);
    }
    static void resetNights() {
        SaveData.setInt(nightTag, 0);
    }
    public static int getNightCount() {
        return SaveData.getInt(nightTag);
    }

    public static void incLastSeenEnemy() {
        SaveData.setInt(lastSeenEnemyTag, SaveData.getInt(lastSeenEnemyTag) + 1);
    }
    static void resetLastSeenEnemy() {
        SaveData.setInt(lastSeenEnemyTag, 0);
    }
    public static int getLastSeenEnemyIndex() {
        return SaveData.getInt(lastSeenEnemyTag);
    }

    public static int getPlayerWeaponIndex() {
        return SaveData.getInt(pWeaponIndex);
    }
    public static void setPlayerWeaponIndex(int ind) {
        SaveData.setInt(pWeaponIndex, ind);
    }

    public static void lockAllBuyables() {
        for(int i = 0; i < SaveData.getInt(buyableCount); i++)
            SaveData.setInt(unlockedBuyableTag(i), 0);
    }
    public static void unlockBuyable(int index) {
        //  index is supposed to be the index of the buyable in the buyable library
        SaveData.setInt(unlockedBuyableTag(index), 1);
    }
    public static bool isBuyableUnlocked(int index) {
        return SaveData.getInt(unlockedBuyableTag(index)) == 1;
    }
}
