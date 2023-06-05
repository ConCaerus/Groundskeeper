using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Defense : Mortal {
    public int dmgAmt;
    public float slowAmt;
    public float btwHitTime = 1f;
    public int tickCount = -1; //  shit like poison that could deal 5 ticks after instance
    public GameInfo.MonsterType target;
    public float soulStealMod = 0.0f;   //  mult of how many souls get taken (multed to the monster's soul drop)
}
