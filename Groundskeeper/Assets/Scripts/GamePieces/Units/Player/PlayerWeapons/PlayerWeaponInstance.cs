using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem;

public class PlayerWeaponInstance : WeaponInstance {
    bool canAttack;
    public bool canAttackG {
        get { return canAttack; }
        set {
            canAttack = value;
        }
    }

    [HideInInspector] public PlayerWeaponVariant variant;

    SetupSequenceManager ssm;
    InputMaster controls;

    private void Awake() {
        ssm = FindObjectOfType<SetupSequenceManager>();
        canAttackG = GameInfo.getNightCount() > 0;
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Attack.performed += attackPerformed;
        controls.Player.Attack.canceled += attackEnded;
        if(variant == null)
            variant = GetComponent<PlayerWeaponVariant>();

        //  sets up the variant
        variant.setEqualTo(this);
        variant.setup();
        Weapon we = null;
        if(variant.GetComponent<PlayerAxeInstance>() != null)
            we = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponName.Axe);
        else if(variant.GetComponent<PlayerPistolInstance>() != null)
            we = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponName.Pistol);
        else if(variant.GetComponent<PlayerShotgunInstance>() != null)
            we = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponName.Shotgun);
        variant.updateReference(we);
        used = false;
    }


    public override void movementLogic() {
        variant.lookAtMouse();
    }

    public override void shootMonster() {
    }

    void attackPerformed(InputAction.CallbackContext c) {
        if(canAttackG && transform.lossyScale.x > 0f && !ssm.isActiveAndEnabled) {
            variant.performOnAttack();
        }
    }

    void attackEnded(InputAction.CallbackContext c) {
        if(canAttackG && transform.lossyScale.x > 0f && !ssm.isActiveAndEnabled) {
            variant.performOnAttackEnd();
        }
    }

    public Weapon getWeapon() {
        return variant.reference;
    }

    private void OnDisable() {
        controls.Disable();
    }
}

public abstract class PlayerWeaponVariant : WeaponInstance {
    [SerializeField] Weapon.weaponName title;

    public abstract void setup();

    public abstract void performOnAttack();

    public abstract void performOnAttackEnd();


    public override void movementLogic() {
        lookAtMouse();
    }
}