using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance : Building {

    public override void die() {
        Destroy(gameObject);
    }
}
