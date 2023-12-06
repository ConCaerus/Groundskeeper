using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {
    List<GameObject> monsterPresets = new List<GameObject>();

    float rad = 75f;
    float maxSpread = 80f;

    PresetLibrary pl;

    float maxTimeBtwWaves() {
        float reg = 60f * 2f;
        //  adds one extra min for each extra direciton
        for(int i = 0; i < 3; i++) {
            if(nightsBetweenDirAddition * i < GameInfo.getNightCount())
                reg += 60f;
        }
        return reg;
    }

    [SerializeField] int nightsBetweenDirAddition;  //  monsters can spawn from 1 additional direction after this number of nights
    bool gameEnded = false;

    waveInfo curWave;
    //  Wave that the monsters belong to <Types of monsters <leaders of those types of monsters <followers of the leaders of those types of monsters>>
    List<List<List<List<MonsterInstance>>>> monsterGroups = new List<List<List<List<MonsterInstance>>>>();

    public enum frequency {
        Frequent, Average, Scarce
    }

    public enum position {
        First, Middle, Last
    }

    public enum direction {
        All, North, East, South, West
    }

    public class waveInfo {
        public List<int> enemyNumbers; //  list of how many of each monster are in the wave. [0, 3] could mean 0 zombies and 3 vampires
        public direction[] dir;
        public int diff;

        public waveInfo(PresetLibrary pl) {
            enemyNumbers = new List<int>();
            for(int i = 0; i < pl.getAvailableMonsters().Count; i++) {
                enemyNumbers.Add(0);    //  sets them all to zero to start with
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.tag == "Environment")
            col.gameObject.GetComponent<EnvironmentInstance>().turnOffCol();
    }

    private void Awake() {
        pl = FindObjectOfType<PresetLibrary>();
        GameInfo.updateLastSeenEnemyIndex(pl);

        //  adds the lists for each wave of the night
        for(int i = 0; i < GameInfo.wavesPerNight() + 1; i++)
            monsterGroups.Add(new List<List<List<MonsterInstance>>>());

        //  adds all of the seen enemies to the monster preset
        monsterPresets.Clear();
        monsterPresets = pl.getAvailableMonsters();
        //  orders the list based on the positions the monsters spawn in
        monsterPresets = monsterPresets.OrderBy(o => o.GetComponent<Monster>().posInWave).ToList();
    }

    public void endGame() {
        gameEnded = true;
        StopAllCoroutines();
        FindObjectOfType<GameUICanvas>().endGame();
        FindObjectOfType<HouseInstance>().showDoorArrow();
        FindObjectOfType<HouseDoorInteractable>().isTheEnd = true;
        enabled = false;
    }


    public waveInfo calcWave() {
        waveInfo info = new waveInfo(pl);
        var diff = calcWaveDiff();
        info.diff = diff;

        //  gets a list of all the possible monsters that could appear in the wave
        List<GameObject> pool = new List<GameObject>();
        int addNum = 0;
        foreach(var i in monsterPresets) {
            //  adds more instances of the monster type based on their set frequency in waves
            switch(i.GetComponent<Monster>().freqInWave) {
                case frequency.Frequent: addNum = 3; break;
                case frequency.Average: addNum = 2; break;
                case frequency.Scarce: addNum = 1; break;
            }
            for(int j = 0; j < addNum; j++) {
                pool.Add(i);
            }
        }

        //  calcs the monsters that will appear in the wave
        while(diff > 0) {
            var m = getRandomMonsterWithinDiff(diff, pool);
            info.enemyNumbers[monsterPresets.IndexOf(m)]++;
            diff -= m.GetComponent<Monster>().diff;
        }

        //  orders the list of monsters in the wave based on how strong and how fast they are

        if(GameInfo.getNightCount() < nightsBetweenDirAddition * 1)
            info.dir = new direction[] { (direction)Random.Range(1, 5) };
        else if(GameInfo.getNightCount() < nightsBetweenDirAddition * 2)
            info.dir = new direction[] { (direction)Random.Range(1, 5), (direction)Random.Range(1, 5) };
        else if(GameInfo.getNightCount() < nightsBetweenDirAddition * 3)
            info.dir = new direction[] { (direction)Random.Range(1, 5), (direction)Random.Range(1, 5), (direction)Random.Range(1, 5) };
        else if(GameInfo.getNightCount() < nightsBetweenDirAddition * 4)
            info.dir = new direction[] { (direction)Random.Range(1, 5), (direction)Random.Range(1, 5), (direction)Random.Range(1, 5), (direction)Random.Range(1, 5) };

        return info;
    }
    GameObject getRandomMonsterWithinDiff(int diff, List<GameObject> pool) {
        GameObject temp = null;
        while(temp == null || temp.GetComponent<Monster>().diff > diff) {
            //  checks if there aren't any monsters left in the pool
            if(pool.Count == 0)
                return null;

            //  if the prev chosen monster didn't work, remove it from the pool
            if(temp != null)
                pool.Remove(temp);

            //  pick out a different monster
            temp = pool[Random.Range(0, pool.Count)];
        }

        //  if the current monster passes, return it
        return temp;
    }

    int calcWaveDiff() {
        int waveMod = GameInfo.wave; //  already is 1 on first wave

        //  greatly increases the diff of the last wave of the night
        if(waveMod == GameInfo.wavesPerNight())
            waveMod =(int)(waveMod * 1.5f);
        int nightMod = GameInfo.getNightCount() + 1;
        int nightModMod = 3;
        return (int)(waveMod * 4f) + (int)(nightMod * nightModMod);
    }


    public void startNewWave() {
        if(gameEnded) return;

        GameInfo.wave++;
        FindObjectOfType<GameUICanvas>().updateCount();
        //  adds another monster group
        curWave = calcWave();
        //  choose a new direction for the wave
        StartCoroutine(spawnMonsters(curWave));
    }

    IEnumerator spawnMonsters(waveInfo info) {
        yield return new WaitForEndOfFrame();
        FindObjectOfType<PlayerWeaponInstance>().canAttackG = true;
        FindObjectOfType<WaveWarnerRose>().warn(info.dir, maxTimeBtwWaves());
        for(int i = 0; i < monsterPresets.Count(); i++) {
            monsterGroups[GameInfo.wave].Add(new List<List<MonsterInstance>>());
        }

        position prevPos = position.First;

        float minDistToLight = 50f; //  CHANGE THIS TO CHANGE HOW FAR MONSTERS SPAWN FROM HOUSE
        int numOfLeaders = 5;

        //  decreases the number of leaders based on how many groups of monsters there will be
        int t = info.enemyNumbers.Count / 2;
        numOfLeaders = Mathf.Clamp(numOfLeaders - t, 1, 5);
        //  loop through the number of directions the monsters can spawn from
        for(int l = 0; l < info.dir.Length; l++) {

            //  loops through the different types of monsters
            for(int i = 0; i < info.enemyNumbers.Count; i++) {
                KdTree<MonsterInstance> leaders = new KdTree<MonsterInstance>();    //  needs the empty list to hold the spot for the empty monsters
                //  if the waves doesn't have any monsters of this type, move on
                if(info.enemyNumbers[i] == 0)
                    continue;

                //  spawns the number of monsters
                for(int j = 0; j <= (info.enemyNumbers[i] - 1) / info.dir.Length; j++) {
                    //  checks if the monster has a different wave position than the previous
                    //  if so, wait a bit
                    if(monsterPresets[i].GetComponent<Monster>().posInWave != prevPos) {
                        prevPos = monsterPresets[i].GetComponent<Monster>().posInWave;
                        //yield return new WaitForSeconds(timeBtwPositions);
                    }

                    //  spawns the monster
                    var temp = Instantiate(monsterPresets[i].gameObject, transform);
                    var tmi = temp.GetComponent<MonsterInstance>();
                    if(tmi == null || temp == null) {
                        Debug.Log("???");
                        continue;
                    }

                    //  finds a position for the monster to have
                    bool tooClose = true;
                    Vector3 pos = j < numOfLeaders ? getPosAlongCircle(transform.position, rad, info.dir[l], (maxSpread / (numOfLeaders - i)) - (maxSpread / 2f)) : getRandomPosAlongCircle(transform.position, rad, info.dir[l]);
                    pos += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)); //  adds random offset to the monster's pos
                    int layer = LayerMask.GetMask("Light");
                    while(tooClose) {
                        pos = getRandomPosAlongCircle(transform.position, rad, info.dir[l]);
                        tooClose = false;
                        RaycastHit2D hit = Physics2D.Raycast(pos, -pos, Vector2.Distance(pos, transform.position), layer);

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

                    //  desides if this monster should be a leader
                    if(j < numOfLeaders || leaders.Count == 0 || monsterGroups[GameInfo.wave][i][leaders.ToList().IndexOf(leaders.FindClosest(temp.transform.position))][0] == null) {
                        tmi.setAsLeader();

                        //  can't use enumerators as indexes you fucking idiot
                        //  create an new grouping under the monster type and set the first monster in that list as this leader
                        monsterGroups[GameInfo.wave][i].Add(new List<MonsterInstance>());
                        monsterGroups[GameInfo.wave][i][monsterGroups[GameInfo.wave][i].Count - 1].Add(tmi);
                        leaders.Add(tmi);
                    }
                    else {
                        monsterGroups[GameInfo.wave][i][leaders.ToList().IndexOf(leaders.FindClosest(temp.transform.position))].Add(tmi);
                        tmi.closestLeader = monsterGroups[GameInfo.wave][i][leaders.ToList().IndexOf(leaders.FindClosest(temp.transform.position))][0];
                    }

                    //  misc shisc
                    tmi.direction = info.dir[l];
                    tmi.relevantWave = GameInfo.wave;
                    tmi.setup();
                    FindObjectOfType<GameBoard>().monsters.Add(tmi);

                    //  wait a random amount of time before spawning the next monster
                    yield return new WaitForSeconds(Random.Range(0f, .2f));
                }
            }
        }
    }

    public void passOnLeadership(MonsterInstance l, int relevantWave) {
        foreach(var i in monsterGroups[relevantWave][(int)l.GetComponent<MonsterInstance>().title]) {
            if(i[0] == l) { //  found the right list
                //  checks if it's the last one in the group
                if(i.Count <= 1) {
                    monsterGroups[relevantWave][(int)l.GetComponent<MonsterInstance>().title].Remove(i);
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

    public void removeMonsterFromGroup(MonsterInstance m, int relevantWave) {
        foreach(var i in monsterGroups[relevantWave][(int)m.GetComponent<MonsterInstance>().title]) {
            if(i.Contains(m)) {
                i.Remove(m);
                return;
            }
        }
    }

    public bool stillHasMonstersFromWave(int waveInd) {
        foreach(var i in monsterGroups[waveInd]) {
            foreach(var j in i) {
                if(j.Count > 0)
                    return true;
            }
        }
        return false;
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

    public List<MonsterInstance> getAllFollowingMonstersForLeader(MonsterInstance leader) {
        var temp = new List<MonsterInstance>();

        foreach(var i in monsterGroups[leader.relevantWave][(int)leader.title]) {
            if(i[0] == leader) {
                for(int x = 1; x < i.Count; x++)
                    temp.Add(i[x]);
            }
        }
        return temp;
    }
}
