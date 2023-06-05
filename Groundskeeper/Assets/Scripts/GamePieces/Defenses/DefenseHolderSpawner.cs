using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseHolderSpawner : MonoBehaviour {
    [SerializeField] GameObject dHolder;
    List<GameObject> holders = new List<GameObject>();

    List<string> usedNames = new List<string>();


    public GameObject spawnDefense(GameObject defense, Vector2 pos) {
        //  create a new holder
        if(!usedNames.Contains(defense.GetComponent<Buyable>().title.ToString())) {
            var h = Instantiate(dHolder, transform);
            h.transform.localPosition = Vector3.zero;
            holders.Add(h);

            var obj = Instantiate(defense, h.transform);
            usedNames.Add(defense.GetComponent<Buyable>().title.ToString());
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(3.0f, 3.0f);
            h.GetComponent<CompositeCollider2D>().GenerateGeometry();

            return obj;
        }
        //  use an existing holder
        else {
            var h = holders[usedNames.IndexOf(defense.GetComponent<Buyable>().title.ToString())];
            var obj = Instantiate(defense, h.transform);
            obj.transform.position = pos;
            obj.transform.localScale = new Vector3(3.0f, 3.0f);
            h.GetComponent<CompositeCollider2D>().GenerateGeometry();

            return obj;
        }
    }
}
