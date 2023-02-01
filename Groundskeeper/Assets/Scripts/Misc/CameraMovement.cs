using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour {
    [SerializeField] float speed, scrollSpeed;
    [SerializeField] float maxZoom = 25, minZoom = 10;
    [SerializeField] bool canZoom = true;
    [SerializeField] Vector3 offsetFromPlayer = Vector2.zero;

    [HideInInspector][SerializeField] public bool movingUp = true;

    Coroutine shakeCenterer = null;
    Transform pt;

    private void Start() {
        pt = GameObject.FindGameObjectWithTag("Player").transform;
        var target = new Vector3(pt.position.x, pt.position.y, transform.position.z) + offsetFromPlayer;
        transform.position = target;
    }

    //  Move to player
    private void LateUpdate() {
        var target = new Vector3(pt.position.x, pt.position.y, transform.position.z) + (movingUp ? offsetFromPlayer : -offsetFromPlayer);

        if(transform.position != target)
            transform.position = Vector3.Lerp(transform.position, target, speed * 100.0f * Time.deltaTime);
    }

    private void Update() {
        if(canZoom) {
            var delta = Input.mouseScrollDelta.y;
            if(delta == 0f)
                return;

            var target = delta < 0f ? scrollSpeed * 100.0f * Time.deltaTime : -scrollSpeed * 100.0f * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + target, minZoom, maxZoom);
        }
    }

    public void shake(float dmg) {
        var s = Mathf.Clamp(dmg / 50.0f, 0.0f, 5.0f);
        transform.parent.DOShakePosition(.25f, s);
        if(shakeCenterer != null)
            StopCoroutine(shakeCenterer);
        shakeCenterer = StartCoroutine(centerShake(.25f));
    }

    IEnumerator centerShake(float t) {
        yield return new WaitForSeconds(t);

        transform.parent.DOLocalMove(Vector3.zero, .15f);
        shakeCenterer = null;
    }
}
