using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementGrid : MonoBehaviour {
    [SerializeField] GameObject holder;
    Tilemap map;

    [HideInInspector] public bool placing = false;
    [SerializeField] GameObject radiusEffect;

    [HideInInspector] public GameObject currentObj = null;

    BuyableLibrary bl;
    PregameCanvas pc;
    SoulTransactionHandler cth;

    SetupSequenceManager ssm;
    GameBoard gb;
    MouseManager mm;
    FreeGamepadCursor fgc;

    InputMaster controls;

    Vector2 prevPos;

    bool justUpdatedBoard = false;
    bool mouseDown = false;

    Coroutine placer = null;

    [SerializeField] AudioClip placeSound;

    [System.Serializable]
    public struct thing {
        public Tile tile;
        public GameObject obj;
    }

    [System.Serializable]
    public enum obstacleTag {
        Helper, defense, Structure
    }


    private void Awake() {
        map = GetComponent<Tilemap>();
        map.ClearAllTiles();
        bl = FindObjectOfType<BuyableLibrary>();
        pc = FindObjectOfType<PregameCanvas>();
        cth = FindObjectOfType<SoulTransactionHandler>();
        ssm = FindObjectOfType<SetupSequenceManager>();
        gb = FindObjectOfType<GameBoard>();
        mm = FindObjectOfType<MouseManager>();
        fgc = FindObjectOfType<FreeGamepadCursor>();

        controls = new InputMaster();
        controls.Enable();
        controls.Player.Place.started += ctx => mouseDown = true;
        controls.Player.Place.canceled += ctx => mouseDown = false;
        controls.Player.Unplace.started += ctx => remove();


        radiusEffect.SetActive(false);
    }

    private void Update() {
        if(placing) {
            var pos = map.WorldToCell(mm.usingKeyboard() ? GameInfo.mousePos() : fgc.getWorldCursorPos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);
            move(p);

            //  checks if the icon moved during the last update
            //  if so, check if it moved over anything it shouldn't have
            if((Vector2)p != prevPos || justUpdatedBoard) {
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

                        case obstacleTag.defense:
                            foreach(var h in gb.defenses) {
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
                justUpdatedBoard = false;
            }
        }
        if(mouseDown && placer == null)
            placer = StartCoroutine(place());
    }

    public void changePlacing(GameObject thing, bool toggle) {
        placing = toggle ? !placing : true;

        if(thing == null || !placing)
            clear();
        currentObj = thing;

        //  checks if the new thing needs the radius effect
        radiusEffect.SetActive(false);
        if(placing && currentObj.GetComponent<Buyable>() != null && currentObj.GetComponent<Buyable>().effectRadius > 0f) {
            radiusEffect.SetActive(true);
            var x = currentObj.GetComponent<Buyable>().effectRadius * 2f;
            radiusEffect.transform.localScale = new Vector3(x, x);
        }
    }

    void move(Vector2 worldPos) {
        clear();
        var pos = map.WorldToCell(mm.usingKeyboard() ? GameInfo.mousePos() : fgc.getWorldCursorPos());

        radiusEffect.transform.position = worldPos;
        map.SetTile(pos, currentObj.GetComponent<Buyable>().tile);
    }
    public void end() {
        radiusEffect.SetActive(false);
        map.color = Color.clear;
        currentObj = null;
        map.enabled = false;
        placing = false;
    }
    IEnumerator place() {
        if(!pc.mouseOverUI() && map.color == Color.green && placing) {
            var pos = map.WorldToCell(mm.usingKeyboard() ? GameInfo.mousePos() : fgc.getWorldCursorPos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);
            //  can't place in this spot
            if(map.color == Color.red)
                yield break;

            //  extracts the info from the thing stuct
            GameObject obj = null;
            //  checks if the player can afford to place
            var title = currentObj.GetComponent<Buyable>().title;
            bool costIsZero = bl.getNightBuyableWasSeen(title) == -1 || (bl.getNightBuyableWasSeen(title) == GameInfo.getNightCount() && !gb.hasBuyableOnBoard(title));
            if(!cth.tryTransaction(costIsZero ? 0f : currentObj.GetComponent<Buyable>().cost, pc.soulsText, false, false))
                yield break;
            if(costIsZero) {
                bl.playerSawBuyable(title);
                FindObjectOfType<BuyableButtonSpawner>().updateBuyableButtons();
                foreach(var i in FindObjectsOfType<PregameBuyableButton>())
                    i.manageNewDot();
            }
            if(currentObj.GetComponent<DefenseInstance>() == null) {
                obj = Instantiate(currentObj.gameObject, holder.transform);

                //  places the object in the correct spot
                obj.transform.position = p;
            }
            else {
                obj = FindObjectOfType<DefenseHolderSpawner>().spawnDefense(currentObj.gameObject, p);
            }

            obj.GetComponent<Buyable>().animateBeingPlaced();
            obj.GetComponent<Buyable>().placedThisNight = true;
            obj.GetComponent<Buyable>().placedForFree = costIsZero;
            FindObjectOfType<AudioManager>().playSound(placeSound, p);
            map.color = Color.red;

            //  checks if the house was just placed. If so, stop placing
            if(currentObj.GetComponent<Buyable>().title == Buyable.buyableTitle.House) {
                currentObj = null;
                placing = false;
                map.color = Color.clear;
                ssm.placedHouse();
            }
            prevPos += new Vector2(50f, 0f);    //  makes the prevPos something completely different, making sure that it gets checked next frame
            StartCoroutine(mapUpdater());
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            //  other shit
            if(obj.GetComponent<BloodFumigatorInstance>() != null)
                obj.GetComponent<BloodFumigatorInstance>().setup();
            else if(obj.GetComponent<ThuribleInstance>() != null)
                obj.GetComponent<ThuribleInstance>().setup();
            placer = null;
        }
    }
    void remove() {
        if(!pc.mouseOverUI()) {
            var pos = map.WorldToCell(mm.usingKeyboard() ? GameInfo.mousePos() : fgc.getWorldCursorPos());
            var p = map.CellToWorld(pos);
            p += new Vector3(map.cellSize.x / 2.0f, map.cellSize.y / 2.0f);
            bool found = false;
            int c = 0;

            GameObject f = null;
            foreach(var i in FindObjectsOfType<Buyable>()) {
                if(i.transform.position == p || (i.GetComponent<HelperInstance>() != null && i.GetComponent<HelperInstance>().startingPos == (Vector2)p)) {
                    c = i.GetComponent<Buyable>().cost;
                    f = i.gameObject;
                    found = true;
                    break;
                }
            }
            if(found && f != FindObjectOfType<HouseInstance>().gameObject) {
                gb.removeFromGameBoard(f.gameObject);
                Destroy(f.gameObject);
                if(f.GetComponent<DefenseInstance>() != null)
                    FindObjectOfType<DefenseHolderSpawner>().generateAllGeometry();
            }
            else return;

            if(!f.GetComponent<Buyable>().placedForFree) {
                bool fullRefund = f.GetComponent<Buyable>().placedThisNight;
                cth.tryTransaction(fullRefund ? -c : -c * .9f, pc.soulsText, false, false);
            }
            map.color = Color.green;
            prevPos += new Vector2(50f, 0f);    //  makes the prevPos something completely different, making sure that it gets checked next frame
            StartCoroutine(mapUpdater());
        }
    }

    IEnumerator mapUpdater() {
        yield return new WaitForEndOfFrame();
        justUpdatedBoard = true;
    }

    public void clear() {
        map.ClearAllTiles();
    }

    private void OnDisable() {
        controls.Disable();
    }
}
