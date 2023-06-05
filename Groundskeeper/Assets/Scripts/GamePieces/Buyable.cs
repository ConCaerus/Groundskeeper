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
    [SerializeField] public bool isAttractive;
    [SerializeField] public float effectRadius;
    [SerializeField] public Vector2 unlockedImagePos;
    [SerializeField] public Vector2 unlockedImageSize;

    [System.Serializable]
    public enum buyType {
        None, Helper, Defense, Structure, Weapon, House
    }
    [System.Serializable]
    public enum buyableTitle {  //  not in order cause that would be a bitch to keep track of
        None, Lumberjack, Spikes, Tar, HealingFountain, Scarecrow, House, VoodooDoll, Priest, Repairman, Demon, 
        HolyPuddle, SeaSalt, SoulNibbler, DevilsTrap, IronMaiden, DevilsShrine, BloodFumigator, Thurible, SoulBooster
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

    public string titleToText() {
        switch(title) {
            //  if the title is just one word, return it as is
            case buyableTitle.Lumberjack:
            case buyableTitle.Spikes:
            case buyableTitle.Tar:
            case buyableTitle.Scarecrow:
            case buyableTitle.House:
            case buyableTitle.Priest:
            case buyableTitle.Repairman:
            case buyableTitle.Demon:
            case buyableTitle.Thurible:
                return title.ToString();

            //  if the title is more than one word, add a space before returning it
            case buyableTitle.HealingFountain: return "Healing Fountain";
            case buyableTitle.VoodooDoll: return "Voodoo Doll";
            case buyableTitle.HolyPuddle: return "Holy Puddle";
            case buyableTitle.SeaSalt: return "Sea Salt";
            case buyableTitle.SoulNibbler: return "Soul Nibbler";
            case buyableTitle.DevilsTrap: return "Devil's Trap";
            case buyableTitle.IronMaiden: return "Iron Maiden";
            case buyableTitle.DevilsShrine: return "Devil's Shrine";
            case buyableTitle.BloodFumigator: return "Blood Fumigator";
            case buyableTitle.SoulBooster: return "Soul Booster";
        }
        return string.Empty;
    }
}
