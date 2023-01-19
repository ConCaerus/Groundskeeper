using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {
    public static bool playing { get; set; } = true;
    public static int wave { get; set; } = 0;
    public static int wavesPerNight() { return 5; } //  changing this fucks up monster spawner

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

    public static string houseTag = "HouseTag";

    public static string envTag = "EnvTag";
    public static string envCount = "envCount";

    //  Buy tree shit
    public static string buyTreeTag = "BuyTreeTag";
    //  buffs
    static string helperDamageBuffTag = "HelperDamageBuff";
    static string helperHealthBuffTag = "HelperHealthBuff";
    static string defenceDamageBuffTag = "DefenceDamageBuff";
    static string structureHealthBuffTag = "StructureHealthBuff";
    static string pWeaponDamageBuffTag = "PlayerWeaponDamageBuff";
    static string pWeaponSpeedBuffTag = "PlayerWeaponSpeedBuff";

    //  unlocked shit

    public static string buyableCount = "numOfBuyables";
    public static string unlockedBuyableTag(Buyable.buyableTitle title) {
        return "UnlockedBuyable: " + title;
    }

    //  options / settings
    static string masterVolumeTag = "MasterVolumeTag";
    static string musicVolumeTag = "MusicVolumeTag";
    static string sfxVolumeTag = "SFXVolumeTag";
    static string screenModeTag = "ScreenMode";
    static string vsyncTag = "VsyncTag";


    public enum MonsterType {
        Both, Physical, Spiritual
    }

    //  makes it so if the player buys something and leaves the game before the board saves, they don't lose any souls
    public static float souls;

    public static void init() {
        playing = true;
        wave = 0;
        monstersKilled = 0;
        monstersKilled = 0;
        souls = getSouls(true);
    }
    public static void resetSave() {
        souls = 100;
        saveSouls();
        resetNights();
        resetLastSeenEnemy();
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
    public static void addSouls(float c, bool saveAfter) {
        souls += c;
        if(saveAfter)
            saveSouls();
    }
    public static void saveSouls() {  //  scary function OwO
        SaveData.setFloat(soulTag, souls);
    }
    public static float getSouls(bool getSaved = false) {
        return getSaved ? SaveData.getFloat(soulTag) : souls;
    }

    //          NIGHTS / WAVES / ENEMIES
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

    //  player weapon index
    public static int getPlayerWeaponIndex() {
        return SaveData.getInt(pWeaponIndex);
    }
    public static void setPlayerWeaponIndex(int ind) {
        SaveData.setInt(pWeaponIndex, ind);
    }

    //  buyables
    public static void lockAllBuyables(BuyableLibrary bl) {
        foreach(var i in bl.buyablesWithUniques()) {
            foreach(var j in i)
                SaveData.setInt(unlockedBuyableTag(j.GetComponent<Buyable>().title), 0);
        }
    }
    public static void unlockBuyable(Buyable.buyableTitle title) {
        SaveData.setInt(unlockedBuyableTag(title), 1);
    }
    public static bool isBuyableUnlocked(Buyable.buyableTitle title) {
        return SaveData.getInt(unlockedBuyableTag(title)) == 1;
    }

    //          BUFFS
    //  helper buffs
    public static void setHelperDamageBuff(float buff) {
        SaveData.setFloat(helperDamageBuffTag, buff);
    }
    public static void incHelperDamageBuff(float buff) {
        setHelperDamageBuff(buff + getHelperDamageBuff());
    }
    public static float getHelperDamageBuff() {
        return SaveData.getFloat(helperDamageBuffTag) == 0f ? 1f : SaveData.getFloat(helperDamageBuffTag);
    }
    public static void setHelperHealthBuff(float buff) {
        SaveData.setFloat(helperHealthBuffTag, buff);
    }
    public static void incHelperHealthBuff(float buff) {
        setHelperHealthBuff(buff + getHelperHealthBuff());
    }
    public static float getHelperHealthBuff() {
        return SaveData.getFloat(helperHealthBuffTag) == 0f ? 1f : SaveData.getFloat(helperHealthBuffTag);
    }
    public static void setDefenceDamageBuff(float buff) {
        SaveData.setFloat(defenceDamageBuffTag, buff);
    }
    public static void incDefenceDamageBuff(float buff) {
        SaveData.setFloat(defenceDamageBuffTag, buff + getDefenceDamageBuff());
    }
    public static float getDefenceDamageBuff() {
        return SaveData.getFloat(defenceDamageBuffTag) == 0f ? 1f : SaveData.getFloat(defenceDamageBuffTag);
    }
    public static void setStructureHealthBuff(float buff) {
        SaveData.setFloat(structureHealthBuffTag, buff);
    }
    public static void incStructureHealthBuff(float buff) {
        SaveData.setFloat(structureHealthBuffTag, buff + getStructureHealthBuff());
    }
    public static float getStructureHealthBuff() {
        return SaveData.getFloat(structureHealthBuffTag) == 0f ? 1f : SaveData.getFloat(structureHealthBuffTag);
    }
    public static void setPWeaponDamageBuff(float buff) {
        SaveData.setFloat(pWeaponDamageBuffTag, buff);
    }
    public static void incPWeaponDamageBuff(float buff) {
        SaveData.setFloat(pWeaponDamageBuffTag, buff + getPWeaponDamageBuff());
    }
    public static float getPWeaponDamageBuff() {
        return SaveData.getFloat(pWeaponDamageBuffTag) == 0f ? 1f : SaveData.getFloat(pWeaponDamageBuffTag);
    }
    public static void setPWeaponSpeedBuff(float buff) {
        SaveData.setFloat(pWeaponSpeedBuffTag, buff);
    }
    public static void incPWeaponSpeedBuff(float buff) {
        SaveData.setFloat(pWeaponSpeedBuffTag, buff + getPWeaponSpeedBuff());
    }
    public static float getPWeaponSpeedBuff() {
        return SaveData.getFloat(pWeaponSpeedBuffTag) == 0f ? 1f : SaveData.getFloat(pWeaponSpeedBuffTag);
    }

    public static void setVolumeOptions(float master, float music, float sfx) {
        SaveData.setFloat(masterVolumeTag, master);
        SaveData.setFloat(musicVolumeTag, music);
        SaveData.setFloat(sfxVolumeTag, sfx);
    }
    public static void resetVolumeOptions() {
        SaveData.deleteKey(masterVolumeTag);
        SaveData.deleteKey(musicVolumeTag);
        SaveData.deleteKey(sfxVolumeTag);
    }
    //  Master, Music, SFX
    public static float[] getVolumeOptions() {
        return new float[3] {
            SaveData.getFloat(masterVolumeTag, 1.0f),
            SaveData.getFloat(musicVolumeTag, 1.0f),
            SaveData.getFloat(sfxVolumeTag, 1.0f)
        };
    }

    public static void setScreenMode(FullScreenMode mode) {
        SaveData.setInt(screenModeTag, (int)mode);
    }
    public static FullScreenMode getScreenMode() {
        return (FullScreenMode)SaveData.getInt(screenModeTag);
    }
    public static void setVsync(bool b) {
        SaveData.setInt(vsyncTag, b ? 1 : 0);
    }
    //  gets set in awake function of transition canvas
    public static bool getVsync() {
        return SaveData.getInt(vsyncTag) == 1;
    }
}
