using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightObj : MonoBehaviour {
    float startSize;
    [SerializeField] FlickerInfo fInfo = null;

    private void Start() {
        startSize = GetComponent<FunkyCode.Light2D>().size;
        if(fInfo != null) {
            fInfo.hardShadowStartSize = fInfo.hardShadow.transform.localScale;
            StartCoroutine(flickerWaiter());
        }
    }

    public void setNewNormalLightSize(float size) {
        startSize = size;
    }

    IEnumerator flickerWaiter() {
        float avgTime = .5f;
        float timeVar = .2f;
        float bigSize = Random.Range(startSize, startSize + (startSize * fInfo.flickerVariation));
        float lilSize = Random.Range(startSize - (startSize * fInfo.flickerVariation), startSize);
        float temp = fInfo.hardShadowStartSize.x;  //  used to modify the hardshadow local size

        //  increase light on first pass
        //  decrease light on second pass
        //  return to normal size light on third pass
        for(int i = 0; i < 3; i++) {
            var t = Random.Range(avgTime - timeVar, avgTime + timeVar);
            DOTween.To(() => GetComponent<FunkyCode.Light2D>().size, x => GetComponent<FunkyCode.Light2D>().size = x, i == 0 ? bigSize : i == 1 ? lilSize : startSize, t);
            DOTween.To(() => temp, x => temp = x, i == 0 ? ((lilSize * fInfo.hardShadowStartSize.x) / startSize) : i == 1 ? ((bigSize * fInfo.hardShadowStartSize.x) / startSize) : fInfo.hardShadowStartSize.x, t).OnUpdate(() => {
                fInfo.hardShadow.transform.localScale = new Vector2(temp, temp);
            });

            yield return new WaitForSeconds(t);
        }

        StartCoroutine(flickerWaiter());
    }
}

[System.Serializable]
public class FlickerInfo {
    public float flickerVariation;
    public GameObject hardShadow;
    [HideInInspector] public Vector2 hardShadowStartSize;
}
