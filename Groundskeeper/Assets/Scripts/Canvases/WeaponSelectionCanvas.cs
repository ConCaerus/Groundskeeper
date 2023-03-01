using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponSelectionCanvas : MenuCanvas {
    [SerializeField] GameObject holder;

    WeaponSelectionInteractable wsi;
    PresetLibrary pl;

    private void Start() {
        wsi = FindObjectOfType<WeaponSelectionInteractable>();
        pl = FindObjectOfType<PresetLibrary>();
        holder.SetActive(false);
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
        var t = GameInfo.getPlayerWeaponTitle(pl);
        int ind = pl.getUnlockedWeaponIndex(t);
        //Debug.Log(t + " " + ind + " " + right + " " + pl.getUnlockedWeapons().Count);
        ind += right ? 1 : -1;
        if(ind < 0)
            ind = pl.getUnlockedWeapons().Count - 1;
        else if(ind >= pl.getUnlockedWeapons().Count)
            ind = 0;

        GameInfo.setPlayerWeapon(pl.getUnlockedWeapons()[ind].title);

        //  displays the new weapon sprite
        wsi.getWeaponSprite().sprite = pl.getUnlockedWeapons()[ind].sprite;
    }
}
