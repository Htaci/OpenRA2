using System;
using TriLibCore.General;
using TriLibCore.Mappers;
using UnityEditor;
using UnityEngine;


namespace TriLibCore.Editor
{
    [CustomEditor(typeof(ByNameHumanoidAvatarMapper))]
    public class ByNameHumanoidAvatarMapperEditor : UnityEditor.Editor
    {
        private bool[] _folded = new bool[2];

        public override void OnInspectorGUI()
        {
            _folded[0] = EditorGUILayout.BeginFoldoutHeaderGroup(_folded[0], "String Comparison");
            if (_folded[0])
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("stringComparisonMode"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("CaseInsensitive"));
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            _folded[1] = EditorGUILayout.BeginFoldoutHeaderGroup(_folded[1], "Bones Mapping");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (_folded[1])
            {
                var bonesMapping = serializedObject.FindProperty("BonesMapping");
                for (var i = 0; i < bonesMapping.arraySize; i++)
                {
                    var boneMapping = bonesMapping.GetArrayElementAtIndex(i);
                    var humanBone = boneMapping.FindPropertyRelative("HumanBone");
                    var enumDisplayNames = humanBone.enumDisplayNames;
                    var boneNames = boneMapping.FindPropertyRelative("BoneNames");
                    EditorGUILayout.PropertyField(boneNames, new GUIContent(enumDisplayNames[humanBone.enumValueIndex]));
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}