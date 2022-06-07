using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerInstance : Attacker {
    [SerializeField] float walkSpeed = 8.0f, runSpeed = 18.0f, accSpeed = .1f, slowSpeed = 18.0f;
    InputMaster controls;

    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    Rigidbody2D rb;

    private void OnCollisionStay2D(Collision2D col) {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(spriteObj);
    }

    #region ---   MOVEMENT SHIT   ---
    private void Awake() {
        controls = new InputMaster();
        rb = GetComponent<Rigidbody2D>();
        DOTween.Init();

        controls.Player.Move.performed += ctx => movementChange(ctx.ReadValue<Vector2>());
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
        moveWithDir(moveInfo, rb, controls.Player.Sprint.IsPressed() ? runSpeed : walkSpeed);

        //  if player is not moving, decrement the movementInfo
        //  if player is moving, increase the movementInfo to target value
        moveInfo = controls.Player.Move.inProgress ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, Vector2.zero, slowSpeed * 100.0f * Time.fixedDeltaTime);
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
            spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
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
    #endregion


    #region ---   ATTACKER SHIT   ---
    public override float getAttackCoolDown() {
        return GetComponentInChildren<PlayerWeaponInstance>().reference.cooldown;
    }
    public override int getDamage() {
        return GetComponentInChildren<PlayerWeaponInstance>().reference.damage;
    }
    public override float getKnockback() {
        return GetComponentInChildren<PlayerWeaponInstance>().reference.knockback;
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
    }
    #endregion
}
