using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShotgunInstance : PlayerWeaponVariant {
    GameTutorialCanvas gtc;
    [SerializeField] public ParticleSystem gunFireParticles;

    int numOfShots = 11; //  keep this number odd because fuck you
    float spreadAmt = 45f * Mathf.Deg2Rad;


    public override void variantSetup() {
        gtc = FindObjectOfType<GameTutorialCanvas>();
        //  destroys all gunfire particles that aren't being used
        foreach(var i in gunFireParticles.transform.parent.GetComponentsInChildren<ParticleSystem>()) {
            if(i != gunFireParticles)
                Destroy(i.gameObject);
        }
    }

    public override void performOnAttack() {
        if(gtc != null) {
            if(pi.weaponAttackMod > 1.01f)
                gtc.hasChargedAttacked();
        }
        //  flair
        gunFireParticles.Play();
        gunLight.size = 20f;
        DOTween.To(() => gunLight.size, x => gunLight.size = x, 0.0f, .15f);

        //  logic
        attack(GameInfo.mousePos(), Vector2.zero, pi, pi.weaponAttackMod);
        pi.weaponAttackMod = 1.0f;
    }

    public override void performOnAttackEnd() {
    }
    public override void lobToMonster() {
    }

    Vector2 rotate_point(float cx, float cy, float angle, Vector2 p) {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        // translate point back to origin
        p.x -= cx;
        p.y -= cy;

        // rotate point
        float xnew = p.x * c - p.y * s;
        float ynew = p.x * s + p.y * c;

        // translate point back
        p.x = xnew + cx;
        p.y = ynew + cy;
        return p;
    }

    public override void shootMonster() {
        LayerMask monsterLayer = LayerMask.GetMask("Monster");
        var curSpread = 0.0f;


        for(int i = 0; i < numOfShots; i++) {
            var p = rotate_point(transform.position.x, transform.position.y, curSpread, GameInfo.mousePos());
            RaycastHit2D hit = Physics2D.Raycast(transform.position, p - (Vector2)transform.position, reference.range, monsterLayer);
            Debug.DrawRay(transform.position, p - (Vector2)transform.position, Color.white);
            curSpread += (i + 1) * (spreadAmt * 2.0f) / numOfShots * (i % 2 == 0 ? -1f : 1f);

            if(hit.collider != null) {
                var o = hit.collider.gameObject.GetComponentInParent<MonsterInstance>().gameObject;
                a.attack(o.gameObject, false);
                if(pi != null) {
                    cm.shake(pi.getDamage());
                }
                o.GetComponentInChildren<SlashEffect>().slash(user.transform.position, rotObj.transform.GetChild(0).localRotation.x != 0f);
            }
        }
    }
}
