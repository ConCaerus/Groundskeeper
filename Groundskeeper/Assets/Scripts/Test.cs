using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    [SerializeField] GameObject z;

    private void Update() {
        Debug.Log(yes(z.transform.position));
    }

    bool yes(Vector2 t) {
        var dir = t - (Vector2)transform.position;
        int lMask = LayerMask.GetMask("House");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Vector2.Distance(transform.position, z.transform.position), lMask);
        Debug.DrawRay(transform.position, dir);
        return hit.collider != null;
    }
}
