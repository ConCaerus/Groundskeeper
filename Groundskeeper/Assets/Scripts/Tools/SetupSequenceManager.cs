using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SetupSequenceManager : MonoBehaviour {
    [SerializeField] SpriteRenderer playerWeaponSr;

    PlayerInstance pi;
    PlayerWeaponInstance pwi;
    PregameCanvas pc;
    DialogCanvas dc;

    bool p = false;

    private void Awake() {
        if(GameInfo.getNightCount() > 0) {
            enabled = false;
            return;
        }

        pi = FindObjectOfType<PlayerInstance>();
        pwi = FindObjectOfType<PlayerWeaponInstance>();
        pc = FindObjectOfType<PregameCanvas>();
        dc = FindObjectOfType<DialogCanvas>();
        StartCoroutine(houseSetup());
    }

    public void placedHouse() {
        //  removes the house from the list of player's buyables
        GameInfo.lockAllBuyables(FindObjectOfType<BuyableLibrary>());
        pc.setup();
        pi.setCanMove(false);
        StartCoroutine(weaponSetup());
    }

    IEnumerator houseSetup() {
        GameInfo.lockAllBuyables(FindObjectOfType<BuyableLibrary>());
        //  disables player's controls
        pi.setCanMove(false);
        //  takes away player's weapon
        pwi.enabled = false;
        pwi.GetComponentInChildren<TrailRenderer>().enabled = false;
        playerWeaponSr.sprite = null;

        //  disables other shit
        pc.startButton.SetActive(false);
        pc.soulsText.enabled = false;

        //  starting aethetics
        FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(Vector2.zero, 5.0f);
        var s = pi.initLightSize;
        pi.initLightSize = 0f;
        yield return new WaitForSeconds(1.0f);
        DOTween.To(() => pi.initLightSize, x => pi.initLightSize = x, s, .15f);
        yield return new WaitForSeconds(1.5f);

        //  dialog from the devil that introduces the player to the world
        //  and tells them that they need to place their house
        dc.loadDialogText(new DialogText(
            new List<string>() { "Welcome to the world", "Place your <color=\"yellow\">house" },
            new List<DialogText.facialExpression>() { DialogText.facialExpression.happy, DialogText.facialExpression.normal}), 
            null);

        yield return new WaitForSeconds(.1f);

        //  allows the player to place house
        pi.setCanMove(true);
        GameInfo.unlockBuyable(Buyable.buyableTitle.House);
        pc.setup();
    }

    IEnumerator weaponSetup() {
        dc.loadDialogText(new DialogText(
            new List<string>() { "Mediocre placement", "Anyways... ", "Here, take this <color=\"red\">" + pwi.getWeapon().title.ToString() },
            new List<DialogText.facialExpression>() { DialogText.facialExpression.normal, DialogText.facialExpression.dismissive, DialogText.facialExpression.normal}),
            delegate {
            p = true;
        });
        yield return new WaitForSeconds(.1f);
        while(!p)
            yield return new WaitForEndOfFrame();
        playerWeaponSr.sprite = FindObjectOfType<PresetLibrary>().getWeapon(GameInfo.getPlayerStats().getWeaponTitle(FindObjectOfType<PresetLibrary>())).sprite;
        pwi.enabled = true;
        pwi.variant.canMove = true;
        pwi.canAttackG = false;
        pi.setCanMove(true);
        pwi.GetComponentInChildren<TrailRenderer>().enabled = true;

        yield return new WaitForSeconds(1.0f);

        pwi.variant.canMove = false;
        pi.setCanMove(false);

        dc.loadDialogText(new DialogText(
            new List<string>() { "Great! You're ready for the game", "I guess...", "...", "Press this <color=\"yellow\">button<color=\"white\"> when you're ready to <color=\"yellow\">start" }, 
            new List<DialogText.facialExpression>() { DialogText.facialExpression.happy, DialogText.facialExpression.dismissive, DialogText.facialExpression.thinking, DialogText.facialExpression.normal}),
            delegate {
            StartCoroutine(endSetup());
        });
    }

    IEnumerator endSetup() {
        yield return new WaitForSeconds(.25f);
        pc.startButton.SetActive(true);
        pc.setup();
        FindObjectOfType<GameTutorialCanvas>().show();
        FindObjectOfType<GameBoard>().saveBoard();
        yield return new WaitForSeconds(1.0f);
        pwi.variant.canMove = true;
        pwi.canAttackG = true;
        pi.setCanMove(true);
        enabled = false;
    }
}
