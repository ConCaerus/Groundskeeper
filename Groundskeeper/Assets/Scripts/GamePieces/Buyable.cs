using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class Buyable : MonoBehaviour {
    [SerializeField] public int cost;
    [SerializeField] public string title;
    [SerializeField] public bool canBePlacedOutsideLight = false;

    [SerializeField] public List<PlacementGrid.obstacleTag> placementObstacleLayers = new List<PlacementGrid.obstacleTag>();
    [SerializeField] public Tile tile;

    [SerializeField] Collider2D mainCollider;
    [SerializeField] SpriteRenderer mainSprite;

    [SerializeField] public GameObject placementCollider;

    [SerializeField] public buyType bType;

    public enum buyType {
        None, Helper, Defence, Misc
    }

    private void Start() {
        FindObjectOfType<LayerSorter>().requestNewSortingLayer(mainCollider, mainSprite);
        Instantiate(placementCollider, transform.position, Quaternion.identity, transform);
    }

    public void animateBeingPlaced() {
        FindObjectOfType<LayerSorter>().waitAndRequestNewSortingLayer(mainCollider, mainSprite);
        transform.DOPunchScale(new Vector3(1.5f, 1.5f), .15f);
    }
}
