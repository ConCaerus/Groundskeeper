using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HouseTutorialCanvas : MonoBehaviour {
    [SerializeField] GameObject mouth;

    //  night should be 0
    private void Start() {
        if(GameInfo.getNightCount() > 0)
            return;

        FindObjectOfType<CameraMovement>().enabled = false;
        StartCoroutine(mouthTut());
    }

    IEnumerator mouthTut() {
        FindObjectOfType<PlayerHouseInstance>().setCanMove(false);
        yield return new WaitForSeconds(1.0f);

        FindObjectOfType<DialogCanvas>().loadDialogText(new DialogText(new List<string>() {
            "You lived?", "I mean, you lived!", "Congratulations!", "...", "Anyways...", "You need to strengthen your defences", "This cabin is outfitted with a soul eater",
            "Give it some of the souls you've collected and it will reward you with...", "with things...", "It's hard to explain. You'll figure it out I'm sure"
        }), delegate { StartCoroutine(mouthTutEnd()); });
    }

    IEnumerator mouthTutEnd() {
        //  pans the camera to look at the mouth
        var sPos = Camera.main.transform.position;
        yield return new WaitForSeconds(.5f);
        Camera.main.transform.DOMoveY(mouth.transform.position.y, .35f);
        yield return new WaitForSeconds(2.0f);
        Camera.main.transform.DOMoveY(sPos.y, .25f);
        yield return new WaitForSeconds(.75f);
        FindObjectOfType<PlayerHouseInstance>().setCanMove(true);
        FindObjectOfType<CameraMovement>().enabled = true;
    }
}
