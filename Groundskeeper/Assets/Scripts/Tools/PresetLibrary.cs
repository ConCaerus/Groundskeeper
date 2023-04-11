using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PresetLibrary : MonoBehaviour {
    [SerializeField] GameObject[] monsters;
    [SerializeField] Weapon[] weapons;
    [SerializeField] GameObject[] environment;
    [SerializeField] GameObject[] deadGuys;

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

        //  determines how many souls it gives based on the night count
        temp.GetComponent<DeadGuyInstance>().soulsGiven = 150 * GameInfo.getNightCount();
        return temp;
    }
    public GameObject getDeadGuy(string title) {
        foreach(var i in deadGuys) {
            if(i.GetComponent<DeadGuyInstance>().title == title)
                return i;
        }
        return null;
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

        //  checks if no weapons are unlocked, if so, unlock the first one
        if(temp.Count == 0) {
            if(unlockWeapon(weapons[0].title))
                return getUnlockedWeapons();
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
        foreach(var i in weapons) {
            if(title == i.title.ToString())
                return i.title;
        }
        return Weapon.weaponTitle.None;
    }
}
