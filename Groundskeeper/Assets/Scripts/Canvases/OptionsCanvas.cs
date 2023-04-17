using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsCanvas : MenuCanvas {
    [SerializeField] Slider masterVolSlider, musicVolSlider, sfxVolSlider;
    [SerializeField] TextMeshProUGUI screenModeText, targetFPSText;
    [SerializeField] Toggle vsyncToggle;
    [SerializeField] GameObject background;
    FullScreenMode curScreenMode;
    GameOptions.TargetFrameRate tFPS;

    List<Selectable> obscured = new List<Selectable>();


    private void Start() {
        setup();
        background.SetActive(false);
    }

    public void setup() {
        var o = GameInfo.getGameOptions();
        //  sound
        masterVolSlider.value = o.masterVol;
        musicVolSlider.value = o.musicVol;
        sfxVolSlider.value = o.sfxVol;

        //  video
        QualitySettings.vSyncCount = o.vSync ? 1 : 0;
        curScreenMode = o.screenMode;
        screenModeText.text = curScreenMode == FullScreenMode.ExclusiveFullScreen ? "Fullscreen" : curScreenMode == FullScreenMode.FullScreenWindow ? "Windowed Fullscreen" :
            curScreenMode == FullScreenMode.MaximizedWindow ? "Borderless Window" : "Windowed";
        tFPS = o.targetFPS;
        targetFPSText.text = tFPS == GameOptions.TargetFrameRate.Unlimited ? "Unlimited" : tFPS == GameOptions.TargetFrameRate.Thirty ? "30" :
            tFPS == GameOptions.TargetFrameRate.Sixty ? "60" : "120";
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;
    }

    public void applyChanges() {
        Screen.fullScreenMode = curScreenMode;
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        Application.targetFrameRate = getDesiredTargetFrameRate();

        //  Save settings
        var o = new GameOptions(masterVolSlider.value, musicVolSlider.value, sfxVolSlider.value, curScreenMode, vsyncToggle.isOn, tFPS);
        GameInfo.saveGameOptions(o);
        //  Apply settings
        if(FindObjectOfType<AudioManager>() != null)
            FindObjectOfType<AudioManager>().updateVolume();
    }

    public void resetOptions() {
        GameInfo.resetGameOptions();
        setup();
    }

    protected override void show() {
        //  uninteracts all buttons so that the navigation doesn't get fucked
        foreach(var i in FindObjectsOfType<Selectable>()) {
            if(i.interactable) {
                i.interactable = false;
                obscured.Add(i);
            }
        }
        if(FindObjectOfType<MortalUnit>() != null)
            Time.timeScale = 0.0f;
        background.SetActive(true);
        masterVolSlider.Select();

    }
    protected override void close() {
        foreach(var i in obscured)
            i.interactable = true;
        obscured.Clear();
        Time.timeScale = 1.0f;
        background.SetActive(false);
    }

    GameOptions.TargetFrameRate getCurrentTargetFrameRate() {
        switch(Application.targetFrameRate) {
            case 30: return GameOptions.TargetFrameRate.Thirty;
            case 60: return GameOptions.TargetFrameRate.Sixty;
            case 120: return GameOptions.TargetFrameRate.OneTwenty;
            default: return GameOptions.TargetFrameRate.Unlimited;
        }
    }
    int getDesiredTargetFrameRate() {
        switch(tFPS) {
            case GameOptions.TargetFrameRate.Thirty: return 30;
            case GameOptions.TargetFrameRate.Sixty: return 60;
            case GameOptions.TargetFrameRate.OneTwenty: return 120;
            default: return -1;
        }
    }

    //buttons
    public void changeScreenMode(bool right) {
        int cur = (int)curScreenMode;
        cur = right ? cur + 1 : cur - 1;
        if(cur < 0)
            cur = 3;
        else if(cur > 3)
            cur = 0;
        curScreenMode = (FullScreenMode)cur;
        screenModeText.text = curScreenMode == FullScreenMode.ExclusiveFullScreen ? "Fullscreen" : curScreenMode == FullScreenMode.FullScreenWindow ? "Windowed Fullscreen" :
            curScreenMode == FullScreenMode.MaximizedWindow ? "Borderless Window" : "Windowed";
    }
    public void changeTargetFPS(bool right) {
        int cur = (int)tFPS;
        cur = right ? cur + 1 : cur - 1;
        if(cur < 0)
            cur = 3;
        else if(cur > 3)
            cur = 0;
        tFPS = (GameOptions.TargetFrameRate)cur;
        targetFPSText.text = tFPS == GameOptions.TargetFrameRate.Unlimited ? "Unlimited" : tFPS == GameOptions.TargetFrameRate.Thirty ? "30" :
            tFPS == GameOptions.TargetFrameRate.Sixty ? "60" : "120";
    }
}
