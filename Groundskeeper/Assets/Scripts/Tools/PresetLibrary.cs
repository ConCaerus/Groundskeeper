using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetLibrary : MonoBehaviour {
    [SerializeField] GameObject[] monsters;
    [SerializeField] Weapon[] weapons;  //  first weapon in this array is the default player weapon
    [SerializeField] GameObject[] environment;

    private void Awake() {
        //  unlocks the first weapon for the player
        if(getUnlockedWeapons().Count == 0) {
            unlockWeapon(weapons[0].title);
        }
    }

    public GameObject getMonster(int index) {
        return monsters[index];
    }
    public GameObject getMonster(Monster.monsterType type) {
        foreach(var i in monsters) {
            if(i.GetComponent<MonsterInstance>().mType == type)
                return i;
        }
        return null;
    }
    public int getMonsterCount() {
        return monsters.Length;
    }

    public GameObject getEnvironment(int index) {
        return environment[index];
    }
    public GameObject getEnvironment(string name) {
        foreach(var i in environment) {
            if(i.GetComponent<EnvironmentInstance>().title == name)
                return i;
        }
        return null;
    }
    public GameObject getRandomEnvironment() {
        return getEnvironment(Random.Range(0, environment.Length));
    }
    public int getEnvironmentCount() {
        return environment.Length;
    }

    public Weapon getWeapon(int index) {
        return weapons[index];
    }
    public Weapon getWeapon(Weapon.weaponTitle title) {
        foreach(var i in weapons) {
            if(i.title == title)
                return i;
        }
        return null;
    }
    public int getWeaponIndex(Weapon.weaponTitle title) {
        for(int i = 0; i < weapons.Length; i++) {
            if(title == weapons[i].title)
                return i;
        }
        return -1;
    }
    public int getUnlockedWeaponIndex(Weapon.weaponTitle title) {
        var ws = getUnlockedWeapons();
        for(int i = 0; i < ws.Count; i++) {
            if(title == ws[i].title)
                return i;
        }
        return -1;
    }
    public Weapon getRandomLockedWeapon() {
        var ws = getLockedWeapons();
        if(ws.Count == 0)
            return null;
        return ws[Random.Range(0, ws.Count)];
    }

    public Weapon[] getWeapons() {
        return weapons;
    }
    public List<Weapon> getLockedWeapons() {
        var temp = new List<Weapon>();
        foreach(var i in weapons) {
            if(!GameInfo.isWeaponUnlocked(i.title))
                temp.Add(i);
        }
        return temp;
    }
    public List<Weapon> getUnlockedWeapons() {
        var temp = new List<Weapon>();
        foreach(var i in weapons) {
            if(GameInfo.isWeaponUnlocked(i.title))
                temp.Add(i);
        }
        return temp;
    }

    public bool unlockWeapon(Weapon.weaponTitle title) {
        if(title == Weapon.weaponTitle.None)
            return false;
        GameInfo.unlockWeapon(title);
        return true;
    }
    public void unlockAllWeapons() {
        foreach(var i in weapons)
            GameInfo.unlockWeapon(i.title);
    }

    //  tried to do it with looping through all weapons and checking if their title.ToString() was equal to the parameter title
    //  didn't work (kept thinking to nonequivalent strings were equal)
    public Weapon.weaponTitle getEquivalentWeaponTitle(string title) {
        switch(title.ToLower()) {
            case "axe": return Weapon.weaponTitle.Axe;
            case "shotgun": return Weapon.weaponTitle.Shotgun;
            case "rifle": return Weapon.weaponTitle.Shotgun;
            default: return Weapon.weaponTitle.None;
        }
    }
}
