using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class LightObj : MonoBehaviour {
    [SerializeField] float outerRadius;

    private void Start() {
        this.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col) {
        //  show enemy when one comes into the light
        if((col.gameObject.tag == "Monster" || col.gameObject.tag == "Helper"))
            col.gameObject.GetComponent<Movement>().show();
        else if(col.gameObject.tag == "Environment")
            FindObjectOfType<EnvironmentManager>().showAllEnvAroundArea(transform.position, outerRadius);
    }

    private void OnTriggerExit2D(Collider2D col) {
        if(!col.enabled || col.gameObject == null)
            return;
        
        if(col.gameObject != null && (col.gameObject.tag == "Monster" || col.gameObject.tag == "Helper") && !col.IsTouching(GetComponent<Collider2D>()))
            col.gameObject.GetComponent<Movement>().hide();
        else if(col.gameObject != null && col.gameObject.tag == "Environment" && !col.IsTouching(GameObject.FindGameObjectWithTag("House").transform.GetChild(0).gameObject.GetComponent<Collider2D>()))
            FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(transform.position, outerRadius);
    }
}
