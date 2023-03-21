using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Buyable : MonoBehaviour {
    [SerializeField] public int cost;
    [SerializeField] public buyableTitle title;

    [SerializeField] public List<PlacementGrid.obstacleTag> placementObstacleLayers = new List<PlacementGrid.obstacleTag>();
    [SerializeField] public Tile tile;

    [SerializeField] Collider2D mainCollider;
    [SerializeField] public SpriteRenderer mainSprite;

    [SerializeField] public GameObject placementCollider;

    [SerializeField] public buyType bType;
    [SerializeField] public string description = "Description";
    public bool isAttractive;

    [System.Serializable]
    public enum buyType {
        None, Helper, Defence, Structure, Weapon, House
    }
    [System.Serializable]
    public enum buyableTitle {  //  not in order cause that would be a bitch to keep track of
        None, Lumberjack, Spikes, Tar, HealingFountain, Scarecrow, House, VoodooDoll
    };

    private void Start() {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(mainCollider, mainSprite);
        if(placementCollider != null)
            Instantiate(placementCollider, transform.position, Quaternion.identity, transform);
    }

    public void animateBeingPlaced() {
        FindObjectOfType<LayerSorter>().waitAndRequestNewSortingLayer(mainCollider, mainSprite);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f), .15f);
    }
}
