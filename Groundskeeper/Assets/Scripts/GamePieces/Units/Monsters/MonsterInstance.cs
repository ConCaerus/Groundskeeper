using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MonsterInstance : Monster {
    Vector2 moveTarget;

    [SerializeField] float attackCoolDown = .5f, attackKnockBack = 0f;


    [HideInInspector][SerializeField] public Transform followingTransform = null;

    public bool infatuated { get; set; } = false;   //  monster is close to the house and will not attack anything else besides the house
    bool leader = false;
    [HideInInspector] public MonsterInstance closestLeader = null;

    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    [HideInInspector] public float affectedMoveAmt = 0f;


    private void OnCollisionStay2D(Collision2D col) {
        if(canAttack) {
            if(col.gameObject.tag == "Player" || col.gameObject.tag == "Helper" || col.gameObject.tag == "Building" || col.gameObject.tag == "House") {
                attack(col.gameObject, true);
                return;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(!infatuated && col.gameObject.tag == "House") {
            infatuated = true;
        }
    }

    private void Start() {
        stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        spriteOriginal = spriteObj.transform.localScale;
        shadowOriginal = shadowObj.transform.localScale;
        hide();
    }

    #region ---   MOVEMENT SHIT   ---

    public void setAsLeader() {
        FindObjectOfType<UnitMovementUpdater>().addMonster(this);
        leader = true;
    }

    public void updateMovement() {
        //  following person
        if(!leader && closestLeader != null) {
            if(closestLeader.followingTransform != null)
                moveTarget = closestLeader.followingTransform.position;
            else
                moveTarget = closestLeader.moveTarget;
        }
        else {
            if(followingTransform != null)
                moveTarget = followingTransform.position;
        }
        moveToPos(moveTarget, GetComponent<Rigidbody2D>(), speed - affectedMoveAmt);
    }
    public void updateTarget() {
        if(!leader && closestLeader != null) {
            followingTransform = closestLeader.followingTransform;
            moveTarget = closestLeader.moveTarget;
        }
        else
            moveTarget = FindObjectOfType<TargetFinder>().getTargetForMonster(gameObject);
    }
    public override bool restartWalkAnim() {
        return canMove;
    }
    public override WalkAnimInfo getWalkInfo() {
        return flying ? new WalkAnimInfo(.25f, .35f, 0f, 0f) : new WalkAnimInfo(.25f, .35f, 15f, 5f);
    }
    public override void updateSprite(Vector2 movingDir) {
        Vector2 offset = moveTarget - (Vector2)transform.position;
        if(offset == Vector2.zero) {
            spriteObj.GetComponent<SpriteRenderer>().sprite = forwardSprite;
            return;
        }
        else if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            spriteObj.GetComponent<SpriteRenderer>().sprite = offset.x > 0.0f ? rightSprite : leftSprite;
        else
            spriteObj.GetComponent<SpriteRenderer>().sprite = offset.y > 0.0f ? backSprite : forwardSprite;
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
        return attackCoolDown;
    }
    public override int getDamage() {
        return attackDamage;
    }
    public override float getKnockback() {
        return attackKnockBack;
    }
    public override void specialEffectOnAttack() {
        if(mType == monsterType.Vampire) {
            health = Mathf.Clamp(health + attackDamage, 0, maxHealth);
        }
    }
    #endregion


    #region ---   MORTAL SHIT   ---

    public override void die() {
        FindObjectOfType<GameBoard>().monsters.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
        if(leader && FindObjectOfType<GameBoard>().monsters.Count > 0)
            FindObjectOfType<MonsterSpawner>().passOnLeadership(this);
        else
            FindObjectOfType<MonsterSpawner>().removeMonsterFromGroup(this);
        GameInfo.monstersKilled++;
        FindObjectOfType<GameUICanvas>().incSouls(soulsGiven);
        transform.DOScale(0f, .25f);
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject, .26f);
        enabled = false;
    }
    #endregion
}
