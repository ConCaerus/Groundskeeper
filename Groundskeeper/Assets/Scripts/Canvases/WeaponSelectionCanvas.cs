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

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Aim.started += ctx => changeWeapon(ctx.ReadValue<Vector2>().x > 0);
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

    private void OnDisable() {
        controls.Disable();
    }


    //  buttons
    public void changeWeapon(bool right) {
        if(isOpen()) {
            //  changes the index of the player's current weapon
            var t = pStats.getWeaponTitle(pl);
            int ind = pl.getUnlockedWeaponIndex(t, true);
            //Debug.Log(t + " " + ind + " " + right + " " + pl.getUnlockedWeapons().Count);
            ind += right ? 1 : -1;
            if(ind < 0)
                ind = pl.getUnlockedWeapons(true).Count - 1;
            else if(ind >= pl.getUnlockedWeapons(true).Count)
                ind = 0;

            //  saves
            pStats.playerWeaponTitle = pl.getUnlockedWeapons(true)[ind].title.ToString();
            GameInfo.setPlayerStats(pStats);

            //  displays the new weapon sprite
            wsi.getWeaponSprite().sprite = pl.getUnlockedWeapons(true)[ind].sprite;
        }
    }
}
