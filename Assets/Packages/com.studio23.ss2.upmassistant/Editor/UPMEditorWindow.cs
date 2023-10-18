using UnityEngine;
using UnityEditor;
using System.IO;


namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMEditorWindow : EditorWindow
    {
        private string markupText = ""; // Variable to store the markup content
        private static string filePath = ""; // Variable to store the file path

        // [MenuItem("Studio-23/Markup Editor")]
        public static void ShowWindow(string filePath)
        {
            if(filePath == "")
                GetWindow<UPMEditorWindow>("UPM Editor Window");
        }

        private void OnGUI()
        {
            GUILayout.Label("Markup Editor", EditorStyles.boldLabel);

            // Text area to edit markup content
            markupText = EditorGUILayout.TextArea(markupText, GUILayout.Height(200));

            // Open button
            if (GUILayout.Button("Open Markup File"))
            {
                string newFilePath =
                    EditorUtility.OpenFilePanel("Open Markup File", "", "xml,json"); // Filter for XML and JSON files

                if (!string.IsNullOrEmpty(newFilePath))
                {
                    filePath = newFilePath;

                    // Read the content of the file and display it in the editor window
                    markupText = File.ReadAllText(filePath);
                }
            }

            // Save button
            GUI.enabled = !string.IsNullOrEmpty(filePath) && !string.IsNullOrEmpty(markupText);
            if (GUILayout.Button("Save Markup File"))
            {
                if (!string.IsNullOrEmpty(filePath))
                {
                    // Save the markup content to the file
                    File.WriteAllText(filePath, markupText);
                    AssetDatabase.Refresh();
                }
                else
                {
                    // If the file path is empty, prompt the user to select a file location
                    SaveFileDialog();
                }
            }

            GUI.enabled = true;
        }

        private void SaveFileDialog()
        {
            string newFilePath =
                EditorUtility.SaveFilePanel("Save Markup File", "", "NewMarkupFile",
                    "xml,json"); // Filter for XML and JSON files

            if (!string.IsNullOrEmpty(newFilePath))
            {
                filePath = newFilePath;

                // Save the markup content to the selected file
                File.WriteAllText(filePath, markupText);
                AssetDatabase.Refresh();
            }
        }
    }

}