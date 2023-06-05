using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour, IPointerEnterHandler {
    [SerializeField] AudioClip sound = null;


    private void Start() {
        if(GetComponent<Button>() != null && sound != null)
            GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<AudioManager>().playSound(sound, Camera.main.transform.position, false); });
    }

    public void OnPointerEnter(PointerEventData eventData) {
        FindObjectOfType<AudioManager>().playSound(sound, Camera.main.transform.position, false);
    }
}
