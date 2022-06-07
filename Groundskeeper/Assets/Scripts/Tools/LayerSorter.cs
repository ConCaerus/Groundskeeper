using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSorter : MonoBehaviour {


    private void Start() {
        foreach(var i in FindObjectsOfType<SpriteRenderer>())
            requestNewSortingLayer(i.gameObject);
    }


    public void requestNewSortingLayer(GameObject obj) {
        obj.GetComponent<SpriteRenderer>().sortingOrder = -(int)(obj.gameObject.transform.position.y * 100.0f);
    }
}
