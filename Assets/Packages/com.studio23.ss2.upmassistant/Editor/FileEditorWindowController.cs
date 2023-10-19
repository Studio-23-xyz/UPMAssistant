using UnityEngine;
using UnityEditor;
using System.IO;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class FileEditorWindowController : EditorWindow
    {
        private static string _markupText = ""; 
        private static string _filePath = "";  
        
        public static void ShowWindow(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                _filePath = path;
                var window =GetWindow<FileEditorWindowController>("UPM Editor Window");
                window.minSize = new Vector2(600, 600); 
                _markupText = File.ReadAllText(_filePath);
            }
            else
            {
                Debug.LogError("File path is empty!");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("UPM Editor", EditorStyles.boldLabel);
            GUILayout.Label($"File Location: {_filePath}", EditorStyles.label);
            
            _markupText = EditorGUILayout.TextField(_markupText, GUILayout.Height(500));
             
            var btnStatus = !string.IsNullOrEmpty(_filePath) && !string.IsNullOrEmpty(_markupText);
            EditorGUI.BeginDisabledGroup(!btnStatus);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(_filePath))
                {
                    File.WriteAllText(_filePath, _markupText);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogError("File path is empty!");
                }
            }
            GUI.backgroundColor = Color.white; // Reset the background color
            EditorGUI.EndDisabledGroup();
        }
    }
}