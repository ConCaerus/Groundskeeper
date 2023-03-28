using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackWeaponInstance : WeaponInstance {
    HelperInstance hi = null;

    private void Awake() {
        hi = user.GetComponent<HelperInstance>(); 
        reference = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponTitle.Axe);
    }

    public override void movementLogic() {
        if(hi.hasTarget) 
            lookAtPos(hi.target);
        else
            lookAtPos((Vector2)user.gameObject.transform.position + Vector2.right);
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }
}
