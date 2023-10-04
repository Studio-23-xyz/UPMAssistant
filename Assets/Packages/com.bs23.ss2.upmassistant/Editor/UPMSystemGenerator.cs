using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMSystemGenerator: EditorWindow
    {
        
        private string folderName = "com.bs23.ss2.testpackage";
       
        private Dictionary<string, bool> folderAndFileSelection = new Dictionary<string, bool>
        {
            {"package.json", true},
            {"README.md", true},
            {"CHANGELOG.md", true},
            {"LICENSE.md", true},
            {"ThirdPartyNotices.md", true},
            {"Editor", true},
            {"Runtime", true},
            {"Tests", true},
            {"Tests/Editor", true},
            {"Tests/Runtime", true},
            {"Samples", true},
            {"Documentation", true},
           
        };

        [MenuItem("UPM/UPMSystemGenerator", priority = 3)]
        private static void ShowWindow()
        {
            GetWindow<UPMSystemGenerator>("Test Window");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Enter Package Name");
           
            if(PlayerPrefs.HasKey("folderName"))
            {
                folderName = PlayerPrefs.GetString("folderName");
            }
            folderName = EditorGUILayout.TextField(folderName);
           
            List<KeyValuePair<string, bool>> folderAndFileList = folderAndFileSelection.ToList();

            foreach (var entry in folderAndFileList)
            {
                folderAndFileSelection[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value);
            }

            if (GUILayout.Button("Generate UMP System"))
            {
                if(folderName == "")
                {
                    Debug.LogError("Please enter a package name");
                    return;
                }
                PlayerPrefs.SetString("folderName", folderName);
                CreateFolderStructure(folderName);
            }
        }

      
       public void CreateFolderStructure(string folderName)
       {
           string rootPath = "Assets/Packages/" + folderName;

           if (!AssetDatabase.IsValidFolder(rootPath))
           {
               AssetDatabase.CreateFolder("Assets/Packages", folderName);
               Debug.Log("Root folder created: " + rootPath);
           }
           else
           {
               Debug.Log("Root folder already exists: " + rootPath);
           }

           foreach (var entry in folderAndFileSelection)
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

           Debug.Log("Folder structure created successfully.");
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

       public void SetData(Dictionary<string, string> fileData)
       {
           // Use the file data to populate the files while creating them
           foreach (var entry in fileData)
           {
               if (folderAndFileSelection.ContainsKey(entry.Key) && folderAndFileSelection[entry.Key])
               {
                   
                   string filePath = Path.Combine("Assets/Packages/" + folderName, entry.Key);
                   if (!File.Exists(filePath))
                   {
                       // show warning
                       Debug.LogError($"Create {entry.Key} using UPM Generator");
                      
                   }
                   else
                   {
                       File.WriteAllText(filePath, entry.Value);
                       Debug.Log("File created with data: " + filePath);
                       // CreateFolderStructure(folderName);
                   }

                  
               }
               else
               {
                   Debug.Log($"File {entry.Key} not found.");
               }
           }
       }
       
    }
}