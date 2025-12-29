using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

namespace TriLibCore.Editor
{
    public class TriLibVersionNotes : EditorWindow
    {
        private class Styles
        {
            public static readonly GUIStyle HeaderStyle = new GUIStyle("label") { fontSize = 19, fontStyle = FontStyle.Bold, margin = new RectOffset(10, 10, 5, 5) };
            public static readonly GUIStyle SubHeaderStyle = new GUIStyle("label") { margin = new RectOffset(10, 10, 5, 5), fontStyle = FontStyle.Bold };
            public static readonly GUIStyle TextStyle = new GUIStyle("label") { margin = new RectOffset(20, 20, 5, 5) };
            public static readonly GUIStyle TextAreaStyle = new GUIStyle(TextStyle) { wordWrap = true };
            public static readonly GUIStyle ButtonStyle = new GUIStyle("button") { margin = new RectOffset(10, 10, 5, 5) };
        }

        private string _text;
        private bool _loaded;
        private Vector2 _scrollPosition;

        private static readonly string ChangelogPattern = @"(?<=Changelog:)(.*?)(?=(Version Notes:|$))";
        private static readonly string VersionNotesPattern = @"(?<=Version Notes:)(.*)";
        private static readonly string Pattern = @"(https?://[^\s]+)";
        private static readonly Regex URIRegex = new Regex(@"^https?://");

        private static TriLibVersionNotes Instance
        {
            get
            {
                var window = GetWindow<TriLibVersionNotes>();
                window.titleContent = new GUIContent("TriLib Version Notes");
                return window;
            }
        }


        public static void ShowWindow()
        {
            Instance.Show();
        }

        private void OnDestroy()
        {
            EditorPrefs.SetBool(TriLibVersionInfo.Instance.SkipVersionInfoKey, true);
        }

        private void OnGUI()
        {
            if (!_loaded)
            {
                var guids = AssetDatabase.FindAssets("TriLibReleaseNotes");
                if (guids.Length > 0)
                {
                    var guid = guids[0];
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                    if (textAsset == null || textAsset.text == null)
                    {
                        AssetDatabase.Refresh();
                        textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                        if (textAsset == null)
                        {
                            Close();
                        }
                        return;
                    }
                    _text = textAsset.text.Replace("\\n", "\n");
                }
                else
                {
                    Close();
                }
                _loaded = true;
            }
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            var changelogMatch = Regex.Match(_text, ChangelogPattern, RegexOptions.Singleline);
            var changelogSection = changelogMatch.Success ? changelogMatch.Value.Trim() : "No changelog found";
            var versionNotesMatch = Regex.Match(_text, VersionNotesPattern, RegexOptions.Singleline);
            var versionNotesSection = versionNotesMatch.Success ? versionNotesMatch.Value.Trim() : "No version notes found";
            GUILayout.Label("Version Notes", Styles.SubHeaderStyle);
            var groups = Regex.Split(versionNotesSection, Pattern);
            foreach (var group in groups)
            {
                if (!string.IsNullOrEmpty(group))
                {
                    if (URIRegex.IsMatch(group))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        if (EditorGUILayout.LinkButton(group))
                        {
                            Application.OpenURL(group);
                        }
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.TextArea(group, Styles.TextAreaStyle);
                    }
                }
            }
            EditorGUILayout.Space();
            GUILayout.Label("Changelog", Styles.SubHeaderStyle);
            EditorGUILayout.TextArea(changelogSection, Styles.TextAreaStyle);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            GUILayout.Label("You can show this window on the Project Settings/TriLib area", Styles.SubHeaderStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", Styles.ButtonStyle))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
