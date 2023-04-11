using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject {
    public enum attackType {
        Swing, Shoot, Lob, Stab
    }

    public enum weaponTitle {
        None, Axe, Rifle, Shotgun, Pitchfork, HolyWater, Dagger, Sledgehammer
    }

    public weaponTitle title;
    public int damage;
    public float knockback;
    public float cooldown;
    public attackType aType;
    public float range; //  only should be relevant for guns
    public float groundedTime;  //  only should be relevant for lobbed weapons

    public Vector2 trailPos;

    public Sprite sprite;
    public AudioClip swingSound;

    public bool playerWeapon;
}