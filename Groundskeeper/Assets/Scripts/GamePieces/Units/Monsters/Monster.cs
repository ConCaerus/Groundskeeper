using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : Attacker {
    public enum targetType {
        All, People, Structures, House
    }

    public enum monsterTitle {
        Zombie, Vampire, Wisp, Wraith, Clown, Ghost, Demon, Wendigo, Cultist, Imp, SkinWalker
    }

    public monsterTitle title;
    public float speed;
    public int attackDamage;
    public int damageTodefenses;
    public int diff = 0;
    public float originalSoulsGiven = 0f, soulsGiven = 0f;
    public int earliestNight = 0;
    public targetType favoriteTarget;
    public GameInfo.MonsterType type;
    public MonsterSpawner.frequency freqInWave;
    public MonsterSpawner.position posInWave;
    public bool flying = false;
    public Sprite sprite;
}
