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


    public int weaponInd = 0;
    public Weapon reference { get; private set; }

    public bool canMove = true;

    private void OnTriggerEnter2D(Collider2D col) {
        if(target == attackTarget.Monsters && col.gameObject.tag == "Monster") {
            user.GetComponent<Attacker>().attack(col.gameObject, false);
            col.gameObject.GetComponentInChildren<SlashEffect>().slash(user.transform.position, rotObj.transform.GetChild(0).localRotation.x != 0f);
        }
        else if(target == attackTarget.People && col.gameObject.tag == "Person")
            user.GetComponent<Attacker>().attack(col.gameObject, false);
    }

    void Start() {
        reference = FindObjectOfType<PresetLibrary>().getWeapon(weaponInd);
        GetComponent<SpriteRenderer>().sprite = reference.sprite;
        GetComponent<Collider2D>().enabled = false;
        trail.gameObject.transform.localPosition = reference.trailPos;
        trail.emitting = false;
        transform.localPosition = offsetFromUser;
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

            FindObjectOfType<LayerSorter>().requestNewSortingLayer(gameObject);
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

            FindObjectOfType<LayerSorter>().requestNewSortingLayer(gameObject);
            rotObj.transform.GetChild(0).localRotation = pos.x > user.transform.position.x ? Quaternion.Euler(0.0f, 0.0f, 0.0f) : Quaternion.Euler(180.0f, 0.0f, 90.0f);
        }
    }





    public void attack() {
        if(anim != null)
            return;
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
