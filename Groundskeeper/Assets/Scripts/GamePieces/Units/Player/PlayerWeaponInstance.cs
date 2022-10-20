using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerWeaponInstance : WeaponInstance {
    [HideInInspector]
    [SerializeField] public bool canAttack = true;

    private void Awake() {
        canAttack = true;
    }

    public override void movementLogic() {
        if(Input.GetMouseButton(0) && canAttack && transform.lossyScale.x > 0f)
            attack();
        lookAtMouse();
    }
}