using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWeaponInstance : WeaponInstance {

    private void Awake() {
        hi = user.GetComponent<HelperInstance>();
        reference = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponTitle.Pitchfork);
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
