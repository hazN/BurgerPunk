using UnityEditor;
using UnityEngine;
using koljo45.MeshTriangleSeparator;
using System.Collections;

[CustomEditor (typeof(EditorChopHandelerBase), true)]
[CanEditMultipleObjects]
public class NewBehaviourScript : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        if (GUILayout.Button("Optimize"))
            foreach (Object obj in serializedObject.targetObjects)
            {
                EditorChopHandelerBase target = obj as EditorChopHandelerBase;
                target.optimizeMesh();
            }
        if (GUILayout.Button("Slice"))
            foreach (Object obj in serializedObject.targetObjects)
            {
                EditorChopHandelerBase target = obj as EditorChopHandelerBase;
                target.sliceMesh();
            }

        serializedObject.ApplyModifiedProperties();
    }
}
