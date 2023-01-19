using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Environment : MonoBehaviour {
    public Tile[] grasses;
    public Tilemap environmentMap;
    [SerializeField] int area;
    [SerializeField] int yStatic = 0, xStatic = 0;


    private void Start() {
        if(yStatic != 0) {
            for(int x = -area; x < area; x++) {
                bool replacable = false;
                foreach(var i in grasses) {
                    if(environmentMap.GetTile(new Vector3Int(x, yStatic, 0)) == i) {
                        replacable = true;
                        break;
                    }
                }
                if(replacable)
                    environmentMap.SetTile(new Vector3Int(x, yStatic, 0), grasses[Random.Range(0, grasses.Length)]);
            }
        }
        if(xStatic != 0) {
            for(int y = -area; y < area; y++) {
                bool replacable = false;
                foreach(var i in grasses) {
                    if(environmentMap.GetTile(new Vector3Int(xStatic, y, 0)) == i) {
                        replacable = true;
                        break;
                    }
                }
                if(replacable)
                    environmentMap.SetTile(new Vector3Int(xStatic, y, 0), grasses[Random.Range(0, grasses.Length)]);
            }
        }
        else if(xStatic == 0 && yStatic == 0) {
            for(int x = -area; x < area; x++) {
                for(int y = -area; y < area; y++) {
                    bool replacable = false;
                    foreach(var i in grasses) {
                        if(environmentMap.GetTile(new Vector3Int(x, y, 0)) == i) {
                            replacable = true;
                            break;
                        }
                    }
                    if(replacable)
                        environmentMap.SetTile(new Vector3Int(x, y, 0), grasses[Random.Range(0, grasses.Length)]);
                }
            }
        }
    }
}
