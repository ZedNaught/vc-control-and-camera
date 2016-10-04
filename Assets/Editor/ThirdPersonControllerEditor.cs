using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ThirdPersonController))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Set Target to null")) {
            ThirdPersonController.ActiveUnit = null;
        }

        if (GUILayout.Button("Set Target to Player")) {
            Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
            ThirdPersonController.ActiveUnit = player;
        }

        if (GUILayout.Button("Set Target to FriendlyUnit")) {
            Unit player = GameObject.FindGameObjectWithTag("FriendlyUnit").GetComponent<Unit>();
            ThirdPersonController.ActiveUnit = player;
        }
    }
}
