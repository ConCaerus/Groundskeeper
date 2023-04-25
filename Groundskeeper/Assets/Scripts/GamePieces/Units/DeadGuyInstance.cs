using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DeadGuyInstance : MonoBehaviour {
    [HideInInspector] public float soulsGiven = 10f; //  have this number go up as the nights increase
    [SerializeField] public string title;

    GameUICanvas guc;
    GameBoard gb;

    private void Start() {
        soulsGiven *= GameInfo.getNightCount();
        FindObjectOfType<GameBoard>().deadGuys.Add(this);
        guc = FindObjectOfType<GameUICanvas>();
        gb = FindObjectOfType<GameBoard>();

        FindObjectOfType<LayerSorter>().requestNewSortingLayer(GetComponent<Collider2D>(), GetComponent<SpriteRenderer>());
    }

    public void interactedWith() {
        //  give souls and destory
        GameInfo.addSouls(soulsGiven, false);
        guc.incSouls(soulsGiven);

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
