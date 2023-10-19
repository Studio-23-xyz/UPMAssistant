using System;
using System.IO;
using System.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;


namespace Studio23.SS2.UPMAssistant.Editor
{
    public class PackageJsonWindowController : EditorWindow
    {
        private PackageJsonData jsonData; 
        
        private string warningMessage = ""; // Warning message to display
        private float warningDuration = 3f; // Duration to display the warning in seconds
        private float warningStartTime; // Time when the warning started
        private MessageType messageType = MessageType.Info;
        public static void CreateWizard()
        {
            PackageJsonWindowController window = GetWindow<PackageJsonWindowController>(true, "Generate package.json");
            window.minSize = new Vector2(820, 700); 
            window.Show();
        }
        private void OnEnable()
        {
            LoadPackageJson();
        }

        private void LoadPackageJson()
        {
            PackageHandler. FilePath = Path.Combine(DataHandler.ROOT + DataHandler.LoadPackageNameData(), DataHandler.PACKAGE_JSON);
            jsonData = PackageHandler.CheckUpdateOnDisabledProperties(PackageHandler.LoadData());
        }
    
        private void OnGUI()
        {
           
            GUILayout.Label($"{DataHandler.PACKAGE_JSON} Settings", EditorStyles.boldLabel);
            

            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.TextField("File Location:", PackageHandler.FilePath);
            GUILayout.Space(10); 
            jsonData.name = EditorGUILayout.TextField("Name:", jsonData.name);
            
            EditorGUILayout.BeginHorizontal();
            jsonData.version = EditorGUILayout.TextField("Version:", jsonData.version);
            GUILayout.Label($"Project Settings -> Player Settings -> Version", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            
            
            
            EditorGUILayout.BeginHorizontal();
            jsonData.displayName = EditorGUILayout.TextField("Display Name:", jsonData.displayName);
            GUILayout.Label($"Project Settings -> Player Settings -> Product Name", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.EndDisabledGroup(); // End disabled group
            GUILayout.Label($"Description", EditorStyles.label);
            jsonData.description = EditorGUILayout.TextArea( jsonData.description, GUILayout.Height(40));
            
            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.BeginHorizontal();
            jsonData.unity = EditorGUILayout.TextField("Unity Version:", jsonData.unity);
            jsonData.unityRelease = EditorGUILayout.TextField("Unity Release:", jsonData.unityRelease);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup(); // End disabled group
            
            jsonData.documentationUrl = EditorGUILayout.TextField("Documentation URL:", jsonData.documentationUrl);
            jsonData.changelogUrl = EditorGUILayout.TextField("Changelog URL:", jsonData.changelogUrl);

           
            #region Licenses
            EditorGUI.BeginDisabledGroup(true);
            jsonData.licensesUrl =  EditorGUILayout.TextField("License URL: ", DataHandler.LoadLicenseURLData());
                   //EditorGUILayout.TextField("Licenses URL:", jsonData.licensesUrl);
            EditorGUI.EndDisabledGroup();
            #endregion

            GUILayout.Space(20); // Add vertical space between sections
            GUILayout.Label("Scoped Registries", EditorStyles.boldLabel);
            foreach (var scopedRegistry in jsonData.scopedRegistries)
            {
                scopedRegistry.name = EditorGUILayout.TextField("Name:", scopedRegistry.name);
                scopedRegistry.url = EditorGUILayout.TextField("URL:", scopedRegistry.url);
                EditorGUILayout.LabelField("Scopes:");
                for (int i = 0; i < scopedRegistry.scopes.Count; i++)
                {
                    scopedRegistry.scopes[i] = EditorGUILayout.TextField($"Scope {i + 1}:", scopedRegistry.scopes[i]);
                }
            }
            GUILayout.Space(10); // Add vertical space between scopes and buttons
            
            
            #region Dependencies
            GUILayout.Label("Dependencies", EditorStyles.boldLabel);
            
            
            
            for (int i = 0; i < jsonData.dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20); // Add left padding

                var dependency = jsonData.dependencies.ElementAt(i);
                string dependencyName = EditorGUILayout.TextField("Name:", dependency.Key);
                string dependencyVersion = EditorGUILayout.TextField("Version:", dependency.Value);
            
                // Update the dictionary entry if the name has changed
                if (dependencyName != dependency.Key)
                {
                    jsonData.dependencies.Remove(dependency.Key);
                    jsonData.dependencies[dependencyName] = dependencyVersion;
                }else if( dependencyVersion != dependency.Value)
                {
                    jsonData.dependencies[dependencyName] = dependencyVersion;
                }
                
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    // Remove the dictionary entry based on the key
                    jsonData.dependencies.Remove(dependency.Key);
                    break; // Exit the loop after removal
                }
                EditorGUILayout.EndHorizontal();
               
            }
            if (GUILayout.Button("Add Dependency", GUILayout.Width(150)))
            {
                // Add a new empty entry to the dictionary
                string uniqueID =  DateTime.Now.Ticks.ToString();//Guid.NewGuid();
                jsonData.dependencies.Add($"Package Name {uniqueID}", "Version");
            }

            #endregion
            
            #region Keywords

            GUILayout.Label("Keywords", EditorStyles.boldLabel);
            for (int i = 0; i < jsonData.keywords.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20); // Add left padding
                jsonData.keywords[i] = EditorGUILayout.TextField($"Keyword {i + 1}:", jsonData.keywords[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    jsonData.keywords.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Keyword", GUILayout.Width(150)))
            {
                jsonData.keywords.Add("");
            }

            GUILayout.Space(20); // Add vertical space between sections

            #endregion

            GUILayout.Label("Author Information", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.BeginHorizontal();
            jsonData.author.name = EditorGUILayout.TextField("Name:", jsonData.author.name);
            GUILayout.Label($"Project Settings -> Player Settings -> Company Name", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            
            jsonData.author.email = EditorGUILayout.TextField("Email:", jsonData.author.email);
            jsonData.author.url = EditorGUILayout.TextField("URL:", jsonData.author.url);

            GUILayout.Space(10); // Add vertical space

            if (!string.IsNullOrEmpty(warningMessage) && Time.realtimeSinceStartup - warningStartTime < warningDuration)
            {
                EditorGUILayout.HelpBox(warningMessage, messageType);
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate", GUILayout.Height(35)))
            {
                PackageHandler.SaveData(jsonData);
            }
            GUI.backgroundColor = Color.white; 
        }
 

       
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            messageType = msgType;
            warningMessage = message;
            warningStartTime = Time.realtimeSinceStartup;
        }
    }
}