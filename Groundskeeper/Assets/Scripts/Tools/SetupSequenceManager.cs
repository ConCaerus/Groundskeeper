using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SetupSequenceManager : MonoBehaviour {
    Sprite weaponSprite;
    [SerializeField] SpriteRenderer playerWeaponSr;

    bool p = false;

    private void Awake() {
        if(GameInfo.getNightCount() > 0) {
            enabled = false;
            return;
        }

        StartCoroutine(houseSetup());
    }

    public void placedHouse() {
        //  removes the house from the list of player's buyables
        GameInfo.lockAllBuyables(FindObjectOfType<BuyableLibrary>());
        FindObjectOfType<PregameCanvas>().setup();
        FindObjectOfType<PlayerInstance>().setCanMove(false);

        StartCoroutine(weaponSetup());
    }

    IEnumerator houseSetup() {
        GameInfo.lockAllBuyables(FindObjectOfType<BuyableLibrary>());
        //  disables player's controls
        FindObjectOfType<PlayerInstance>().setCanMove(false);
        //  takes away player's weapon
        FindObjectOfType<PlayerWeaponInstance>().enabled = false;
        FindObjectOfType<PlayerWeaponInstance>().GetComponentInChildren<TrailRenderer>().enabled = false;
        weaponSprite = playerWeaponSr.sprite;
        playerWeaponSr.sprite = null;

        //  disables other shit
        FindObjectOfType<PregameCanvas>().startButton.SetActive(false);
        FindObjectOfType<PregameCanvas>().soulsText.enabled = false;

        //  starting aethetics
        FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(Vector2.zero, 5.0f);
        var s = FindObjectOfType<PlayerInstance>().initLightSize;
        FindObjectOfType<PlayerInstance>().initLightSize = 0f;
        yield return new WaitForSeconds(1.0f);
        DOTween.To(() => FindObjectOfType<PlayerInstance>().initLightSize, x => FindObjectOfType<PlayerInstance>().initLightSize = x, s, .15f);
        yield return new WaitForSeconds(1.5f);

        //  dialog from the devil that introduces the player to the world
        //  and tells them that they need to place their house
        FindObjectOfType<DialogCanvas>().loadDialogText(new DialogText(
            new List<string>() { "Welcome to the world", "Place your house" },
            new List<DialogText.facialExpression>() { DialogText.facialExpression.happy, DialogText.facialExpression.normal}), 
            null);

        yield return new WaitForSeconds(.1f);

        //  allows the player to place house
        FindObjectOfType<PlayerInstance>().setCanMove(true);
        GameInfo.unlockBuyable(Buyable.buyableTitle.House);
        FindObjectOfType<PregameCanvas>().setup();
    }

    IEnumerator weaponSetup() {
        FindObjectOfType<DialogCanvas>().loadDialogText(new DialogText(
            new List<string>() { "Mediocre placement", "Anyways... ", "Here, take this axe" },
            new List<DialogText.facialExpression>() { DialogText.facialExpression.normal, DialogText.facialExpression.dismissive, DialogText.facialExpression.normal}),
            delegate {
            p = true;
        });
        yield return new WaitForSeconds(.1f);
        while(!p)
            yield return new WaitForEndOfFrame();
        playerWeaponSr.sprite = weaponSprite;
        FindObjectOfType<PlayerWeaponInstance>().enabled = true;
        FindObjectOfType<PlayerWeaponInstance>().variant.canMove = true;
        FindObjectOfType<PlayerWeaponInstance>().canAttackG = false;
        FindObjectOfType<PlayerInstance>().setCanMove(true);
        FindObjectOfType<PlayerWeaponInstance>().GetComponentInChildren<TrailRenderer>().enabled = true;

        yield return new WaitForSeconds(1.0f);

        FindObjectOfType<PlayerWeaponInstance>().variant.canMove = false;
        FindObjectOfType<PlayerInstance>().setCanMove(false);

        FindObjectOfType<DialogCanvas>().loadDialogText(new DialogText(
            new List<string>() { "Great! You're ready for the game", "I guess...", "...", "Press this button when you're ready to start" }, 
            new List<DialogText.facialExpression>() { DialogText.facialExpression.happy, DialogText.facialExpression.dismissive, DialogText.facialExpression.thinking, DialogText.facialExpression.normal}),
            delegate {
            StartCoroutine(endSetup());
        });
    }

    IEnumerator endSetup() {
        yield return new WaitForSeconds(.25f);
        FindObjectOfType<PregameCanvas>().startButton.SetActive(true);
        FindObjectOfType<PregameCanvas>().setup();
        FindObjectOfType<GameTutorialCanvas>().show();
        FindObjectOfType<GameBoard>().saveBoard();
        yield return new WaitForSeconds(1.0f);
        FindObjectOfType<PlayerWeaponInstance>().variant.canMove = true;
        FindObjectOfType<PlayerWeaponInstance>().canAttackG = true;
        FindObjectOfType<PlayerInstance>().setCanMove(true);
        enabled = false;
    }
}
