using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackInstance : Helper {
    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    float accSpeed = .1f, slowSpeed = 18.0f;

    [HideInInspector] public Transform followingTransform;
    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    [HideInInspector] public bool hasTarget = false;
    [HideInInspector] public bool inReach = false;
    public Vector2 target { get; private set; }
    [HideInInspector] public Vector2 startingPos;

    [SerializeField] GameObject bloodParticles;

    private void Start() {
        mortalInit();
        startingPos = transform.position;
        hi = FindObjectOfType<HouseInstance>();
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Environment"));
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponents<Collider2D>()[0].isTrigger ? GetComponents<Collider2D>()[1] : GetComponents<Collider2D>()[0], spriteObj.GetComponent<SpriteRenderer>());
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        FindObjectOfType<UnitMovementUpdater>().addHelper(this);
        FindObjectOfType<HelperAttackManager>().addHelper(this);
        spriteOriginal = spriteObj.transform.localScale;
        shadowOriginal = shadowObj.transform.localScale;
        targetMoveInfo = transform.position;

        //  apply health buff
        maxHealth = (int)(maxHealth * GameInfo.getHelperHealthBuff());
        health = maxHealth;
    }

    #region ---   MOVEMENT SHIT   ---
    public void updateMovement() {
        //  doesn't need to move anywhere specific
        if(!hasTarget || followingTransform == null) {
            target = startingPos;
            hasTarget = false;
            followingTransform = null;
            inReach = false;
            //  already at starting pos
            if(Vector2.Distance(transform.position, startingPos) < .001f)
                return;
        }
        if(followingTransform != null) {
            target = followingTransform.position;
        }
        //  corrects if needs correcting
        if(Vector2.Distance(transform.position, startingPos) > .001f)
            target = hi.getNextPointOnPath(transform.position, target);
        if(!inReach)
            moveToPos(target, GetComponentInParent<Rigidbody2D>(), speed);
        moveInfo = (hasTarget && !inReach) ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, startingPos, slowSpeed * 100.0f * Time.fixedDeltaTime);
    }
    public override bool restartWalkAnim() {
        return hasTarget && !inReach;
    }
    public override void updateSprite(Vector2 movingDir) {
        //  not moving, face forward
        if(movingDir == Vector2.zero) {
            spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
            return;
        }

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
        return spriteOriginal;
    }
    public override Vector2 getShadowOriginalScale() {
        return shadowOriginal;
    }
    #endregion


    #region ---   ATTACKER SHIT   ---
    public override float getAttackCoolDown() {
        return GetComponentInChildren<LumberjackWeaponInstance>().reference.cooldown;
    }
    public override int getDamage() {
        int baseDmg = GetComponentInChildren<LumberjackWeaponInstance>().reference.damage;
        float boostyWoosty = GameInfo.getHelperDamageBuff();
        return (int)(baseDmg * boostyWoosty);
    }
    public override float getKnockback() {
        return GetComponentInChildren<LumberjackWeaponInstance>().reference.knockback;
    }
    public override void specialEffectOnAttack(GameObject defender) {
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
        FindObjectOfType<GameBoard>().aHelpers.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
    #endregion
}
