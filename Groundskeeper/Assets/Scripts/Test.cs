using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    private void Start() {
        StartCoroutine(player());
    }

    IEnumerator player() {
        GetComponent<AudioSource>().PlayOneShot(clip);
        yield return new WaitForSeconds(.5f);
        StartCoroutine(player());
    }
}
