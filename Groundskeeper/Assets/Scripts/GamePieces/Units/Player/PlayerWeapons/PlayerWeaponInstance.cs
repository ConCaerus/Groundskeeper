using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
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
    GameTutorialCanvas gtc;
    TransitionCanvas tc;
    PregameCanvas pc;
    PauseMenu pm;

    Coroutine queuedAttack = null;

    void Awake() {
        ssm = FindObjectOfType<SetupSequenceManager>();
        canAttackG = GameInfo.getNightCount() > 0;
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Attack.started += attackPerformed;
        controls.Player.Attack.canceled += attackEnded;
        gtc = FindObjectOfType<GameTutorialCanvas>();
        tc = FindObjectOfType<TransitionCanvas>();
        pc = FindObjectOfType<PregameCanvas>();
        pm = FindObjectOfType<PauseMenu>();


        //  sets up the variant
        Weapon we = FindObjectOfType<PresetLibrary>().getWeapon(GameInfo.getPlayerStats().getWeaponTitle(FindObjectOfType<PresetLibrary>()));
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
            case Weapon.weaponTitle.Dagger:
                variant = GetComponent<PlayerDaggerInstance>();
                break;
            case Weapon.weaponTitle.Sledgehammer:
                variant = GetComponent<PlayerSledgehammerInstance>();
                break;
            default:
                Debug.Log("PlayerWeaponInstance could not find a relevant variant script");
                break;
        }
        foreach(var i in GetComponents<PlayerWeaponVariant>()) {
            if(i != variant)
                i.enabled = false;
        }

        wAnimator.enabled = true;
        variant.setEqualTo(this);
        variant.variantSetup();
        variant.updateReference(we);

        if(we.aType != Weapon.attackType.Shoot)
            gunLight.gameObject.SetActive(false);
        else {
            gunLight.size = 0f;
            gtc.hasChargedAttacked();
        }

        if(GameInfo.getNightCount() == 0)
            GetComponent<SpriteRenderer>().sprite = null;

        used = false;
    }


    public override void movementLogic() {
        variant.lookAtMouse();
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }

    void attackPerformed(InputAction.CallbackContext c) {
        if((ssm != null && ssm.isActiveAndEnabled) || !tc.finishedLoading || this == null || pm.isOpen() || Time.timeScale == 0.0f)
            return;
        if(canAttackG && transform.lossyScale.x > 0f && a.getCanAttack()) {
            if(gtc != null)
                gtc.hasAttacked();
            variant.performOnAttack();
        }
        else {
            sr.DOKill();
            sr.color = Color.red;
            sr.DOColor(Color.white, .35f);
            if(pc == null)
                queuedAttack = StartCoroutine(attackAfterCooldown());
        }
    }

    void attackEnded(InputAction.CallbackContext c) {
        if(queuedAttack != null)
            StopCoroutine(queuedAttack);
        queuedAttack = null;
        if(canAttackG && transform.lossyScale.x > 0f && !ssm.isActiveAndEnabled) {
            variant.performOnAttackEnd();
        }
    }

    IEnumerator attackAfterCooldown() {
        while(!(canAttackG && transform.lossyScale.x > 0f && a.getCanAttack()))
            yield return new WaitForEndOfFrame();
        attackPerformed(new InputAction.CallbackContext());
    }

    public Weapon getWeapon() {
        return variant.reference;
    }

    private void OnDisable() {
        controls.Player.Attack.started -= attackPerformed;
        controls.Player.Attack.canceled -= attackEnded;
        controls.Disable();
    }
}

public abstract class PlayerWeaponVariant : WeaponInstance {
    [SerializeField] Weapon.weaponTitle title;

    protected Coroutine charger = null;
    protected PlayerUICanvas puc;

    public abstract void variantSetup();

    public abstract void performOnAttack();

    public abstract void performOnAttackEnd();


    public override void movementLogic() {
        lookAtMouse();
    }

    protected IEnumerator chargeTimer() {
        float maxCharge = 2.0f;
        float tickTime = .3f, origTickTime = .3f;
        float ticksToComplete = 5;  //  zero doens't count, so it'll seem like it'll take this +1 ticks to complete
        float target = 0f;
        bool firstTime = true;

        for(int i = 0; i <= ticksToComplete; i++) {
            //  not charging anymore
            if(charger == null)
                yield return 0;



            var s = pi.isSprinting();
            //  the player is sprinting
            if(s) {
                tickTime = .25f;
            }

            yield return new WaitForSeconds(tickTime * .85f);  //  tick time

            if(pi.isSprinting()) {
                i--;
                if(i < 0)
                    i = 0;
                s = true;
            }

            //  charge is empty
            if(i == 0) {
                pi.weaponAttackMod = 1.0f;
                target = 1.01f;
                //  initial wait time that only exists for before ticks are counted
                if(firstTime) {
                    //yield return new WaitForSeconds(tickTime);   //  - ticktime because ticktime gets waited for at the end
                    s = pi.isSprinting();
                    firstTime = false;
                }
            }
            //  charge is full
            else if(i == ticksToComplete) {
                target = maxCharge;
                if(!s)
                    yield return new WaitForSeconds(tickTime * .15f);
                i -= 1;
            }
            else if(i > 0 && i < ticksToComplete) {
                if(!s)
                    yield return new WaitForSeconds(tickTime * .15f);
                target = ((i * (maxCharge - 1f)) / ticksToComplete) + 1f;
            }
            else if(!s)
                yield return new WaitForSeconds(tickTime * .15f);

            DOTween.To(() => pi.weaponAttackMod, x => pi.weaponAttackMod = x, target, .1f).OnUpdate(() => {
                if(charger != null)
                    puc.updateChargeSlider(pi.weaponAttackMod - 1f, maxCharge - 1f);
                else
                    puc.updateChargeSlider(0f, maxCharge);
            });

            if(s)
                i--;

            tickTime = origTickTime;
        }
    }
}