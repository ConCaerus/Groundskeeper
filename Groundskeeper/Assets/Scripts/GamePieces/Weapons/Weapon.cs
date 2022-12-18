using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
public class Weapon : ScriptableObject {
    public string title;
    public int damage;
    public float knockback;
    public float cooldown;

    public GameInfo.MonsterType targetType;

    public Vector2 trailPos;

    public Sprite sprite;
    public AudioClip swingSound;
}