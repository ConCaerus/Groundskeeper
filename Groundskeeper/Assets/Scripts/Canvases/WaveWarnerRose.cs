using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class WaveWarnerRose : MonoBehaviour {
    [SerializeField] GameObject[] dots; //  0 - north, 1 - east, 2 - south, 3 - west
    [SerializeField] GameObject warning;
    [SerializeField] CircularSlider timer;

    [SerializeField] Color[] timerColors;

    Coroutine[] dAnim = { null, null, null, null };

    private void Start() {
        foreach(var i in dots)
            i.transform.localScale = Vector3.zero;
        warning.transform.localScale = Vector3.zero;
    }

    public void hide() {
        foreach(var i in dAnim) {
            if(i != null)
                StopCoroutine(i);
        }

        timer.doValueKill();

        float disTime = .15f;
        foreach(var i in dots) {
            i.transform.DOKill();
            i.transform.DOScale(0f, disTime);
        }

        timer.setValue(1.0f);
        timer.resetColor();
    }

    public void warn(MonsterSpawner.direction[] dir, float timeTillNextWave) {
        foreach(var i in dAnim) {
            if(i != null)
                StopCoroutine(i);
        }

        bool[] seen = { false, false, false, false };

        timer.setValue(1.0f);
        timer.doValue(0.0f, timeTillNextWave, true, delegate { FindObjectOfType<MonsterSpawner>().startNewWave(); });

        timer.resetColor();
        timer.setColor(timerColors[0]);
        timer.doColor(timerColors[1], timeTillNextWave);

        float disTime = .15f;
        foreach(var i in dots) {
            i.transform.DOKill();
            i.transform.DOScale(0f, disTime);
        }


        //  shows any lingering monsters' incoming direction
        foreach(var i in FindObjectsOfType<MonsterInstance>()) {
            int ind = (int)i.direction - 1;
            if(seen[ind] || i.health <= 0.0f || i.isDead)
                continue;
            dAnim[ind] = StartCoroutine(dotAnim(dots[ind], 1.25f));
            seen[ind] = true;
        }

        //  shows the new, incoming wave direction
        foreach(var i in dir) {
            int ind = (int)i - 1;
            if(seen[ind])
                continue;
            dAnim[ind] = StartCoroutine(dotAnim(dots[ind], 1f));
            seen[ind] = true;
        }

        StartCoroutine(attentionGrabber());
    }


    public void updateForDirection(MonsterSpawner.direction dir) {
        int ind = (int)dir - 1;
        if(dAnim[ind] == null)
            return;

        foreach(var i in FindObjectOfType<GameBoard>().monsters) {
            if(i.direction == dir)
                return;
        }

        StopCoroutine(dAnim[ind]);
    }


    IEnumerator dotAnim(GameObject d, float mult) {
        yield return new WaitForSeconds(1.25f);

        float sizeTime = .15f;
        while(true) {
            //  grow big
            d.transform.DOScale(1.0f * mult, sizeTime);
            yield return new WaitForSeconds(sizeTime * 2.0f);

            //  shrink small
            d.transform.DOScale(.75f * mult, sizeTime);
            yield return new WaitForSeconds(sizeTime * 2.0f);
        }
    }

    IEnumerator attentionGrabber() {
        transform.GetChild(0).DOScale(1.5f, .15f);
        warning.transform.DOScale(.65f, .15f);

        yield return new WaitForSeconds(.15f);

        yield return new WaitForSeconds(1f);

        transform.GetChild(0).DOScale(1.0f, .25f);
        warning.transform.DOScale(0.0f, .25f);
    }
}
