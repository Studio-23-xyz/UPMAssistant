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
    public class PackageJsonController : EditorWindow
    {
        private PackageJsonData jsonData;

        private string filePath;

        // warning message
        private string warningMessage = ""; // Warning message to display
        private float warningDuration = 3f; // Duration to display the warning in seconds
        private float warningStartTime; // Time when the warning started
        private MessageType messageType = MessageType.Info;
        
        [MenuItem("UPM/package.json Generator", priority = 1)]
        static void CreateWizard()
        {
            PackageJsonController window = GetWindow<PackageJsonController>(true, "Generate package.json");
            window.Show();
        }

        private void OnEnable()
        {
            LoadPreDefineValue();
        }

        private void LoadPreDefineValue()
        {
            jsonData = new PackageJsonData();
            filePath = Path.Combine(UPMAssistantManager.Root + UPMAssistantManager.PackageName, UPMAssistantManager.PACKAGE_JSON);
            LoadData();
        }
    
private void OnGUI()
{
   
    GUILayout.Label($"{UPMAssistantManager.PACKAGE_JSON} Settings", EditorStyles.boldLabel);

    GUILayout.Space(10); // Add top margin

    EditorGUI.BeginDisabledGroup(true); // Begin disabled group
    EditorGUILayout.TextField("URL:", filePath);
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
    jsonData.unityVersion = EditorGUILayout.TextField("Unity Version:", jsonData.unityVersion);
    jsonData.unityRelease = EditorGUILayout.TextField("Unity Release:", jsonData.unityRelease);
    EditorGUILayout.EndHorizontal();
    EditorGUI.EndDisabledGroup(); // End disabled group
    
    jsonData.documentationUrl = EditorGUILayout.TextField("Documentation URL:", jsonData.documentationUrl);
    jsonData.changelogUrl = EditorGUILayout.TextField("Changelog URL:", jsonData.changelogUrl);
    jsonData.licensesUrl = EditorGUILayout.TextField("Licenses URL:", jsonData.licensesUrl);

    GUILayout.Space(20); // Add vertical space between sections

    GUILayout.Label("Scoped Registries", EditorStyles.boldLabel);
    EditorGUILayout.BeginHorizontal();
    GUILayout.Space(20); // Add left padding
    EditorGUILayout.BeginVertical();
    
    jsonData.scopedRegistryName = EditorGUILayout.TextField("Name:", jsonData.scopedRegistryName);
    jsonData.scopedRegistryUrl = EditorGUILayout.TextField("URL:", jsonData.scopedRegistryUrl);
    
    EditorGUILayout.LabelField("Scopes:");
    for (int i = 0; i < jsonData.scopedRegistryScopes.Count; i++)
    {
        jsonData.scopedRegistryScopes[i] = EditorGUILayout.TextField($"Scope {i + 1}:", jsonData.scopedRegistryScopes[i]);
    }
    GUILayout.Space(10); // Add vertical space between scopes and buttons
    EditorGUILayout.EndVertical();
    EditorGUILayout.EndHorizontal();
    
    GUILayout.Space(20); // Add vertical space between sections

    GUILayout.Label("Dependencies", EditorStyles.boldLabel);
    for (int i = 0; i < jsonData.dependencies.Count; i++)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20); // Add left padding
        jsonData.dependencies[i].name = EditorGUILayout.TextField("Name:", jsonData.dependencies[i].name);
        jsonData.dependencies[i].version = EditorGUILayout.TextField("Version:", jsonData.dependencies[i].version);
        if (GUILayout.Button("Remove", GUILayout.Width(80)))
        {
            jsonData.dependencies.RemoveAt(i);
        }
        EditorGUILayout.EndHorizontal();
    }

    if (GUILayout.Button("Add Dependency", GUILayout.Width(150)))
    {
        jsonData.dependencies.Add(new PackageDependency());
    }

    GUILayout.Space(20); // Add vertical space between sections

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

    GUILayout.Label("Author Information", EditorStyles.boldLabel);
    
    EditorGUI.BeginDisabledGroup(true); // Begin disabled group
    EditorGUILayout.BeginHorizontal();
    jsonData.authorName = EditorGUILayout.TextField("Name:", jsonData.authorName);
    GUILayout.Label($"Project Settings -> Player Settings -> Company Name", EditorStyles.label);
    EditorGUILayout.EndHorizontal();
    EditorGUI.EndDisabledGroup();
    
    jsonData.authorEmail = EditorGUILayout.TextField("Email:", jsonData.authorEmail);
    jsonData.authorUrl = EditorGUILayout.TextField("URL:", jsonData.authorUrl);

    GUILayout.Space(10); // Add vertical space

    if (!string.IsNullOrEmpty(warningMessage) && Time.realtimeSinceStartup - warningStartTime < warningDuration)
    {
        EditorGUILayout.HelpBox(warningMessage, messageType);
    }
    GUI.backgroundColor = Color.green;
    if (GUILayout.Button("Generate", GUILayout.Height(35)))
    {
        GeneratePackageJson();
    }
    GUI.backgroundColor = Color.white; 
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
        
        public void SetData(string jsonString)
        {
            if (UPMAssistantManager.FolderAndFilesList.ContainsKey(UPMAssistantManager.PACKAGE_JSON))
            {
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, jsonString);
                    ShowNotification($"File created with data: " + filePath);
                }
                else
                {
                    // show warning
                    ShowNotification($"Create {UPMAssistantManager.PACKAGE_JSON} using UPM System Generator", MessageType.Error);
                    UPMAssistantManager.ShowWindow();
                }
            }
            else
            {
                ShowNotification($"File {UPMAssistantManager.PACKAGE_JSON} not found. Create {UPMAssistantManager.PACKAGE_JSON} using UPM Generator.", MessageType.Warning);
            }
        }
     
        private void LoadData()
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    AssignPreDefaultValues();
                    ShowNotification($"package.json is empty. Default data is loaded");
                    return;
                }
                
                jsonData = JsonConvert.DeserializeObject<PackageJsonData>(jsonString);
                ShowNotification($"Previous data is loaded");
            }
            else
            {
                 AssignPreDefaultValues();
                 ShowNotification($"Default data loaded");
            }
        }
        private void AssignPreDefaultValues()
        {
            jsonData.name = UPMAssistantManager.PackageName;
            jsonData.version = Application.version; // 1.0.1
            jsonData.displayName = Application.productName; // UPM Assistant
            jsonData.description = "The UPM Assistant is an editor extension tool designed to simplify the process of creating folder structures required for Unity packages that are published on  openupm.com. This tool automates the generation of the necessary directory hierarchy, ensuring that package assets are organized correctly and adhere to the standards of Unity's package management system.";
            string[] unityVersion = Application.unityVersion.Split(".");
            jsonData.unityVersion = unityVersion[0] +"."+ unityVersion[1];//"2022.3";
            jsonData.unityRelease = unityVersion[2]; //"9f1";
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
            jsonData.authorName = Application.companyName;// Studio 23
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