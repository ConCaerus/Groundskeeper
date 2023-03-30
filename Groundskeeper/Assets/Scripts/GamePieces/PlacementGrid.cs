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
    GameBoard gb;

    Vector2 prevPos;

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
        gb = FindObjectOfType<GameBoard>();
    }

    private void Update() {
        if(placing) {
            move();
            var pos = map.WorldToCell(GameInfo.mousePos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);

            //  checks if the icon moved during the last update
            //  if so, check if it moved over anything it shouldn't have
            if((Vector2)p != prevPos) {
                prevPos = p;
                map.color = Color.green;

                //  checks if the icon is hovering over either the player, or the house
                LayerMask layermask = LayerMask.GetMask("Player");
                layermask += LayerMask.GetMask("HouseFloor");
                /*
                foreach(var i in currentObj.GetComponent<Buyable>().placementObstacleLayers) {
                    layermask += LayerMask.GetMask(i.ToString());
                }
                */
                RaycastHit2D hit = Physics2D.Raycast(p, Vector3.forward, Mathf.Infinity, layermask);
                if(hit) {
                    map.color = Color.red;
                }
                //  checks if the icon is hovering over another buyable that would restrict it from being placed
                foreach(var i in currentObj.GetComponent<Buyable>().placementObstacleLayers) {
                    switch(i) {
                        case obstacleTag.Helper:
                            foreach(var h in gb.helpers) {
                                //  does have the same position (causing bad juju)
                                if((Vector2)p == h.startingPos) {
                                    map.color = Color.red;
                                    break;
                                }
                            }
                            break;

                        case obstacleTag.Defence:
                            foreach(var h in gb.defences) {
                                //  does have the same position (causing bad juju)
                                if((Vector2)p == (Vector2)h.gameObject.transform.position) {
                                    map.color = Color.red;
                                    break;
                                }
                            }
                            break;

                        case obstacleTag.Structure:
                            foreach(var h in gb.structures) {
                                //  does have the same position (causing bad juju)
                                if((Vector2)p == (Vector2)h.gameObject.transform.position) {
                                    map.color = Color.red;
                                    break;
                                }
                            }
                            break;
                    }

                    if(map.color == Color.red)
                        break;
                }
            }

            if(Input.GetMouseButton(0) && !pc.mouseOverUI() && map.color == Color.green) {
                place(p);
                prevPos += new Vector2(50f, 0f);    //  makes the prevPos something completely different, making sure that it gets checked next frame
            }
            else if(Input.GetMouseButton(1) && !pc.mouseOverUI()) {
                remove(p);
                prevPos += new Vector2(50f, 0f);    //  makes the prevPos something completely different, making sure that it gets checked next frame
            }
        }
    }

    public void changePlacing(GameObject thing, bool toggle) {
        placing = toggle ? !placing : true;
        currentObj = thing;
    }

    void move() {
        clear();
        var pos = map.WorldToCell(GameInfo.mousePos());

        map.SetTile(pos, currentObj.GetComponent<Buyable>().tile);
    }
    public void end() {
        map.color = Color.clear;
        currentObj = null;
        map.enabled = false;
        placing = false;
    }
    void place(Vector2 pos) {
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
        if(!bl.hasPlayerSeenBuyable(title)) {
            bl.playerSawBuyable(title);
            FindObjectOfType<BuyableButtonSpawner>().updateBuyableButtons();
            foreach(var i in FindObjectsOfType<PregameBuyableButton>())
                i.manageNewDot();
        }
        if(currentObj.GetComponent<DefenceInstance>() == null) {
            obj = Instantiate(currentObj.gameObject, holder.transform);

            //  places the object in the correct spot
            obj.transform.position = pos;

            if(obj.GetComponent<HelperInstance>() != null)
                obj.GetComponent<HelperInstance>().startingPos = pos;
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
    void remove(Vector2 pos) {
        if(map.color == Color.green)
            return;
        bool found = false;
        int c = 0;

        GameObject f = null;
        foreach(var i in FindObjectsOfType<Buyable>()) {
            if(i.transform.position == (Vector3)pos || (i.GetComponent<HelperInstance>() != null && i.GetComponent<HelperInstance>().startingPos == (Vector2)pos)) {
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
