using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarSpawner : MonoBehaviour {
    [SerializeField] GameObject healthBar;



    //  NOTE: turned off cause I think the game looks better,
    //          also gives the game an extra 20 fps

    public void giveHealthBar(GameObject obj) {
        if(obj == null || obj.GetComponent<Mortal>().health <= 0.0f || obj.tag == "Monster")
            return;
        var bar = Instantiate(healthBar.gameObject, obj.transform.position, Quaternion.identity, obj.transform);
        bar.GetComponentInChildren<HealthBar>().setParent(obj);
    }

    public void hideHealthBar(GameObject obj) {
        return;
        if(obj.GetComponent<Mortal>().healthBar != null) {
            Destroy(obj.GetComponent<Mortal>().healthBar.transform.parent.gameObject);
            obj.GetComponent<Mortal>().healthBar = null;
        }
    }
}
