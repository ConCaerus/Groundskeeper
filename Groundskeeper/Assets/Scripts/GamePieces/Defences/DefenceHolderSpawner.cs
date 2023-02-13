using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceHolderSpawner : MonoBehaviour {
    [SerializeField] GameObject dHolder;
    List<GameObject> holders = new List<GameObject>();

    List<string> usedNames = new List<string>();


    public GameObject spawnDefence(GameObject defence, Vector2 pos) {
        //  create a new holder
        if(!usedNames.Contains(defence.GetComponent<Buyable>().title.ToString())) {
            var h = Instantiate(dHolder, transform);
            h.transform.localPosition = Vector3.zero;
            holders.Add(h);

            var obj = Instantiate(defence, h.transform);
            usedNames.Add(defence.GetComponent<Buyable>().title.ToString());
            obj.transform.position = pos;

            return obj;
        }
        //  use an existing holder
        else {
            var h = holders[usedNames.IndexOf(defence.GetComponent<Buyable>().title.ToString())];
            var obj = Instantiate(defence, h.transform);
            obj.transform.position = pos;
            h.GetComponent<CompositeCollider2D>().GenerateGeometry();

            return obj;
        }
    }
}
