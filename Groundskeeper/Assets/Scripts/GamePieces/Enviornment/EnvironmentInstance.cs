using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentInstance : MonoBehaviour {
    public string title;
    [SerializeField] int hits = 3;

    public void takeHit() {
        hits--;
        transform.DOComplete();
        transform.DOShakePosition(.15f);
        transform.DOShakeRotation(.15f, 30);

        if(hits <= 0) {
            transform.DOScale(0.0f, .25f);
            FindObjectOfType<GameBoard>().environment.RemoveAll(x => x.gameObject.GetInstanceID() == gameObject.GetInstanceID());
            GetComponent<Collider2D>().enabled = false;
            GetComponentInParent<CompositeCollider2D>().GenerateGeometry();
            Destroy(gameObject, .25f);
            GameInfo.addSouls(Random.Range(1, 6));
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
}
