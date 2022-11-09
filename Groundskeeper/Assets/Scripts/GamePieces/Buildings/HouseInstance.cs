using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HouseInstance : Building {
    [HideInInspector]
    [SerializeField] public float viewDist = 80f;
    [SerializeField] GameObject arrow;

    List<MortalUnit> inTopUnits = new List<MortalUnit>();   //  fuckers that want the house to hide it's top

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Monster") {
            var c = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().DOComplete();
            GetComponent<SpriteRenderer>().DOColor(new Color(c.r, c.g, c.b, .5f), .15f);
            inTopUnits.Add(col.gameObject.GetComponent<MortalUnit>());
        }
    }


    private void OnTriggerExit2D(Collider2D col) {
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Monster") {
            inTopUnits.Remove(col.gameObject.GetComponent<MortalUnit>());
            for(int i = inTopUnits.Count - 1; i >= 0; i--) {
                if(inTopUnits[i] == null || inTopUnits[i].health <= 0f)
                    inTopUnits.RemoveAt(i);
            }
            if(inTopUnits.Count == 0) {
                var c = GetComponent<SpriteRenderer>().color;
                GetComponent<SpriteRenderer>().DOComplete();
                GetComponent<SpriteRenderer>().DOColor(new Color(c.r, c.g, c.b, 1f), .25f);
            }
        }
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
            showDoorArrow();
    }

    public void showDoorArrow() {
        arrow.SetActive(true);
        StartCoroutine(doorArrowAnim());
    }

    IEnumerator doorArrowAnim() {
        float t = .5f, m = 1.5f;
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y - m, t);
        yield return new WaitForSeconds(t);
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y + m, t);
        yield return new WaitForSeconds(t);
        StartCoroutine(doorArrowAnim());
    }

    public override void die() {
        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        Destroy(gameObject);
    }
}
