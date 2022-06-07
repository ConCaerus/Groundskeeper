using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetLibrary : MonoBehaviour {
    [SerializeField] Weapon[] weapons;

    public Weapon getWeapon(int index) {
        return weapons[index];
    }
}
