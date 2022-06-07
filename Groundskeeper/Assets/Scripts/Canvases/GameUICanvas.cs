using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUICanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI waveCount;


    public void updateCount() {
        waveCount.text = GameInfo.wave.ToString() + "/" + GameInfo.wavesPerNight().ToString();
    }
}
