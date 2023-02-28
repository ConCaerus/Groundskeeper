using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WeaponSelectionCanvas : MenuCanvas {
    [SerializeField] GameObject playerUITarget;
    [SerializeField] GameObject background;
    [SerializeField] Image weaponSprite;
    PresetLibrary pl;

    private void Start() {
        pl = FindObjectOfType<PresetLibrary>();
        background.SetActive(false);
    }

    protected override void show() {
        transform.position = playerUITarget.transform.position;
        weaponSprite.sprite = pl.getWeapon(GameInfo.getPlayerWeaponTitle(pl)).sprite;
        //weaponSprite.SetNativeSize();
        background.SetActive(true);
    }

    protected override void close() {
        background.SetActive(false);
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
        weaponSprite.sprite = pl.getUnlockedWeapons()[ind].sprite;
    }
}
