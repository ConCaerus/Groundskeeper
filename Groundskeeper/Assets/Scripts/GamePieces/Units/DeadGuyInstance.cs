using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeadGuyInstance : MonoBehaviour {
    [HideInInspector] float soulsGiven = 5f; //  have this number go up as the nights increase
    [SerializeField] public string title;
    [SerializeField] AudioClip interactSound;

    GameUICanvas guc;
    GameBoard gb;

    private void Start() {
        soulsGiven *= (GameInfo.getNightCount() + 1);
        guc = FindObjectOfType<GameUICanvas>();
        gb = FindObjectOfType<GameBoard>();

        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
    }

    public void interactedWith() {
        //  give souls and destory
        GameInfo.addSouls(soulsGiven, false);
        guc.incSouls(soulsGiven);
        FindObjectOfType<AudioManager>().playSound(interactSound, transform.position);

        //  dying animation
        StartCoroutine(cleanup());
    }

    IEnumerator cleanup() {
        transform.DOScale(0.0f, .15f);
        yield return new WaitForSeconds(.15f);
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        GetComponentInParent<DeadGuyHolder>().updateCollider();
        Destroy(gameObject);
    }

    public void hardCleanup() {
        FindObjectOfType<GameBoard>().removeFromGameBoard(gameObject);
        GetComponentInParent<DeadGuyHolder>().updateCollider();
        Destroy(gameObject);
    }
}
