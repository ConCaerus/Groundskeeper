using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Helper : Attacker {
    public enum helperType {
        All, Attack, Repair, Heal
    }

    public float speed;
    public int helpAmount;
    public helperType helpType;
    public Sprite sprite;
}
