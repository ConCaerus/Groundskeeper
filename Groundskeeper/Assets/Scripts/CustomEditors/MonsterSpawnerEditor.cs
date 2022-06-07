using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MonsterSpawner))]
public class LevelScriptEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        var t = (MonsterSpawner)target;

        if(GUILayout.Button("Sort By Waves"))
            t.sortMonsters();
    }
}

#endif