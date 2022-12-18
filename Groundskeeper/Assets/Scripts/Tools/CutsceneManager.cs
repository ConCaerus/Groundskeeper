using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CutsceneManager : MonoBehaviour {
    [SerializeField] Image cover;
    [SerializeField] float introWaitTime;
    [SerializeField] sceneInfo[] scenes;

    public enum transitionType {
        blackout, whiteout, fadeout
    }

    [System.Serializable]
    public struct sceneInfo {
        public GameObject holder;
        public float durationNegation;
        public transitionType transition;
        public float transitionDuration;
    }



    private void Start() {
        DOTween.Init();
        GetComponent<Canvas>().worldCamera = Camera.main;
        cover.gameObject.SetActive(true);
        cover.color = Color.black;

        foreach(var i in scenes) {
            i.holder.SetActive(false);
        }
        scenes[0].holder.SetActive(true);

        StartCoroutine(runCutscene());
    }


    IEnumerator runCutscene() {
        cover.DOColor(Color.clear, introWaitTime);
        yield return new WaitForSeconds(introWaitTime);


        int index = 0;
        while(index < scenes.Length) {
            scenes[index].holder.SetActive(true);
            float duration = 0.0f;
            if(scenes[index].holder.GetComponent<Animator>() != null)
                duration = scenes[index].holder.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.averageDuration - scenes[index].durationNegation;
            else
                duration = scenes[index].transitionDuration;
            yield return new WaitForSeconds(duration);

            //  transitions
            switch(scenes[index].transition) {
                case transitionType.blackout:
                    cover.DOKill();
                    cover.DOColor(Color.black, scenes[index].transitionDuration);
                    yield return new WaitForSeconds(scenes[index].transitionDuration * scenes[index].transitionDuration);
                    break;

                case transitionType.whiteout:
                    cover.DOKill();
                    cover.DOColor(Color.white, scenes[index].transitionDuration);
                    yield return new WaitForSeconds(scenes[index].transitionDuration * scenes[index].transitionDuration);
                    break;

                case transitionType.fadeout:
                    foreach(var i in scenes[index].holder.GetComponentsInChildren<Image>()) {
                        i.DOColor(Color.clear, scenes[index].transitionDuration);
                    }

                    if(index + 1 < scenes.Length) {
                        var nextScene = scenes[index + 1];
                        nextScene.holder.SetActive(true);
                        foreach(var i in nextScene.holder.GetComponentsInChildren<Image>()) {
                            var realColor = i.color;
                            i.color = Color.clear;
                            i.DOColor(realColor, scenes[index].transitionDuration);
                        }
                        foreach(var i in nextScene.holder.GetComponentsInChildren<TextMeshProUGUI>()) {
                            var realColor = i.color;
                            i.color = Color.clear;
                            i.DOColor(realColor, scenes[index].transitionDuration);
                        }
                    }

                    yield return new WaitForSeconds(scenes[index].transitionDuration);
                    break;
            }
            scenes[index].holder.SetActive(false);

            index++;
        }
    }
}
