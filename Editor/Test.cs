using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor.Test
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
        public string name = "com.studio23.ss2.upmassistant";
        public string version = "1.0.0";
        public string displayName = "UPM Assistant";
        public string description = "The Unity Package Manager Structure Generator Tool is an editor extension designed to simplify the process of creating folder structures required for Unity packages that are published on https://openupm.com.";
        public string unityVersion = "2022.3";
        public string unityRelease = "9f1";
        public string documentationUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
        public string changelogUrl = "https://openupm.com/packages/com.studio23.ss2.upmassistant";
        public string licensesUrl = "https://opensource.org/license/mit/";
        public string scopedRegistryName = "com.studio23.ss2";
        public string scopedRegistryUrl = "https://studio-23.xyz";

        public List<string> scopedRegistryScopes = new List<string> { "https://package.openupm.com" };
        public List<PackageDependency> dependencies = new List<PackageDependency>
        {
            new PackageDependency { name = "com.unity.nuget.newtonsoft-json", version = "3.2.1" }
        };
        public List<string> keywords = new List<string> { "UPM", "Assistant", "System" };
        public string authorName = "Studio 23";
        public string authorEmail = "contact@studio-23.xyz";
        public string authorUrl = "https://studio-23.xyz";
    }

    public class Test : EditorWindow
    {
        private PackageJsonData jsonData = new PackageJsonData();

        [MenuItem("UPM/Test", priority = 2)]
        static void CreateWizard()
        {
            PackageJsonWizard window = GetWindow<PackageJsonWizard>(true, "Generate package.json");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Package.json Settings", EditorStyles.boldLabel);

            jsonData.name = EditorGUILayout.TextField("Name:", jsonData.name);
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

            
            // Dependencies, Keywords, and Author Information can be handled similarly using jsonData structure

            if (GUILayout.Button("Generate", GUILayout.Height(35)))
            {
                GeneratePackageJson();
            }
        }

        private void GeneratePackageJson()
        {
            string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            GenerateUMPSystem(jsonString);
            AssetDatabase.Refresh();
        }

        private void GenerateUMPSystem(string jsonString)
        {
            UPMSystemGenerator UPMSystemGenerator = GetWindow<UPMSystemGenerator>("UMP System Generator");
            Dictionary<string, string> fileData = new Dictionary<string, string>
            {
                {"package.json", jsonString},
            };

            UPMSystemGenerator.SetData(fileData);
        }
    }
}