using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticlePooler : MonoBehaviour {
    int maxParticleCount = 50;  //  max number of lights that can be present in activeLights at any given moment
    [SerializeField] GameObject particlePrefab;
    Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

    private void Start() {
        //  populates the pool with objects
        for(int i = 0; i < maxParticleCount; i++) {
            var temp = Instantiate(particlePrefab, transform);
            temp.transform.position = Vector3.zero;
            pool.Enqueue(temp.GetComponent<ParticleSystem>());
        }
    }

    public void showParticle(Vector2 pos) {
        if(pool.Count > 0) {
            var part = pool.Dequeue();
            part.transform.position = pos;
            part.Play();
            StartCoroutine(completeCycle(part));
        }
    }

    public void hideParticle(ParticleSystem particle) {
        particle.Stop();
        pool.Enqueue(particle);
    }

    IEnumerator completeCycle(ParticleSystem particle) {
        yield return new WaitForSeconds(particle.main.duration);
        hideParticle(particle);
    }
}
