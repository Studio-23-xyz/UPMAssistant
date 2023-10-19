using UnityEngine;
using UnityEditor;
using System.IO;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class FileEditorWindowController : EditorWindow
    {
        private static string markupText = ""; 
        private static string filePath = "";  
        
        public static void ShowWindow(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                filePath = path;
                var window =GetWindow<FileEditorWindowController>("UPM Editor Window");
                window.minSize = new Vector2(600, 600); 
                markupText = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File path is empty!");
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("UPM Editor", EditorStyles.boldLabel);
            GUILayout.Label($"File Location: {filePath}", EditorStyles.label);
            
            markupText = EditorGUILayout.TextArea(markupText, GUILayout.Height(500));
             
            var btnStatus = !string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(markupText);
            EditorGUI.BeginDisabledGroup(!btnStatus);
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save", GUILayout.Height(40)))
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    File.WriteAllText(filePath, markupText);
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