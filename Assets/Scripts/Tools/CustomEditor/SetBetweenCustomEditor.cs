using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FactWorld.Tools
{
    [CustomEditor(typeof(SetBetween))]
    public class SetBetweenCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SetBetween setBetween = (SetBetween)target;
            EditorGUILayout.Space();
            if (GUILayout.Button("Set"))
            {
                setBetween.Set();
            }
        }
    }
}
