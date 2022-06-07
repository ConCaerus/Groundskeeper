using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInstance : Building {

    public override void die() {
        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        Destroy(gameObject);
    }
}
