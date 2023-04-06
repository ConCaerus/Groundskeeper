//C# Example (LookAtPointEditor.cs)
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PresetLibrary))]
public class PresetLibraryEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var p = (PresetLibrary)target;

        if(GUILayout.Button("Sort Monsters")) {
            p.sortMonsters();
            Debug.Log("monsters sorted");
        }
    }
}
#endif