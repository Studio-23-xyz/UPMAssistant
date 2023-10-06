using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMAssistantManager: EditorWindow
    {
        
        public static string Root = "Assets/Packages/";
        public static string packageName;
        
       

       public static readonly string PACKAGE_JSON = "package.json";
        // warning message
        private string warningMessage = ""; // Warning message to display
        private float warningDuration = 3f; // Duration to display the warning in seconds
        private float warningStartTime; // Time when the warning started
        private MessageType messageType = MessageType.Info;
        
        public static Dictionary<string, bool> FolderAndFilesList = new Dictionary<string, bool>
        {
            {PACKAGE_JSON, false},
            {"README.md", false},
            {"CHANGELOG.md", false},
            {"LICENSE.md", false},
            {"ThirdPartyNotices.md", false},
            {"Editor", false},
            {$"Editor/[[packagename]].editor.asmdef", false},
            {"Runtime", false},
            {"Tests", false},
            {"Tests/Editor", false},
            {"Tests/Runtime", false},
            {"Samples", false},
            {"Documentation", false},
           
        };

        [MenuItem("Studio-23/UPM Assistant/Generator", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<UPMAssistantManager>("PackageJsonController Window");
           
           
        }
        public void LoadExistenceValue()
        {
            // Initialize the dictionary with file/folder existence check
            foreach (var entry in FolderAndFilesList.ToList())
            {
                if (entry.Key.Contains("."))
                {
                    // For files, check if the file exists
                    FolderAndFilesList[entry.Key] = File.Exists(Root + packageName + "/" + entry.Key);
                }
                else
                {
                    // For folders, check if the folder exists
                    FolderAndFilesList[entry.Key] = AssetDatabase.IsValidFolder(Root + packageName + "/" + entry.Key);
                }
            }
        }

        private void OnEnable()
        {
           
           LoadExistenceValue();
        }

        
        private void OnGUI()
        {
            GUILayout.Label("Enter Package Name:", EditorStyles.boldLabel);
            GUILayout.Label("Assets/Packages/", EditorStyles.boldLabel);

            // Use custom colors for the GUI elements
            if (string.IsNullOrEmpty(packageName))
            {
                packageName = PlayerPrefs.GetString("packageName","com.[companyname].[project].[packagename]");
            }
            packageName = EditorGUILayout.TextField("Package Name", packageName, GUILayout.Width(position.width - 20));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check All", GUILayout.Width(position.width / 2 - 10)))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = true;
                }
            }
            if (GUILayout.Button("Uncheck All", GUILayout.Width(position.width / 2 - 10)))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = false;
                }
            }
            EditorGUILayout.EndHorizontal();

            foreach (var entry in FolderAndFilesList.ToList())
            {
               
                FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value, GUILayout.Width(position.width - 20));

               
                string extension = entry.Key.Split(".").LastOrDefault();
                if (entry.Key.Contains(".") && extension == "asmdef")
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Configure"))
                    {
                        PackageJsonController.CreateWizard();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                    
                    
            }

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate UPM System", GUILayout.Height(40)))
            {
                if (packageName == "")
                {
                    ShowNotification("Please enter a package name", MessageType.Error);
                    return;
                }
                
                PlayerPrefs.SetString("packageName", packageName);
                
                CreateFolderStructure(packageName);
            }

            GUI.backgroundColor = Color.white; // Reset the background color
        }

       
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            messageType = msgType;
            warningMessage = message;
            warningStartTime = Time.realtimeSinceStartup;
        }
       public void CreateFolderStructure(string folderName)
       {
           string rootPath = Root + folderName;

           if (!AssetDatabase.IsValidFolder(rootPath))
           {
               AssetDatabase.CreateFolder("Assets/Packages", folderName);
               Debug.Log("Root folder created: " + rootPath);
           }
           else
           {
               Debug.Log("Root folder already exists: " + rootPath);
           }

           foreach (var entry in FolderAndFilesList)
           {
               if (entry.Value)
               {
                   if (entry.Key.Contains("."))
                   {
                       CreateFile(rootPath, entry.Key);
                   }
                   else
                   {
                       CreateFolder(rootPath + "/" + entry.Key);
                   }
               }
           }

           
           ShowNotification($"Folder structure created successfully.");
       }

       private void CreateFile(string path, string fileName)
       {
           
           string extension = fileName.Split(".").LastOrDefault();
           
           string filePath = Path.Combine(path, fileName);
           if (!File.Exists(filePath) && extension == "asmdef")
           {

               CreateEditorAssemblyDefinition( path,  fileName);
           }
           else if (!File.Exists(filePath))
           {
               File.WriteAllText(filePath, string.Empty);
               Debug.Log("File created: " + filePath);
           }
           else
           {
               Debug.Log("File already exists: " + filePath);
           }
       }

       private static void CreateFolder(string path)
       {
           if (!AssetDatabase.IsValidFolder(path))
           {
               AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
               Debug.Log("Folder created: " + path);
           }
           else
           {
               Debug.Log("Folder already exists: " + path);
           }
       }

       void CreateEditorAssemblyDefinition(string path, string fileName)
       {
           string filePath = Path.Combine(path,$"{fileName.Replace("[[packagename]]",$"{packageName}")}");

           // Define the content of the assembly definition file
           string assemblyDefinitionContent = 
               "{\n" +
               $"  \"name\": \"{packageName}.editor\",\n" +
               "  \"references\": [],\n" +
               "  \"optionalUnityReferences\": [],\n" +
               "  \"includePlatforms\": [\"Editor\"],\n" +
               "  \"excludePlatforms\": [],\n" +
               "  \"allowUnsafeCode\": false,\n" +
               "  \"overrideReferences\": false,\n" +
               "  \"precompiledReferences\": [],\n" +
               "  \"autoReferenced\": true,\n" +
               "  \"defineConstraints\": []\n" +
               "}";

           // Write the content to the file
           File.WriteAllText(filePath, assemblyDefinitionContent);

           // Refresh the AssetDatabase to let Unity know about the new file
           AssetDatabase.Refresh();

           Debug.Log("Assembly Definition file created at: " + path);
       }
       
       /*public void SetData(Dictionary<string, string> fileData)
       {
           // Use the file data to populate the files while creating them
           foreach (var entry in fileData)
           {
               if (FolderAndFilesList.ContainsKey(entry.Key) && FolderAndFilesList[entry.Key])
               {
                   
                   string filePath = Path.Combine(Root + PackageName, entry.Key);
                   if (!File.Exists(filePath))
                   {
                       // show warning
                       Debug.LogError($"Create {entry.Key} using UPM Generator");
                   }
                   else
                   {
                       File.WriteAllText(filePath, entry.Value);
                       // CreateFolderStructure(PackageName);
                       ShowNotification($"File created with data: " + filePath);
                   }
               }
               else
               {
                   ShowNotification($"File {entry.Key} not found. Create {entry.Key} using UPM Generator.", MessageType.Warning);
               }
               
           }
       } */

       

       
    }
}