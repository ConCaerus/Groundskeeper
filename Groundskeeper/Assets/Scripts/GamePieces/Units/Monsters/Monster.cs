using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : Attacker {
    public enum targetType {
        All, People, Buildings, House
    }

    public enum monsterType {
        Zombie, Vampire, Wisp, Wraith
    }

    public monsterType mType;
    public float speed;
    public int attackDamage;
    public int damageToDefences;
    public int diff = 0;
    public float soulsGiven = 0f;
    public int earliestNight = 0;
    public targetType favoriteTarget;
    public GameInfo.MonsterType type;
    public bool flying = false;
    public Sprite sprite;
}
