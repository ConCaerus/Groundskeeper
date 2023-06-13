using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour {
    TransitionCanvas tc;

    VideoPlayer vp;

    private void Awake() {
        tc = FindObjectOfType<TransitionCanvas>();
        vp = GetComponent<VideoPlayer>();

        StartCoroutine(waitToPlay());
    }

    IEnumerator waitToPlay() {
        while(!tc.finishedLoading)
            yield return new WaitForEndOfFrame();

        vp.Play();

        yield return new WaitForSeconds((float)vp.clip.length);

        tc.loadScene("Game");
    }
}
