using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : Attacker {
    public enum targetType {
        All, People, Buildings, House
    }

    public string name;
    public float speed;
    public int attackDamage;
    public int damageToDefences;
    public int diff = 0;
    public int earliestWave = 0;
    public targetType favoriteTarget;
    public GameInfo.EnemyType type;
    public float scareRadius;
    public Sprite sprite;
}
