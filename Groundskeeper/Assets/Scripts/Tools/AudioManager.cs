using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour {
    [SerializeField] AudioSource effectPlayer, musicPlayer, ambientPlayer;
    [SerializeField] AudioClip[] playOnStartAndKeepPlayingMusic;
    [SerializeField] AudioClip[] playOnStartAndKeepPlayingAmbient;


    List<AudioClip> playedClips = new List<AudioClip>();
    float normPitch = 1f, normVolume, normMusicVol;

    float distCutOff = 35f;


    private void Awake() {
        effectPlayer.volume = 0f;
        musicPlayer.volume = 0f;
        ambientPlayer.volume = 0f;
        StartCoroutine(refreshPlaylist());
    }

    public void updateVolume() {
        var o = GameInfo.getGameOptions();
        if(FindObjectOfType<PlayerInstance>() == null)
            musicPlayer.volume = o.masterVol * o.musicVol;
        effectPlayer.volume = o.masterVol * o.sfxVol;
        ambientPlayer.volume = o.masterVol * o.sfxVol;
        normVolume = o.masterVol * o.sfxVol;
        normMusicVol = o.masterVol * o.musicVol;
    }

    public void playSound(AudioClip clip, Vector2 pos, bool randomize = true) {
        if(playedClips.Contains(clip) || clip == null)
            return;
        //  lower volume based on distance
        var dist = Vector2.Distance(Camera.main.transform.position, pos);
        if(dist > distCutOff)
            return;
        dist = distCutOff - dist;
        dist = dist / distCutOff;

        effectPlayer.volume = normVolume * dist;
        //  randomize the pitch
        effectPlayer.pitch = normPitch;
        if(randomize)
            effectPlayer.pitch = Random.Range(0.6f, 1.25f);

        effectPlayer.PlayOneShot(clip);
        playedClips.Add(clip);
    }
    public void playMusic(AudioClip clip, bool repeat) {
        if(!repeat)
            musicPlayer.PlayOneShot(clip);
        else
            StartCoroutine(musicRepeater(clip));
    }
    public void playAmbient(AudioClip clip, bool repeat) {
        if(!repeat)
            ambientPlayer.PlayOneShot(clip);
        else
            StartCoroutine(ambientRepeater(clip));
    }
    public void stopMusic() {
        musicPlayer.Stop();
    }

    public void reduceAllVolume(float dur) {
        DOTween.To(() => effectPlayer.volume, x => effectPlayer.volume = x, 0f, dur);
        DOTween.To(() => musicPlayer.volume, x => musicPlayer.volume = x, 0f, dur);
        DOTween.To(() => ambientPlayer.volume, x => ambientPlayer.volume = x, 0f, dur);
    }
    public void increaseAllVolume(float dur) {
        var o = GameInfo.getGameOptions();
        float effTarget = o.masterVol * o.sfxVol;
        float musTarget = o.masterVol * o.musicVol;
        DOTween.To(() => effectPlayer.volume, x => effectPlayer.volume = x, effTarget, dur);
        if(FindObjectOfType<PlayerInstance>() == null)
            DOTween.To(() => musicPlayer.volume, x => musicPlayer.volume = x, musTarget, dur);
        DOTween.To(() => ambientPlayer.volume, x => ambientPlayer.volume = x, effTarget, dur);
        StartCoroutine(volumeSetup(dur));
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
    IEnumerator ambientRepeater(AudioClip clip) {
        ambientPlayer.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        StartCoroutine(ambientRepeater(clip));
    }
    IEnumerator volumeSetup(float dur) {
        foreach(var i in playOnStartAndKeepPlayingMusic)
            playMusic(i, true);
        foreach(var i in playOnStartAndKeepPlayingAmbient)
            playAmbient(i, true);
        yield return new WaitForSeconds(dur);
        var o = GameInfo.getGameOptions();
        normVolume = o.masterVol * o.sfxVol;
        normMusicVol = o.masterVol * o.musicVol;
    }

    //  sets music volume using a percentage
    public void setMusicVolume(float perc, float dur) {
        if(dur <= 0f)
            musicPlayer.volume = normMusicVol * perc;
        else {
            var t = normMusicVol * perc;
            DOTween.To(() => musicPlayer.volume, x => musicPlayer.volume = x, t, dur);
        }
    }
}
