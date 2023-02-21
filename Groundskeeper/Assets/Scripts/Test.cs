using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Test : MonoBehaviour {
    [SerializeField] GameObject one, two;
    [SerializeField] float rad = 1f;

    private void Start() {
        StartCoroutine(r());
    }

    private void Update() {
    }

    Vector2 rotatePoint(Vector2 origin, Vector2 target, float rad) {
        var l = target.x - origin.x;
        var h = target.y - origin.y;
        var theta = Mathf.Atan2(h, l);
        var x = target.x * Mathf.Cos(theta) - target.y * Mathf.Sin(theta);
        var y = target.y * Mathf.Cos(theta) + target.x * Mathf.Sin(theta);
        return new Vector2(x, y);
    }

    Vector2 rotate_point(float cx, float cy, float angle, Vector2 p) {
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        // translate point back to origin
        p.x -= cx;
        p.y -= cy;

        // rotate point
        float xnew = p.x * c - p.y * s;
        float ynew = p.x * s + p.y * c;

        // translate point back
        p.x = xnew + cx;
        p.y = ynew + cy;
        return p;
    }

    IEnumerator r() {
        while(true) {
            two.transform.position = rotate_point(one.transform.position.x, one.transform.position.y, rad * Mathf.Deg2Rad, two.transform.position);
            yield return new WaitForSeconds(.1f);
        }
    }
}
