using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {
    List<GameObject> monsterPresets = new List<GameObject>();

    float rad = 150f;
    float maxSpread = 80f;

    public bool spawning { get; set; } = false;
    //  Types of monsters<leaders of those types of monsters<followers of the leaders of those types of monsters>>
    List<List<List<MonsterInstance>>> monsterGroups = new List<List<List<MonsterInstance>>>();

    public enum direction {
        All, North, East, South, West
    }

    class waveInfo {
        public List<int> enemyNumbers;
        public direction dir;

        public waveInfo() {
            enemyNumbers = new List<int>();
            for(int i = 0; i <= GameInfo.getLastSeenEnemyIndex(); i++) {
                enemyNumbers.Add(0);
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Environment")
            col.gameObject.GetComponent<EnvironmentInstance>().turnOffCol();
    }

    private void Awake() {
        monsterPresets.Clear();
        for(int i = 0; i < FindObjectOfType<PresetLibrary>().getMonsterCount(); i++)
            monsterPresets.Add(FindObjectOfType<PresetLibrary>().getMonster(i));
    }


    private void Update() {
        if(spawning && GameObject.FindGameObjectsWithTag("Monster").Length == 0) {
            GameInfo.wave++;
            if(GameInfo.wave > GameInfo.wavesPerNight()) {
                FindObjectOfType<GameUICanvas>().addSoulsToBank();
                FindObjectOfType<NightOverCanvas>().show();
                FindObjectOfType<HouseInstance>().showDoorArrow();
                enabled = false;
            }
            FindObjectOfType<GameUICanvas>().updateCount();
            startNewWave();
        }
    }



    void startNewWave() {
        //  check if a new enemy should be seen
        while(GameInfo.getLastSeenEnemyIndex() < monsterPresets.Count - 1 && monsterPresets[GameInfo.getLastSeenEnemyIndex() + 1].GetComponent<Monster>().earliestNight <= GameInfo.getNightCount()) {
            GameInfo.incLastSeenEnemy();
        }

        //  choose a new direction for the wave

        StartCoroutine(spawnMonsters(calcWave()));
    }

    IEnumerator spawnMonsters(waveInfo info) {
        FindObjectOfType<GameUICanvas>().warnForWave(info.dir);
        FindObjectOfType<GameBoard>().monsters.Clear();
        monsterGroups.Clear();

        for(int i = 0; i <= GameInfo.getLastSeenEnemyIndex(); i++) {
            monsterGroups.Add(new List<List<MonsterInstance>>());
        }

        float minDistToLight = 25f;
        int numOfLeaders = 5;

        for(int i = 0; i < info.enemyNumbers.Count; i++) {  //  loop through the types of monsters
            KdTree<MonsterInstance> leaders = new KdTree<MonsterInstance>();
            for(int j = 0; j <= info.enemyNumbers[i]; j++) {    //  spawn the number of that type of monster
                var temp = Instantiate(monsterPresets[i].gameObject, transform);

                bool tooClose = true;
                Vector3 pos = j < numOfLeaders ? getPosAlongCircle(transform.position, rad, info.dir, (maxSpread / (numOfLeaders - i)) - (maxSpread / 2f)) : getRandomPosAlongCircle(transform.position, rad, info.dir);
                int layer = LayerMask.GetMask("Light");
                while(tooClose) {
                    pos = getRandomPosAlongCircle(transform.position, rad, info.dir);
                    tooClose = false;
                    RaycastHit2D hit = Physics2D.Raycast(pos, -pos, Vector2.Distance(pos, Vector2.zero), layer);

                    if(hit.collider != null) {
                        //  immideitly hit
                        if(hit.point == (Vector2)pos) {
                            rad += 25f;
                            tooClose = true;
                        }

                        //  get the deets from the triangle formed between the hit.point and the pos
                        var dist = Vector2.Distance(pos, hit.point);
                        var xOff = hit.point.x - pos.x;
                        var theta = Mathf.Acos(xOff / dist);


                        //  find the point along the line that is the minDist away from the hit
                        var d = dist - minDistToLight;
                        var x = d * Mathf.Cos(theta);
                        var y = d * Mathf.Sin(theta);
                        y *= pos.y > 0.0f ? -1f : 1f;

                        /*
                        Debug.DrawLine(hit.point, pos, Color.white, 100f);
                        Debug.DrawLine(pos + new Vector3(x, y), pos, Color.green, 100f);
                        Debug.DrawLine(pos, new Vector2(pos.x + xOff, pos.y), Color.white, 100f);
                        Debug.DrawLine(hit.point, new Vector2(pos.x + xOff, pos.y), Color.cyan, 100f);
                        */
                        pos += new Vector3(x, y);
                    }
                }

                temp.transform.position = pos;
                if(j < numOfLeaders) {
                    temp.GetComponent<MonsterInstance>().setAsLeader();

                    //  create an new grouping under the monster type and set the first monster in that list as this leader
                    monsterGroups[(int)temp.GetComponent<MonsterInstance>().mType].Add(new List<MonsterInstance>());
                    monsterGroups[(int)temp.GetComponent<MonsterInstance>().mType][monsterGroups[(int)temp.GetComponent<MonsterInstance>().mType].Count - 1].Add(temp.GetComponent<MonsterInstance>());
                    leaders.Add(temp.GetComponent<MonsterInstance>());
                }
                else {
                    monsterGroups[(int)temp.GetComponent<MonsterInstance>().mType][leaders.ToList().IndexOf(leaders.FindClosest(temp.transform.position))].Add(temp.GetComponent<MonsterInstance>());
                    temp.GetComponent<MonsterInstance>().closestLeader = monsterGroups[(int)temp.GetComponent<MonsterInstance>().mType][leaders.ToList().IndexOf(leaders.FindClosest(temp.transform.position))][0];
                }
                FindObjectOfType<GameBoard>().monsters.Add(temp.GetComponent<MonsterInstance>());
                yield return new WaitForSeconds(Random.Range(0f, .2f));
            }
        }
    }

    public void passOnLeadership(MonsterInstance l) {
        foreach(var i in monsterGroups[(int)l.GetComponent<MonsterInstance>().mType]) {
            if(i[0] == l) { //  found the right list
                //  checks if it's the last one in the group
                if(i.Count <= 1) {
                    monsterGroups[(int)l.GetComponent<MonsterInstance>().mType].Remove(i);
                    return;
                }

                //  pass on leadership to the next monster in line
                else {
                    i.RemoveAt(0);
                    i[0].GetComponent<MonsterInstance>().setAsLeader();
                    foreach(var m in i)
                        m.closestLeader = i[0];
                }
                return;
            }
        }
    }

    public void removeMonsterFromGroup(MonsterInstance m) {
        foreach(var i in monsterGroups[(int)m.GetComponent<MonsterInstance>().mType]) {
            if(i.Contains(m)) {
                i.Remove(m);
                return;
            }
        }
    }

    Vector3 getRandomPosAlongCircle(Vector3 center, float radius, direction dir) {
        return getPosAlongCircle(center, radius, dir, Random.Range(maxSpread / 2f, maxSpread));
    }

    Vector3 getPosAlongCircle(Vector3 center, float radius, direction dir, float spread) {
        float ang = 0.0f;
        if(dir == direction.North)
            ang = Random.value * spread - 45;
        else if(dir == direction.East)
            ang = Random.value * spread + 45;
        else if(dir == direction.South)
            ang = Random.value * spread + 45 + 90;
        else if(dir == direction.West)
            ang = Random.value * spread + 45 + 180;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }


    waveInfo calcWave() {
        waveInfo info = new waveInfo();
        var diff = calcWaveDiff();

        while(diff > 0) {
            info.enemyNumbers[0]++;
            diff -= monsterPresets[0].GetComponent<Monster>().diff;
        }

        info.dir = (direction)Random.Range(1, 5);

        return info;
    }

    int calcWaveDiff() {
        int diff = GameInfo.wave + 1;
        return diff * 10 + (diff / 10) * 10;
    }


    public void sortMonsters() {
        var temp = new List<GameObject>();
        foreach(var i in monsterPresets)
            temp.Add(i);
        monsterPresets.Clear();

        while(temp.Count > 0) {
            var lowest = -1;
            GameObject t = null;

            foreach(var m in temp) {
                if(lowest == -1 || t.GetComponent<Monster>().earliestNight > m.GetComponent<Monster>().earliestNight) {
                    lowest = m.GetComponent<Monster>().earliestNight;
                    t = m;
                }
            }

            monsterPresets.Add(t);
            temp.Remove(t);
        }
    }
}
