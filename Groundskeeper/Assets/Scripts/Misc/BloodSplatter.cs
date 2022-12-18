using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BloodSplatter : MonoBehaviour {
    [SerializeField] Sprite[] sprites;

    private void Start() {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        GetComponent<SpriteRenderer>().color = new Color(Random.Range(.4f, 1.0f), Random.Range(.4f, 1.0f), Random.Range(.4f, 1.0f), Random.Range(.25f, .5f));
        var randScale = Random.Range(.5f, 1f);
        transform.localScale = new Vector3(randScale, randScale);
        enabled = false;
    }
}
