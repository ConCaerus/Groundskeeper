using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresetLibrary : MonoBehaviour {
    [SerializeField] GameObject[] monsters;
    [SerializeField] Weapon[] weapons;
    [SerializeField] GameObject[] environment;

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

    public Weapon getWeapon(int index) {
        return weapons[index];
    }


    public Weapon[] getWeapons() {
        return weapons;
    }
}
