using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoulParticles : MonoBehaviour {
    ParticleSystem.Particle[] p = new ParticleSystem.Particle[50];
    [SerializeField] FunkyCode.Light2D lightPreset;
    FunkyCode.Light2D curLight;
    SoulParticlePooler spm;

    public ParticleSystem ps { get; private set; }

    private void Start() {
        spm = FindObjectOfType<SoulParticlePooler>();
        ps = GetComponent<ParticleSystem>();

        curLight = Instantiate(lightPreset, transform.position, Quaternion.identity, transform);
        curLight.enabled = false;
    }

    public void use() {
        ps.Play();
        curLight.enabled = true;
        StartCoroutine(stopper());
        float s = Mathf.Clamp(ps.GetParticles(p), lightPreset.size, lightPreset.size * 5f);
        var ss = ps.main.simulationSpeed;
        curLight.size = s;
        curLight.transform.position = transform.position;
        StartCoroutine(lightKiller(curLight.gameObject, 1f));
    }

    IEnumerator lightKiller(GameObject l, float t) {
        float introTime = .15f, outroTime = .25f;

        float s = Mathf.Clamp(ps.GetParticles(p), lightPreset.size, lightPreset.size * 3f);
        DOTween.To(() => l.GetComponent<FunkyCode.Light2D>().size, x => l.GetComponent<FunkyCode.Light2D>().size = x, s, introTime);
        yield return new WaitForSeconds(introTime + .01f);
        //  waits for the last .25 seconds before the paritcle system ends and destroys itself
        yield return new WaitForSeconds(t - outroTime);

        //  scales the light size down to 0 and destroys the light after .25f seconds
        DOTween.To(() => l.GetComponent<FunkyCode.Light2D>().size, x => l.GetComponent<FunkyCode.Light2D>().size = x, 0.0f, outroTime);
    }

    IEnumerator stopper() {
        yield return new WaitForSeconds(ps.main.duration + .01f);
        curLight.enabled = false;
        spm.hideParticle(this);
    }
}
