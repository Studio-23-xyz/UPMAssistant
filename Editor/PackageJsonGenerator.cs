using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.UPMAssistant.Editor
{
    [System.Serializable]
    public class ScopedRegistry
    {
        public string name;
        public string url;
        public List<string> scopes;
    }

    [System.Serializable]
    public class PackageDependency
    {
        public string name;
        public string version;
    }

     
    [System.Serializable]
    public class PackageJsonData
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unityVersion;
        public string unityRelease;
        public string documentationUrl;
        public string changelogUrl;
        public string licensesUrl;
        public string scopedRegistryName;
        public string scopedRegistryUrl;
        public List<string> scopedRegistryScopes = new List<string>();
        public List<PackageDependency> dependencies = new List<PackageDependency>();
        public List<string> keywords = new List<string>();
        public string authorName;
        public string authorEmail;
        public string authorUrl;
    }


    public class PackageJsonGenerator : EditorWindow
    {
       // private string name = "package.json";
        private PackageJsonData jsonData;

        private string filePath;

        // warning message
        private string warningMessage = ""; // Warning message to display
        private float warningDuration = 3f; // Duration to display the warning in seconds
        private float warningStartTime; // Time when the warning started
        private MessageType messageType = MessageType.Info;
        
        [MenuItem("UPM/PackageJsonGenerator", priority = 2)]
        static void CreateWizard()
        {
            PackageJsonGenerator window = GetWindow<PackageJsonGenerator>(true, "Generate package.json");
            window.Show();
        }

        private void OnEnable()
        {
            LoadPreDefineValue();
        }

        private void LoadPreDefineValue()
        {
           
            jsonData = new PackageJsonData();
            AssignPreDefaultValues();
        }
        private void OnGUI()
        {
            filePath = Path.Combine(UPMSystemGenerator.Root + UPMSystemGenerator.PackageName, UPMSystemGenerator.PACKAGE_JSON);
            
            GUILayout.Label($"{UPMSystemGenerator.PACKAGE_JSON} Settings", EditorStyles.boldLabel);
           
            EditorGUI.BeginDisabledGroup(true); // Begin disabled group
            EditorGUILayout.TextField("URL:", filePath);
            jsonData.name = EditorGUILayout.TextField("Name:", jsonData.name);
            EditorGUI.EndDisabledGroup(); // End disabled group
            
            jsonData.version = EditorGUILayout.TextField("Version:", jsonData.version);
            jsonData.displayName = EditorGUILayout.TextField("Display Name:", jsonData.displayName);
            jsonData.description = EditorGUILayout.TextField("Description:", jsonData.description);
            jsonData.unityVersion = EditorGUILayout.TextField("Unity Version:", jsonData.unityVersion);
            jsonData.unityRelease = EditorGUILayout.TextField("Unity Release:", jsonData.unityRelease);
            jsonData.documentationUrl = EditorGUILayout.TextField("Documentation URL:", jsonData.documentationUrl);
            jsonData.changelogUrl = EditorGUILayout.TextField("Changelog URL:", jsonData.changelogUrl);
            jsonData.licensesUrl = EditorGUILayout.TextField("Licenses URL:", jsonData.licensesUrl);

            GUILayout.Label("Scoped Registries", EditorStyles.boldLabel);
            jsonData.scopedRegistryName = EditorGUILayout.TextField("Name:", jsonData.scopedRegistryName);
            jsonData.scopedRegistryUrl = EditorGUILayout.TextField("URL:", jsonData.scopedRegistryUrl);

            EditorGUILayout.LabelField("Scopes:");
            for (int i = 0; i < jsonData.scopedRegistryScopes.Count; i++)
            {
                jsonData.scopedRegistryScopes[i] = EditorGUILayout.TextField($"Scope {i + 1}:", jsonData.scopedRegistryScopes[i]);
            }
            // ... (previous code remains unchanged)
            
            GUILayout.Label("Dependencies", EditorStyles.boldLabel);
            for (int i = 0; i < jsonData.dependencies.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                jsonData.dependencies[i].name = EditorGUILayout.TextField("Name:", jsonData.dependencies[i].name);
                jsonData.dependencies[i].version = EditorGUILayout.TextField("Version:", jsonData.dependencies[i].version);
                if (GUILayout.Button("Remove"))
                {
                    jsonData.dependencies.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Dependency"))
            {
                jsonData.dependencies.Add(new PackageDependency());
            }

            GUILayout.Label("Keywords", EditorStyles.boldLabel);
            for (int i = 0; i < jsonData.keywords.Count; i++)
            {
                jsonData.keywords[i] = EditorGUILayout.TextField($"Keyword {i + 1}:", jsonData.keywords[i]);
            }

            if (GUILayout.Button("Add Keyword"))
            {
                jsonData.keywords.Add("");
            }

            GUILayout.Label("Author Information", EditorStyles.boldLabel);
            jsonData.authorName = EditorGUILayout.TextField("Name:", jsonData.authorName);
            jsonData.authorEmail = EditorGUILayout.TextField("Email:", jsonData.authorEmail);
            jsonData.authorUrl = EditorGUILayout.TextField("URL:", jsonData.authorUrl);

            
            if (!string.IsNullOrEmpty(warningMessage) && Time.realtimeSinceStartup - warningStartTime < warningDuration)
            {
                EditorGUILayout.HelpBox(warningMessage, messageType);
            }

            if (GUILayout.Button("Generate", GUILayout.Height(35)))
            {
               
                GeneratePackageJson();
            }
        }

        private void GeneratePackageJson()
        {
            
            string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            
            JObject jsonObject = JObject.Parse(jsonString);
            
            if ((jsonObject == null) || !jsonObject.HasValues)
            {
                ShowNotification($"Data not found!", MessageType.Warning);
                return;
            }
            
            SetData(jsonString);
            AssetDatabase.Refresh();
        }

        /*private void SetData(string jsonString)
        {
            /*UPMSystemGenerator UPMSystemGenerator = GetWindow<UPMSystemGenerator>("UMP System Generator");#1#
            Dictionary<string, string> fileData = new Dictionary<string, string>
            {
                {name, jsonString},
            };

            SetData(fileData);
        }*/
        /*public void SetData(string jsonString)
        {
            File.WriteAllText(filePath, jsonString);
            ShowNotification($"File created with data: " + filePath + " " + jsonString);
        }*/
        public void SetData(string jsonString)
        {
          
           
            if (UPMSystemGenerator.FolderAndFilesList.ContainsKey(UPMSystemGenerator.PACKAGE_JSON))
            {
                // string filePath = Path.Combine(UPMSystemGenerator.Root + UPMSystemGenerator.PackageName, entry.Key);
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, jsonString);
                    // CreateFolderStructure(PackageName);
                    ShowNotification($"File created with data: " + filePath);
                     
                }
                else
                {
                    // show warning
                    ShowNotification($"Create {UPMSystemGenerator.PACKAGE_JSON} using UPM Generator", MessageType.Error);
                }
            }
            else
            {
                ShowNotification($"File {UPMSystemGenerator.PACKAGE_JSON} not found. Create {UPMSystemGenerator.PACKAGE_JSON} using UPM Generator.", MessageType.Warning);
            }
        }
        public void AssignPreDefaultValues()
        {
            jsonData.name = UPMSystemGenerator.PackageName;
            jsonData.version = "1.0.0";
            jsonData.displayName = "UPM Assistant";
            jsonData.description = "The Unity Package Manager Structure Generator Tool is an editor extension designed to simplify the process of creating folder structures required for Unity packages that are published on https://openupm.com.";
            jsonData.unityVersion = "2022.3";
            jsonData.unityRelease = "9f1";
            jsonData.documentationUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
            jsonData.changelogUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
            jsonData.licensesUrl = "https://opensource.org/license/mit/";
            jsonData.scopedRegistryName = "com.studio23.ss2";
            jsonData.scopedRegistryUrl = "https://studio-23.xyz";
            jsonData.scopedRegistryScopes = new List<string> { "https://package.openupm.com" };
            jsonData.dependencies = new List<PackageDependency>
            {
                new PackageDependency { name = "com.unity.nuget.newtonsoft-json", version = "3.2.1" }
            };
            jsonData.keywords = new List<string> { "UPM", "Assistant", "System" };
            jsonData.authorName = "Studio 23";
            jsonData.authorEmail = "contact@studio-23.xyz";
            jsonData.authorUrl = "https://studio-23.xyz";
        } 
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            messageType = msgType;
            warningMessage = message;
            warningStartTime = Time.realtimeSinceStartup;
        }
    }
}