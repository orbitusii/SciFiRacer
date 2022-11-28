using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("This is a Node object");
        base.OnInspectorGUI();
    }
}
