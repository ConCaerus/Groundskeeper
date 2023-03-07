using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {
    public static bool playing { get; set; } = true;
    public static int wave { get; set; } = 0;
    public static int wavesPerNight() {
        int nc = getNightCount();
        if(nc < 5)
            return 5;
        if(nc < 10)
            return 7;
        if(nc < 20)
            return 10;
        if(nc < 50)
            return 15;
        return 20;
    }

    public static int monstersKilled { get; set; } = 0;

    //  nights / waves / enemies shit
    static string soulTag = "souls";
    static string nightTag = "nights";
    static string lastSeenEnemyTag = "LastSeenEnemyTag";

    //  player / house shit
    static string pWeaponTag = "PlayerWeaponInex";

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
    public static string buyTreeSubNodeTickTag(Buyable.buyType m, BuyTreeCanvas.subType s) {
        if(s == BuyTreeCanvas.subType.None || m == Buyable.buyType.None) {
            Debug.LogError("Yall's done fucked up");
            return null;
        }
        return "TickForSubNode: " + s.ToString() + " of: " + m.ToString();
    }
    public static string buyTreeSubNodeTierTag(Buyable.buyType m, BuyTreeCanvas.subType s) {
        if(s == BuyTreeCanvas.subType.None || m == Buyable.buyType.None) {
            Debug.LogError("Yall's done fucked up");
            return null;
        }
        return "TierForSubNode: " + s.ToString() + " of: " + m.ToString();
    }
    //  buffs
    static string helperDamageBuffTag = "HelperDamageBuff";
    static string helperHealthBuffTag = "HelperHealthBuff";
    static string defenceDamageBuffTag = "DefenceDamageBuff";
    static string structureHealthBuffTag = "StructureHealthBuff";
    static string pWeaponDamageBuffTag = "PlayerWeaponDamageBuff";
    static string pWeaponSpeedBuffTag = "PlayerWeaponSpeedBuff";

    //  house buffs
    static string houseHealthTag = "HouseHealthTag";
    static string houseMaxHealthTag = "HouseMaxHealthTag";
    static string houseLightRadTag = "HouseLightRadiusTag";

    //  unlocked shit
    public static string weaponCount = "numOfWeapons";
    public static string unlockedWeaponTag(Weapon.weaponTitle title) {
        return "UnlockedWeapon: " + title;
    }
    public static string buyableCount = "numOfBuyables";
    public static string unlockedBuyableTag(Buyable.buyableTitle title) {
        return "UnlockedBuyable: " + title;
    }

    //  options / settings
    static string gameOptionsTag = "GameOptions";


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
    public static void resetSave(BuyableLibrary bl, PresetLibrary pl) {
        souls = 100;
        lockAllBuyables(bl);
        lockAllWeaponsExceptStarting(pl);
        saveSouls();
        resetNights();
        resetLastSeenEnemy();
        clearBoard();

        //  resets the house
        setHouseMaxHealth(1000);
        setHouseLightRad(30f);
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
    public static Weapon.weaponTitle getPlayerWeaponTitle(PresetLibrary pl) {
        if(pl.getEquivalentWeaponTitle(SaveData.getString(pWeaponTag)) != Weapon.weaponTitle.None) {
            return pl.getEquivalentWeaponTitle(SaveData.getString(pWeaponTag));
        }

        //  the game hasn't set the player's weapon yet, so set it to the first unlocked weapon in the PresetLibrary
        setPlayerWeapon(pl.getUnlockedWeapons()[0].title);
        return pl.getUnlockedWeapons()[0].title;
    }
    public static void setPlayerWeapon(Weapon.weaponTitle title) {
        SaveData.setString(pWeaponTag, title.ToString());
    }

    //  player's weapons
    //  scary function that does scary things
    static void lockAllWeaponsExceptStarting(PresetLibrary pl) {
        //  locks everything
        foreach(var i in pl.getWeapons())
            SaveData.setInt(unlockedWeaponTag(i.title), 0);

        //  except default
        unlockWeapon(Weapon.weaponTitle.Axe);
    }
    public static void unlockWeapon(Weapon.weaponTitle title) {
        SaveData.setInt(unlockedWeaponTag(title), 1);
    }
    public static bool isWeaponUnlocked(Weapon.weaponTitle title) {
        return SaveData.getInt(unlockedWeaponTag(title)) == 1;
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

    //  buy tree
    public static void setBuyTreeSubNodeTick(Buyable.buyType m, BuyTreeCanvas.subType s, int tick) {
        SaveData.setInt(buyTreeSubNodeTickTag(m, s), tick);
    }
    public static int getBuyTreeSubNodeTick(Buyable.buyType m, BuyTreeCanvas.subType s) {
        return SaveData.getInt(buyTreeSubNodeTickTag(m, s));
    }
    public static void setBuyTreeSubNodeTier(Buyable.buyType m, BuyTreeCanvas.subType s, int tier) {
        SaveData.setInt(buyTreeSubNodeTierTag(m, s), tier);
    }
    public static int getBuyTreeSubNodeTier(Buyable.buyType m, BuyTreeCanvas.subType s) {
        return SaveData.getInt(buyTreeSubNodeTierTag(m, s));
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
    //  defence buffs
    public static void setDefenceDamageBuff(float buff) {
        SaveData.setFloat(defenceDamageBuffTag, buff);
    }
    public static void incDefenceDamageBuff(float buff) {
        SaveData.setFloat(defenceDamageBuffTag, buff + getDefenceDamageBuff());
    }
    public static float getDefenceDamageBuff() {
        return SaveData.getFloat(defenceDamageBuffTag) == 0f ? 1f : SaveData.getFloat(defenceDamageBuffTag);
    }
    //  structure buffs
    public static void setStructureHealthBuff(float buff) {
        SaveData.setFloat(structureHealthBuffTag, buff);
    }
    public static void incStructureHealthBuff(float buff) {
        SaveData.setFloat(structureHealthBuffTag, buff + getStructureHealthBuff());
    }
    public static float getStructureHealthBuff() {
        return SaveData.getFloat(structureHealthBuffTag) == 0f ? 1f : SaveData.getFloat(structureHealthBuffTag);
    }
    //  player buffs
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

    //  house buffs
    public static void setHouseHealth(int h) {
        SaveData.setInt(houseHealthTag, h);
    }
    public static int getHouseHealth() {
        return SaveData.getInt(houseHealthTag);
    }
    public static void setHouseMaxHealth(int h) {
        SaveData.setInt(houseMaxHealthTag, h);
    }
    public static int getHouseMaxHealth() {
        return SaveData.getInt(houseMaxHealthTag);
    }
    public static void setHouseLightRad(float r) {
        SaveData.setFloat(houseLightRadTag, r);
    }
    public static float getHouseLightRad() {
        return SaveData.getFloat(houseLightRadTag);
    }

    public static void saveGameOptions(GameOptions go) {
        SaveData.setString(gameOptionsTag, JsonUtility.ToJson(go));
    }
    public static GameOptions getGameOptions() {
        var data = SaveData.getString(gameOptionsTag);
        if(!string.IsNullOrEmpty(data))
            return JsonUtility.FromJson<GameOptions>(data);

        //  doesn't have saved options, so create, save, and return default ones
        var o = new GameOptions(1.0f, 1.0f, 1.0f, FullScreenMode.ExclusiveFullScreen, true, GameOptions.TargetFrameRate.Unlimited);
        saveGameOptions(o);
        return o;
    }

    //  Resets all volume sliders to 1
    //  keeps old screen mode
    //  turns on vSync and sets targetFrameRate to Unlimited
    public static void resetGameOptions() {
        var p = getGameOptions();
        var o = new GameOptions(1.0f, 1.0f, 1.0f, p.screenMode, true, GameOptions.TargetFrameRate.Unlimited);
        saveGameOptions(o);
    }

    /*
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
    */
}

[System.Serializable]
public class GameOptions {
    public enum TargetFrameRate {
        Unlimited, Thirty, Sixty, OneTwenty
    }

    public float masterVol, musicVol, sfxVol;
    public FullScreenMode screenMode;
    public bool vSync;
    public TargetFrameRate targetFPS;

    public GameOptions(float masterVol, float musicVol, float sfxVol, FullScreenMode screenMode, bool vSync, TargetFrameRate targetFPS) {
        this.masterVol = masterVol;
        this.musicVol = musicVol;
        this.sfxVol = sfxVol;
        this.screenMode = screenMode;
        this.vSync = vSync;
        this.targetFPS = targetFPS;
    }
}
