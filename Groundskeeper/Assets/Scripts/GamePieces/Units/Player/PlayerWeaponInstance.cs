using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWeaponInstance : WeaponInstance {
    [HideInInspector]
    [SerializeField] public bool canAttack = false;
    public override void movementLogic() {
        if(Input.GetMouseButtonDown(0) && canAttack)
            attack();
        lookAtMouse();
    }
}