using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponSelectionCanvas : MenuCanvas {
    [SerializeField] GameObject holder;

    WeaponSelectionInteractable wsi;
    PresetLibrary pl;
    PlayerStats pStats;

    private void Start() {
        wsi = FindObjectOfType<WeaponSelectionInteractable>();
        pl = FindObjectOfType<PresetLibrary>();
        holder.SetActive(false);
        pStats = GameInfo.getPlayerStats();
    }

    protected override void show() {
        holder.SetActive(true);
    }

    protected override void close() {
        holder.SetActive(false);
    }


    //  buttons
    public void changeWeapon(bool right) {
        //  changes the index of the player's current weapon
        var t = pStats.getWeaponTitle(pl);
        int ind = pl.getUnlockedWeaponIndex(t);
        //Debug.Log(t + " " + ind + " " + right + " " + pl.getUnlockedWeapons().Count);
        ind += right ? 1 : -1;
        if(ind < 0)
            ind = pl.getUnlockedWeapons().Count - 1;
        else if(ind >= pl.getUnlockedWeapons().Count)
            ind = 0;

        //  saves
        pStats.playerWeaponTitle = pl.getUnlockedWeapons()[ind].title.ToString();
        GameInfo.setPlayerStats(pStats);

        //  displays the new weapon sprite
        wsi.getWeaponSprite().sprite = pl.getUnlockedWeapons()[ind].sprite;
    }
}
