using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class WeaponInstance : MonoBehaviour {
    public enum attackTarget {
        None, People, Monsters
    }

    [SerializeField] public Vector2 offsetFromUser;
    float speed = 5f;
    [SerializeField] public GameObject rotObj;
    public GameObject user;
    [SerializeField] public attackTarget target;

    public TrailRenderer trail;
    Coroutine anim = null;

    public Transform pt;
    protected PlayerInstance pi;
    LayerSorter ls;
    SpriteRenderer sr;
    protected Attacker a;
    Collider2D c;
    EnvironmentManager em;
    protected CameraMovement cm;
    AudioManager am;


    public Weapon reference { get; private set; }

    public bool canMove = true;
    [HideInInspector] public bool used = true;

    private void OnTriggerEnter2D(Collider2D col) {
        if(reference.aType == Weapon.attackType.Shoot)
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
        GetComponent<Collider2D>().enabled = false;
        trail.emitting = false;
        transform.localPosition = offsetFromUser;
        pt = FindObjectOfType<PlayerInstance>().transform;
        a = user.GetComponent<Attacker>();
        c = GetComponent<Collider2D>();
        em = FindObjectOfType<EnvironmentManager>();
        cm = FindObjectOfType<CameraMovement>();
        am = FindObjectOfType<AudioManager>();

        //  sets reference
        if(GetComponent<PlayerAxeInstance>() != null)
            reference = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponName.Axe);
        else if(GetComponent<PlayerPistolInstance>() != null)
            reference = FindObjectOfType<PresetLibrary>().getWeapon(Weapon.weaponName.Pistol);
    }

    public void updateReference(Weapon.weaponName title) {
        updateReference(FindObjectOfType<PresetLibrary>().getWeapon(title));
    }
    public void updateReference(Weapon we) {
        reference = we;
        sr.sprite = reference.sprite;
        trail.gameObject.transform.localPosition = reference.trailPos;
    }

    private void Update() {
        if(anim == null && used)
            lookAtMouse();
    }

    public abstract void movementLogic();   //  needed for lumberjack instances

    public void lookAtMouse() {
        if(canMove) {
            Vector3 orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            orbVector = Input.mousePosition - orbVector;
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

            rotObj.transform.localPosition = Vector3.zero;
            rotObj.transform.rotation = Quaternion.Lerp(rotObj.transform.rotation, Quaternion.AngleAxis(angle + 45, Vector3.forward), speed * Time.deltaTime);

            ls.requestNewSortingLayer(transform.position.y, sr);
            rotObj.transform.GetChild(0).localRotation = mousePos.x > user.transform.position.x ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(180.0f, 0.0f, 90.0f);
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
    }


    //  set attackingPos to Vector2.zero to not use it
    public void attack(Vector2 attackingPos, float mod = 0.0f) {
        if(anim != null || !used || !a.getCanAttack())
            return;
        if(reference.swingSound != null)
            am.playSound(reference.swingSound, transform.position);
        if(reference.aType == Weapon.attackType.Swing)
            anim = StartCoroutine(swingAttackAnim(mod, attackingPos));
        else if(reference.aType == Weapon.attackType.Shoot)
            anim = StartCoroutine(shootAttackAnim(mod, attackingPos));
    }


    public abstract void shootMonster();

    IEnumerator shootAttackAnim(float mod, Vector2 attackingPos) {
        canMove = false;
        trail.emitting = false;
        this.c.enabled = false;
        float recRad = 45f;
        float c = a.getAttackCoolDown();
        float recPerc = .15f;
        if(attackingPos != Vector2.zero)
            user.GetComponent<Movement>().lookAtPos(attackingPos);

        //  fire gun
        //  shoots monster
        shootMonster();

        //  flair

        //  recoil
        transform.parent.DOKill();
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, recRad), c * recPerc, RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.1f, c * recPerc);


        //  push the player back from recoil
        float lungeMod = 1.5f * mod;
        var origin = (Vector2)pt.position;
        var target = GameInfo.mousePos();
        var px = target.x - origin.x;
        var py = target.y - origin.y;
        var theta = Mathf.Atan2(py, px);
        var t = new Vector2(lungeMod * Mathf.Cos(theta), lungeMod * Mathf.Sin(theta));
        pt.DOMove(origin - t, .15f);

        yield return new WaitForSeconds(c * recPerc);

        //  attack is done

        //  return to norm rot / scale
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, -recRad), c * (1f - recPerc), RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.0f, c * (1f - recPerc));
        yield return new WaitForSeconds(c * (1f - recPerc));

        //  start moving again
        canMove = true;
        anim = null;
    }

    IEnumerator swingAttackAnim(float mod, Vector2 attackingPos) {
        canMove = false;
        trail.emitting = true;
        float windTime = .05f, swingTime = .15f;
        float swingRadius = 180f;
        a.startCooldown();
        if(attackingPos != Vector2.zero)
            user.GetComponent<Movement>().lookAtPos(attackingPos);

        //  wind back
        transform.parent.DOScale(1.5f, windTime + swingTime * .5f);
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, swingRadius / 2.0f), windTime, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(windTime);
        c.enabled = true;
        trail.emitting = true;

        //  swing
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, -swingRadius), swingTime, RotateMode.LocalAxisAdd);
        //  lunge the player towards the fucker
        float lungeMod = 1.5f * mod;
        var origin = (Vector2)pt.position;
        var target = GameInfo.mousePos();
        var px = target.x - origin.x;
        var py = target.y - origin.y;
        var theta = Mathf.Atan2(py, px);
        var t = new Vector2(lungeMod * Mathf.Cos(theta), lungeMod * Mathf.Sin(theta));
        pt.DOMove(t + origin, .15f);

        yield return new WaitForSeconds(swingTime);

        //  attack is done
        c.enabled = false;
        trail.emitting = false;

        //  return to norm rot / scale
        Vector3 orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
        orbVector = Input.mousePosition - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

        transform.parent.DOLocalRotate(Quaternion.ToEulerAngles(Quaternion.AngleAxis(angle + 45, Vector3.forward)), swingTime, RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.0f, swingTime);
        yield return new WaitForSeconds(swingTime);

        //  start moving again
        canMove = true;
        anim = null;
    }
}
