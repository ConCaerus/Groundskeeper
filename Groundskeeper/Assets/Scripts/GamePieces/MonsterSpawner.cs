using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour {
    [SerializeField] List<GameObject> monsters;

    public bool spawning { get; set; } = false;

    public enum direction {
        All, North, East, South, West
    }

    class waveInfo {
        public List<int> enemyNumbers;
        public direction dir;

        public waveInfo() {
            enemyNumbers = new List<int>();
            for(int i = 0; i <= GameInfo.lastSeenEnemyIndex; i++) {
                enemyNumbers.Add(0);
            }
        }
    }


    private void Update() {
        if(spawning && GameObject.FindGameObjectsWithTag("Monster").Length == 0) {
            GameInfo.wave++;
            FindObjectOfType<GameUICanvas>().updateCount();
            if(GameInfo.wave >= GameInfo.wavesPerNight()) {
                GameInfo.addCoins(GameInfo.calcCoins());
                FindObjectOfType<NightOverCanvas>().show();
                enabled = false;
            }
            startNewWave();
        }
    }



    void startNewWave() {
        //  check if a new enemy should be seen
        while(GameInfo.lastSeenEnemyIndex < monsters.Count - 1 && monsters[GameInfo.lastSeenEnemyIndex + 1].GetComponent<MonsterInstance>().earliestWave <= GameInfo.wave) {
            GameInfo.lastSeenEnemyIndex++;
        }

        //  choose a new direction for the wave

        StartCoroutine(spawnMonsters(calcWave()));
    }

    IEnumerator spawnMonsters(waveInfo info) {
        Debug.Log(info.dir);
        for(int i = 0; i < info.enemyNumbers.Count; i++) {
            for(int j = 0; j <= info.enemyNumbers[i]; j++) {
                var temp = Instantiate(monsters[i].gameObject, transform);
                temp.transform.position = getRandomPosAlongCircle(transform.position, 150f, info.dir);
                yield return new WaitForSeconds(Random.Range(0f, .2f));
            }
        }
    }

    Vector3 getRandomPosAlongCircle(Vector3 center, float radius, direction dir) {
        float spread = Random.Range(45f, 80f);
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
            diff -= monsters[0].GetComponent<MonsterInstance>().diff;
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
        foreach(var i in monsters)
            temp.Add(i);
        monsters.Clear();

        while(temp.Count > 0) {
            var lowest = -1;
            GameObject t = null;

            foreach(var m in temp) {
                if(lowest == -1 || t.GetComponent<MonsterInstance>().earliestWave > m.GetComponent<MonsterInstance>().earliestWave) {
                    lowest = m.GetComponent<MonsterInstance>().earliestWave;
                    t = m;
                }
            }

            monsters.Add(t);
            temp.Remove(t);
        }
    }
}
