using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackWeaponInstance : WeaponInstance {
    GameBoard gb;

    private void Awake() {
        hi = user.GetComponent<HelperInstance>(); 
        reference = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponTitle.Axe);
        gb = FindObjectOfType<GameBoard>();
    }

    public override void movementLogic() {
        if(hi.hasTarget && gb.monsters.Count > 0) 
            lookAtPos(gb.monsters.FindClosest(hi.transform.position).transform.position);
        else
            lookAtPos((Vector2)user.gameObject.transform.position + Vector2.right);
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }
}
