using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Helper : Attacker {
    public enum helpType {
        All, Attack, Repair
    }

    public string name;
    public float speed;
    public int cost;
    public int helpAmount;
    public helpType help;
    public Sprite sprite;
}
