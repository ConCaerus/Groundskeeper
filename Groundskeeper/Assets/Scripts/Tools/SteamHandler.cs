using UnityEngine;
using Steamworks;
using System.Collections;
using Newtonsoft.Json.Bson;

public class SteamHandler : MonoBehaviour {
    static bool initted = false;

    protected Callback<GameOverlayActivated_t> overlayIsOn;

    public enum achievements {
        Live, Defender, InNeedOfHelp, StructurallySound, LiveMore, WeaponsMaster, None
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
        initted = true;
        overlayIsOn = Callback<GameOverlayActivated_t>.Create(pauseGameIfSteamOverlayOn);

        //StartCoroutine(callbacker());
    }

    void pauseGameIfSteamOverlayOn(GameOverlayActivated_t obj) {
        var pm = FindObjectOfType<PauseMenu>();
        if(pm != null && !pm.isOpen())
            pm.tryShow();
    }

    public void resetAchievements() {
        if(!initted || !SteamManager.Initialized)
            return;
        int i = 0;
        while((achievements)i != achievements.None) {
            SteamUserStats.ClearAchievement(tagToSteamName((achievements)i));
            i++;
        }
        SteamUserStats.StoreStats();
    }
    public bool isAchievementUnlocked(achievements ach) {
        if(!initted || !SteamManager.Initialized)
            return false;
        SteamUserStats.GetAchievement(tagToSteamName(ach), out bool achievementCompleted);
        return achievementCompleted;
    }
    public void unlockAchievement(achievements ach) {
        if(!initted || !SteamManager.Initialized)
            return;
        if(isAchievementUnlocked(ach))
            return;
        SteamUserStats.SetAchievement(tagToSteamName(ach));
        SteamUserStats.StoreStats();
    }

    string tagToSteamName(achievements ach) {
        if(!initted || !SteamManager.Initialized)
            return "";
        switch(ach) {
            case achievements.Live: return "COS_firstNight";
            case achievements.Defender: return "COS_maxedDefenses";
            case achievements.InNeedOfHelp: return "COS_maxedHelpers";
            case achievements.StructurallySound: return "COS_maxedStructures";
            case achievements.LiveMore: return "COS_tenthNight";
            case achievements.WeaponsMaster: return "COS_maxedWeapons";
            default: return string.Empty;
        }
    }

    IEnumerator callbacker() {
        if(initted && SteamManager.Initialized) {
            SteamAPI.RunCallbacks();
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(callbacker());
    }
}
