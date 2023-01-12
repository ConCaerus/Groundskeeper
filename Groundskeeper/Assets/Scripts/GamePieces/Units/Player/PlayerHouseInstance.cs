using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerHouseInstance : Movement {
    [SerializeField] float walkSpeed = 8.0f, accSpeed = .1f, slowSpeed = 18.0f;
    InputMaster controls;

    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    Rigidbody2D rb;

    [SerializeField] GameObject bloodParticles;

    #region ---   MOVEMENT SHIT   ---
    private void Awake() {
        controls = new InputMaster();
        rb = GetComponent<Rigidbody2D>();
        DOTween.Init();

        controls.Player.Move.performed += ctx => movementChange(ctx.ReadValue<Vector2>());
    }

    private void FixedUpdate() {
        //  move the object
        moveWithDir(moveInfo, rb, walkSpeed);

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
        return new WalkAnimInfo(.2f, .7f, 25f, 5f);
    }


    public override Vector2 getSpriteOriginalScale() {
        return Vector2.zero;
    }
    public override Vector2 getShadowOriginalScale() {
        return Vector2.zero;
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
        //  the player shouldn't die inside the house
    }
    #endregion
}
