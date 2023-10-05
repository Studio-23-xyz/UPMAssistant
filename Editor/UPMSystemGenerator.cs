using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMSystemGenerator: EditorWindow
    {
        
        public static string Root = "Assets/Packages/";
        private static string packageName;
       public static string PackageName
       {
           get
           {
               packageName = "com.studio23.ss2.testproject";
               if(PlayerPrefs.HasKey("PackageName"))
               {
                   packageName = PlayerPrefs.GetString("PackageName");
                   
               }
               return packageName;
           }
           set
           {
               packageName = value;
               PlayerPrefs.SetString("PackageName", packageName);
           }
       }

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
            {"Runtime", false},
            {"Tests", false},
            {"Tests/Editor", false},
            {"Tests/Runtime", false},
            {"Samples", false},
            {"Documentation", false},
           
        };

        [MenuItem("UPM/UPM System Generator", priority = 0)]
        private static void ShowWindow()
        {
            GetWindow<UPMSystemGenerator>("PackageJsonGenerator Window");
           
           
        }
        public void LoadExistenceValue()
        {
            // Initialize the dictionary with file/folder existence check
            foreach (var entry in FolderAndFilesList.ToList())
            {
                if (entry.Key.Contains("."))
                {
                    // For files, check if the file exists
                    FolderAndFilesList[entry.Key] = File.Exists(Root + PackageName + "/" + entry.Key);
                }
                else
                {
                    // For folders, check if the folder exists
                    FolderAndFilesList[entry.Key] = AssetDatabase.IsValidFolder(Root + PackageName + "/" + entry.Key);
                }
            }
        }

        private void OnEnable()
        {
            
           if(PlayerPrefs.HasKey("PackageName"))
           {
               PackageName = PlayerPrefs.GetString("PackageName");
           }
           LoadExistenceValue();
        }

        public void CreateGUI()
        {
           
           
        }
        private void OnGUI()
        {
            
            GUILayout.Label("Enter Package Name:");
            GUILayout.Label("Assets:Packages/");
           
           
            PackageName = EditorGUILayout.TextField(PackageName);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check All"))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = true;
                }
            }
            if (GUILayout.Button("Uncheck All"))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = false;
                }
            }
            EditorGUILayout.EndHorizontal();
            foreach (var entry in FolderAndFilesList.ToList())
            {
                FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value);
            }

            if (!string.IsNullOrEmpty(warningMessage) && Time.realtimeSinceStartup - warningStartTime < warningDuration)
            {
                EditorGUILayout.HelpBox(warningMessage, messageType);
            }
            
            if (GUILayout.Button("Generate UMP System"))
            {
                if(PackageName == "")
                {
                    Debug.LogError("Please enter a package name");
                    return;
                }
                PlayerPrefs.SetString("PackageName", PackageName);
                CreateFolderStructure(PackageName);
            }
           
           
            
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

       private static void CreateFile(string path, string fileName)
       {
           string filePath = Path.Combine(path, fileName);
           if (!File.Exists(filePath))
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