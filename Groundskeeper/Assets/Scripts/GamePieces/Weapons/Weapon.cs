using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject {
    public enum attackType {
        Swing, Shoot
    }

    public enum weaponTitle {
        None, Axe, Rifle, Shotgun
    }

    public weaponTitle title;
    public int damage;
    public float knockback;
    public float cooldown;
    public attackType aType;
    public float range; //  only should be relevant for guns

    public Vector2 trailPos;

    public Sprite sprite;
    public AudioClip swingSound;
}