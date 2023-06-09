using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class PlayerInstance : Attacker {
    [SerializeField] float walkSpeed = 8.0f, runSpeed = 18.0f, accSpeed = .1f, slowSpeed = 18.0f;
    [HideInInspector] public bool slowed = false;
    float unitStamina = 100f;
    public float stamina {
        get {
            return unitStamina;
        }

        set {
            unitStamina = value;
            FindObjectOfType<PlayerUICanvas>().updateStamSlider(value, 100f);
        }
    }
    float stamInc = 0.0f;
    InputMaster controls;

    [HideInInspector] public float weaponAttackMod = 1.0f; //  this should only be changed by playerWeaponInstance scripts (please)

    [HideInInspector] public float initLightSize;


    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    Rigidbody2D rb;
    PlayerWeaponInstance pwi;

    [SerializeField] GameObject bloodParticles;

    [SerializeField] Light2D pLight;
    public Vector2 hCenter;
    Collider2D c;
    PlayerStats pStats;

    private void OnCollisionStay2D(Collision2D col) {
        ls.requestNewSortingLayer(c, sr);
    }


    private void Awake() {
        mortalInit();
        controls = new InputMaster();
        rb = GetComponent<Rigidbody2D>();
        DOTween.Init();
        movementInit(FindObjectOfType<SetupSequenceManager>(), FindObjectOfType<LayerSorter>(), true);
        pwi = GetComponentInChildren<PlayerWeaponInstance>();
        c = GetComponent<Collider2D>();
        pStats = GameInfo.getPlayerStats();

        controls.Player.Move.performed += ctx => movementChange(ctx.ReadValue<Vector2>());
        GetComponentInChildren<HealthBar>().setParent(gameObject);
        initLightSize = pLight.size;
        spriteOriginal = spriteObj.transform.localScale;
        shadowOriginal = shadowObj.transform.localScale;
    }

    private void Start() {
        canMove = false;
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        if(FindObjectOfType<HouseInstance>() != null) {
            transform.position = FindObjectOfType<HouseInstance>().playerSpawnPos.transform.position;
        }
        FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(transform.position, 5f);
        StartCoroutine(passiveHealthRegen());
        if(pLight.isActiveAndEnabled)
            StartCoroutine(moveLightWithPlayer());
    }


    #region ---   MOVEMENT SHIT   ---
    private void FixedUpdate() {
        //  check if game is over
        if(!GameInfo.playing) {
            controls.Disable();
            pwi.enabled = false;
            enabled = false;
            return;
        }

        //  move the object
        moveWithDir(moveInfo, rb, (isSprinting() ? runSpeed : walkSpeed) * (slowed ? .5f : 1.0f));
        stamina = Mathf.Clamp(stamina - (controls.Player.Sprint.IsPressed() && controls.Player.Move.inProgress ? 1 : -.5f - stamInc), 0f, 100f);
        stamInc += controls.Player.Sprint.IsPressed() && controls.Player.Move.inProgress ? -stamInc : .01f;

        if(gtc != null) {
            if(isSprinting())
                gtc.hasSprinted();
            if(moveInfo != Vector2.zero)
                gtc.hasMoved();
        }

        //  if player is not moving, decrement the movementInfo
        //  if player is moving, increase the movementInfo to target value
        moveInfo = controls.Player.Move.inProgress ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, Vector2.zero, slowSpeed * 100.0f * Time.fixedDeltaTime);
        
        //  if the player's not moving, look at the mouse
        if(!controls.Player.Move.inProgress) {
            lookAtPos(mm.usingKeyboard() ? GameInfo.mousePos() : !fgc.isActiveAndEnabled ? ggc.getMousePosInWorld() : fgc.getWorldCursorPos());
        }

        //  adjusts the player's light based on how far they are to the edge
        //      player is out of bounds, so turn off their light
        if(hi == null || !pLight.isActiveAndEnabled)
            return;
        if(Vector2.Distance(transform.position, hCenter) >= 75f) {
            DOTween.To(() => pLight.size, x => pLight.size = x, 0f, .25f);
        }
        //  player is in bounds, so tune their light based on how far they are from the edge
        else {
            var distPerc = Vector2.Distance(transform.position, hCenter) / 100f;   //  a little over the edge to give the player a little light at the edge
            pLight.size = initLightSize * (1 - distPerc);
        }
    }
    void movementChange(Vector2 dir) {
        targetMoveInfo = dir;
    }
    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }
    public override bool restartWalkAnim() {
        return controls.Player.Move.inProgress;
    }
    public override void updateSprite(Vector2 movingDir, bool opposite) {
        //  not moving
        if(movingDir == Vector2.zero)
            return;

        //  moving more along the y axis, set to a y axis sprite
        else if(Mathf.Abs(movingDir.x) < .1f && Mathf.Abs(movingDir.y) > .1f) {
            if(movingDir.y > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = backSprite;
            else
                spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
        }
        //  set to a x axis sprite
        else {
            if(movingDir.x > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = rightSprite;
            else if(movingDir.x < 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = leftSprite;
        }
    }
    public override WalkAnimInfo getWalkInfo() {
        return new WalkAnimInfo(.2f, .7f, 20f, 0f);
    }

    public override Vector2 getSpriteOriginalScale() {
        return spriteOriginal;
    }
    public override Vector2 getShadowOriginalScale() {
        return shadowOriginal;
    }
    public bool isSprinting() {
        return controls.Player.Sprint.IsPressed() && stamina > 0f;
    }

    IEnumerator moveLightWithPlayer() {
        while(true) {
            pLight.transform.position = transform.position;
            yield return new WaitForSeconds(0f);
        }
    }
    #endregion


    #region ---   ATTACKER SHIT   ---
    public override float getAttackCoolDown() {
        return pwi.getWeapon().cooldown / pStats.playerWeaponSpeedBuff;
    }
    public override int getDamage() {
        //  weaponAttackMod is reset back to 1.0f in the swing animation coroutine
        var temp = (int)(pwi.getWeapon().damage * pStats.playerWeaponDamageBuff * weaponAttackMod);
        return temp;
    }
    public override float getKnockback() {
        return pwi.getWeapon().knockback;
    }
    public override void specialEffectOnAttack(GameObject defender) {
        stopInhibitingMovement();
    }
    #endregion


    #region ---   MORTAL SHIT   ---
    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }
    public override void die() {
        //  create a new empty player obj
        var obj = new GameObject("deadPlayer");
        obj.tag = "Player";
        obj.transform.position = transform.position;

        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        Destroy(gameObject);
        Destroy(healthBar.gameObject);
    }
    IEnumerator passiveHealthRegen() {
        while(health > 0.0f) {
            if(health < maxHealth)
                health++;
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion
}
