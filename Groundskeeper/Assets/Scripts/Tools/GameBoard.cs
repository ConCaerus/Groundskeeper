using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameBoard : MonoBehaviour {

    [SerializeField] GameObject holder, envHolder, deadGuyHolder;


    [HideInInspector] public KdTree<Helper> helpers = new KdTree<Helper>();
    [HideInInspector] public KdTree<StructureInstance> structures = new KdTree<StructureInstance>();
    [HideInInspector] public KdTree<DefenseInstance> defenses = new KdTree<DefenseInstance>();
    [HideInInspector] public KdTree<DeadGuyInstance> deadGuys = new KdTree<DeadGuyInstance>();
    [HideInInspector] public KdTree<MonsterInstance> monsters = new KdTree<MonsterInstance>();  //  used for attack logic with helpers and whatnot
    [HideInInspector] public KdTree<EnvironmentInstance> environment = new KdTree<EnvironmentInstance>();

    const int minDeadGuyCount = 5;

    const float boardRadius = 75f;
    Coroutine saver = null;
    const string houseTag = "HouseTag";

    //  gets set to true in the EnvironmentManager script after all of the colliders have been set up
    [HideInInspector] public bool loaded = false;
    [HideInInspector] public bool fastSave = false;

    AudioManager am;
    Transform pt;
    float distToStartMusic = 25f;


    private void Awake() {
        if(GameInfo.getNightCount() == 0)
            GameInfo.clearBoard();
        loadBoard();
        am = FindObjectOfType<AudioManager>();
        pt = FindObjectOfType<PlayerInstance>().transform;
        //StartCoroutine(musicModder(0f));  //  energetic and upbeat music doesn't fit this game
    }

    //  NOTE: BOARD RESETS AUTOMATICALLY ON NIGHT 0
    public void saveBoard() {
        if(saver != null)
            return;
        saver = StartCoroutine(saveWaiter());
    }
    public void loadBoard() {
        var bl = FindObjectOfType<BuyableLibrary>();
        var pl = FindObjectOfType<PresetLibrary>();
        var dhs = FindObjectOfType<DefenseHolderSpawner>();
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedHelperCount); i++) {
            var d = SaveData.getString(GameInfo.helperTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = bl.getHelper(data.name, false);
            Instantiate(obj, data.pos, Quaternion.identity, holder.transform);
        }
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedDefenseCount); i++) {
            var d = SaveData.getString(GameInfo.defenseTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = bl.getDefense(data.name, false);
            dhs.spawnDefense(obj, data.pos);
        }
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedStructureCount); i++) {
            var d = SaveData.getString(GameInfo.structureTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = bl.getStructure(data.name, false);
            Instantiate(obj, data.pos, Quaternion.identity, holder.transform);
        }
        for(int i = 0; i < SaveData.getInt(GameInfo.lastSavedDeadGuyCount); i++) {
            var d = SaveData.getString(GameInfo.deadGuyTag + i.ToString());
            var data = JsonUtility.FromJson<ObjectSaveData>(d);

            var obj = pl.getDeadGuy(data.name);
            deadGuys.Add(Instantiate(obj, data.pos, Quaternion.identity, deadGuyHolder.transform).GetComponent<DeadGuyInstance>());
        }

        //  spawn the first set of dead guys
        if(GameInfo.getNightCount() == 0 && deadGuys.Count == 0) {
            for(int i = 0; i < minDeadGuyCount; i++) {
                var obj = pl.getRandomDeadGuy();
                deadGuys.Add(Instantiate(obj, getRandomMapPos(), Quaternion.identity, deadGuyHolder.transform).GetComponent<DeadGuyInstance>());
            }
        }
        //  spawns new dead guys if needs to spawn more
        else if(SaveData.getInt(GameInfo.lastSavedDeadGuyCount) < minDeadGuyCount) {
            int spawnNum = Random.Range(1, minDeadGuyCount - deadGuys.Count + 1);
            for(int i = 0; i < spawnNum; i++) {
                var obj = pl.getRandomDeadGuy();
                deadGuys.Add(Instantiate(obj, getRandomMapPos(), Quaternion.identity, deadGuyHolder.transform).GetComponent<DeadGuyInstance>());
            }
        }

        deadGuyHolder.GetComponent<CompositeCollider2D>().GenerateGeometry();


        if(GameInfo.getNightCount() > 0) {
            var hD = SaveData.getString(houseTag);
            var hData = JsonUtility.FromJson<ObjectSaveData>(hD);
            var hObj = bl.getHouseStructure();
            Instantiate(hObj, hData.pos, Quaternion.identity, holder.transform);
        }

        spawnEnvironment();
    }

    public void spawnEnvironment() {
        StartCoroutine(environmentSpawner(SaveData.getInt(GameInfo.envCount) != 0));
    }

    IEnumerator saveWaiter() {
        int hIndex = 0, dIndex = 0, pIndex = 0, dgIndex = 0, thingsPerFrame = 1;
        //      clears all of the shit before saving new shit
        GameInfo.clearBoard();

        //      saves buyables
        foreach(var i in GameObject.FindGameObjectsWithTag("Helper")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            if(i.GetComponent<HelperInstance>() != null)
                save = new ObjectSaveData(i.GetComponent<HelperInstance>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.helperTag + hIndex.ToString(), data);
            hIndex++;
            if(!fastSave && hIndex % thingsPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }
        foreach(var i in GameObject.FindGameObjectsWithTag("Defense")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.defenseTag + dIndex.ToString(), data);
            dIndex++;
            if(!fastSave && dIndex % thingsPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }
        foreach(var i in GameObject.FindGameObjectsWithTag("Structure")) {
            var save = new ObjectSaveData(i.GetComponent<Buyable>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.structureTag + pIndex.ToString(), data);
            pIndex++;
            if(!fastSave && pIndex % thingsPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }

        //  saves dead guys
        foreach(var i in GameObject.FindGameObjectsWithTag("DeadGuy")) {
            var save = new ObjectSaveData(i.GetComponent<DeadGuyInstance>());
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.deadGuyTag + dgIndex.ToString(), data);
            dgIndex++;
            if(!fastSave && pIndex % thingsPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }

        //  specific cases of unique buyables
        var hSave = new ObjectSaveData(FindObjectOfType<HouseInstance>().GetComponent<Buyable>());
        var hData = JsonUtility.ToJson(hSave);
        SaveData.setString(houseTag, hData);

        GameInfo.saveSouls();
        //      saves how many new shit got saved for the next time the shit gets cleared
        SaveData.setInt(GameInfo.lastSavedHelperCount, hIndex);
        SaveData.setInt(GameInfo.lastSavedDefenseCount, dIndex);
        SaveData.setInt(GameInfo.lastSavedStructureCount, pIndex);
        SaveData.setInt(GameInfo.lastSavedDeadGuyCount, dgIndex);

        //     SAVES THE ENVIRONMENT
        //  clears the save data
        for(int i = 0; i < SaveData.getInt(GameInfo.envCount); i++)
            SaveData.deleteKey(GameInfo.envTag + i.ToString(), false);

        //  store the list
        for(int i = 0; i < environment.Count; i++) {
            var save = new ObjectSaveData(environment[i]);
            var data = JsonUtility.ToJson(save);
            SaveData.setString(GameInfo.envTag + i.ToString(), data);
            if(!fastSave && i % thingsPerFrame == 0)
                yield return new WaitForEndOfFrame();
        }
        SaveData.setInt(GameInfo.envCount, environment.Count);

        //  other stuff
        saver = null;
    }

    public Vector2 getRandomMapPos() {
        var hDiag = 6.83f * Mathf.Sqrt(2);  //  6.83 = FindObjectOfType<HouseInstance>().GetComponent<SpriteRenderer>().bounds.size.x / 2.0f
        return Random.insideUnitCircle.normalized * Random.Range(hDiag + 1, boardRadius);
    }

    IEnumerator environmentSpawner(bool hasEnvs) {
        float cutOffCount = 200;    //  after spawning this number of shit, start slowing the spawing shit.
        var em = FindObjectOfType<EnvironmentManager>();
        var ls = FindObjectOfType<LayerSorter>();
        var pl = FindObjectOfType<PresetLibrary>();
        Vector2 center = FindObjectOfType<HouseInstance>() != null ? FindObjectOfType<HouseInstance>().getCenter() : (Vector2)FindObjectOfType<PlayerInstance>().transform.position;

        if(!hasEnvs) {
            int count = Random.Range((int)(boardRadius * 1.5f), (int)(boardRadius * 3.0f));
            cutOffCount = count + 1;    //  NULLIFIES THE PURPOSE OF THIS FUCKING THING

            var hDiag = 6.83f * Mathf.Sqrt(2);  //  6.83 = FindObjectOfType<HouseInstance>().GetComponent<SpriteRenderer>().bounds.size.x / 2.0f
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
            //poses.Sort((a, b) => Vector2.Distance(a, center).CompareTo(Vector2.Distance(b, center)));

            //  randomizes the poses
            int n = poses.Count;
            while(n > 1) {
                n--;
                int k = Random.Range(0, n);
                var value = poses[k];
                poses[k] = poses[n];
                poses[n] = value;
            }

            //  store the list
            //  sort the environment based on what type they are
            var presetEnvCount = pl.getEnvironmentCount();
            for(int e = 0; e < presetEnvCount; e++) {
                for(int i = 0; i < count / presetEnvCount; i++) {
                    var o = em.spawnEnv(pl.getEnvironment(e), poses[i + (count / presetEnvCount) * e]);
                    var oei = o.GetComponent<EnvironmentInstance>();
                    var save = new ObjectSaveData(oei);
                    var data = JsonUtility.ToJson(save);
                    SaveData.setString(GameInfo.envTag + (i + (count / presetEnvCount) * e).ToString(), data);

                    environment.Add(oei);
                    if(i > cutOffCount)
                        yield return new WaitForEndOfFrame();
                }
            }
            SaveData.setInt(GameInfo.envCount, count);
        }
        else {
            //  just spawns the shit
            for(int i = 0; i < SaveData.getInt(GameInfo.envCount); i++) {
                var d = SaveData.getString(GameInfo.envTag + i.ToString());
                if(string.IsNullOrEmpty(d))
                    continue;
                var data = JsonUtility.FromJson<ObjectSaveData>(d);

                var o = em.spawnEnv(pl.getEnvironment(data.name), data.pos);

                if(o.GetComponent<Collider2D>() != null)
                    ls.requestNewSortingLayer(o.GetComponent<Collider2D>(), o.transform.GetChild(0).GetComponent<SpriteRenderer>());
                else
                    ls.requestNewSortingLayer(o.transform.position.y, o.transform.GetChild(0).GetComponent<SpriteRenderer>());

                environment.Add(o.GetComponent<EnvironmentInstance>());
                if(i > cutOffCount)
                    yield return new WaitForEndOfFrame();
            }
        }

        FindObjectOfType<EnvironmentManager>().finishSpawning();
        loaded = true;
    }

    public bool saving() {
        return saver != null;
    }

    public void removeFromGameBoard(GameObject thing) {
        if(thing.GetComponent<Helper>() != null)
            helpers.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
        else if(thing.GetComponent<StructureInstance>() != null)
            structures.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
        else if(thing.GetComponent<DefenseInstance>() != null)
            defenses.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
        else if(thing.GetComponent<DeadGuyInstance>() != null)
            deadGuys.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
        else if(thing.GetComponent<MonsterInstance>() != null)
            monsters.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
        else if(thing.GetComponent<EnvironmentInstance>() != null)
            environment.RemoveAll(x => x.gameObject.GetInstanceID() == thing.gameObject.GetInstanceID());
    }
    public float getBoardRad() {
        return boardRadius;
    }

    IEnumerator musicModder(float minPerc, float changeTime = .25f) {
        if(GameInfo.wave >= GameInfo.wavesPerNight()) {
            Debug.Log(monsters.Count);
            if(monsters.Count > 0) {
                var c = monsters.FindClosest(pt.position);
                var d = Vector2.Distance(pt.position, c.transform.position);
                if(d < distToStartMusic) {
                    var t = (1f - d / distToStartMusic);
                    am.setMusicVolume(Mathf.Clamp(t, minPerc, 1f), changeTime);
                    minPerc = t > minPerc ? t : minPerc *= .95f;
                }
                else
                    am.setMusicVolume(minPerc, changeTime);
            }
            else
                am.setMusicVolume(minPerc, changeTime);
        }
        yield return new WaitForSeconds(changeTime + .01f);
        StartCoroutine(musicModder(minPerc));
    }

    public bool hasBuyableOnBoard(Buyable.buyableTitle title) {
        foreach(var i in FindObjectsOfType<Buyable>()) {
            if(i.title == title) 
                return true;
        }
        return false;
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
    public ObjectSaveData(DeadGuyInstance b) {
        name = b.title.ToString();
        pos = b.transform.position;
    }

    public ObjectSaveData(HelperInstance l) {
        name = l.GetComponent<Buyable>().title.ToString();
        pos = l.startingPos;
    }

    public ObjectSaveData(string n, Vector2 p) {
        name = n;
        pos = p;
    }
}
