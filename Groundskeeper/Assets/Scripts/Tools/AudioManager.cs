using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] AudioSource effectPlayer, musicPlayer;
    [SerializeField] AudioClip[] playOnStartAndKeepPlaying;


    List<AudioClip> playedClips = new List<AudioClip>();


    private void Awake() {
        StartCoroutine(refreshPlaylist());
    }

    private void Start() {
        updateVolume();
        foreach(var i in playOnStartAndKeepPlaying)
            playMusic(i, true);
    }

    public void updateVolume() {
        var o = GameInfo.getGameOptions();
        musicPlayer.volume = 1.0f * (o.masterVol * o.musicVol);
        effectPlayer.volume = 1.0f * (o.masterVol * o.sfxVol);
    }

    public void playSound(AudioClip clip, Vector2 pos, bool randomize = true) {
        foreach(var i in playedClips) {
            if(i == clip)
                return;
        }

        transform.position = pos;

        if(randomize)
            randomizePitch();
        effectPlayer.PlayOneShot(clip);
        playedClips.Add(clip);
    }
    public void playMusic(AudioClip clip, bool repeat) {
        if(!repeat)
            musicPlayer.PlayOneShot(clip);
        else 
            StartCoroutine(musicRepeater(clip));
    }
    public void stopMusic() {
        musicPlayer.Stop();
    }

    public void randomizePitch() {
        effectPlayer.pitch = Random.Range(0.6f, 1.25f);
    }

    IEnumerator refreshPlaylist() {
        yield return new WaitForEndOfFrame();

        playedClips.Clear();
        StartCoroutine(refreshPlaylist());
    }
    IEnumerator musicRepeater(AudioClip clip) {
        musicPlayer.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        StartCoroutine(musicRepeater(clip));
    }
}
