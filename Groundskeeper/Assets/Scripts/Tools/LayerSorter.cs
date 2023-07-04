using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSorter : MonoBehaviour {



    public void requestNewSortingLayer(Collider2D col, SpriteRenderer sprite) {
        if(col == null || sprite == null)
            return;
        requestNewSortingLayer(col.bounds.center.y, sprite);
    }

    public void requestNewSortingLayer(float y, SpriteRenderer sprite) {
        if(sprite == null)
            return;
        sprite.sortingOrder = -(int)(y * 100.0f);
    }

    public void waitAndRequestNewSortingLayer(Collider2D col, SpriteRenderer sprite) {
        if(col == null || sprite == null)
            return;
        StartCoroutine(waitAndSetSortingLayer(col, sprite));
    }

    IEnumerator waitAndSetSortingLayer(Collider2D col, SpriteRenderer sprite) {
        if(col == null || sprite == null)
            yield break;
        yield return new WaitForSeconds(.1f);
        requestNewSortingLayer(col, sprite);
    }
}
