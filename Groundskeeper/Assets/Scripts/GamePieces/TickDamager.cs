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
        public int touchingCount = 1;

        public TickInfo(GameObject m, action ac, float btwTime, int endAfterCount = -1) {
            obj = m;
            tickAction = ac;
            timeBtwTicks = btwTime;
            determinedTickCount = endAfterCount;
        }

        public void updateInfo(action ac, float btwTime, int endAfterCount = -1) {
            tickAction = ac;
            timeBtwTicks = btwTime;
            determinedTickCount = endAfterCount;
        }
    }

    List<TickInfo> ticks = new List<TickInfo>();

    public void addTick(GameObject m, DefenceInstance def) {
        int index = getTickIndex(m);
        //  create a tick for a new object
        if(index == -1) {
            var t = new TickInfo(m, def.dealDamage, def.btwHitTime, def.tickCount);
            ticks.Add(t);
            StartCoroutine(tick(ticks[ticks.Count - 1]));
        }

        //  increment the touching count of the already ticking object
        else {
            ticks[index].updateInfo(def.dealDamage, def.btwHitTime, def.tickCount);
            ticks[index].touchingCount++;
        }

        m.GetComponent<MonsterInstance>().affectedMoveAmt = def.slowAmt;
    }
    public void removeTick(GameObject m) {
        int index = getTickIndex(m);

        if(index == -1)
            return;
        else {
            ticks[index].touchingCount--;
            if(ticks[index].touchingCount == 0) {
                ticks.RemoveAt(index);
                m.GetComponent<MonsterInstance>().affectedMoveAmt = 0f;
            }
        }
    }
    public void removeTick(int index) {
        ticks.RemoveAt(index);
    }

    public int getTickIndex(GameObject m) {
        for(int i = 0; i < ticks.Count; i++) {
            if(ticks[i].obj == m)
                return i;
        }
        return -1;
    }

    public IEnumerator tick(TickInfo info) {
        info.tickAction(info.obj);

        if(info.determinedTickCount == 0)
            yield break;

        if(info.determinedTickCount > -1)
            info.determinedTickCount--;

        yield return new WaitForSeconds(info.timeBtwTicks);

        if(info.obj != null)
            info.ticker = StartCoroutine(tick(info));
        else 
            ticks.Remove(info);
    }
}
