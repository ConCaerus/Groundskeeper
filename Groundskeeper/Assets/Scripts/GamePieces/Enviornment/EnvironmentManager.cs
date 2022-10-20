using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentManager : MonoBehaviour {
    [SerializeField] GameObject holder;
    GameObject curHolder = null;
    int envCountPerHolder = 100;
    int numToNextHolder;

    List<GameObject> holders = new List<GameObject>();


    public GameObject spawnEnv(GameObject env, Vector2 pos) {
        if(curHolder == null || numToNextHolder <= 0) {
            curHolder = Instantiate(holder.gameObject, transform);
            holders.Add(curHolder);
            numToNextHolder = envCountPerHolder;
        }
        var o = Instantiate(env.gameObject, pos, Quaternion.identity, curHolder.transform);

        if(o.GetComponent<Collider2D>() != null)
            FindObjectOfType<LayerSorter>().requestNewSortingLayer(o.GetComponent<Collider2D>(), o.transform.GetChild(0).GetComponent<SpriteRenderer>());
        else
            FindObjectOfType<LayerSorter>().requestNewSortingLayer(pos.y, o.transform.GetChild(0).GetComponent<SpriteRenderer>());

        numToNextHolder--;
        return o;
    }
    public void finishSpawning() {
        foreach(var i in holders)
            i.GetComponent<CompositeCollider2D>().GenerateGeometry();
    }


    public void showAllEnvAroundArea(Vector2 center, float rad) {
        KdTree<EnvironmentInstance> used = new KdTree<EnvironmentInstance>();
        var closest = FindObjectOfType<GameBoard>().environment.FindClosest(center);
        while(Vector2.Distance(closest.transform.position, center) < rad) {
            show(closest.gameObject);
            used.Add(closest);
            FindObjectOfType<GameBoard>().environment.RemoveAll(x => x.gameObject.GetInstanceID() == closest.gameObject.GetInstanceID());
            closest = FindObjectOfType<GameBoard>().environment.FindClosest(center);
        }

        foreach(var i in used)
            FindObjectOfType<GameBoard>().environment.Add(i);
    }

    public void hideAllEnvAroundArea(Vector2 center, float rad) {
        KdTree<EnvironmentInstance> used = new KdTree<EnvironmentInstance>();
        var closest = FindObjectOfType<GameBoard>().environment.FindClosest(center);
        while(Vector2.Distance(closest.transform.position, center) < rad) {
            hide(closest.gameObject);
            used.Add(closest);
            FindObjectOfType<GameBoard>().environment.RemoveAll(x => x.gameObject.GetInstanceID() == closest.gameObject.GetInstanceID());
            closest = FindObjectOfType<GameBoard>().environment.FindClosest(center);
        }

        foreach(var i in used)
            FindObjectOfType<GameBoard>().environment.Add(i);
    }

    public void hitEnvironment(Vector2 pos) {
        FindObjectOfType<GameBoard>().environment.FindClosest(pos).takeHit();
    }


    void show(GameObject obj) {
        obj.transform.GetChild(0).transform.DOComplete();
        obj.transform.GetChild(0).transform.DOScale(1.0f, .15f);
        //obj.transform.GetChild(0).transform.DOLocalMoveY(transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y / 2.0f, .15f);
    }

    void hide(GameObject obj) {
        obj.transform.GetChild(0).transform.DOComplete();
        obj.transform.GetChild(0).transform.DOScale(0.0f, .25f);
        //obj.transform.GetChild(0).transform.DOLocalMoveY(-transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.size.y / 2.0f, .25f);
    }
}
