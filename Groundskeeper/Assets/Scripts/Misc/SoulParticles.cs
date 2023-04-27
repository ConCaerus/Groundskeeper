using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoulParticles : MonoBehaviour {
    ParticleSystem.Particle[] p = new ParticleSystem.Particle[50];
    [SerializeField] FunkyCode.Light2D lightPreset;
    List<FunkyCode.Light2D> lights = new List<FunkyCode.Light2D>();

    int maxNumOfLights = 3;

    private void Start() {
        StartCoroutine(stopper());
        StartCoroutine(lightMover());
    }

    IEnumerator lightMover() {
        yield return new WaitForSeconds(.1f);

        int c = Mathf.Clamp(GetComponent<ParticleSystem>().GetParticles(p), 0, maxNumOfLights);
        float s = Mathf.Clamp(GetComponent<ParticleSystem>().GetParticles(p), lightPreset.size, lightPreset.size * 3f);
        var ss = GetComponent<ParticleSystem>().main.simulationSpeed;
        for(int i = 0; i < c; i++) {
            if(i >= lights.Count) {
                var temp = Instantiate(lightPreset, p[i].position, Quaternion.identity, transform);
                temp.size = s;
                lights.Add(temp);
                StartCoroutine(lightKiller(lights[lights.Count - 1].gameObject, p[i].remainingLifetime / ss));
            }
            else if(lights[i] == null)
                continue;
            else
                lights[i].transform.position = p[i].position;
        }
        StartCoroutine(lightMover());
    }

    IEnumerator lightKiller(GameObject l, float t) {
        float timeDiff = .25f;
        yield return new WaitForSeconds(t - timeDiff);

        DOTween.To(() => l.GetComponent<FunkyCode.Light2D>().size, x => l.GetComponent<FunkyCode.Light2D>().size = x, 0.0f, timeDiff);
        Destroy(l.gameObject, timeDiff);
    }

    IEnumerator stopper() {
        yield return new WaitForSeconds(GetComponent<ParticleSystem>().main.duration + .15f);
        enabled = false;
    }
}
