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
    }

    public override void interact() {
        FindObjectOfType<WeaponSelectionCanvas>().tryShow();
    }

    public override void deinteract() {
        FindObjectOfType<WeaponSelectionCanvas>().tryClose();
    }

    public override void anim(bool b) {
        weaponSprite.sprite = FindObjectOfType<PresetLibrary>().getWeapon(GameInfo.getPlayerWeaponTitle(FindObjectOfType<PresetLibrary>())).sprite;
        GetComponent<Animator>().SetBool("showing", b);
    }

    public SpriteRenderer getWeaponSprite() {
        return weaponSprite;
    }
}
