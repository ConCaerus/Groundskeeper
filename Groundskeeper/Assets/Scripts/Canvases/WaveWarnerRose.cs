using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveWarnerRose : MonoBehaviour {
    [SerializeField] GameObject[] dots; //  0 - north, 1 - east, 2 - south, 3 - west
    [SerializeField] GameObject warning;

    Coroutine dAnim = null;

    private void Start() {
        foreach(var i in dots)
            i.transform.localScale = Vector3.zero;
        warning.transform.localScale = Vector3.zero;
    }

    public void warn(MonsterSpawner.direction[] dir) {
        if(dAnim != null)
            StopCoroutine(dAnim);

        float disTime = .15f;
        foreach(var i in dots) {
            i.transform.DOKill();
            i.transform.DOScale(0f, disTime);
        }
        foreach(var i in dir) {
            switch(i) {
                case MonsterSpawner.direction.North:
                    dAnim = StartCoroutine(dotAnim(dots[0]));
                    break;
                case MonsterSpawner.direction.East:
                    dAnim = StartCoroutine(dotAnim(dots[1]));
                    break;
                case MonsterSpawner.direction.South:
                    dAnim = StartCoroutine(dotAnim(dots[2]));
                    break;
                case MonsterSpawner.direction.West:
                    dAnim = StartCoroutine(dotAnim(dots[3]));
                    break;
            }
        }

        StartCoroutine(attentionGrabber());
    }


    IEnumerator dotAnim(GameObject d) {
        yield return new WaitForSeconds(1.25f);

        float sizeTime = .15f;
        while(true) {
            //  grow big
            d.transform.DOScale(1.0f, sizeTime);
            yield return new WaitForSeconds(sizeTime * 2.0f);

            //  shrink small
            d.transform.DOScale(.75f, sizeTime);
            yield return new WaitForSeconds(sizeTime * 2.0f);
        }
    }

    IEnumerator attentionGrabber() {
        transform.GetChild(0).DOScale(1.5f, .15f);
        warning.transform.DOScale(.45f, .15f);

        yield return new WaitForSeconds(.15f);

        yield return new WaitForSeconds(1f);

        transform.GetChild(0).DOScale(1.0f, .25f);
        warning.transform.DOScale(0.0f, .25f);
    }
}
