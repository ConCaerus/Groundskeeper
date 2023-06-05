using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PresetLibrary : MonoBehaviour {
    [SerializeField] GameObject[] monsters;
    [SerializeField] Weapon[] weapons;
    [SerializeField] GameObject[] environment;
    [SerializeField] GameObject[] deadGuys;
    [SerializeField] public Weapon.weaponTitle defaultWeapon;

    public GameObject[] getMonsters() {
        return monsters;
    }
    public GameObject getMonster(int index) {
        return monsters[index];
    }
    public GameObject getMonster(Monster.monsterTitle type) {
        foreach(var i in monsters) {
            if(i.GetComponent<MonsterInstance>().title == type)
                return i;
        }
        return null;
    }
    public int getMonsterCount() {
        return monsters.Length;
    }

    public List<GameObject> getAvailableMonsters() {
        List<GameObject> temp = new List<GameObject>();
        foreach(var i in monsters) {
            if(i.GetComponent<MonsterInstance>().earliestNight <= GameInfo.getNightCount()) {
                temp.Add(i.gameObject);
            }
        }
        return temp;
    }

    public void sortMonsters() {
        //  creates a pool that can have monsters taken out of
        var pool = new List<GameObject>();
        foreach(var i in monsters)
            pool.Add(i);

        //  does the sorting
        for(int i = 0; i < monsters.Length; i++) {
            var winner = pool[0];

            for(int j = pool.Count - 1; j > 0; j--) {
                if(pool[j].GetComponent<Monster>().earliestNight < winner.GetComponent<MonsterInstance>().earliestNight)
                    winner = pool[j];
            }
            monsters[i] = winner;
            pool.Remove(winner);
        }
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

    public GameObject getRandomDeadGuy() {
        //  selects a random dead guy from the array
        var temp = deadGuys[Random.Range(0, deadGuys.Length)];
        return temp;
    }
    public GameObject getDeadGuy(string title) {
        GameObject temp = null;
        foreach(var i in deadGuys) {
            if(i.GetComponent<DeadGuyInstance>().title == title) {
                temp = i.gameObject;
                break;
            }
        }
        return temp;
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
    public int getUnlockedWeaponIndex(Weapon.weaponTitle title, bool playerExclusive) {
        var ws = getUnlockedWeapons(playerExclusive);
        for(int i = 0; i < ws.Count; i++) {
            if(title == ws[i].title)
                return i;
        }
        return -1;
    }
    public Weapon getRandomLockedWeapon(bool playerExclusive) {
        var ws = getLockedWeapons(playerExclusive);
        if(ws.Count == 0)
            return null;
        return ws[Random.Range(0, ws.Count)];
    }

    public List<Weapon> getWeapons(bool playerExclusive) {
        List<Weapon> temp = new List<Weapon>();
        foreach(var i in weapons) {
            if(i.playerWeapon || !playerExclusive)
                temp.Add(i);
        }
        return temp;
    }
    public List<Weapon> getLockedWeapons(bool playerExclusive) {
        var temp = new List<Weapon>();
        foreach(var i in weapons) {
            if(!GameInfo.isWeaponUnlocked(i.title) && (!playerExclusive || i.playerWeapon))
                temp.Add(i);
        }
        //  makes sure that the default weapon is not in this list
        foreach(var i in temp) {
            if(i.title == defaultWeapon) {
                unlockWeapon(defaultWeapon);
                return getLockedWeapons(playerExclusive);
            }
        }
        return temp;
    }
    public List<Weapon> getUnlockedWeapons(bool playerExclusive) {
        var temp = new List<Weapon>();
        foreach(var i in weapons) {
            if(GameInfo.isWeaponUnlocked(i.title) && (!playerExclusive || i.playerWeapon))
                temp.Add(i);
        }
        //  makes sure that the list contains the default weapon
        foreach(var i in temp) {
            if(i.title == defaultWeapon)
                return temp;
        }
        //  if it doesn't, unlock the default weapon and go through the function again
        unlockWeapon(defaultWeapon);
        return getUnlockedWeapons(playerExclusive);
    }

    public bool unlockWeapon(Weapon.weaponTitle title) {
        if(title == Weapon.weaponTitle.None)
            return false;
        GameInfo.unlockWeapon(title);
        return true;
    }
    public void unlockAllWeapons() {
        foreach(var i in weapons) {
            if(i.playerWeapon)
                GameInfo.unlockWeapon(i.title);
        }
    }


    public Weapon.weaponTitle getEquivalentWeaponTitle(string title) {
        foreach(var i in weapons) {
            if(title == i.title.ToString())
                return i.title;
        }
        return Weapon.weaponTitle.None;
    }
}
