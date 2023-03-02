using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HouseInstance : Building {
    [HideInInspector]
    [SerializeField] public float viewDist = 80f;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject bloodParticles;
    [SerializeField] public CircleCollider2D radiusCol;
    [SerializeField] public GameObject playerSpawnPos;
    [SerializeField] List<GameObject> corners = new List<GameObject>();
    KdTree<Transform> cs = new KdTree<Transform>();

    private void Start() {
        //FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(transform.position, 10f);
        //FindObjectOfType<MonsterSpawner>().transform.position = getCenter();
        //FindObjectOfType<PlayerInstance>().hCenter = getCenter();
        //foreach(var i in FindObjectsOfType<Mortal>())
            //i.hi = this;
    }

    public Vector2 getCenter() {
        return transform.position;
    }

    public void showDoorArrow() {
        arrow.SetActive(true);
        var or = arrow.transform.localScale;
        arrow.transform.localScale = Vector3.zero;
        arrow.transform.DOScale(or, .25f);
        StartCoroutine(doorArrowAnim(.25f));
    }

    IEnumerator doorArrowAnim(float waitForArrowToShowTime = 0.0f) {
        yield return new WaitForSeconds(waitForArrowToShowTime);
        float t = .5f, m = 1.5f;
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y - m, t);
        yield return new WaitForSeconds(t);
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y + m, t);
        yield return new WaitForSeconds(t);
        StartCoroutine(doorArrowAnim());
    }

    public Vector2 getNextPointOnPath(Vector2 pos, Vector2 target) {
        var dir = target - pos;
        int lm = LayerMask.GetMask("House");
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, Vector2.Distance(pos, target), lm);
        if(cs.Count == 0) {
            foreach(var i in corners)
                cs.Add(i.transform);
        }

        //  path is obstructed
        if(hit.collider != null) {
            //  get the midpoint between the two points and find the closest corner to that point
            var x = (pos.x + target.x) / 2.0f;
            var y = (pos.y + target.y) / 2.0f;
            return cs.FindClosest(new Vector3(x, y)).position;
        }

        //  nothing in the way
        return target;
    }

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        Destroy(gameObject);
    }
}
