using JetBrains.Annotations;
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
        if(nc < 15)
            return 7;
        return 10;
    }

    public static MouseManager curMouseManager = null;

    static int houseHealthHealedPerNight = 100;

    public static int monstersKilled { get; set; } = 0;

    //  nights / waves / enemies shit
    static string soulTag = "souls";
    static string nightTag = "nights";
    static string lastSeenEnemyTag = "LastSeenEnemyTag";

    //  player / house shit
    static string playerStatsTag = "PlayerStatsTag";

    //  Game Board shit
    public static string helperTag = "helperTag";
    public static string lastSavedHelperCount = "LastSavedHelperCount";

    public static string defenseTag = "defenseTag";
    public static string lastSavedDefenseCount = "LastSavedDefenseCount";

    public static string structureTag = "miscTag";
    public static string lastSavedStructureCount = "LastSavedMiscCount";

    public static string deadGuyTag = "deadGuyTag";
    public static string lastSavedDeadGuyCount = "LastSavedDeadGuyCount";

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
    static string helperStatsTag = "HelperStatsTag";
    static string defenseStatsTag = "DamageStatsTag";
    static string structureStatsTag = "StructureStatsTag";

    //  house buffs
    static string houseStatsTag = "HouseStatsTag";

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

    //  which scene the player is in
    static string currentSceneTag = "CurrentScene";


    public enum MonsterType {
        Both, Physical, Nonphysical
    }
    public enum GamePiece {
        None, Player, Helper, Defense, Structure, House, Monster, Environment
    }
    public static GamePiece tagToGamePiece(string tag) {
        switch(tag) {
            case "Player": return GamePiece.Player;
            case "Helper": return GamePiece.Helper;
            case "Defense": return GamePiece.Defense;
            case "Structure": return GamePiece.Structure;
            case "House": return GamePiece.House;
            case "Monster": return GamePiece.Monster;
            case "Environment": return GamePiece.Environment;
        }
        return GamePiece.None;
    }
    public static string gamePieceToTag(GamePiece piece) {
        return piece.ToString();
    }
    public enum SceneType {
        None, Game, House
    }

    //  makes it so if the player buys something and leaves the game before the board saves, they don't lose any souls
    public static float souls;

    public static void init() {
        Cursor.lockState = CursorLockMode.Confined;
        playing = true;
        wave = 0;
        monstersKilled = 0;
        monstersKilled = 0;
        souls = getSouls(true);
    }
    public static void resetSave(BuyableLibrary bl, PresetLibrary pl) {
        souls = 0;
        lockAllBuyables(bl);
        lockAllWeaponsExceptStarting(pl);
        saveSouls();
        resetNights();
        resetLastSeenEnemy();
        clearBoard();
        setCurrentScene(SceneType.Game);

        //  resets the house
        setHouseStats(new HouseStats(1000, 1000, 30f));
    }
    public static void clearBoard() {
        //      clears all of the shit before saving new shit
        for(int i = 0; i < SaveData.getInt(lastSavedHelperCount) + 1; i++)
            SaveData.deleteKey(helperTag + i.ToString(), false);
        for(int i = 0; i < SaveData.getInt(lastSavedDefenseCount) + 1; i++)
            SaveData.deleteKey(defenseTag + i.ToString(), false);
        for(int i = 0; i < SaveData.getInt(lastSavedStructureCount) + 1; i++)
            SaveData.deleteKey(structureTag + i.ToString(), false);
        for(int i = 0; i < SaveData.getInt(envCount) + 1; i++)
            SaveData.deleteKey(envTag + i.ToString(), false);

        SaveData.setInt(lastSavedHelperCount, 0);
        SaveData.setInt(lastSavedDefenseCount, 0);
        SaveData.setInt(lastSavedStructureCount, 0);
        SaveData.setInt(envCount, 0);
    }

    public static Vector2 mousePos() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static int calcPartygoerCount() {
        return 5 + getNightCount() * 3;
    }


    //  saves
    public static void addSouls(float c, bool saveImmediately) {
        souls = Mathf.Clamp(souls + c, 0f, Mathf.Infinity);
        if(saveImmediately)
            saveSouls();
    }
    public static void saveSouls() {  //  scary function OwO
        SaveData.setFloat(soulTag, souls);
    }
    public static float getSouls(bool getSaved) {
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

    public static void updateLastSeenEnemyIndex(PresetLibrary pl) {
        int temp = 0;
        foreach(var i in pl.getMonsters()) {
            if(i.GetComponent<MonsterInstance>().earliestNight <= getNightCount()) {
                temp++;
            }
        }
        SaveData.setInt(lastSeenEnemyTag, temp);
    }
    static void resetLastSeenEnemy() {
        SaveData.setInt(lastSeenEnemyTag, 0);
    }
    public static int getLastSeenEnemyIndex() {
        return SaveData.getInt(lastSeenEnemyTag);
    }

    //  player's weapons
    //  scary function that does scary things
    static void lockAllWeaponsExceptStarting(PresetLibrary pl) {
        //  locks everything
        foreach(var i in pl.getWeapons(false))
            SaveData.setInt(unlockedWeaponTag(i.title), 0);

        //  except default
        unlockWeapon(pl.defaultWeapon);
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
    public static void setHelperStats(HelperStats stats) {
        var data = JsonUtility.ToJson(stats);
        SaveData.setString(helperStatsTag, data);
    }
    public static HelperStats getHelperStats() {
        var data = SaveData.getString(helperStatsTag);
        if(string.IsNullOrEmpty(data))
            return new HelperStats(1.0f, 1.0f);
        return JsonUtility.FromJson<HelperStats>(data);
    }
    //  defense buffs
    public static void setdefenseStats(defenseStats stats) {
        var data = JsonUtility.ToJson(stats);
        SaveData.setString(defenseStatsTag, data);
    }
    public static defenseStats getdefenseStats() {
        var data = SaveData.getString(defenseStatsTag);
        if(string.IsNullOrEmpty(data))
            return new defenseStats(1.0f);
        return JsonUtility.FromJson<defenseStats>(data);
    }
    //  structure buffs
    public static void setStructureStats(StructureStats stats) {
        var data = JsonUtility.ToJson(stats);
        SaveData.setString(structureStatsTag, data);
    }
    public static StructureStats getStructureStats() {
        var data = SaveData.getString(structureStatsTag);
        if(string.IsNullOrEmpty(data))
            return new StructureStats(1.0f);
        return JsonUtility.FromJson<StructureStats>(data);
    }
    //  player buffs
    public static void setPlayerStats(PlayerStats stats) {
        var data = JsonUtility.ToJson(stats);
        SaveData.setString(playerStatsTag, data);
    }
    public static PlayerStats getPlayerStats() {
        var data = SaveData.getString(playerStatsTag);
        if(string.IsNullOrEmpty(data))
            return new PlayerStats(Weapon.weaponTitle.Axe, 1.0f, 1.0f);
        return JsonUtility.FromJson<PlayerStats>(data);
    }

    //  house buffs
    public static void setHouseStats(HouseStats stats) {
        //  clamps stats to be realistic
        stats.houseHealth = Mathf.Clamp(stats.houseHealth, -1, stats.houseMaxHealth);

        var data = JsonUtility.ToJson(stats);
        SaveData.setString(houseStatsTag, data);
    }
    public static HouseStats getHouseStats() {
        var data = SaveData.getString(houseStatsTag);
        if(string.IsNullOrEmpty(data))
            return new HouseStats(1000, 1000, 30f);
        return JsonUtility.FromJson<HouseStats>(data);
    }
    public static void healHousePerNight() {
        var temp = getHouseStats();
        temp.houseHealth = (int)Mathf.Clamp(temp.houseHealth + houseHealthHealedPerNight, 0.0f, temp.houseMaxHealth);
        setHouseStats(temp);
    }   //  called from the door interactable of the house interior

    public static void saveGameOptions(GameOptions go) {
        SaveData.setUniversalString(gameOptionsTag, JsonUtility.ToJson(go));
    }
    public static GameOptions getGameOptions() {
        var data = SaveData.getUniversalString(gameOptionsTag);
        if(!string.IsNullOrEmpty(data))
            return JsonUtility.FromJson<GameOptions>(data);

        //  doesn't have saved options, so create, save, and return default ones
        var o = new GameOptions(1.0f, 1.0f, 1.0f, FullScreenMode.ExclusiveFullScreen, GameOptions.TargetFrameRate.Unlimited);
        saveGameOptions(o);
        return o;
    }

    //  Resets all volume sliders to default
    //  keeps old screen mode
    //  turns on vSync and sets targetFrameRate to Unlimited
    public static void resetGameOptions() {
        var p = getGameOptions();
        var o = new GameOptions(0.75f, 1.0f, 1.0f, p.screenMode, GameOptions.TargetFrameRate.Unlimited);
        saveGameOptions(o);
    }

    public static void setCurrentScene(SceneType scene) {
        SaveData.setString(currentSceneTag, scene.ToString());
    }
    public static string getCurrentScene() {
        return SaveData.getString(currentSceneTag);
    }
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

    public GameOptions(float masterVol, float musicVol, float sfxVol, FullScreenMode screenMode, TargetFrameRate targetFPS, bool vSync = true) {
        this.masterVol = masterVol;
        this.musicVol = musicVol;
        this.sfxVol = sfxVol;
        this.screenMode = screenMode;
        this.vSync = vSync;
        this.targetFPS = targetFPS;
    }
}

[System.Serializable]
public class PlayerStats {
    public string playerWeaponTitle;
    public float playerWeaponDamageBuff;
    public float playerWeaponSpeedBuff;

    public PlayerStats(Weapon.weaponTitle w, float wDmgBuff, float wSpdBuff) {
        playerWeaponTitle = w.ToString();
        playerWeaponDamageBuff = wDmgBuff;
        playerWeaponSpeedBuff = wSpdBuff;
    }
    public Weapon.weaponTitle getWeaponTitle(PresetLibrary pl) {
        return pl.getEquivalentWeaponTitle(playerWeaponTitle);
    }
}

[System.Serializable]
public class HelperStats {
    public float helperWeaponDamageBuff;
    public float helperWeaponHealthBuff;

    public HelperStats(float wDmgBuff, float hBuff) {
        helperWeaponDamageBuff = wDmgBuff;
        helperWeaponHealthBuff = hBuff;
    }
}

[System.Serializable]
public class defenseStats {
    public float defenseDamageBuff;

    public defenseStats(float dmgBuff) {
        defenseDamageBuff = dmgBuff;
    }
}

[System.Serializable]
public class StructureStats {
    public float structureHealthBuff;

    public StructureStats(float hlthBuff) {
        structureHealthBuff = hlthBuff;
    }
}

[System.Serializable]
public class HouseStats {
    public int houseHealth;
    public int houseMaxHealth;
    public float houseLightRad;

    public HouseStats(int h, int maxH, float lightRad) {
        houseHealth = h;
        houseMaxHealth = maxH;
        houseLightRad = lightRad;
    }
}
