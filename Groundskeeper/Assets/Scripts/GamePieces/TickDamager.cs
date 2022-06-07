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

        public TickInfo(GameObject m, action ac, float btwTime, int endAfterCount = -1) {
            obj = m;
            tickAction = ac;
            timeBtwTicks = btwTime;
            determinedTickCount = endAfterCount;
        }
    }

    List<TickInfo> ticks = new List<TickInfo>();

    public void addTick(TickInfo info) {
        ticks.Add(info);
        ticks[ticks.Count - 1].ticker = StartCoroutine(tick(ticks[ticks.Count - 1]));
    }
    public void removeTick(GameObject m) {
        foreach(var i in ticks) {
            if(m == i.obj) {
                StopCoroutine(i.ticker);
                ticks.Remove(i);
                return;
            }
        }
    }
    public void removeTick(int index) {
        ticks.RemoveAt(index);
    }

    public bool contains(GameObject obj) {
        foreach(var i in ticks) {
            if(i.obj == obj)
                return true;
        }
        return false;
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
