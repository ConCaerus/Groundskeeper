using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionInteractable : Interactable {
    [SerializeField] SpriteRenderer weaponSprite;
    PresetLibrary pl;

    public override bool canInteract() {
        return true;
    }

    private void Start() {
        pl = FindObjectOfType<PresetLibrary>();
        weaponSprite.sprite = pl.getWeapon(GameInfo.getPlayerWeaponTitle(pl)).sprite;
    }

    public override void interact() {
        FindObjectOfType<WeaponSelectionCanvas>().tryShow();
    }

    public override void deinteract() {
        FindObjectOfType<WeaponSelectionCanvas>().tryClose();
    }

    public override void anim(bool b) {
        GetComponent<Animator>().SetBool("showing", b);
        if(b)
            FindObjectOfType<WeaponSelectionCanvas>().tryShow();
        else
            FindObjectOfType<WeaponSelectionCanvas>().tryClose();
    }

    public SpriteRenderer getWeaponSprite() {
        return weaponSprite;
    }
}
