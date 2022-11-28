using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SplineTrack))]
public class SplineTrackEditor : Editor
{
    SplineTrack _target;
    static Node selectedNode;
    static NodeViewMode viewMode = NodeViewMode.Local;
    static bool foldout = false;
    static ReorderableList NodeList;
    static bool SomeValueChanged = false;

    private void OnEnable()
    {
        _target = (SplineTrack)target;

        NodeList = new ReorderableList (serializedObject, serializedObject.FindProperty("Nodes"))
        {
            displayAdd = true,
            displayRemove = true,
            drawElementCallback = DrawElement,
            elementHeightCallback = GetElementHeight,
            drawHeaderCallback = DrawHeader,
        };
    }

    private void OnDisable()
    {
        _target = null;
        selectedNode = null;
    }

    private float GetElementHeight(int index)
    {
        return foldout ? ((EditorGUIUtility.singleLineHeight + 2) * 7) : 0;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Undo.RecordObject(_target, "Inspector");
        SomeValueChanged = false;

        bool newClose = EditorGUILayout.Toggle("Close Path", _target.Close);
        if(newClose != _target.Close)
        {
            _target.Close = newClose;
            _target.RegenerateSplines();
            SomeValueChanged = true;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("UseUniformDistances"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("PointsMultiplier"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("WidthSteps"));

        viewMode = (NodeViewMode)EditorGUILayout.EnumPopup("Handle coordinate space", viewMode);

        NodeList.DoLayoutList();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SplineTrack.Target)));

        EditorGUILayout.BeginHorizontal();
        bool RegenFull = GUILayout.Button("Regen All");
        if(GUILayout.Button("Bake Meshes"))
        {
            _target.BakePoints();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("Splines"));

        RegenFull |= serializedObject.ApplyModifiedProperties();

        if (RegenFull) _target.RegenerateSplines();

        if (SomeValueChanged || RegenFull) EditorUtility.SetDirty(target);
    }

    private void DrawHeader (Rect rect)
    {
        Rect drawRect = new Rect(rect.x + 10, rect.y, rect.width - 10, rect.height);

        foldout = EditorGUI.Foldout(drawRect, foldout, "Nodes");
    }

    private void DrawElement (Rect rect, int index, bool isActive, bool isFocused)
    {
        if (!foldout) return;

        SerializedProperty nodeProp = serializedObject.FindProperty("Nodes").GetArrayElementAtIndex(index);
        Node n = _target.Nodes[index];
        if (isFocused) selectedNode = n;

        if (MoveHandleInspector(rect, 0, "Point", n.Point, out n.Point))
        {
            SomeValueChanged = true;
        }

        Vector3 newFwd, newBack;

        switch(viewMode)
        {
            case NodeViewMode.Local:
                if(MoveHandleInspector(rect, 1, "Fwd (Local)", n.Fwd_Local, out newFwd))
                {
                    n.Fwd_Local = newFwd;
                    SomeValueChanged = true;
                }
                else if (MoveHandleInspector(rect, 2, "Back (Local)", n.Back_Local, out newBack))
                {
                    n.Back_Local = newBack;
                    SomeValueChanged = true;
                }
                break;
            default:
                if (MoveHandleInspector(rect, 1, "Fwd (World)", n.Fwd_World, out newFwd))
                {
                    n.Fwd_World = newFwd;
                    SomeValueChanged = true;
                }
                else if (MoveHandleInspector(rect, 2, "Back (World)", n.Back_World, out newBack))
                {
                    n.Back_World = newBack;
                    SomeValueChanged = true;
                }
                break;
        }

        float singleLineHeight = EditorGUIUtility.singleLineHeight + 2;
        float enumYPos = rect.y + 2 + (3 * singleLineHeight);
        var _nodeMode = (NodeMode)EditorGUI.EnumPopup(new Rect(rect.x, enumYPos, rect.width, singleLineHeight), "Handle Mode", n.Mode);
        if(_nodeMode != n.Mode)
        {
            n.Mode = _nodeMode;

            // Set Fwd_Local to itself to trigger the MoveOppositeNode method -
            // This will snap the Back node to a valid position if it wasn't before.
            n.Fwd_Local = n.Fwd_Local;
            SomeValueChanged = true;
        }

        SomeValueChanged |= FloatInspector(rect, 4, "Rotation", ref n.Rotation);
        SomeValueChanged |= FloatInspector(rect, 5, "Width", ref n.Width);
        SomeValueChanged |= FloatInspector(rect, 6, "Curvature", ref n.Curvature, true, - Mathf.PI, Mathf.PI);
    }

    private bool MoveHandleInspector (Rect rect, int fieldIndex, string name, Vector3 position, out Vector3 moved)
    {
        moved = position;

        float singleLineHeight = EditorGUIUtility.singleLineHeight + 2;
        float finalYPos = rect.y + 2 + (fieldIndex * singleLineHeight);

        Rect drawRect = new Rect(rect.x, finalYPos, rect.width, singleLineHeight);
        Vector3 afterMove = EditorGUI.Vector3Field(drawRect, name, position);

        if (afterMove != position)
        {
            moved = afterMove;
            return true;
        }

        return false;
    }

    private bool FloatInspector (Rect rect, int fieldIndex, string name, ref float targetValue, bool isRange = false, float min = 0, float max = 1)
    {
        float singleLineHeight = EditorGUIUtility.singleLineHeight + 2;
        float finalYPos = rect.y + 2 + (fieldIndex * singleLineHeight);

        Rect drawRect = new Rect(rect.x, finalYPos, rect.width, singleLineHeight);

        float newValue = isRange ?
            EditorGUI.Slider(drawRect, name, targetValue, min, max) :
            EditorGUI.FloatField(drawRect, name, targetValue);

        if(newValue != targetValue)
        {
            targetValue = newValue;
            return true;
        }
        return false;
    }

    private void OnSceneGUI()
    {
        foreach(Node n in _target.Nodes)
        {
            Color lastColor = Handles.color;

            if(n == selectedNode)
            {
                Undo.RecordObject(_target, "Move Node points");

                MoveHandle(n.Point, out n.Point);
                if(MoveHandle(n.Fwd_World, out Vector3 newFwd))
                {
                    n.Fwd_World = newFwd;
                }
                else if(MoveHandle(n.Back_World, out Vector3 newBack))
                {
                    n.Back_World = newBack;
                }
            }
            else
            {
                float hSize = HandleUtility.GetHandleSize(n.Point) * 0.25f;
                bool buttonPressed = Handles.Button(n.Point, Handles.matrix.rotation, hSize, hSize * 2, Handles.RectangleHandleCap);

                if (buttonPressed) selectedNode = n;
            }

            Handles.color = lastColor;
        }
    }

    private bool MoveHandle (Vector3 position, out Vector3 moved)
    {
        moved = position;
        Vector3 afterMove = Handles.DoPositionHandle(position, Quaternion.identity);

        if(afterMove != position)
        {
            moved = afterMove;
            return true;
        }

        return false;
    }
}
