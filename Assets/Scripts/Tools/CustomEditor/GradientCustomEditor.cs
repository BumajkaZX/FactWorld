using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FactWorld.Tools
{
    [CustomEditor(typeof(Gradient))]
    public class GradientCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Gradient gradient = (Gradient)target;
            EditorGUILayout.Space();
            if(GUILayout.Button("Generate texture"))
            {
                gradient.Create();
            }
        }
    }
}
