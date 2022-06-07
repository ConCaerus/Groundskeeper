using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashEffect : MonoBehaviour {

    Coroutine effect = null;


    public void slash(Vector2 pos, bool flipped) {
        var orbVector = Camera.main.WorldToScreenPoint(transform.position);
        var screenPos = Camera.main.WorldToScreenPoint(pos);
        orbVector = screenPos - orbVector;
        float angle = Mathf.Atan2(orbVector.y, orbVector.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle + 90 + Random.Range(-15f, 15f), Vector3.forward);

        var lr = GetComponent<LineRenderer>();
        if(effect != null)
            StopCoroutine(effect);
        effect = StartCoroutine(slashAnim(lr, flipped));
    }


    IEnumerator slashAnim(LineRenderer lr, bool flipped) {
        float speed = 100.0f;
        var pos = flipped ? new Vector2(3f, 0f) : new Vector2(-3f, 0f);
        lr.SetPosition(0, pos);
        lr.SetPosition(1, pos);
        lr.enabled = true;

        while(Vector3.Distance(lr.GetPosition(1), -pos) > 0f) {
            lr.SetPosition(1, Vector3.MoveTowards(lr.GetPosition(1), -pos, speed * Time.deltaTime));
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(.1f);
        while(Vector3.Distance(lr.GetPosition(0), -pos) > .1f) {
            lr.SetPosition(0, Vector3.Lerp(lr.GetPosition(0), -pos, 100 * Time.deltaTime));
            yield return new WaitForSeconds(.01f);
        }

        lr.enabled = false;
        effect = null;
    }
}
