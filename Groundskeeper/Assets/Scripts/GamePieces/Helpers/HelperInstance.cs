using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HelperInstance : Helper {
    Vector2 moveInfo;
    Vector2 targetMoveInfo;
    float accSpeed = .1f, slowSpeed = 18.0f;

    [HideInInspector] public Transform followingTransform;
    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    public bool moving = false;
    public bool hasTarget = false;
    public bool inReach = false;
    public bool tooClose = false;
    public Vector2 target { get; private set; }

    [SerializeField] Animator weaponAnim;
    protected WeaponInstance wi = null;
    protected SightCollider sc;
    protected Collider2D scc;
    protected Collider2D hircc;
    Rigidbody2D rb;
    protected GameBoard gb;
    protected HelperInReachCollider hirc;

    private void Start() {
        mortalInit();
        movementInit(FindObjectOfType<SetupSequenceManager>(), FindObjectOfType<LayerSorter>(), true);
        helperInit();
        hi = FindObjectOfType<HouseInstance>();
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Environment"));
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("DeadGuy"));
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("House"));
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Helper"));  //  sucks but needs
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Structure"));
        FindObjectOfType<UnitMovementUpdater>().addHelper(this);
        //FindObjectOfType<HelperAttackManager>().addHelper(this);
        spriteOriginal = spriteObj.transform.localScale;
        shadowOriginal = shadowObj.transform.localScale;
        targetMoveInfo = transform.position;
        if(GetComponentInChildren<WeaponInstance>() != null)
            wi = GetComponentInChildren<WeaponInstance>();
        hasTarget = false;
        inReach = false;
        followingTransform = null;
        rb = GetComponentInParent<Rigidbody2D>();
        sc = GetComponentInChildren<SightCollider>();
        gb = FindObjectOfType<GameBoard>();
        hirc = GetComponentInChildren<HelperInReachCollider>();
        scc = sc.GetComponent<Collider2D>();
        hircc = hirc.GetComponent<Collider2D>();
    }

    public abstract void inReachEnterAction(GameObject other);
    public abstract void inReachExitAction(GameObject other);

    #region ---   MOVEMENT SHIT   ---
    public void updateMovement() {
        //  doesn't need to move anywhere specific
        if(!hasTarget || followingTransform == null || Vector2.Distance(transform.position, target) < .01f) {
            target = startingPos;
            hasTarget = false;
            followingTransform = null;
            inReach = false;
            tooClose = false;
            //  already at starting pos
            if(Vector2.Distance(transform.position, startingPos) < .01f) {
                target = Vector2.zero;
                updateSprite(new Vector3(0.0f, -1.0f), false);
                //if(moving)
                    //sc.resetCollider(this); //  if just got to starting pos, reset collider
                moving = false;
                return;
            }
        }

        //  check if it's too far away from home
        if(Vector2.Distance(transform.position, startingPos) > 25) {
            target = startingPos;
            hasTarget = false;
            followingTransform = null;
            inReach = false;
            tooClose = false;
        }

        moving = true;
        if(followingTransform != null) {
            target = followingTransform.position;
        }
        //  corrects if needs correcting
        if(Vector2.Distance(transform.position, startingPos) > .001f) {
            //  checks if it's position is already on a house point
            if(!hi.alreadyGoingTowardsPathPoint(transform.position))
                target = hi.getNextPointOnPath(transform.position, target);
        }

        //  moves if isn't close enough to do anything
        if(!inReach && !tooClose)
            moveToPos(target, rb, speed);
        //  if too close, move away
        else if(tooClose)
            moveToPos(target, rb, -speed);

        moveInfo = (hasTarget && !inReach) ? Vector2.MoveTowards(moveInfo, targetMoveInfo, accSpeed * 100.0f * Time.fixedDeltaTime) : Vector2.MoveTowards(moveInfo, startingPos, slowSpeed * 100.0f * Time.fixedDeltaTime);
    }
    public override bool restartWalkAnim() {
        return hasTarget && !inReach && !moving;
    }
    public override void updateSprite(Vector2 movingDir, bool opposite) {
        if(queuedUpdatedSpriteChange != null)
            StopCoroutine(queuedUpdatedSpriteChange);
        //  not moving, face forward
        if(movingDir == Vector2.zero) {
            queuedUpdatedSpriteChange = StartCoroutine(queuedSpriteChange(forwardSprite));
            return;
        }

        //  moving more along the y axis, set to a y axis sprite
        else if(Mathf.Abs(movingDir.y) > Mathf.Abs(movingDir.x)) {
            if(movingDir.y > 0.0f) 
                queuedUpdatedSpriteChange = StartCoroutine(queuedSpriteChange(!opposite ? backSprite : forwardSprite));
            else
                queuedUpdatedSpriteChange = StartCoroutine(queuedSpriteChange(!opposite ? forwardSprite : backSprite));
        }
        //  set to a x axis sprite
        else {
            if(movingDir.x > 0.0f)
                queuedUpdatedSpriteChange = StartCoroutine(queuedSpriteChange(!opposite ? rightSprite : leftSprite));
            else if(movingDir.x < 0.0f)
                queuedUpdatedSpriteChange = StartCoroutine(queuedSpriteChange(!opposite ? leftSprite : rightSprite));
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
        if(wi == null)
            return 0f;
        return wi.reference.cooldown;
    }
    public override int getDamage() {
        if(wi == null)
            return 0;
        int baseDmg = wi.reference.damage / 2;
        float boostyWoosty = hStats.helperWeaponDamageBuff;
        return (int)(baseDmg * boostyWoosty);
    }
    public override float getKnockback() {
        if(wi == null)
            return 0f;
        return wi.reference.knockback;
    }
    public override void specialEffectOnAttack(GameObject defender) {
    }
    public override Weapon.weaponTitle getWeapon() {
        return wi != null && wi.reference != null ? wi.reference.title : Weapon.weaponTitle.None;
    }
    #endregion


    #region ---   MORTAL SHIT   ---
    public override Color getStartingColor() {
        return Color.white;
    }
    public override void die() {
        isDead = true;
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
    #endregion
}
