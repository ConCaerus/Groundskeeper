using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class WeaponInstance : MonoBehaviour {
    public enum attackTarget {
        None, People, Monsters
    }

    [SerializeField] Vector2 offsetFromUser;
    float speed = 10f;
    [SerializeField] GameObject rotObj;
    public GameObject user;
    [SerializeField] attackTarget target;

    public TrailRenderer trail;
    Coroutine anim = null;


    public Weapon reference { get; private set; }

    public bool canMove = true;

    private void OnTriggerEnter2D(Collider2D col) {
        if(target == attackTarget.Monsters && col.gameObject.tag == "Monster" && (reference.targetType == col.gameObject.GetComponent<Monster>().type || reference.targetType == GameInfo.MonsterType.Both)) {
            user.GetComponent<Attacker>().attack(col.gameObject, false);
            col.gameObject.GetComponentInChildren<SlashEffect>().slash(user.transform.position, rotObj.transform.GetChild(0).localRotation.x != 0f);
        }
        else if(target == attackTarget.People && col.gameObject.tag == "Helper") {
            user.GetComponent<Attacker>().attack(col.gameObject, false);
        }
        else if(col.gameObject.tag == "Environment") {
            FindObjectOfType<EnvironmentManager>().hitEnvironment(col.ClosestPoint(transform.position));
        }
    }

    void Start() {
        updateReference(GameInfo.getPlayerWeaponIndex());
        GetComponent<Collider2D>().enabled = false;
        trail.emitting = false;
        transform.localPosition = offsetFromUser;
    }
    
    public void updateReference(int index) {
        reference = FindObjectOfType<PresetLibrary>().getWeapon(index);

        //  makes it so the player can attack both physical and spiritual monsters
        if(user.tag == "Player")
            reference.targetType = GameInfo.MonsterType.Both;
        GetComponent<SpriteRenderer>().sprite = reference.sprite;
        trail.gameObject.transform.localPosition = reference.trailPos;
    }

    private void Update() {
        if(canMove)
            movementLogic();
    }

    public abstract void movementLogic();

    public void lookAtMouse() {
        if(canMove) {
            Vector3 orbVector = Camera.main.WorldToScreenPoint(user.transform.position);
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            orbVector = Input.mousePosition - orbVector;
            float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

            rotObj.transform.localPosition = Vector3.zero;
            rotObj.transform.rotation = Quaternion.Lerp(rotObj.transform.rotation, Quaternion.AngleAxis(angle + 45, Vector3.forward), speed * Time.deltaTime);

            FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
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

            FindObjectOfType<LayerSorter>().requestNewSortingLayer(transform.position.y, GetComponent<SpriteRenderer>());
            rotObj.transform.GetChild(0).localRotation = pos.x > user.transform.position.x ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(180.0f, 0.0f, 90.0f);
        }
    }





    public void attack() {
        if(anim != null)
            return;
        if(reference.swingSound != null)
            FindObjectOfType<AudioManager>().playSound(reference.swingSound, transform.position);
        anim = StartCoroutine(attackAnim());
    }

    IEnumerator attackAnim() {
        canMove = false;
        trail.emitting = true;
        float windTime = .05f, swingTime = .15f;
        float swingRadius = 270f;

        //  wind back
        transform.parent.DOScale(1.5f, windTime + swingTime * .5f);
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, swingRadius / 2.0f), windTime, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(windTime);
        GetComponent<Collider2D>().enabled = true;
        trail.emitting = true;

        //  swing
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, -swingRadius), swingTime, RotateMode.LocalAxisAdd);
        yield return new WaitForSeconds(swingTime);

        //  attack is done
        GetComponent<Collider2D>().enabled = false;
        trail.emitting = false;

        //  return to norm rot / scale
        transform.parent.DOLocalRotate(new Vector3(0.0f, 0.0f, swingRadius / 2.0f), swingTime, RotateMode.LocalAxisAdd);
        transform.parent.DOScale(1.0f, swingTime);
        yield return new WaitForSeconds(swingTime);

        //  start moving again
        canMove = true;
        user.GetComponent<Attacker>().startCooldown();

        //  wait for cooldown
        yield return new WaitForSeconds(reference.cooldown);
        anim = null;
    }
}
