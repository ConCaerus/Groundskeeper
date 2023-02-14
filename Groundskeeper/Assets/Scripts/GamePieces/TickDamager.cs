using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickDamager : MonoBehaviour {
    public delegate void action(GameObject m);
    public class TickInfo {
        public GameObject obj;
        public action tickAction;
        public float timeBtwTicks;
        public int determinedTickCount; // set to -1 for no determined tick count
        public Coroutine ticker = null;
        public Buyable.buyableTitle effect;

        public TickInfo(GameObject m, action ac, float btwTime, Buyable.buyableTitle t, int endAfterCount = -1) {
            obj = m;
            tickAction = ac;
            timeBtwTicks = btwTime;
            determinedTickCount = endAfterCount;
            effect = t;
        }

        public void updateInfo(action ac, float btwTime, Buyable.buyableTitle t, int endAfterCount = -1) {
            tickAction = ac;
            timeBtwTicks = btwTime;
            determinedTickCount = endAfterCount;
            effect = t;
        }
    }

    List<TickInfo> ticks = new List<TickInfo>();

    public void addTick(GameObject m, DefenceInstance def) {
        int index = getTickIndex(m);
        //  create a tick for a new object
        if(index == -1) {
            var t = new TickInfo(m, def.dealDamage, def.btwHitTime, def.GetComponent<Buyable>().title, def.tickCount);
            ticks.Add(t);
            StartCoroutine(tick(ticks[ticks.Count - 1]));
        }

        //  increment the touching count of the already ticking object
        else {
            ticks[index].updateInfo(def.dealDamage, def.btwHitTime, def.GetComponent<Buyable>().title, def.tickCount);
        }

        m.GetComponent<MonsterInstance>().affectedMoveAmt = def.slowAmt;
    }
    public void removeTick(GameObject m) {
        int index = getTickIndex(m);

        if(index == -1)
            return;
        else {
            ticks.RemoveAt(index);
            m.GetComponent<MonsterInstance>().affectedMoveAmt = 0f;
        }
    }
    public void removeTick(int index) {
        ticks.RemoveAt(index);
    }

    public int getTickIndex(GameObject m) {
        for(int i = 0; i < ticks.Count; i++) {
            if(ticks[i].obj.GetInstanceID() == m.GetInstanceID())
                return i;
        }
        return -1;
    }
    public TickInfo getTickInfo(GameObject m) {
        int ind = getTickIndex(m);
        return ind == -1 ? null : ticks[ind];
    }

    public IEnumerator tick(TickInfo info) {
        info.tickAction(info.obj);

        if(info.determinedTickCount == 0)
            yield break;

        if(info.determinedTickCount > -1)
            info.determinedTickCount--;

        yield return new WaitForSeconds(info.timeBtwTicks);

        if(info.obj != null && ticks.Contains(info))
            info.ticker = StartCoroutine(tick(info));
        else 
            ticks.Remove(info);
    }
}
