using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class HouseInstance : StructureInstance {
    [HideInInspector]
    [SerializeField] public float viewDist = 80f;
    [SerializeField] GameObject arrow;
    [SerializeField] public CircleCollider2D radiusCol;
    [SerializeField] public GameObject playerSpawnPos;
    [SerializeField] List<GameObject> corners = new List<GameObject>();
    KdTree<Transform> cs = new KdTree<Transform>();
    [SerializeField] Light2D hLight;

    private void OnCollisionStay2D(Collision2D col) {
        if(col.gameObject.tag == "DeadGuy")
            Destroy(col.gameObject);
    }

    private void Start() {
        FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(transform.position, 10f);
        FindObjectOfType<DeadGuyHolder>().clearDeadGuysWithinRadius(transform.position, 10f);
        FindObjectOfType<MonsterSpawner>().transform.position = getCenter();
        FindObjectOfType<PlayerInstance>().hCenter = getCenter();
        FindObjectOfType<GameBoard>().structures.Add(this);
        foreach(var i in FindObjectsOfType<Mortal>())
            i.hi = this;

        //  sets up the house based on the saves in GameInfo
        var stats = GameInfo.getHouseStats();
        //  health
        maxHealth = stats.houseMaxHealth;
        health = stats.houseHealth;

        //  Light
        hLight.size = stats.houseLightRad;
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
    public bool alreadyGoingTowardsPathPoint(Vector2 target) {
        foreach(var i in corners) {
            if(Vector2.Distance(target, i.transform.position) < .001f)
                return true;
        }
        return false;
    }

    public void saveCurrentHouseStats() {
        GameInfo.setHouseStats(new HouseStats(health, maxHealth, hLight.size));
    }

    public override void aoeEffect(GameObject effected) {
        //  does nothing
    }
    public override void aoeLeaveEffect(GameObject effected) {
        //  does nothing
    }

    public override void die() {
        if(isDead)
            return;
        isDead = true;
        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        FindObjectOfType<MonsterSpawner>().enabled = false;
        foreach(var i in FindObjectsOfType<MonsterInstance>())
            i.enabled = false;
        enabled = false;
    }
}
