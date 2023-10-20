using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;


namespace Studio23.SS2.UPMAssistant.Editor
{
    public class PackageJsonWindowController : EditorWindow
    {
        private PackageJsonData _jsonData; 
        
        private string _warningMessage = ""; // Warning message to display
        private const float WarningDuration = 3f; // Duration to display the warning in seconds
        private float _warningStartTime; // Time when the warning started
        private MessageType _messageType = MessageType.Info;
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
            string packageNameData = DataHandler.GetSavedPackagedName();
            string filePath = Path.Combine(DataHandler.Root, packageNameData, DataHandler.PackageJson);
            PackageHandler.FilePath = filePath;
            _jsonData = PackageHandler.LoadData();
            _jsonData = PackageHandler.CheckUpdateOnDisabledProperties(_jsonData);
        }
    
        private void OnGUI()
        {
           
            GUILayout.Label($"{DataHandler.PackageJson} Settings", EditorStyles.boldLabel);
            

            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.TextField("File Location:", PackageHandler.FilePath);
            GUILayout.Space(10); 
            _jsonData.Name = EditorGUILayout.TextField("Name:", _jsonData.Name);
            
            EditorGUILayout.BeginHorizontal();
            _jsonData.Version = EditorGUILayout.TextField("Version:", _jsonData.Version);
            GUILayout.Label($"Project Settings -> Player Settings -> Version", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            
            
            
            EditorGUILayout.BeginHorizontal();
            _jsonData.DisplayName = EditorGUILayout.TextField("Display Name:", _jsonData.DisplayName);
            GUILayout.Label($"Project Settings -> Player Settings -> Product Name", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.EndDisabledGroup(); // End disabled group
            GUILayout.Label($"Description", EditorStyles.label);
            _jsonData.Description = EditorGUILayout.TextArea( _jsonData.Description, GUILayout.Height(40));
            
            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.BeginHorizontal();
            _jsonData.Unity = EditorGUILayout.TextField("Unity Version:", _jsonData.Unity);
            _jsonData.UnityRelease = EditorGUILayout.TextField("Unity Release:", _jsonData.UnityRelease);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup(); // End disabled group
            
            _jsonData.DocumentationUrl = EditorGUILayout.TextField("Documentation URL:", _jsonData.DocumentationUrl);
            _jsonData.ChangelogUrl = EditorGUILayout.TextField("Changelog URL:", _jsonData.ChangelogUrl);

           
            #region Licenses
            EditorGUI.BeginDisabledGroup(true);
            _jsonData.LicensesUrl =  EditorGUILayout.TextField("License URL: ", DataHandler.GetSavedLicenseURL());
                   //EditorGUILayout.TextField("Licenses URL:", jsonData.licensesUrl);
            EditorGUI.EndDisabledGroup();
            #endregion

            GUILayout.Space(20); // Add vertical space between sections
            GUILayout.Label("Scoped Registries", EditorStyles.boldLabel);
            foreach (var scopedRegistry in _jsonData.ScopedRegistries)
            {
                scopedRegistry.Name = EditorGUILayout.TextField("Name:", scopedRegistry.Name);
                scopedRegistry.URL = EditorGUILayout.TextField("URL:", scopedRegistry.URL);
                EditorGUILayout.LabelField("Scopes:");
                for (int i = 0; i < scopedRegistry.Scopes.Count; i++)
                {
                    scopedRegistry.Scopes[i] = EditorGUILayout.TextField($"Scope {i + 1}:", scopedRegistry.Scopes[i]);
                }
            }
            GUILayout.Space(10); // Add vertical space between scopes and buttons
            
            
            #region Dependencies
            GUILayout.Label("Dependencies", EditorStyles.boldLabel);
            
            
            
            for (int i = 0; i < _jsonData.Dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20); // Add left padding

                var dependency = _jsonData.Dependencies.ElementAt(i);
                string dependencyName = EditorGUILayout.TextField("Name:", dependency.Key);
                string dependencyVersion = EditorGUILayout.TextField("Version:", dependency.Value);
                
                List<KeyValuePair<string, string>> itemsToUpdate = new List<KeyValuePair<string, string>>();
                if (dependencyName != dependency.Key || dependencyVersion != dependency.Value)
                {
                    itemsToUpdate.Add(new KeyValuePair<string, string>(dependency.Key, dependencyVersion));
                }
                foreach (var item in itemsToUpdate)
                {
                    _jsonData.Dependencies.Remove(item.Key);
                    _jsonData.Dependencies.Add(dependencyName, item.Value);
                }
                
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    _jsonData.Dependencies.Remove(dependency.Key);
                    break;
                }
                EditorGUILayout.EndHorizontal();
               
            }

            if (GUILayout.Button("Add Dependency", GUILayout.Width(150)))
            {
                string uniqueID = DateTime.Now.Ticks.ToString();
                _jsonData.Dependencies.Add($"Package Name {uniqueID}", "Version");
            }

            #endregion
            
            #region Keywords

            GUILayout.Label("Keywords", EditorStyles.boldLabel);
            for (int i = 0; i < _jsonData.Keywords.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20); // Add left padding
                _jsonData.Keywords[i] = EditorGUILayout.TextField($"Keyword {i + 1}:", _jsonData.Keywords[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(80)))
                {
                    _jsonData.Keywords.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Keyword", GUILayout.Width(150)))
            {
                _jsonData.Keywords.Add("");
            }

            GUILayout.Space(20); // Add vertical space between sections

            #endregion

            GUILayout.Label("Author Information", EditorStyles.boldLabel);
            
            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.BeginHorizontal();
            _jsonData.Author.Name = EditorGUILayout.TextField("Name:", _jsonData.Author.Name);
            GUILayout.Label($"Project Settings -> Player Settings -> Company Name", EditorStyles.label);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            
            _jsonData.Author.Email = EditorGUILayout.TextField("Email:", _jsonData.Author.Email);
            _jsonData.Author.URL = EditorGUILayout.TextField("URL:", _jsonData.Author.URL);

            GUILayout.Space(10); // Add vertical space

            if (!string.IsNullOrEmpty(_warningMessage) && Time.realtimeSinceStartup - _warningStartTime < WarningDuration)
            {
                EditorGUILayout.HelpBox(_warningMessage, _messageType);
            }
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate", GUILayout.Height(35)))
            {
                PackageHandler.SaveData(_jsonData);
            }
            GUI.backgroundColor = Color.white; 
        }
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            _messageType = msgType;
            _warningMessage = message;
            _warningStartTime = Time.realtimeSinceStartup;
        }
    }
}