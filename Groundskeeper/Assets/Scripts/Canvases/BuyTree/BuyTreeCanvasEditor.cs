using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


[CustomEditor(typeof(BuyTreeCanvas))]
public class BuyTreeCanvasEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        var target = FindObjectOfType<BuyTreeCanvas>();

        if(GUILayout.Button("Reset Tree")) {
            target.resetTree();
            Debug.Log("Did shit");
        }
    }
}

#endif