using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsCanvas : MonoBehaviour {
    [SerializeField] Slider masterVolSlider, musicVolSlider, sfxVolSlider;
    [SerializeField] TextMeshProUGUI screenModeText;
    [SerializeField] Toggle vsyncToggle;
    FullScreenMode curMode;


    private void Start() {
        setup();
    }

    public void setup() {
        //  sound
        var temp = GameInfo.getVolumeOptions();
        masterVolSlider.value = temp[0];
        musicVolSlider.value = temp[1];
        sfxVolSlider.value = temp[2];

        //  video
        QualitySettings.vSyncCount = GameInfo.getVsync() ? 1 : 0;
        curMode = GameInfo.getScreenMode();
        screenModeText.text = curMode == FullScreenMode.ExclusiveFullScreen ? "Fullscreen" : curMode == FullScreenMode.FullScreenWindow ? "Windowed Fullscreen" :
            curMode == FullScreenMode.MaximizedWindow ? "Borderless Window" : "Windowed";
        vsyncToggle.isOn = QualitySettings.vSyncCount == 1;
    }

    public void applyChanges() {
        //  sound
        GameInfo.setVolumeOptions(masterVolSlider.value, musicVolSlider.value, sfxVolSlider.value);
        if(FindObjectOfType<AudioManager>() != null)
            FindObjectOfType<AudioManager>().updateVolume();

        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
        GameInfo.setVsync(vsyncToggle.isOn);
        GameInfo.setScreenMode(curMode);
        Screen.fullScreenMode = curMode;
    }

    public void resetOptions() {
        GameInfo.resetVolumeOptions();
        setup();
    }

    //buttons
    public void changeScreenMode(bool right) {
        int cur = (int)curMode;
        cur = right ? cur + 1 : cur - 1;
        if(cur < 0)
            cur = 3;
        else if(cur > 3)
            cur = 0;
        curMode = (FullScreenMode)cur;
        screenModeText.text = curMode == FullScreenMode.ExclusiveFullScreen ? "Fullscreen" : curMode == FullScreenMode.FullScreenWindow ? "Windowed Fullscreen" :
            curMode == FullScreenMode.MaximizedWindow ? "Borderless Window" : "Windowed";
    }
}
