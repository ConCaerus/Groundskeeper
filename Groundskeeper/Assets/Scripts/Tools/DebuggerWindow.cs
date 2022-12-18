using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


public class DebuggerWindow : EditorWindow {

    [MenuItem("Window/Debugger")]
    public static void showWindow() {
        GetWindow<DebuggerWindow>("Debugger");
    }


    private void OnGUI() {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("+100s"))
            GameInfo.addSouls(100);
        if(GUILayout.Button("+1000s"))
            GameInfo.addSouls(1000);
        if(GUILayout.Button("+100000s"))
            GameInfo.addSouls(100000);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Save Board"))
            FindObjectOfType<GameBoard>().saveBoard();
        if(GUILayout.Button("Clear Board Save"))
            GameInfo.clearBoard();
        if(GUILayout.Button("Clear Save")) {
            SaveData.deleteCurrentSave();
            GameInfo.resetSave();
        }
        GUILayout.EndHorizontal();

        GUILayout.Label("Info");
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Night Count"))
            Debug.Log("Night: " + GameInfo.getNightCount());
        if(GUILayout.Button("Inc Night"))
            GameInfo.addNights(1);
        if(GUILayout.Button("Coin Count"))
            Debug.Log("Night: " + GameInfo.getSouls());
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Unlock Everything"))
            FindObjectOfType<BuyableLibrary>().unlockAll();
        GUILayout.EndHorizontal();
    }
}

#endif