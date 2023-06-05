using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SoulTransactionHandler : MonoBehaviour {
    [SerializeField] AudioClip buySound, buyFailSound;

    List<CostText> costTexts = new List<CostText>();
    class CostText {
        public TextMeshProUGUI mainText;
        public TextMeshProUGUI[] subTexts = new TextMeshProUGUI[3];

        public CostText(TextMeshProUGUI m) {
            mainText = m;

            for(int i = 0; i < subTexts.Length; i++) {
                subTexts[i] = Instantiate(mainText.gameObject, m.transform.position, Quaternion.identity, m.transform).GetComponent<TextMeshProUGUI>();
                for(int c = 0; c < subTexts[i].transform.childCount; c++)
                    Destroy(subTexts[i].transform.GetChild(c).gameObject);
                subTexts[i].text = "";
                subTexts[i].color = Color.clear;
            }
        }

        public TextMeshProUGUI getNextAvailableSubText() {
            var lowestText = subTexts[0];
            foreach(var i in subTexts) {
                if(i.color == Color.clear)
                    return i;
                else if(lowestText.color.a > i.color.a)
                    lowestText = i;
            }

            //  gets the lowest text ready to be the next one
            lowestText.DOComplete();
            lowestText.GetComponent<RectTransform>().DOComplete();
            lowestText.GetComponent<RectTransform>().position = mainText.GetComponent<RectTransform>().position;
            return lowestText;
        }
    }

    private void Start() {
        DOTween.Init();
    }

    public bool tryTransaction(float cost, TextMeshProUGUI soulsText, bool saveSouls, bool playSound) {
        soulsText.DOComplete();
        soulsText.transform.DOComplete();

        float poorTime = .25f, richTime = .5f;

        //  player ain't bitchin'
        if(cost > GameInfo.getSouls(false)) {
            soulsText.color = Color.red;
            soulsText.transform.DOPunchScale(new Vector3(.25f, .25f), poorTime);
            soulsText.DOColor(Color.white, poorTime);
            if(playSound)
                FindObjectOfType<AudioManager>().playSound(buyFailSound, Camera.main.transform.position, true);
            return false;
        }

        //  player do be bitchin'
        //  transaction
        float prevSouls = GameInfo.getSouls(false);
        GameInfo.addSouls(-cost, saveSouls);
        float aftSouls = GameInfo.getSouls(false);
        if(playSound)
            FindObjectOfType<AudioManager>().playSound(buySound, Camera.main.transform.position, true);

        //  make spent texts
        //  checks if already has a costText group for the soulsText

        //  flair
        if(cost != 0f) {
            var spentText = Instantiate(soulsText.gameObject, soulsText.transform.position, Quaternion.identity, soulsText.transform).GetComponent<TextMeshProUGUI>();
            for(int c = 0; c < spentText.transform.childCount; c++)
                Destroy(spentText.transform.GetChild(c).gameObject);
            Destroy(spentText.gameObject, richTime * 2f);
            spentText.color = cost >= 0f ? Color.red : Color.green;
            spentText.text = (aftSouls - prevSouls).ToString("0.0");
            spentText.GetComponent<RectTransform>().DOLocalMoveY(-50f, richTime * 2f);
            spentText.GetComponent<RectTransform>().DOScale(.5f, richTime * 2f);
            spentText.DOColor(Color.clear, richTime * 2f);



            DOTween.To(() => prevSouls, x => prevSouls = x, aftSouls, richTime).OnUpdate(() => {
                soulsText.text = prevSouls.ToString("0.0") + "s";
            });
        }

        return true;
    }
}
