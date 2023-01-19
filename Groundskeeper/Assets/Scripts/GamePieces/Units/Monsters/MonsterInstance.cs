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
    [SerializeField] GameObject bloodParticles, soulParticles;
    Color normColor;

    [HideInInspector] public int relevantWave;
    [HideInInspector] public MonsterSpawner.direction direction;


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
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Environment"));
        stopMovingForATime(.2f);    //  so the character doesn't jump ahead at the start
        FindObjectOfType<HealthBarSpawner>().giveHealthBar(gameObject);

        //  randomize the look of the monster
        float sizeDiff = Random.Range(1.0f - .2f, 1.0f + .2f), minColor = .4f, maxColor = .9f;
        transform.localScale = new Vector3(sizeDiff, sizeDiff);
        normColor = new Color(Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), Random.Range(minColor, maxColor), spriteObj.GetComponent<SpriteRenderer>().color.a);
        spriteObj.GetComponent<SpriteRenderer>().color = normColor;

        spriteOriginal = spriteObj.transform.localScale;
        if(shadowObj != null)
            shadowOriginal = shadowObj.transform.localScale;
    }

    #region ---   MOVEMENT SHIT   ---

    public void setAsLeader() {
        FindObjectOfType<UnitMovementUpdater>().addMonster(this);
        leader = true;
    }

    public void updateMovement() {
        //  following person
        if(!leader && closestLeader != null) {
            //  move towards the same person / structure as the leader
            if(closestLeader.followingTransform != null)
                moveTarget = closestLeader.followingTransform.position;
            //  move towards the same position as the leader with the same offset as current
            else {
                moveTarget = closestLeader.moveTarget;
            }
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
    public override void specialEffectOnAttack(GameObject defender) {
        if(mType == monsterType.Vampire) {
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
        //  removes the monster from the game board monsters
        FindObjectOfType<GameBoard>().monsters.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());

        //  passes on leader ship if is leader and there are still monsters from this wave
        if(leader && FindObjectOfType<MonsterSpawner>().stillHasMonstersFromWave(relevantWave))
            FindObjectOfType<MonsterSpawner>().passOnLeadership(this, relevantWave);
        //  either not a leader, or there are no more monsters from this wave. Either way, remove it from the monsterSpawner list
        else
            FindObjectOfType<MonsterSpawner>().removeMonsterFromGroup(this, relevantWave);
        //  start a new wave if this was the last monster of the wave
        if(!FindObjectOfType<MonsterSpawner>().stillHasMonstersFromWave(relevantWave)) {
            if(relevantWave == GameInfo.wavesPerNight() - 1)    //  this is the last monster of the night
                FindObjectOfType<MonsterSpawner>().endGame();
            else if(relevantWave == GameInfo.wave)              //  this monster is the last monster of the current wave
                FindObjectOfType<MonsterSpawner>().startNewWave();  //  this updates the rose also
            else                                                //  this monster isn't important, so update the rose
                FindObjectOfType<WaveWarnerRose>().updateForDirection(direction);
        }

        //  boring stuff
        FindObjectOfType<HouseInstance>().removeUnitFromInTopUnits(this);
        GameInfo.monstersKilled++;
        FindObjectOfType<GameUICanvas>().incSouls(soulsGiven);

        //  fliar
        var s = Instantiate(soulParticles.gameObject, transform.position, Quaternion.identity, null);
        s.GetComponent<ParticleSystem>().emission.SetBurst(0, new ParticleSystem.Burst(.5f, soulsGiven * 4.0f));
        s.GetComponent<ParticleSystem>().Play();
        Destroy(s, s.GetComponent<ParticleSystem>().main.duration);
        transform.DOScale(0f, .25f);

        //  cleanup
        if(healthBar != null)
            Destroy(healthBar.gameObject);
        Destroy(gameObject, .26f);
        enabled = false;
    }
    #endregion
}