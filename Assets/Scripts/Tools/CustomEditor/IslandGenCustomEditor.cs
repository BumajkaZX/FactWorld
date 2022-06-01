using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FactWorld.Tools
{
    [CustomEditor(typeof(IslandGen))]
    public class IslandGenCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            IslandGen islandGen = (IslandGen)target;
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate island"))
            {
                islandGen.GenerateIsland();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Delete island"))
            {
                islandGen.DeleteIsland();
            }

        }
    }
}
