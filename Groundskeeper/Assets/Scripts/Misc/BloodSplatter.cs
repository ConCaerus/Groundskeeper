using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BloodSplatter : MonoBehaviour {
    [SerializeField] Sprite[] sprites;
    SpriteRenderer sr;

    public delegate void func(GameObject stain);

    func cleaner;

    public void setup(func f) {
        cleaner = f;
        //  setup
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprites[Random.Range(0, sprites.Length)];
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
        sr.color = new Color(Random.Range(.4f, 1.0f), Random.Range(.4f, 1.0f), Random.Range(.4f, 1.0f), Random.Range(.25f, .5f));
        var randScale = Random.Range(.5f, 1f);
        transform.localScale = new Vector3(randScale, randScale);
        enabled = false;

        //  kills itself
        StartCoroutine(destroyer(5f));
    }

    IEnumerator destroyer(float disappearTime) {
        yield return new WaitForSeconds(disappearTime - .5f);
        sr.DOColor(Color.clear, .5f);
        yield return new WaitForSeconds(.51f);
        cleaner(gameObject);
    }
}
