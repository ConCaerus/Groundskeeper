using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerInstance : Attacker {
    [SerializeField] float walkSpeed = 8.0f, runSpeed = 18.0f, accSpeed = .1f, slowSpeed = 18.0f;
    float unitStamina = 100f;
    public float stamina {
        get {
            return unitStamina;
        }

        set {
            unitStamina = value;
            FindObjectOfType<StaminaSlider>().updateSlider(value, 100f);
        }
    }
    float stamInc = 0.0f;
    InputMaster controls;

    float initLightSize;


    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    Rigidbody2D rb;

    private void OnCollisionStay2D(Collision2D col) {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), spriteObj.GetComponent<SpriteRenderer>());
    }

    #region ---   MOVEMENT SHIT   ---
    private void Awake() {
        controls = new InputMaster();
        rb = GetComponent<Rigidbody2D>();
        DOTween.Init();

        controls.Player.Move.performed += ctx => movementChange(ctx.ReadValue<Vector2>());
        GetComponentInChildren<HealthBar>().setParent(gameObject);
    }

    private void Start() {
        initLightSize = GetComponentInChildren<FunkyCode.Light2D>().size;
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        spriteOriginal = spriteObj.transform.localScale;
        shadowOriginal = shadowObj.transform.localScale;
    }
    private void FixedUpdate() {
        //  check if game is over
        if(!GameInfo.playing) {
            controls.Disable();
            GetComponentInChildren<PlayerWeaponInstance>().enabled = false;
            enabled = false;
            return;
        }

        //  move the object
        moveWithDir(moveInfo, rb, controls.Player.Sprint.IsPressed() && stamina > 0f ? runSpeed : walkSpeed);
        stamina = Mathf.Clamp(stamina - (controls.Player.Sprint.IsPressed() && controls.Player.Move.inProgress ? 1 : -.5f - stamInc), 0f, 100f);
        stamInc += controls.Player.Sprint.IsPressed() && controls.Player.Move.inProgress ? -stamInc : .01f;

        //  if player is not moving, decrement the movementInfo
        //  if player is moving, increase the movementInfo to target value
        moveInfo = controls.Player.Move.inProgress ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, Vector2.zero, slowSpeed * 100.0f * Time.fixedDeltaTime);

        //  adjusts the player's light based on how far they are to the edge
        //      player is out of bounds, so turn off their light
        if(Vector2.Distance(transform.position, Vector2.zero) >= 75f) {
            DOTween.To(() => GetComponentInChildren<FunkyCode.Light2D>().size, x => GetComponentInChildren<FunkyCode.Light2D>().size = x, 0f, .25f);
        }
        //  player is in bounds, so tune their light based on how far they are from the edge
        else {
            var distPerc = Vector2.Distance(transform.position, Vector2.zero) / 100f;   //  a little over the edge to give the player a little light at the edge
            GetComponentInChildren<FunkyCode.Light2D>().size = initLightSize * (1 - distPerc);
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
    public override void updateSprite(Vector2 movingDir) {
        //  not moving, face forward
        if(movingDir == Vector2.zero) {
            return;
        }

        //  moving more along the y axis, set to a y axis sprite
        else if(Mathf.Abs(movingDir.x) < .5f && Mathf.Abs(movingDir.y) > .5f) {
            if(movingDir.y > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = backSprite;
            else
                spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
        }

        //  set to a x axis sprite
        else {
            if(movingDir.x > 0.0f)
                spriteObj.GetComponent<SpriteRenderer>().sprite = rightSprite;
            else
                spriteObj.GetComponent<SpriteRenderer>().sprite = leftSprite;
        }
    }
    public override WalkAnimInfo getWalkInfo() {
        return new WalkAnimInfo(.2f, .7f, 25f, 5f);
    }

    public override Vector2 getSpriteOriginalScale() {
        return spriteOriginal;
    }
    public override Vector2 getShadowOriginalScale() {
        return shadowOriginal;
    }
    #endregion


    #region ---   ATTACKER SHIT   ---
    public override float getAttackCoolDown() {
        return GetComponentInChildren<PlayerWeaponInstance>().reference.cooldown * GameInfo.getPWeaponBuff(GameInfo.getPlayerWeaponIndex()).speedBuff;
    }
    public override int getDamage() {
        return (int)(GetComponentInChildren<PlayerWeaponInstance>().reference.damage * GameInfo.getPWeaponBuff(GameInfo.getPlayerWeaponIndex()).dmgBuff);
    }
    public override float getKnockback() {
        return GetComponentInChildren<PlayerWeaponInstance>().reference.knockback;
    }
    public override void specialEffectOnAttack() {
    }
    #endregion


    #region ---   MORTAL SHIT   ---
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
    #endregion
}
