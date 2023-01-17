using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HouseInstance : Building {
    [HideInInspector]
    [SerializeField] public float viewDist = 80f;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject bloodParticles;
    [SerializeField] public CircleCollider2D radiusCol;
    [SerializeField] public GameObject playerSpawnPos;

    List<MortalUnit> inTopUnits = new List<MortalUnit>();   //  fuckers that want the house to hide it's top

    private void OnTriggerEnter2D(Collider2D col) {
        //  let the house be clear
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Monster") {
            var c = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().DOBlendableColor(new Color(c.r, c.g, c.b, 0.5f), .25f);
            inTopUnits.Add(col.gameObject.GetComponent<MortalUnit>());
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        //  let the house not be clear anymore
        if(col.gameObject.tag == "Player" || col.gameObject.tag == "Monster") {
            removeUnitFromInTopUnits(col.gameObject.GetComponent<MortalUnit>());
        }
    }

    private void Start() {
        FindObjectOfType<EnvironmentManager>().hideAllEnvAroundArea(transform.position, 10f);
    }

    public void removeUnitFromInTopUnits(MortalUnit unit) {
        inTopUnits.Remove(unit);
        for(int i = inTopUnits.Count - 1; i >= 0; i--) {
            if(inTopUnits[i] == null || inTopUnits[i].health <= 0f)
                inTopUnits.RemoveAt(i);
        }
        if(inTopUnits.Count == 0) {
            var c = GetComponent<SpriteRenderer>().color;
            GetComponent<SpriteRenderer>().DOBlendableColor(new Color(c.r, c.g, c.b, 1f), .25f);
        }
    }

    public Vector2 getCenter() {
        return transform.position;
    }

    public void showDoorArrow() {
        arrow.SetActive(true);
        var or = arrow.transform.localScale;
        arrow.transform.localScale = Vector3.zero;
        arrow.transform.DOScale(or, .25f);
        StartCoroutine(doorArrowAnim(.25f));
    }

    IEnumerator doorArrowAnim(float waitForArrowToShowTime = 0.0f) {
        yield return new WaitForSeconds(waitForArrowToShowTime);
        float t = .5f, m = 1.5f;
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y - m, t);
        yield return new WaitForSeconds(t);
        arrow.transform.DOLocalMoveY(arrow.transform.localPosition.y + m, t);
        yield return new WaitForSeconds(t);
        StartCoroutine(doorArrowAnim());
    }

    public override GameObject getBloodParticles() {
        return bloodParticles;
    }
    public override Color getStartingColor() {
        return Color.white;
    }

    public override void die() {
        GameInfo.playing = false;
        FindObjectOfType<GameOverCanvas>().show();
        Destroy(gameObject);
    }
}
