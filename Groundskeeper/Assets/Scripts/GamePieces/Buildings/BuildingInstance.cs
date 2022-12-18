using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance : Building {
    [SerializeField] GameObject bloodParticles;

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        Destroy(gameObject);
    }
}
