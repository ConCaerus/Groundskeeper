using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject {
    public enum attackType {
        Swing, Shoot
    }

    public enum weaponName {
        None, Axe, Pistol, Shotgun
    }

    public weaponName title;
    public int damage;
    public float knockback;
    public float cooldown;
    public attackType aType;
    public float range; //  only should be relevant for guns

    public Vector2 trailPos;

    public Sprite sprite;
    public AudioClip swingSound;
}