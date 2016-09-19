using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Set Target to null")) {
            GameManager.instance.ActiveUnit = null;
        }

        if (GUILayout.Button("Set Target to Player")) {
            Unit player = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
            GameManager.instance.ActiveUnit = player;
        }

        if (GUILayout.Button("Set Target to FriendlyUnit")) {
            Unit player = GameObject.FindGameObjectWithTag("FriendlyUnit").GetComponent<Unit>();
            GameManager.instance.ActiveUnit = player;
        }
    }
}
