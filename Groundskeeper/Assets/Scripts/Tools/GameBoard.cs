using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    [SerializeField] GameObject holder, envHolder;


    [HideInInspector] public KdTree<LumberjackInstance> aHelpers = new KdTree<LumberjackInstance>();
    [HideInInspector] public KdTree<MonsterInstance> monsters = new KdTree<MonsterInstance>();  //  used for attack logic with helpers and whatnot
    [HideInInspector] public KdTree<EnvironmentInstance> environment = new KdTree<EnvironmentInstance>();


    public const float boardRadius = 250f;


    private void Start() {
        if(GameInfo.getNightCount() == 0)
            GameInfo.clearBoard();
        loadBoard();
    }

    //  NOTE: BOARD RESETS AUTOMATICALLY ON NIGHT 0
    public void saveBoard() {
        int hIndex = 0, dIndex = 0, pIndex = 0;
        //      clears all of the shit before saving new shit
        GameInfo.clearBoard();

        //      saves new shit
        foreach(var i in GameObject.FindGameObjectsWithTag("Helper")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.helperTag + hIndex.ToString(), data);
            hIndex++;
        }
        foreach(var i in GameObject.FindGameObjectsWithTag("Defence")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.defenceTag + dIndex.ToString(), data);
            dIndex++;
        }
        foreach(var i in GameObject.FindGameObjectsWithTag("Passive")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.miscTag + pIndex.ToString(), data);
            pIndex++;
        }

        GameInfo.saveSouls();
        //      saves how many new shit got saved for the next time the shit gets cleared
        SaveData.setInt(GameInfo.lastSavedHelperCount, hIndex);
        SaveData.setInt(GameInfo.lastSavedDefenceCount, dIndex);
        SaveData.setInt(GameInfo.lastSavedMiscCount, pIndex);

        saveEnvironment();
    }
    public void loadBoard() {
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedHelperCount); i++) {
            var d = SaveData.getString(GameInfo.helperTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = FindObjectOfType<BuyableLibrary>().getHelper(data.name);
            Instantiate(obj, data.pos, Quaternion.identity, holder.transform);
        }
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedDefenceCount); i++) {
            var d = SaveData.getString(GameInfo.defenceTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = FindObjectOfType<BuyableLibrary>().getDefence(data.name);
            FindObjectOfType<DefenceHolderSpawner>().spawnDefence(obj, data.pos);
        }
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedMiscCount); i++) {
            var d = SaveData.getString(GameInfo.miscTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = FindObjectOfType<BuyableLibrary>().getStructure(data.name);
            Instantiate(obj, data.pos, Quaternion.identity, holder.transform);
        }

        spawnEnvironment();
    }

    public void saveEnvironment() {
        //  clears the save data
        for(int i = 0; i < SaveData.getInt(GameInfo.envCount); i++)
            SaveData.deleteKey(GameInfo.envTag + i.ToString());

        //  store the list
        for(int i = 0; i < environment.Count; i++) {
            var save = new ObjectSaveData(environment[i]);
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.envTag + i.ToString(), data);
        }
        SaveData.setInt(GameInfo.envCount, environment.Count);
    }

    public void spawnEnvironment() {
        StartCoroutine(environmentSpawner(SaveData.getInt(GameInfo.envCount) != 0));
        FindObjectOfType<EnvironmentManager>().finishSpawning();
    }

    IEnumerator environmentSpawner(bool hasEnvs) {
        float cutOffCount = 200;    //  after spawning this number of shit, start slowing the spawing shit.

        if(!hasEnvs) {
            int count = Random.Range((int)(boardRadius * 1.5f), (int)(boardRadius * 3.0f));

            var hDiag = (FindObjectOfType<HouseInstance>().GetComponent<SpriteRenderer>().bounds.size.x / 2.0f) * Mathf.Sqrt(2);
            List<Vector2> poses = new List<Vector2>();

            for(int i = 0; i < count; i++) {
                var pos = Vector2.zero;
                if(i < count * .15f) {
                    pos = Random.insideUnitCircle.normalized * Random.Range(hDiag + 1, boardRadius * .25f);
                }
                else if(i < count * .65f) {
                    pos = Random.insideUnitCircle.normalized * Random.Range(boardRadius * .25f, boardRadius * .75f);
                }
                else {
                    pos = Random.insideUnitCircle.normalized * Random.Range(boardRadius * .75f, boardRadius);
                }
                poses.Add(pos);
            }

            //  sort the list by how close it is to (0, 0)
            poses.Sort((a, b) => Vector2.Distance(a, Vector2.zero).CompareTo(Vector2.Distance(b, Vector2.zero)));

            //  store the list
            for(int i = 0; i < count; i++) {
                var o = FindObjectOfType<EnvironmentManager>().spawnEnv(FindObjectOfType<PresetLibrary>().getRandomEnvironment(), poses[i]);

                var save = new ObjectSaveData(o.GetComponent<EnvironmentInstance>());
                var data = JsonUtility.ToJson(save);
                SaveData.setString(GameInfo.envTag + i.ToString(), data);

                environment.Add(o.GetComponent<EnvironmentInstance>());
                if(i > cutOffCount)
                    yield return new WaitForSeconds(.01f);
            }
            SaveData.setInt(GameInfo.envCount, count);
        }
        else {
            //  just spawns the shit
            for(int i = 0; i < SaveData.getInt(GameInfo.envCount); i++) {
                var d = SaveData.getString(GameInfo.envTag + i.ToString());
                var data = JsonUtility.FromJson<ObjectSaveData>(d);

                var o = FindObjectOfType<EnvironmentManager>().spawnEnv(FindObjectOfType<PresetLibrary>().getEnvironment(data.name), data.pos);

                if(o.GetComponent<Collider2D>() != null)
                    FindObjectOfType<LayerSorter>().requestNewSortingLayer(o.GetComponent<Collider2D>(), o.transform.GetChild(0).GetComponent<SpriteRenderer>());
                else
                    FindObjectOfType<LayerSorter>().requestNewSortingLayer(o.transform.position.y, o.transform.GetChild(0).GetComponent<SpriteRenderer>());

                environment.Add(o.GetComponent<EnvironmentInstance>());
                if(i > cutOffCount)
                    yield return new WaitForSeconds(.01f);
            }
        }
    }
}

[System.Serializable]
public class ObjectSaveData {
    public string name;
    public Vector2 pos;

    public ObjectSaveData(EnvironmentInstance env) {
        name = env.title;
        pos = env.transform.position;
    }

    public ObjectSaveData(Buyable b) {
        name = b.title.ToString();
        pos = b.transform.position;
    }

    public ObjectSaveData(string n, Vector2 p) {
        name = n;
        pos = p;
    }
}
