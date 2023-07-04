using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public abstract class MonsterInstance : Monster {
    Vector2 moveTarget;

    [SerializeField] float attackCoolDown = .5f, attackKnockBack = 0f;


    [SerializeField] public Transform followingTransform = null;
    [SerializeField] public bool hasTarget = false, hasAttractiveTarget = false;
    [HideInInspector] public bool hasCultistBuff = false;

    //  cannot be confused if is leader
    public bool confused = false;
    [HideInInspector] public bool leader = false;
    [HideInInspector] public MonsterInstance closestLeader = null;

    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    [HideInInspector] public float affectedMoveAmt = 0f;
    Color normColor;

    [HideInInspector] public int relevantWave;
    [HideInInspector] public MonsterSpawner.direction direction;
    [SerializeField] GameObject sCol;

    [SerializeField] public List<GameInfo.GamePiece> sightEffectedPieces = new List<GameInfo.GamePiece>();

    [HideInInspector] public SlashEffect slash { get; private set; }

    //  storage
    Transform pt;
    Vector2 houseCenter;
    Rigidbody2D rb;
    GameBoard gb;
    WaveWarnerRose wwr;
    MonsterSpawner ms;
    Collider2D c;
    SoulParticlePooler spp;


    private void OnCollisionStay2D(Collision2D col) {
        if(canAttack) {
            //  normal attack
            if(col.gameObject.tag == "House")
                attack(col.gameObject, true, 0.0f);
            else if(col.gameObject.tag == "Player" || col.gameObject.tag == "Helper" || col.gameObject.tag == "Structure") {
                //  if can attack all targets, skip testing and just attack
                if(favoriteTarget == targetType.All)
                    attack(col.gameObject, true, 0.0f);
                //  check if the target is attackable by this monster
                else if((col.gameObject.tag == "Helper" || col.gameObject.tag == "Player") && favoriteTarget == targetType.People)
                    attack(col.gameObject, true, 0.0f);
                else if(col.gameObject.tag == "Structure" && favoriteTarget == targetType.Structures)
                    attack(col.gameObject, true, 0.0f);
            }


            //  if the unit is confused, have it attack other monsters 
            if(confused && col.gameObject.tag == "Monster") {
                attack(col.gameObject, true, 0.0f);
            }
        }
    }


    public void setup() {
        mortalInit();
        movementInit(null, FindObjectOfType<LayerSorter>(), !flying);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Environment"));
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("DeadGuy"));
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("PlayerBoundsCollider"));
        //stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
        //FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        if(FindObjectOfType<PlayerInstance>() != null)
            pt = FindObjectOfType<PlayerInstance>().transform;
        houseCenter = FindObjectOfType<HouseInstance>().getCenter();
        rb = GetComponent<Rigidbody2D>();
        sr = spriteObj.GetComponent<SpriteRenderer>();
        gb = FindObjectOfType<GameBoard>();
        wwr = FindObjectOfType<WaveWarnerRose>();
        ms = FindObjectOfType<MonsterSpawner>();
        c = GetComponent<Collider2D>();
        slash = GetComponentInChildren<SlashEffect>();
        spp = FindObjectOfType<SoulParticlePooler>();

        //  roll chance to become a boss monster
        bool boss = Random.Range(0, 10) == 0;
        float bossSizeMod = 1.5f;

        if(boss && false) {
            soulsGiven *= 2f;
            maxHealth += (int)(maxHealth * 1.5f);
            health += (int)(maxHealth * 1.5f);
            attackDamage *= 2;
        }
        else
            soulsGiven = originalSoulsGiven;

        //  randomize the look of the monster
        float sizeDiff = Random.Range(1.0f - .2f, 1.0f + .2f), minColor = .4f, maxColor = .9f;
        transform.localScale = new Vector3(!boss ? sizeDiff : sizeDiff * bossSizeMod, !boss ? sizeDiff : sizeDiff * bossSizeMod);
        normColor = new Color(Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), sr.color.a);
        sr.color = normColor;

        spriteOriginal = spriteObj.transform.localScale;
        if(shadowObj != null)
            shadowOriginal = !boss ? shadowObj.transform.localScale : shadowObj.transform.localScale * bossSizeMod;

        if(!leader)
            sCol.SetActive(false);
        if(favoriteTarget == targetType.People)
            followingTransform = pt.transform;
        else
            moveTarget = houseCenter;
        FindObjectOfType<UnitMovementUpdater>().addMonster(this);
    }

    public void setAsLeader() {
        sCol.SetActive(true);
        leader = true;
    }
    public void setConfused(bool b) {
        confused = b;
        StartCoroutine(unconfuseSelf());
    }

    public abstract void sightEnterEffect(GameObject other);
    public abstract void sightExitEffect(GameObject other);

    IEnumerator unconfuseSelf() {
        if(leader)
            sCol.GetComponent<SightCollider>().shrinkArea();
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        followingTransform = null;
        hasTarget = false;
        if(leader)
            sCol.GetComponent<SightCollider>().expandArea();
        else if(closestLeader != null) {
            moveTarget = closestLeader.moveTarget;
            hasTarget = true;
        }

        confused = false;
    }

    #region ---   MOVEMENT SHIT   ---
    //  targets get set in the child sight collider
    public void updateMovement() {
        //  checks if confused
        if(confused) {
            //  checks if it's the last monster
            if(gb.monsters.Count <= 1) {
                confused = false;
                followingTransform = closestLeader != null ? closestLeader.transform : null;
                hasTarget = followingTransform != null;
            }
            else {
                gb.monsters.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
                followingTransform = gb.monsters.FindClosest(transform.position).transform;
                gb.monsters.Add(this);
                moveToPos(followingTransform.position, rb, Mathf.Clamp(speed - affectedMoveAmt, .075f, Mathf.Infinity));
                return;

            }
        }

        //  if not confused, set the movetarget to be something normal in case the monster has any followers
        //  following person
        if(!leader && closestLeader != null) {
            //  move towards the same person / structure as the leader
            if(closestLeader.hasTarget && closestLeader.followingTransform != null)
                moveTarget = closestLeader.followingTransform.position;
            //  move towards the same position as the leader with the same offset as current
            else
                moveTarget = closestLeader.moveTarget;
        }
        else {
            if(hasTarget && followingTransform != null)
                moveTarget = followingTransform.position;

            //  if doesn't have a target, give it a generic target to follow
            else {
                if(pt != null)
                    moveTarget = favoriteTarget == targetType.People ? (Vector2)pt.position : houseCenter;
                else
                    moveTarget = houseCenter;
            }
        }
        moveToPos(moveTarget, rb, Mathf.Clamp(speed - affectedMoveAmt, .075f, Mathf.Infinity));
    }
    public override bool restartWalkAnim() {
        return canMove;
    }
    public override WalkAnimInfo getWalkInfo() {
        return flying ? new WalkAnimInfo(.25f, .35f, 0f, 0f) : new WalkAnimInfo(.25f, .35f, 15f, 5f);
    }
    public override void updateSprite(Vector2 movingDir, bool opposite) {
        Vector2 offset = moveTarget - (Vector2)transform.position;
        if(offset == Vector2.zero) {
            sr.sprite = forwardSprite;
            return;
        }
        //  should never have opposite sprite so im not gonna add it
        else if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y)) {
            if(offset.x > 0.0f)
                sr.sprite = !opposite ? rightSprite : leftSprite;
            else
                sr.sprite = !opposite ? leftSprite : rightSprite;
        }
        else {
            if(offset.y > 0.0f)
                sr.sprite = !opposite ? backSprite : forwardSprite;
            else
                sr.sprite = !opposite ? forwardSprite : backSprite;
        }
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
        return (int)(attackDamage * (hasCultistBuff ? 1.25f : 1f));
    }
    public override float getKnockback() {
        return attackKnockBack;
    }
    public override void specialEffectOnAttack(GameObject defender) {
        if(title == monsterTitle.Vampire) {
            var rand = Random.Range(0, 10);
            if(rand == 0)
                health = Mathf.Clamp(health + (attackDamage), 0, maxHealth);
        }
        if(defender.GetComponent<PlayerInstance>() != null)
            defender.GetComponent<Movement>().inhibitMovementCauseBeingAttacked();
    }
    public override Weapon.weaponTitle getWeapon() {
        return Weapon.weaponTitle.None;
    }
    #endregion


    #region ---   MORTAL SHIT   ---
    public override Color getStartingColor() {
        return normColor;
    }
    public override void die() {
        //  logic
        isDead = true;
        //  souls 
        guc.incSouls(soulsGiven);
        GameInfo.addSouls(soulsGiven, guc.ended);

        //  falir
        spp.showParticle(transform.position, soulsGiven);

        unshownDie();
    }
    public void unshownDie() {
        isDead = true;
        c.enabled = false;
        //  removes the monster from the game board monsters
        gb.monsters.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());

        //  passes on leader ship if is leader and there are still monsters from this wave
        if(leader && ms.stillHasMonstersFromWave(relevantWave))
            ms.passOnLeadership(this, relevantWave);
        //  either not a leader, or there are no more monsters from this wave. Either way, remove it from the monsterSpawner list
        else
            ms.removeMonsterFromGroup(this, relevantWave);
        //  start a new wave if this was the last monster of the wave
        if(!ms.stillHasMonstersFromWave(relevantWave) || gb.monsters.Count == 0) {
            if(relevantWave == GameInfo.wavesPerNight())    //  this is the last monster of the night
                ms.endGame();
            else if(relevantWave == GameInfo.wave)              //  this monster is the last monster of the current wave
                ms.startNewWave();  //  this updates the rose also
            else                                                //  this monster isn't important, so update the rose
                wwr.updateForDirection(direction);
        }

        //  boring stuff
        GameInfo.monstersKilled++;

        //  fliar
        transform.DOScale(0f, .25f);

        //  cleanup
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject, .26f);
        enabled = false;
    }
    #endregion
}
