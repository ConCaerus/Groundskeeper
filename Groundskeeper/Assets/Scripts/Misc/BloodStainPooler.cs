using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStainPooler : MonoBehaviour {
    [SerializeField] GameObject stainPrefab;
    Queue<GameObject> pool = new Queue<GameObject>();
    int maxStainCount = 100;

    private void Start() {
        //  populates the pool with objects
        for(int i = 0; i < maxStainCount; i++) {
            var temp = Instantiate(stainPrefab, transform);
            temp.transform.position = Vector3.zero;
            pool.Enqueue(temp);
            temp.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void showStain(Vector2 pos) {
        if(pool.Count > 0) { 
            var stain = pool.Dequeue();
            stain.GetComponent<SpriteRenderer>().enabled = true;
            stain.GetComponent<BloodSplatter>().setup(hideStain);
            stain.transform.position = pos;
        }
    }

    public void hideStain(GameObject stain) {
        stain.GetComponent<SpriteRenderer>().enabled = false;
        pool.Enqueue(stain);
    }
}
