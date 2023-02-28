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


        //  sets up the variant
        Weapon we = FindObjectOfType<PresetLibrary>().getWeapon(GameInfo.getPlayerWeaponTitle(FindObjectOfType<PresetLibrary>()));
        switch(we.title) {
            case Weapon.weaponTitle.Axe:
                variant = GetComponent<PlayerAxeInstance>();
                break;
            case Weapon.weaponTitle.Shotgun:
                variant = GetComponent<PlayerShotgunInstance>();
                break;
            case Weapon.weaponTitle.Rifle:
                variant = GetComponent<PlayerRifleInstance>();
                break;
            default:
                Debug.LogError("PlayerWeaponInstance could not find a relevant variant script");
                break;
        }
        foreach(var i in GetComponents<PlayerWeaponVariant>()) {
            if(i != variant)
                i.enabled = false;
        }

        wAnimator.enabled = true;
        variant.setEqualTo(this);
        variant.setup();
        variant.updateReference(we);

        if(we.aType != Weapon.attackType.Shoot)
            gunLight.gameObject.SetActive(false);
        else
            gunLight.size = 0f;

        if(GameInfo.getNightCount() == 0)
            GetComponent<SpriteRenderer>().sprite = null;

        used = false;
    }


    public override void movementLogic() {
        variant.lookAtMouse();
    }

    public override void shootMonster() {
    }

    void attackPerformed(InputAction.CallbackContext c) {
        if(ssm.isActiveAndEnabled)
            return;
        if(canAttackG && transform.lossyScale.x > 0f && a.getCanAttack()) {
            variant.performOnAttack();
        }
        else {
            sr.DOKill();
            sr.color = Color.red;
            sr.DOColor(Color.white, .35f);
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
    [SerializeField] Weapon.weaponTitle title;

    public abstract void setup();

    public abstract void performOnAttack();

    public abstract void performOnAttackEnd();


    public override void movementLogic() {
        lookAtMouse();
    }
}