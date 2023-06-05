using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamManager : MonoBehaviour {
    static bool initted = false;

    PauseMenu pm;

    public enum achievements {
        Live, Defender, InNeedOfHelp, StructurallySound, LiveMore, WeaponsMaster, None
    }

    private void Start() {
        if(initted)
            return;
        try {
            Steamworks.SteamClient.Init(2394770);
            initted = true;
        }
        catch { }

        pm = FindObjectOfType<PauseMenu>();
    }

    private void Update() {
        if(initted) {
            Steamworks.SteamClient.RunCallbacks();
            if(Steamworks.SteamUtils.IsOverlayEnabled && !pm.isOpen())
                pm.tryShow();
        }
    }

    private void OnApplicationQuit() {
        Steamworks.SteamClient.Shutdown();
    }

    public void resetAchievements() {
        int i = 0;
        while((achievements)i != achievements.None) {
            var ach = new Steamworks.Data.Achievement(tagToSteamName((achievements)i));
            ach.Clear();
            i++;
        }
    }
    public bool isAchievementUnlocked(achievements ach) {
        var i = new Steamworks.Data.Achievement(tagToSteamName(ach));
        return i.State;
    }
    public void unlockAchievement(achievements ach) {
        if(isAchievementUnlocked(ach))
            return;
        var i = new Steamworks.Data.Achievement(tagToSteamName(ach));
        i.Trigger();
    }

    string tagToSteamName(achievements ach) {
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
}
