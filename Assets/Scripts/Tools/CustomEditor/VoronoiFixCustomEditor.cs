using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FactWorld.Tools
{
    [CustomEditor(typeof(VoronoiFix))]
    public class VoronoiFixCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            VoronoiFix voronoi = (VoronoiFix)target;
            EditorGUILayout.Space();
            if(GUILayout.Button("Generate texture"))
            {
                voronoi.Create();
            }
        }
    }
}
