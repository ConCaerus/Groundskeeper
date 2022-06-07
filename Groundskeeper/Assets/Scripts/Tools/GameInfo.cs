using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo {
    public static bool playing { get; set; } = true;
    public static int coins { get; set; } = 0;
    public static int wave { get; set; } = 0;
    public static int wavesPerNight() { return 5; }
    public static int night { get; set; } = 0;
    public static int lastSeenEnemyIndex { get; set; } = 0;

    public static int monstersKilled { get; set; } = 0;
    public static int weightedMonstersKilled { get; set; } = 0;


    public enum EnemyType {
        Both, Physical, Spiritual
    }


    public static void resetVars() {
        playing = true;
        wave = 0;
        monstersKilled = 0;
        weightedMonstersKilled = 0;
    }

    public static Vector2 mousePos() {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static int calcCoins() {
        return (int)(weightedMonstersKilled / 2f);
    }
    public static void addCoins(int c) {
        coins += c;
    }
}
