using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementGrid : MonoBehaviour {
    [SerializeField] List<thing> helpers, defences;
    [SerializeField] GameObject holder;
    Tilemap map;

    [HideInInspector]
    [SerializeField] public int state = 0;  //  0 - nothing,    1 - helpers,    2 - defences

    List<Vector3Int> unusableHelperPoses = new List<Vector3Int>();
    List<Vector3Int> unusableDefencePoses = new List<Vector3Int>();

    [System.Serializable]
    public struct thing {
        public Tile tile;
        public GameObject obj;
    }


    private void Awake() {
        map = GetComponent<Tilemap>();
        map.ClearAllTiles();
    }

    private void Start() {
        //  add all of the tiles that are under the house into the unusable lists
        var h = GameObject.FindGameObjectWithTag("House");
        var l = map.WorldToCell(new Vector3(h.transform.position.x - h.transform.localScale.x / 2f, 0, 0)).x;
        var r = map.WorldToCell(new Vector3(h.transform.position.x + h.transform.localScale.x / 2f, 0, 0)).x;
        var t = map.WorldToCell(new Vector3(0, h.transform.position.y + h.transform.localScale.y / 2f, 0)).y;
        var b = map.WorldToCell(new Vector3(0, h.transform.position.y - h.transform.localScale.y / 2f, 0)).y;
        for(int x = l; x < r; x++) {
            for(int y = b; y < t; y++) {
                unusableHelperPoses.Add(new Vector3Int(x, y, 0));
                unusableDefencePoses.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    private void Update() {
        if(state > 0) {
            move();

            if(Input.GetMouseButton(0) && !FindObjectOfType<PregameCanvas>().mouseOverUI()) {
                place();
            }
            else if(Input.GetMouseButton(1) && !FindObjectOfType<PregameCanvas>().mouseOverUI()) {
                remove();
            }
        }
    }

    public void move() {
        clear();
        var pos = map.WorldToCell(GameInfo.mousePos());

        map.color = Color.green;
        foreach(var i in state == 1 ? unusableHelperPoses : unusableDefencePoses) {
            if(i == new Vector3Int(pos.x, pos.y, i.z)) {
                map.color = Color.red;
            }
        }

        if(state == 1)
            map.SetTile(pos, helpers[0].tile);
        else if(state == 2)
            map.SetTile(pos, defences[0].tile);
    }

    public void end() {
        map.enabled = false;
        state = 0;
    }

    public void place() {
        //  can't place in this spot
        if(map.color == Color.red)
            return;

        //  extracts the info from the thing stuct
        GameObject obj = null;
        if(state == 1) {
            //  checks if the player can afford to place
            if(!FindObjectOfType<PregameCanvas>().updateCoins(helpers[0].obj.GetComponent<HelperInstance>().cost))
                return;
            obj = Instantiate(helpers[0].obj, holder.transform);
        }
        else if(state == 2) {
            //  checks if the player can afford to place
            if(!FindObjectOfType<PregameCanvas>().updateCoins(defences[0].obj.GetComponent<DefenceInstance>().cost))
                return;
            obj = Instantiate(defences[0].obj, holder.transform);
        }

        //  places the object in the correct spot
        var pos = map.CellToWorld(map.WorldToCell(GameInfo.mousePos())) + new Vector3(map.cellSize.x / 2f, map.cellSize.y / 2f);
        obj.transform.position = pos;


        //  updates unusable lists with this pos
        if(state == 1) {
            unusableHelperPoses.Add(map.WorldToCell(GameInfo.mousePos()));
            obj.GetComponent<HelperInstance>().startingPos = pos;
        }
        else if(state == 2)
            unusableDefencePoses.Add(map.WorldToCell(GameInfo.mousePos()));
    }

    public void remove() {
        if(map.color == Color.green)
            return;
        bool found = false;

        var pos = map.CellToWorld(map.WorldToCell(GameInfo.mousePos())) + new Vector3(map.cellSize.x / 2f, map.cellSize.y / 2f);
        if(state == 1) {
            if(!unusableHelperPoses.Contains(map.WorldToCell(GameInfo.mousePos())))
                return;

            foreach(var i in GameObject.FindGameObjectsWithTag("Person")) {
                if(i.GetComponent<HelperInstance>().startingPos == (Vector2)pos) {
                    Destroy(i.gameObject);
                    found = true;
                    break;
                }
            }
            if(found)
                unusableHelperPoses.Remove(map.WorldToCell(GameInfo.mousePos()));
            else return;
        }
        else if(state == 2) {
            if(!unusableDefencePoses.Contains(map.WorldToCell(GameInfo.mousePos())))
                return;

            foreach(var i in GameObject.FindGameObjectsWithTag("Defence")) {
                if((Vector2)i.transform.position == (Vector2)pos) {
                    Destroy(i.gameObject);
                    found = true;
                    break;
                }
            }
            if(found)
                unusableDefencePoses.Remove(map.WorldToCell(GameInfo.mousePos()));
            else return;
        }

        map.color = Color.green;
    }

    public void clear() {
        map.ClearAllTiles();
    }
}
