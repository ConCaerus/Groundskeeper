using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementGrid : MonoBehaviour {
    [SerializeField] GameObject holder;
    Tilemap map;

    [SerializeField][HideInInspector] public bool placing = false;

    [HideInInspector] public GameObject currentObj = null;

    BuyableLibrary bl;
    PregameCanvas pc;
    SoulTransactionHandler cth;

    SetupSequenceManager ssm;

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
        bl = FindObjectOfType<BuyableLibrary>();
        pc = FindObjectOfType<PregameCanvas>();
        cth = FindObjectOfType<SoulTransactionHandler>();
        ssm = FindObjectOfType<SetupSequenceManager>();
    }

    private void Update() {
        if(placing) {
            move();
            var pos = map.WorldToCell(GameInfo.mousePos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);

            //  checks if the icon is hovering over anything it shouldn't
            LayerMask layermask = LayerMask.GetMask("Player");
            layermask += LayerMask.GetMask("HouseFloor");
            foreach(var i in currentObj.GetComponent<Buyable>().placementObstacleLayers) {
                layermask += LayerMask.GetMask(i.ToString());
            }
            RaycastHit2D hit = Physics2D.Raycast(p, Vector3.forward, Mathf.Infinity, layermask);
            if(hit) {
                map.color = Color.red;
            }

            if(Input.GetMouseButton(0) && !pc.mouseOverUI() && map.color == Color.green) {
                place();
            }
            else if(Input.GetMouseButton(1) && !pc.mouseOverUI()) {
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
        map.color = Color.clear;
        currentObj = null;
        map.enabled = false;
        placing = false;
    }
    public void place() {
        //  can't place in this spot
        if(map.color == Color.red)
            return;

        //  extracts the info from the thing stuct
        GameObject obj = null;
        //  checks if the player can afford to place
        var title = currentObj.GetComponent<Buyable>().title;
        bool costIsZero = !bl.hasPlayerSeenBuyable(title) && currentObj.GetComponent<Buyable>().bType != Buyable.buyType.Structure;
        if(!cth.tryTransaction(costIsZero ? 0f : currentObj.GetComponent<Buyable>().cost, pc.soulsText, false))
            return;
        if(costIsZero) {
            bl.playerSawBuyable(title);
            FindObjectOfType<BuyableButtonSpawner>().updateBuyableButtons();
        }
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

        //  checks if the house was just placed. If so, stop placing
        if(currentObj.GetComponent<Buyable>().title == Buyable.buyableTitle.House) {
            currentObj = null;
            placing = false;
            map.color = Color.clear;
            ssm.placedHouse();
        }
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

        cth.tryTransaction(-c, pc.soulsText, false);
        map.color = Color.green;
    }

    public void clear() {
        map.ClearAllTiles();
    }
}
