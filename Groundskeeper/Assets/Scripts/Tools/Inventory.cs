using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Inventory {
    public static string weaponsTag = "InventoryWeapons";

    public static void clear() {
        saveInventoryWeapons(new InventoryWeapons());
    }

    public static void addWeapon(int ind) {
        var w = getInventoryWeapons();
        if(!w.weapons.Contains(ind))
            w.weapons.Add(ind);
        saveInventoryWeapons(w);
    }
    public static void removeWeapon(int ind) {
        var w = getInventoryWeapons();
        if(!w.weapons.Contains(ind))
            w.weapons.Remove(ind);
        saveInventoryWeapons(w);
    }
    static InventoryWeapons getInventoryWeapons() {
        var data = SaveData.getString(weaponsTag);
        var thing = JsonUtility.FromJson<InventoryWeapons>(data);
        return thing == null ? new InventoryWeapons() : thing;
    }
    static void saveInventoryWeapons(InventoryWeapons w) {
        var data = JsonUtility.ToJson(w);
        SaveData.setString(weaponsTag, data);
    }

    public static List<int> getAllWeaponIndexes() {
        return getInventoryWeapons().weapons;
    }
    public static bool hasWeapon(int ind) {
        return getAllWeaponIndexes().Contains(ind);
    }
}


[System.Serializable]
public class InventoryWeapons {
    public List<int> weapons = new List<int>();
}
