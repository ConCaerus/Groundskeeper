using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentInstance : MonoBehaviour {
    public string title;
    [SerializeField] public int hits = 3;
    [SerializeField] public triggerInteractionType tit;

    [SerializeField] bool sways = false;
    [SerializeField] AudioClip hitSound;

    SoulParticlePooler spp;

    AudioManager am;

    public enum triggerInteractionType {
        notTrigger, slowDown, none
    }

    private void Start() {
        am = FindObjectOfType<AudioManager>();
        spp = FindObjectOfType<SoulParticlePooler>();
        if(sways) {
            StartCoroutine(swayWaiter());
        }
    }

    public void takeHit() {
        hits--;
        transform.DOComplete();
        transform.DOShakePosition(.15f);
        transform.DOShakeRotation(.15f, 30);
        am.playSound(hitSound, transform.position);
        if(hits <= 0) {
            remove(true);
        }
    }

    public void remove(bool giveSouls) {
        transform.DOScale(0.0f, .25f);
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        GetComponent<Collider2D>().enabled = false;
        GetComponentInParent<CompositeCollider2D>().GenerateGeometry();
        Destroy(gameObject, .25f);
        float soulsGiven = Random.Range(1, 4);
        if(giveSouls) {
            GameInfo.addSouls(soulsGiven, false);
            spp.showParticle(transform.position, soulsGiven);
            FindObjectOfType<GameUICanvas>().incSouls(soulsGiven);
        }
    }

    public void turnOffCol() {
        StartCoroutine(waitToTurnOffCol());
    }

    IEnumerator waitToTurnOffCol() {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        GetComponent<Collider2D>().enabled = true;
    }

    IEnumerator swayWaiter() {
        float distFromLeft = -FindObjectOfType<GameBoard>().getBoardRad() - transform.position.x;
        float waitPercent = distFromLeft / (FindObjectOfType<GameBoard>().getBoardRad() * 2f);
        waitPercent = Mathf.Abs(waitPercent);
        float freq = 10f;
        float wt = waitPercent * freq;
        float loopDur = 4.5f;
        while(wt >= loopDur)
            wt -= loopDur;
        yield return new WaitForSeconds(wt);
        GetComponent<Animator>().SetTrigger("sway");
    }
}
