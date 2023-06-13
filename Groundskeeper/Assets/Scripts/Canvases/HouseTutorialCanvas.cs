using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HouseTutorialCanvas : MonoBehaviour {
    [SerializeField] GameObject mouth, bed;

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
            "Aha! It lived!", "I can't believe that it actually lived!", "I'm a genius!",
            "...", "....", "It's not moving again...", "HEY, GUY!", "GOOD JOB AT NOT DYING!",
            "GRADE A STUFF! REALLY!", "THERE'S A SOUL EATER IN THIS HOUSE!", "BIG MOUTH, CAN'T MISS IT!",
            "please don't miss it.", "USE IT TO BUY AND UPGRADE YOUR DEFENSES!", "THERE'S ALSO A BED TO SLEEP IN WHEN YOU'RE DONE!",
            "AND A FISH TO KEEP YOU COMPANY!", "please don't eat him.", "humans always loved eating things...", "...",
            "YOU'LL BE FINE HERE FOR NOW!", "JUST KEEP NOT DYING!"
        }, new List<DialogText.facialExpression>() {
            DialogText.facialExpression.happy, DialogText.facialExpression.normal, DialogText.facialExpression.happy,
            DialogText.facialExpression.normal, DialogText.facialExpression.thinking, DialogText.facialExpression.dismissive, DialogText.facialExpression.normal, DialogText.facialExpression.normal,
            DialogText.facialExpression.happy, DialogText.facialExpression.normal, DialogText.facialExpression.normal,
            DialogText.facialExpression.dismissive, DialogText.facialExpression.normal, DialogText.facialExpression.normal,
            DialogText.facialExpression.happy, DialogText.facialExpression.dismissive, DialogText.facialExpression.dismissive, DialogText.facialExpression.thinking,
            DialogText.facialExpression.normal, DialogText.facialExpression.happy
        }),
        delegate { StartCoroutine(mouthTutEnd()); });
    }

    IEnumerator mouthTutEnd() {
        //  pans the camera to look at the mouth
        var sPos = Camera.main.transform.position;
        yield return new WaitForSeconds(.5f);
        Camera.main.transform.DOMoveY(mouth.transform.position.y, .35f);
        yield return new WaitForSeconds(2.0f);
        Camera.main.transform.DOMoveY(bed.transform.position.y, .35f);
        yield return new WaitForSeconds(2.0f);
        Camera.main.transform.DOMoveY(sPos.y, .25f);
        yield return new WaitForSeconds(.75f);
        FindObjectOfType<PlayerHouseInstance>().setCanMove(true);
        FindObjectOfType<CameraMovement>().enabled = true;
        enabled = false;
    }
}
