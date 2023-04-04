using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public abstract class MonsterInstance : Monster {
    Vector2 moveTarget;

    [SerializeField] float attackCoolDown = .5f, attackKnockBack = 0f;


    [HideInInspector][SerializeField] public Transform followingTransform = null;
    [HideInInspector] public bool hasTarget = false;
    [HideInInspector] public bool hasCultistBuff = false;

    //  cannot be confused if is leader
    public bool confused { get; private set; } = false;
    [HideInInspector] public bool leader = false;
    [HideInInspector] public MonsterInstance closestLeader = null;

    Vector2 spriteOriginal, shadowOriginal;   //  for showing and hiding

    [HideInInspector] public float affectedMoveAmt = 0f;
    [SerializeField] GameObject bloodParticles, soulParticles;
    Color normColor;

    [HideInInspector] public int relevantWave;
    [HideInInspector] public MonsterSpawner.direction direction;
    [SerializeField] GameObject sCol;

    [SerializeField] public List<GameInfo.GamePiece> sightEffectedPieces = new List<GameInfo.GamePiece>();

    //  storage
    Transform pt;
    Vector2 houseCenter;
    Rigidbody2D rb;
    GameBoard gb;
    WaveWarnerRose wwr;
    MonsterSpawner ms;
    Collider2D c;


    private void OnCollisionStay2D(Collision2D col) {
        if(canAttack) {
            //  if the unit is confused, have it attack other monsters 
            if(confused && !leader && col.gameObject.tag == "Monster") 
                attack(col.gameObject, true);

            //  normal attack
            else if(col.gameObject.tag == "Player" || col.gameObject.tag == "Helper" || col.gameObject.tag == "Structure" || col.gameObject.tag == "House") {
                //  if can attack all targets, skip testing and just attack
                if(favoriteTarget == targetType.All)
                    attack(col.gameObject, true);
                //  check if the target is attackable by this monster
                else if(col.gameObject.tag == "House" && (favoriteTarget == targetType.Structures || favoriteTarget == targetType.House))
                    attack(col.gameObject, true);
                else if(col.gameObject.tag == "Helper" && favoriteTarget == targetType.People)
                    attack(col.gameObject, true);
                else if(col.gameObject.tag == "Structure" && favoriteTarget == targetType.Structures)
                    attack(col.gameObject, true);
            }
        }
    }


    public void setup() {
        mortalInit();
        movementInit(null, FindObjectOfType<LayerSorter>());
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Environment"));
        //stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
        //FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);
        pt = FindObjectOfType<PlayerInstance>().transform;
        houseCenter = FindObjectOfType<HouseInstance>().getCenter();
        rb = GetComponent<Rigidbody2D>();
        sr = spriteObj.GetComponent<SpriteRenderer>();
        gb = FindObjectOfType<GameBoard>();
        wwr = FindObjectOfType<WaveWarnerRose>();
        ms = FindObjectOfType<MonsterSpawner>();
        c = GetComponent<Collider2D>();

        //  randomize the look of the monster
        float sizeDiff = Random.Range(1.0f - .2f, 1.0f + .2f), minColor = .4f, maxColor = .9f;
        transform.localScale = new Vector3(sizeDiff, sizeDiff);
        normColor = new Color(Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), sr.color.a);
        sr.color = normColor;

        spriteOriginal = spriteObj.transform.localScale;
        if(shadowObj != null)
            shadowOriginal = shadowObj.transform.localScale;

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
        confused = false;
        leader = true;
    }
    public void setConfused(bool b) {
        confused = !leader && b;
        StartCoroutine(unconfuseSelf());
    }

    public abstract void sightEnterEffect(GameObject other);
    public abstract void sightExitEffect(GameObject other);

    IEnumerator unconfuseSelf() {
        yield return new WaitForSeconds(10f);
        confused = false;
    }

    #region ---   MOVEMENT SHIT   ---
    //  targets get set in the child sight collider
    public void updateMovement() {
        //  following person
        if(!leader && closestLeader != null) {
            //  move towards the same person / structure as the leader
            if(closestLeader.hasTarget)
                moveTarget = closestLeader.followingTransform.position;
            //  move towards the same position as the leader with the same offset as current
            else {
                moveTarget = closestLeader.moveTarget;
            }
        }
        else {
            if(hasTarget)
                moveTarget = followingTransform.position;

            //  if doesn't have a target, give it a generic target to follow
            else {
                moveTarget = favoriteTarget == targetType.People ? (Vector2)pt.position : houseCenter;
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
        else if(Mathf.Abs(offset.x) > Mathf.Abs(offset.y))
            sr.sprite = offset.x > 0.0f ? rightSprite : leftSprite;
        else
            sr.sprite = offset.y > 0.0f ? backSprite : forwardSprite;
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
        return (int)(attackDamage * (hasCultistBuff ? 1.5f : 1f));
    }
    public override float getKnockback() {
        return attackKnockBack;
    }
    public override void specialEffectOnAttack(GameObject defender) {
        if(title == monsterTitle.Vampire) {
            health = Mathf.Clamp(health + attackDamage, 0, maxHealth);
        }
        if(defender.GetComponent<PlayerInstance>() != null)
            defender.GetComponent<Movement>().inhibitMovementCauseBeingAttacked();
    }
    #endregion


    #region ---   MORTAL SHIT   ---
    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return normColor;
    }
    public override void die() {
        //  souls 
        guc.incSouls(soulsGiven);
        GameInfo.addSouls(soulsGiven, guc.ended);

        //  falir
        var s = Instantiate(soulParticles.gameObject, transform.position, Quaternion.identity, null).GetComponent<ParticleSystem>();
        s.emission.SetBurst(0, new ParticleSystem.Burst(.5f, soulsGiven * 4.0f));
        s.Play();

        unshownDie();
    }
    public void unshownDie() {
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
        if(!ms.stillHasMonstersFromWave(relevantWave)) {
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
