using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulParticlePooler : MonoBehaviour {
    //  NOTE: maxLightCount has nothing to do with how many funkycode lights are present in the actual particle system
    int maxLightCount = 10;  //  max number of lights that can be present in activeLights at any given moment

    [SerializeField] GameObject particlePrefab;
    Queue<SoulParticles> pool = new Queue<SoulParticles>();

    private void Start() {
        //  populates the pool with objects
        for(int i = 0; i < maxLightCount; i++) {
            var temp = Instantiate(particlePrefab, transform);
            temp.transform.position = Vector3.zero;
            pool.Enqueue(temp.GetComponent<SoulParticles>());
        }
    }

    public void showParticle(Vector2 pos, float soulsGiven) {
        if(pool.Count > 0) {
            var soulP = pool.Dequeue();
            soulP.transform.position = pos;
            soulP.ps.emission.SetBurst(0, new ParticleSystem.Burst(.5f, Mathf.RoundToInt(soulsGiven * 4.0f)));
            soulP.use();
        }
    }

    public void hideParticle(SoulParticles particle) {
        particle.ps.Stop();
        pool.Enqueue(particle);
    }
}
