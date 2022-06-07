using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : BuildingMortal {
    public enum buildingType {
        None, ActiveDefence, PassiveDefence
    }

    public string name;
    public buildingType type;
    public Sprite sprite;
}
