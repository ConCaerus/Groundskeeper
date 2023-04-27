using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentManager : MonoBehaviour {
    [SerializeField] GameObject holder;
    GameObject curHolder = null;
    string curReference = null;
    int envCountPerHolder = 50;
    int numToNextHolder;

    List<GameObject> holders = new List<GameObject>();

    GameBoard gb;

    private void Awake() {
        gb = FindObjectOfType<GameBoard>();
    }

    public GameObject spawnEnv(GameObject env, Vector2 pos) {
        if(curHolder == null || numToNextHolder <= 0 || env.GetComponent<EnvironmentInstance>().title != curReference) {
            curHolder = Instantiate(holder.gameObject, transform);
            holders.Add(curHolder);
            numToNextHolder = envCountPerHolder;
        }
        var o = Instantiate(env.gameObject, pos, Quaternion.identity, curHolder.transform);
        curReference = o.GetComponent<EnvironmentInstance>().title;

        if(o.GetComponent<Collider2D>() != null)
            FindObjectOfType<LayerSorter>().requestNewSortingLayer(o.GetComponent<Collider2D>(), o.transform.GetChild(0).GetComponent<SpriteRenderer>());
        else
            FindObjectOfType<LayerSorter>().requestNewSortingLayer(pos.y, o.transform.GetChild(0).GetComponent<SpriteRenderer>());

        numToNextHolder--;
        return o;
    }
    public void finishSpawning() {
        foreach(var i in holders) {
            if(i.transform.GetChild(0).GetComponent<EnvironmentInstance>().tit != EnvironmentInstance.triggerInteractionType.notTrigger) {
                i.GetComponent<CompositeCollider2D>().isTrigger = true;
                i.GetComponent<EnvironmentHolderCollider>().enabled = false;
            }
            else if(i.transform.GetChild(0).GetComponent<EnvironmentInstance>().tit == EnvironmentInstance.triggerInteractionType.none)
                i.GetComponent<EnvironmentHolderCollider>().enabled = false;
            i.GetComponent<CompositeCollider2D>().GenerateGeometry();
        }
        gb.loaded = true;
    }

    public void hideAllEnvAroundArea(Vector2 center, float rad) {
        StartCoroutine(waitAndHideAllEnvAroundArea(center, rad));
    }

    IEnumerator waitAndHideAllEnvAroundArea(Vector2 center, float rad) {
        while(!gb.loaded)
            yield return new WaitForEndOfFrame();
        KdTree<EnvironmentInstance> used = new KdTree<EnvironmentInstance>();
        var closest = gb.environment.FindClosest(center);
        while(Vector2.Distance(closest.transform.position, center) < rad) {
            closest.gameObject.GetComponent<EnvironmentInstance>().remove(false);
            used.Add(closest);
            closest = gb.environment.FindClosest(center);
        }

        //  regenerats all geometry
        foreach(var i in holders)
            i.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }

    public void hitEnvironment(Vector2 pos) {
        gb.environment.FindClosest(pos).takeHit();
    }


    void show(GameObject obj) {
        obj.transform.GetChild(0).transform.DOComplete();
        obj.transform.GetChild(0).transform.DOScale(1.0f, .15f);
        //obj.transform.GetChild(0).transform.DOLocalMoveY(transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y / 2.0f, .15f);
    }
}
