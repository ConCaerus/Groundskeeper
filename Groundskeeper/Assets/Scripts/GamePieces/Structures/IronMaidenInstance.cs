using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IronMaidenInstance : StructureInstance {
    Coroutine eaterWaiter = null;
    [SerializeField] Sprite openSprite, closedSprite;
    SpriteRenderer sr;

    private void Start() {
        mortalInit();
        structureInit();
        sr = GetComponent<SpriteRenderer>();
    }

    public override void aoeEffect(GameObject effected) {
        //  if doesn't have a monster already in it, eat it
        if(eaterWaiter == null) {
            saoe.shrinkArea();

            //  kill the monster without giving the player any souls'
            var mi = effected.GetComponent<MonsterInstance>();
            effected.GetComponent<MonsterInstance>().unshownDie();

            //  start a coroutine that expands the area of effect after the monster is dead and also rewards the player with souls
            eaterWaiter = StartCoroutine(eater(mi));
        }
    }

    public override void aoeLeaveEffect(GameObject effected) {
    }

    IEnumerator eater(MonsterInstance mi) {
        //  anim
        sr.sprite = closedSprite;
        transform.DOPunchScale(new Vector3(1.1f, 1.1f), .15f);
        //  waits for the monsters health to run out
        yield return new WaitForSeconds(mi.health / 10f);

        //  rewards the player with 3x the normal amt of souls
        var sg = mi.soulsGiven * 3f;
        guc.incSouls(sg);
        GameInfo.addSouls(sg, guc.ended);

        //  resets
        saoe.expandArea();
        sr.sprite = openSprite;
        eaterWaiter = null;
    }
}
