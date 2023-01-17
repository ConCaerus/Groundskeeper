using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Environment : MonoBehaviour {
    public Tile[] grasses;
    public Tilemap environmentMap;
    [SerializeField] int area;


    private void Start() {
        for(int x = -area; x < area; x++) {
            for(int y = -area; y < area; y++) {
                environmentMap.SetTile(new Vector3Int(x, y, 0), grasses[Random.Range(0, grasses.Length)]);
            }
        }
    }
}
