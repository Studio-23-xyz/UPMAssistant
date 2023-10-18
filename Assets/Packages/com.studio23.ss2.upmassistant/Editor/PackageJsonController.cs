using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class PackageJsonController : GitHubLicenseHandler
    {
        private PackageJsonData jsonData; 
        
        private string jsonFilePath;
       private string licenseFilePath;

        // warning message
        private string warningMessage = ""; // Warning message to display
        private float warningDuration = 3f; // Duration to display the warning in seconds
        private float warningStartTime; // Time when the warning started
        private MessageType messageType = MessageType.Info;
        
        
        public static void CreateWizard()
        {
            PackageJsonController window = GetWindow<PackageJsonController>(true, "Generate package.json");
            window.Show();
        }

        private void OnEnable()
        {
            LoadPreDefineValue();
            FetchOnlineGitLicenses();
            SelectedLicenceURL = jsonData.licensesUrl;
        }

        private void LoadPreDefineValue()
        {
            jsonData = new PackageJsonData();
            jsonFilePath = Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.PACKAGE_JSON);
            licenseFilePath = Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.LICENSE_MD);
            LoadData();
        }
    
private void OnGUI()
{
   
    GUILayout.Label($"{DataManager.PACKAGE_JSON} Settings", EditorStyles.boldLabel);

    GUILayout.Space(10); // Add top margin

    EditorGUI.BeginDisabledGroup(true); // Begin disabled group
    EditorGUILayout.TextField("URL:", jsonFilePath);
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

        if (gitHubLicense != null && gitHubLicense.Count > 0)
        {
            List<string> licenseNames = new List<string>();
            foreach (var license in gitHubLicense)  licenseNames.Add(license.name);

            SelectedLicenceIndex = EditorGUILayout.Popup("Select License", SelectedLicenceIndex, licenseNames.ToArray());
        }
        else
        {
            GUILayout.Label("Online licenses are not found! Loading...");
             
        }
    
    EditorGUI.BeginDisabledGroup(true);
   
        var selectedLicenceURL =  gitHubLicense != null ? gitHubLicense[SelectedLicenceIndex].url : "";
        jsonData.licensesUrl =  EditorGUILayout.TextField("-", selectedLicenceURL);
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
         
            if (File.Exists(jsonFilePath))
            {
                File.WriteAllText(jsonFilePath, jsonString);
                ShowNotification($"File created with data: " + jsonFilePath);
            }
            else
            {
                // show warning
                ShowNotification($"Create {DataManager.PACKAGE_JSON} using UPM System Generator", MessageType.Error);
                UPMAssistantManager.ShowWindow();
            }
                
            if (File.Exists(licenseFilePath))
            {
                SaveLicense(licenseFilePath);
                ShowNotification($"File created with data: " + licenseFilePath);
            }
            else
            {
                // show warning
                ShowNotification($"Create {DataManager.LICENSE_MD} using UPM System Generator", MessageType.Error);
                UPMAssistantManager.ShowWindow();
            }
            
            /*if (UPMAssistantManager.FolderAndFilesList.ContainsKey(UPMAssistantManager.PACKAGE_JSON))
            {
               
            }
            else
            {
                ShowNotification($"File {UPMAssistantManager.PACKAGE_JSON} not found. Create {UPMAssistantManager.PACKAGE_JSON} using UPM Generator.", MessageType.Warning);
            }*/
        }
     
        private void LoadData()
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                
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
            jsonData.name = DataManager.LoadPackageNameData();
            jsonData.version = Application.version; // 1.0.1
            jsonData.displayName = Application.productName; // UPM Assistant
            jsonData.description = "The UPM Assistant is an editor extension tool designed to simplify the process of creating folder structures required for Unity packages that are published on  openupm.com. This tool automates the generation of the necessary directory hierarchy, ensuring that package assets are organized correctly and adhere to the standards of Unity's package management system.";
            string[] unityVersion = Application.unityVersion.Split(".");
            jsonData.unity = unityVersion[0] +"."+ unityVersion[1];//"2022.3";
            jsonData.unityRelease = unityVersion[2]; //"9f1";
            jsonData.documentationUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
            jsonData.changelogUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
            jsonData.licensesUrl = "https://api.github.com/licenses/mit";

            ScopedRegistry scopedRegistry = new ScopedRegistry()
            {
                name = "Studio 23",
                url = "https://package.openupm.com",
                scopes = new List<string> { "com.studio23.ss2"}
            };
            jsonData.scopedRegistries = new List<ScopedRegistry>
            {
                scopedRegistry,
            };
           
           
            jsonData.dependencies = new Dictionary<string, string>(){
                {"com.unity.nuget.newtonsoft-json", "3.2.1"},
                
            };
            
            jsonData.keywords = new List<string> { "UPM", "Assistant", "System" };
            jsonData.author.name = Application.companyName;// Studio 23
            jsonData.author.email = "contact@studio-23.xyz";
            jsonData.author.url = "https://studio-23.xyz";
        } 
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            messageType = msgType;
            warningMessage = message;
            warningStartTime = Time.realtimeSinceStartup;
        }
    }
}