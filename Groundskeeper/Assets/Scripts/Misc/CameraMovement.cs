using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    [SerializeField] float speed, scrollSpeed;
    [SerializeField] float maxZoom = 25, minZoom = 10;

    private void Start() {
        var pos = GameObject.FindGameObjectWithTag("Player").transform.position;
        var target = new Vector3(pos.x, pos.y, transform.position.z);
        transform.position = target;
    }

    //  Move to player
    private void LateUpdate() {
        var pos = GameObject.FindGameObjectWithTag("Player").transform.position;
        var target = new Vector3(pos.x, pos.y, transform.position.z);

        if(transform.position != target)
            transform.position = Vector3.Lerp(transform.position, target, speed * 100.0f * Time.deltaTime);
    }

    private void Update() {
        var delta = Input.mouseScrollDelta.y;
        if(delta == 0f)
            return;

        var target = delta < 0f ? scrollSpeed * 100.0f * Time.deltaTime : -scrollSpeed * 100.0f * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + target, minZoom, maxZoom);
    }
}
