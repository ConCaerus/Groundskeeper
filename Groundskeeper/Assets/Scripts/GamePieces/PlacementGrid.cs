using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementGrid : MonoBehaviour {
    [SerializeField] GameObject holder;
    Tilemap map;

    [SerializeField][HideInInspector] public bool placing = false;

    [HideInInspector] public GameObject currentObj = null;
    [SerializeField] CircleCollider2D houseRadiusCollider;

    [System.Serializable]
    public struct thing {
        public Tile tile;
        public GameObject obj;
    }

    [System.Serializable]
    public enum obstacleTag {
        Helper, Defence, Structure
    }


    private void Awake() {
        map = GetComponent<Tilemap>();
        map.ClearAllTiles();
    }

    private void Update() {
        if(placing) {
            move();
            var pos = map.WorldToCell(GameInfo.mousePos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);

            if(!currentObj.GetComponent<Buyable>().canBePlacedOutsideLight) {
                var r = houseRadiusCollider.gameObject.transform.lossyScale.x * houseRadiusCollider.radius;
                var d = Mathf.Sqrt(Mathf.Pow(p.x, 2) + Mathf.Pow(p.y, 2));
                bool inBounds = d < r;

                if(!inBounds)
                    map.color = Color.red;
            }

            //  checks if the icon is hovering over anything it shouldn't
            LayerMask layermask = LayerMask.GetMask("Player");
            layermask += LayerMask.GetMask("HouseFloor");
            foreach(var i in currentObj.GetComponent<Buyable>().placementObstacleLayers) {
                layermask += LayerMask.GetMask(i.ToString());
            }
            RaycastHit2D hit = Physics2D.Raycast(p, Vector3.forward, Mathf.Infinity, layermask);
            Debug.DrawRay(p, Vector3.forward, Color.white, 1f);
            if(hit) {
                map.color = Color.red;
            }

            if(Input.GetMouseButton(0) && !FindObjectOfType<PregameCanvas>().mouseOverUI() && map.color == Color.green) {
                place();
            }
            else if(Input.GetMouseButton(1) && !FindObjectOfType<PregameCanvas>().mouseOverUI()) {
                remove();
            }
        }
    }

    public void changePlacing(GameObject thing, bool toggle) {
        placing = toggle ? !placing : true;
        currentObj = thing;
    }

    public void move() {
        clear();
        var pos = map.WorldToCell(GameInfo.mousePos());

        map.color = Color.green;

        map.SetTile(pos, currentObj.GetComponent<Buyable>().tile);
    }
    public void end() {
        map.enabled = false;
        placing = false;
        currentObj = null;
    }
    public void place() {
        //  can't place in this spot
        if(map.color == Color.red)
            return;

        //  extracts the info from the thing stuct
        GameObject obj = null;
        //  checks if the player can afford to place
        if(!FindObjectOfType<SoulTransactionHandler>().tryTransaction(currentObj.GetComponent<Buyable>().cost, FindObjectOfType<PregameCanvas>().soulsText, false))
            return;
        var pos = map.CellToWorld(map.WorldToCell(GameInfo.mousePos())) + new Vector3(map.cellSize.x / 2f, map.cellSize.y / 2f);
        if(currentObj.GetComponent<DefenceInstance>() == null) {
            obj = Instantiate(currentObj.gameObject, holder.transform);

            //  places the object in the correct spot
            obj.transform.position = pos;

            if(obj.GetComponent<LumberjackInstance>() != null)
                obj.GetComponent<LumberjackInstance>().startingPos = pos;
        }
        else {
            obj = FindObjectOfType<DefenceHolderSpawner>().spawnDefence(currentObj.gameObject, pos);
        }

        obj.GetComponent<Buyable>().animateBeingPlaced();
    }
    public void remove() {
        if(map.color == Color.green)
            return;
        bool found = false;
        int c = 0;

        var pos = map.CellToWorld(map.WorldToCell(GameInfo.mousePos())) + new Vector3(map.cellSize.x / 2f, map.cellSize.y / 2f);
        GameObject f = null;
        foreach(var i in FindObjectsOfType<Buyable>()) {
            if(i.transform.position == pos || (i.GetComponent<LumberjackInstance>() != null && i.GetComponent<LumberjackInstance>().startingPos == (Vector2)pos)) {
                c = i.GetComponent<Buyable>().cost;
                f = i.gameObject;
                found = true;
                break;
            }
        }
        if(found) {
            Destroy(f.gameObject);
        }
        else return;

        FindObjectOfType<SoulTransactionHandler>().tryTransaction(-c, FindObjectOfType<PregameCanvas>().soulsText, false);
        map.color = Color.green;
    }

    public void clear() {
        map.ClearAllTiles();
    }
}
