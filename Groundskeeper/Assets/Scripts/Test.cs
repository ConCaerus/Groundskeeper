using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Test : MonoBehaviour {

    private void Start() {
        GameInfo.setPlayerWeapon(Weapon.weaponTitle.Shotgun);
        //GameInfo.getPlayerWeaponTitle(FindObjectOfType<PresetLibrary>());
    }
}
