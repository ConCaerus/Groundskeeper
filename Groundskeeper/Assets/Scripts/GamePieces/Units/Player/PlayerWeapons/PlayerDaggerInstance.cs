using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDaggerInstance : PlayerWeaponVariant {
    GameTutorialCanvas gtc;
    TransitionCanvas tc;

    public override void variantSetup() {
        gtc = FindObjectOfType<GameTutorialCanvas>();
        tc = FindObjectOfType<TransitionCanvas>();
    }

    public override void performOnAttack() {
        wAnimator.SetTrigger("windup");
    }

    public override void performOnAttackEnd() {
        if(tc.finishedLoading) {
            if(gtc != null) {
                gtc.hasAttacked();
                if(pi.weaponAttackMod > 1.01f)
                    gtc.hasChargedAttacked();
            }
            attack(GameInfo.mousePos(), Vector2.zero, pi.weaponAttackMod);
            pi.weaponAttackMod = 1.0f;
        }
    }

    public override void shootMonster() {
    }
    public override void lobToMonster() {
    }
}
