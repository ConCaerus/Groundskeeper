using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class WeaponInstance : MonoBehaviour {
    public enum attackTarget {
        None, People, Monsters
    }

    [SerializeField] public Vector2 offsetFromUser;
    float speed = 20f;
    [SerializeField] public GameObject rotObj;
    public GameObject user;
    [SerializeField] public attackTarget target;

    public TrailRenderer trail;
    Coroutine anim = null;

    [HideInInspector] public Transform pt;
    protected PlayerInstance pi;
    protected LayerSorter ls;
    protected SpriteRenderer sr;
    protected Attacker a;
    Collider2D c;
    EnvironmentManager em;
    protected CameraMovement cm;
    AudioManager am;
    [SerializeField] public Animator wAnimator;
    [SerializeField] public GameObject gunFireParticlesPos;
    [SerializeField] public FunkyCode.Light2D gunLight;
    [SerializeField] GameObject lobObject;
    [SerializeField] AudioClip attackSound;

    MouseManager mm;
    GameGamepadCursor ggc;
    FreeGamepadCursor fgc;

    bool isPlayerWeapon = false;


    public Weapon reference { get; protected set; }

    public bool canMove = true;
    [HideInInspector] public bool used = true;

    private void OnTriggerEnter2D(Collider2D col) {
        if(reference == null || reference.aType == Weapon.attackType.Shoot)
            return;
        if(target == attackTarget.Monsters && col.gameObject.tag == "Monster") {
            a.attack(col.gameObject, false);
            if(pi != null) {
                cm.shake(pi.getDamage());
            }
            col.gameObject.GetComponentInChildren<SlashEffect>().slash(user.transform.position, rotObj.transform.GetChild(0).localRotation.x != 0f);
        }
        else if(target == attackTarget.People && col.gameObject.tag == "Helper") {
            a.attack(col.gameObject, false);
        }
        else if(col.gameObject.tag == "Environment") {
            em.hitEnvironment(col.ClosestPoint(transform.position));
        }
    }

    void Start() {
        pi = FindObjectOfType<PlayerInstance>();
        ls = FindObjectOfType<LayerSorter>();
        sr = GetComponent<SpriteRenderer>();
        pt = FindObjectOfType<PlayerInstance>().transform;
        if(user != null)
            a = user.GetComponent<Attacker>();
        c = GetComponent<Collider2D>();
        em = FindObjectOfType<EnvironmentManager>();
        cm = FindObjectOfType<CameraMovement>();
        am = FindObjectOfType<AudioManager>();
        mm = FindObjectOfType<MouseManager>();
        ggc = FindObjectOfType<GameGamepadCursor>();
        fgc = FindObjectOfType<FreeGamepadCursor>();

        //  sets reference
        if(GetComponentInParent<HelperInstance>() == null)
            isPlayerWeapon = true;

        if(c != null)
            c.enabled = false;
        trail.emitting = false;
        transform.localPosition = offsetFromUser;
    }

    public void updateReference(Weapon.weaponTitle title) {
        updateReference(FindObjectOfType<PresetLibrary>().getWeapon(title));
    }
    public void updateReference(Weapon we) {
        reference = we;
        sr.sprite = reference.sprite;
        trail.gameObject.transform.localPosition = reference.trailPos;
    }

    private void Update() {
        if(anim == null && used)
            movementLogic();
    }

    public abstract void movementLogic();   //  needed for lumberjack instances

    public void lookAtMouse() {
        if(canMove) {
            Vector3 orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
            var mousePos = Vector2.zero;
            if(mm.usingKeyboard()) {
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                orbVector = Input.mousePosition - orbVector;
            }
            else if(!fgc.isActiveAndEnabled) {
                mousePos = ggc.getMousePosInWorld();
                orbVector = (Vector3)ggc.getMousePosInScreen() - orbVector;
            }
            else {
                mousePos = fgc.getWorldCursorPos();
                orbVector = (Vector3)fgc.getScreenCursorPos() - orbVector;
            }
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;
            angle += reference.aType == Weapon.attackType.Swing ? 35 : -35;

            rotObj.transform.localPosition = Vector3.zero;
            rotObj.transform.rotation = Quaternion.Lerp(rotObj.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), speed * Time.deltaTime);

            ls.requestNewSortingLayer(transform.position.y, sr);
            rotObj.transform.GetChild(0).localRotation = mousePos.x > user.transform.position.x ? Quaternion.Euler(0.0f, 0.0f, 0.0f) :
                reference.aType == Weapon.attackType.Swing ? Quaternion.Euler(180.0f, 0.0f, 90.0f - 35f / 2.0f) : Quaternion.Euler(180.0f, 0.0f, -90.0f + 35f / 2.0f);
        }
    }
    public void lookAtPos(Vector2 pos) {
        if(canMove) {
            var orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
            var screenPos = Camera.main.WorldToScreenPoint(pos);
            orbVector = screenPos - orbVector;
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

            rotObj.transform.localPosition = Vector3.zero;
            rotObj.transform.rotation = Quaternion.Lerp(rotObj.transform.rotation, Quaternion.AngleAxis(angle + 45, Vector3.forward), speed * Time.deltaTime);

            ls.requestNewSortingLayer(transform.position.y, sr);
            rotObj.transform.GetChild(0).localRotation = pos.x > user.transform.position.x ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(180.0f, 0.0f, 90.0f);
        }
    }


    public void setEqualTo(WeaponInstance other) {
        offsetFromUser = other.offsetFromUser;
        rotObj = other.rotObj;
        user = other.user;
        target = other.target;
        trail = other.trail;
        pt = FindObjectOfType<PlayerInstance>().transform;
        ls = FindObjectOfType<LayerSorter>();
        sr = GetComponent<SpriteRenderer>();
        pi = FindObjectOfType<PlayerInstance>();
        transform.localPosition = offsetFromUser;
        a = user.GetComponent<Attacker>();
        c = GetComponent<Collider2D>();
        em = FindObjectOfType<EnvironmentManager>();
        cm = FindObjectOfType<CameraMovement>();
        am = FindObjectOfType<AudioManager>();
        gunFireParticlesPos = other.gunFireParticlesPos;
        wAnimator = other.wAnimator;
        gunLight = other.gunLight;
    }


    //  set attackingPos to Vector2.zero to not use it
    //  set targetPos to Vector2.zero to not use it
    public void attack(Vector2 attackingPos, Vector2 targetPos, PlayerInstance pi = null, float mod = 0.0f) {
        if(anim != null || !used || !a.getCanAttack())
            return;
        if(reference.swingSound != null)
            am.playSound(reference.swingSound, transform.position);

        if(reference.aType == Weapon.attackType.Swing)
            anim = StartCoroutine(swingAttackAnim(mod, attackingPos, pi));
        else if(reference.aType == Weapon.attackType.Stab)
            anim = StartCoroutine(stabAttackAnim(mod, attackingPos, pi));
        else if(reference.aType == Weapon.attackType.Shoot)
            anim = StartCoroutine(shootAttackAnim(mod, attackingPos, pi));
        else if(reference.aType == Weapon.attackType.Lob)
            StartCoroutine(lobAttackAnim(targetPos, pi));
    }


    public abstract void shootMonster();
    public abstract void lobToMonster();

    IEnumerator shootAttackAnim(float mod, Vector2 attackingPos, PlayerInstance pi) {
        canMove = false;
        trail.emitting = false;
        this.c.enabled = false;
        //float recRad = 45f;
        float c = a.getAttackCoolDown();
        //float recPerc = .15f;
        if(attackingPos != Vector2.zero)
            user.GetComponent<Movement>().lookAtPos(attackingPos);
        wAnimator.SetTrigger("recoil");
        a.startCooldown();

        //  fire gun
        //  shoots monster
        if(attackSound != null)
            am.playSound(attackSound, transform.position);
        shootMonster();

        //  push the player back from recoil
        if(isPlayerWeapon) {
            float lungeMod = 1.5f * mod;
            var origin = (Vector2)pt.position;
            var target = mm.usingKeyboard() ? GameInfo.mousePos() : ggc.getMousePosInWorld();
            var px = target.x - origin.x;
            var py = target.y - origin.y;
            var theta = Mathf.Atan2(py, px);
            var t = new Vector2(lungeMod * Mathf.Cos(theta), lungeMod * Mathf.Sin(theta));
            pt.DOMove(origin - t, .15f);
        }

        yield return new WaitForSeconds(.25f);

        //  start moving again
        canMove = true;
        anim = null;

        /*
        //  flair

        //  recoil
        transform.parent.DOKill();
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, recRad), c * recPerc, RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.1f, c * recPerc);

        */
        /*
        yield return new WaitForSeconds(c * recPerc);

        //  attack is done

        //  return to norm rot / scale
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, -recRad), c * (1f - recPerc), RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.0f, c * (1f - recPerc));
        yield return new WaitForSeconds(c * (1f - recPerc));
        */
    }

    IEnumerator swingAttackAnim(float mod, Vector2 attackingPos, PlayerInstance pi) {
        canMove = false;
        trail.emitting = true;
        a.startCooldown();
        if(attackingPos != Vector2.zero)
            user.GetComponent<Movement>().lookAtPos(attackingPos);
        wAnimator.SetTrigger(isPlayerWeapon ? "swing" : "instantSwing");

        if(attackSound != null)
            am.playSound(attackSound, transform.position);

        float ableToHitMonsterTime = .25f;

        //  lunge the player towards the fucker
        if(isPlayerWeapon) {
            float lungeMod = 1.5f * mod;
            var origin = (Vector2)pt.position;
            var target = mm.usingKeyboard() ? GameInfo.mousePos() : ggc.getMousePosInWorld();
            var px = target.x - origin.x;
            var py = target.y - origin.y;
            var theta = Mathf.Atan2(py, px);
            var t = new Vector2(lungeMod * Mathf.Cos(theta), lungeMod * Mathf.Sin(theta));
            pt.DOMove(t + origin, .15f);
        }

        c.enabled = true;
        yield return new WaitForSeconds(ableToHitMonsterTime);
        c.enabled = false;

        if(isPlayerWeapon && pi != null)
            pi.weaponAttackMod = 1.0f;
        //  start moving again
        c.enabled = false;
        trail.emitting = false;
        canMove = true;
        anim = null;


        /*
        float windTime = .05f, swingTime = .15f;
        float swingRadius = 180f;

        //  wind back
        transform.parent.DOScale(1.5f, windTime + swingTime * .5f);
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, swingRadius / 2.0f), windTime, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(windTime);
        c.enabled = true;
        trail.emitting = true;

        //  swing
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, -swingRadius), swingTime, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(swingTime);

        //  attack is done

        //  return to norm rot / scale
        Vector3 orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
        orbVector = Input.mousePosition - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

        transform.parent.DOLocalRotate(Quaternion.ToEulerAngles(Quaternion.AngleAxis(angle + 45, Vector3.forward)), swingTime, RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.0f, swingTime);
        yield return new WaitForSeconds(swingTime);
        */
    }

    IEnumerator stabAttackAnim(float mod, Vector2 attackingPos, PlayerInstance pi) {
        canMove = false;
        trail.emitting = true;
        a.startCooldown();
        if(attackingPos != Vector2.zero)
            user.GetComponent<Movement>().lookAtPos(attackingPos);
        wAnimator.SetTrigger("stabWindup");

        if(attackSound != null)
            am.playSound(attackSound, transform.position);

        float ableToHitMonsterTime = .25f;

        //  lunge the player towards the fucker
        if(isPlayerWeapon) {
            float lungeMod = 1.5f * mod;
            var origin = (Vector2)pt.position;
            var target = mm.usingKeyboard() ? GameInfo.mousePos() : ggc.getMousePosInWorld();
            var px = target.x - origin.x;
            var py = target.y - origin.y;
            var theta = Mathf.Atan2(py, px);
            var t = new Vector2(lungeMod * Mathf.Cos(theta), lungeMod * Mathf.Sin(theta));
            pt.DOMove(t + origin, .15f);
        }

        c.enabled = true;
        yield return new WaitForSeconds(ableToHitMonsterTime);
        c.enabled = false;

        //  start moving again
        c.enabled = false;
        trail.emitting = false;
        canMove = true;
        anim = null;
    }

    IEnumerator lobAttackAnim(Vector2 targetPos, PlayerInstance pi) {
        float duration = .75f;
        a.startCooldown();
        user.GetComponent<Movement>().lookAtPos(targetPos);

        var obj = Instantiate(lobObject.gameObject);
        obj.transform.position = transform.position;

        obj.transform.DOMove(targetPos, duration);
        obj.GetComponent<LobbedInstance>().lob(duration);

        if(attackSound != null)
            am.playSound(attackSound, transform.position);

        //  starts the lobbed anim
        obj.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360.0f * 3f), duration, RotateMode.FastBeyond360);

        //  starts half animation
        obj.transform.DOScale(new Vector3(1.5f, 1.5f), duration / 2.0f);
        yield return new WaitForSeconds(duration / 2.0f);
        obj.transform.DOScale(new Vector3(1.0f, 1.0f), duration / 2.0f);
    }
}
