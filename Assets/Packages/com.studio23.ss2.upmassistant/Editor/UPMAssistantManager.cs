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
    public class UPMAssistantManager: GitHubLicenseHandler
    {
        
        
        public static string Root = "Assets/Packages/";
       
        public static string packageName;
        
        public static readonly string PACKAGE_JSON = "package.json";
        public static readonly string LICENSE_MD = "LICENSE.md";
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
            {"Tests/EditMode", false},
            {$"Tests/EditMode/editmode.tests.asmdef", false},
            {"Tests/PlayMode", false},
            {$"Tests/PlayMode/playmode.tests.asmdef", false},
            {"Samples", false},
            {"Documentation", false},
           
        };

        [MenuItem("Studio-23/UPM Assistant/Generator", priority = 0)]
        public static void ShowWindow()
        {
            GetWindow<UPMAssistantManager>("PackageJsonController Window");
        }
       

        private void OnEnable()
        {
           
            Refresh();
        }

        private void Refresh()
        {
            UPMAssistantDataManager.Initialized();
            AssetDatabase.Refresh();
            LoadPackageName();
            LoadExistenceValue();
            FetchLicenses();
        }
         private void LoadPackageName()
        {
           //  if (string.IsNullOrEmpty(packageName))
                packageName = UPMAssistantDataManager.LoadPackageNameData(); 
            // "com.[companyname].[project].[packagename]";
        }
        private void LoadExistenceValue()
        {
            // Initialize the dictionary with file/folder existence check
            foreach (var entry in FolderAndFilesList.ToList())
            {
                Debug.Log(Root + packageName + "/" + entry.Key);
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
       
        private void DeleteFolder(string folderPath)
        {
            // Check if the folder exists
            if (Directory.Exists(folderPath))
            {
                // Delete all files in the folder
                string[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    File.Delete(file);
                }

                // Delete all subdirectories and their contents
                string[] subdirectories = Directory.GetDirectories(folderPath);
                foreach (string subdirectory in subdirectories)
                {
                    DeleteFolder(subdirectory);
                }

                // Finally, delete the main folder
                Directory.Delete(folderPath);
                Debug.Log("Folder deleted: " + folderPath);
            }
            else
            {
                Debug.LogWarning("Folder not found: " + folderPath);
            }
        }
        private void OnGUI()
        {

            #region TopMenu

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = true;
                }
            }
            if (GUILayout.Button("Uncheck All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in FolderAndFilesList.ToList())
                {
                    FolderAndFilesList[entry.Key] = false;
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(position.width / 4 - 5)))
            {
                Refresh();
            }
            if (GUILayout.Button("Delete System", GUILayout.Width(position.width / 4 - 5)))
            {
              
                if (UPMAssistantDataManager.LoadPackageNameData() != null)
                {
                    bool userConfirmed = EditorUtility.DisplayDialog("Delete Folder",
                        "Are you sure you want to delete the folder and its contents?",
                        "Yes", "No");

                    if (userConfirmed)
                    {
                        //DeleteFolder(Root + packageName);
                        //PlayerPrefs.DeleteKey("packageName");
                        UPMAssistantDataManager.DeletedSaveData();
                        Refresh();
                        ShowNotification("Folder deleted successfully.");
                    }
                    else
                    {
                       
                        ShowNotification("Deletion cancelled by user.");
                    }
                }
                else
                {
                    ShowNotification("Nothing to delete.");
                }
                
                    
                    
            }
            EditorGUILayout.EndHorizontal();
            

            #endregion
            
            GUILayout.Label("Enter Package Name:", EditorStyles.boldLabel);
           

            LoadPackageName();
            packageName = EditorGUILayout.TextField("Assets/Packages/", packageName, GUILayout.Width(position.width - 20));

         

            bool _isCreateAsseblyDefinition_Editor = false;
            foreach (var entry in FolderAndFilesList.ToList())
            {
               
                string extension = entry.Key.Split(".").LastOrDefault();

                if (extension == "asmdef")
                {
                    if (_isCreateAsseblyDefinition_Editor)
                    {
                        
                        var entryKey = $"{entry.Key.Replace("[[packagename]]", $"{packageName}")}";
                        FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entryKey, entry.Value,
                            GUILayout.Width(position.width - 20));
                    }
                }
                else  FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value);
                
                
                if (entry.Key == PACKAGE_JSON)
                {
                    if (entry.Value)
                    { 
                       var isThisFileAlreadyCreated = File.Exists(Root + packageName + "/" + entry.Key);
                       
                       if(isThisFileAlreadyCreated)
                        if (GUILayout.Button("Configure"))
                        {
                            PackageJsonController.CreateWizard();
                        }
                    }
                      
                } 
                else if (entry.Key == LICENSE_MD)
                {
                    if (entry.Value)
                    { 
                        var isThisFileAlreadyCreated = File.Exists(Root + packageName + "/" + entry.Key);

                        if (isThisFileAlreadyCreated)
                        {
                            
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
                            #endregion
                            
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Configure"))
                            {
                                PackageJsonController.CreateWizard();
                            }
                            if(GUILayout.Button("Download"))
                            {
                                Application.OpenURL("https://choosealicense.com/");
                            }
                            GUILayout.EndHorizontal();
                        }
                           
                    }
                      
                } 
                else  if (entry.Key == "Editor") _isCreateAsseblyDefinition_Editor = entry.Value; 
                    
            }

            if (!string.IsNullOrEmpty(warningMessage) && Time.realtimeSinceStartup - warningStartTime < warningDuration)
            {
                EditorGUILayout.HelpBox(warningMessage, messageType);
            }
            
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate UPM System", GUILayout.Height(40)))
            {
                if (packageName == "")
                {
                    ShowNotification("Please enter a package name", MessageType.Error);
                    return;
                }
                UPMAssistantDataManager.SavePackageNameData(packageName);
                 
                
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

           Refresh();
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
           string assemblyDefinitionContent = GetAssemblyDefinitionContent(fileName);
           
           //if(string.IsNullOrEmpty(assemblyDefinitionContent)) 
               File.WriteAllText(filePath, assemblyDefinitionContent);
          
       }

       private string GetAssemblyDefinitionContent(string fileName)
       {
           string assemblyDefinitionContent = "";

           if (fileName.Contains(".editor"))
           {
               assemblyDefinitionContent = 
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
           }else if (fileName.Contains(".tests"))
           {
               if (fileName.Contains("editmode"))
               {
                    assemblyDefinitionContent =
                       "{\n" +
                       $"  \"name\": \"editmode.tests\",\n" +
                       "  \"allowUnsafeCode\": false,\n" +
                       "  \"autoReferenced\": false,\n" +
                       "  \"overrideReferences\": true,\n" +
                       "  \"references\": [\n" +
                       "    \"UnityEngine.TestRunner\",\n" +
                       "    \"UnityEditor.TestRunner\"\n" +
                       "  ],\n" +
                       "  \"optionalUnityReferences\": [],\n" +
                       "  \"includePlatforms\": [\"Editor\"],\n" +
                       "  \"excludePlatforms\": [],\n" +
                       "  \"precompiledReferences\": [\n" +
                       "    \"nunit.framework.dll\"\n" + 
                       "  ],\n" +
                       "  \"defineConstraints\": [\n" +
                       "    \"UNITY_INCLUDE_TESTS\"\n" +
                       "  ]\n" +
                       "}";
               }
               else if (fileName.Contains("playmode"))
               {
                   assemblyDefinitionContent =
                       "{\n" +
                       $"  \"name\": \"playmode.tests\",\n" +
                       "  \"allowUnsafeCode\": false,\n" +
                       "  \"autoReferenced\": false,\n" +
                       "  \"overrideReferences\": true,\n" +
                       "  \"references\": [\n" +
                       "    \"UnityEngine.TestRunner\",\n" +
                       "    \"UnityEditor.TestRunner\"\n" +
                       "  ],\n" +
                       "  \"optionalUnityReferences\": [],\n" +
                       "  \"includePlatforms\": [],\n" +
                       "  \"excludePlatforms\": [],\n" +
                       "  \"precompiledReferences\": [\n" +
                       "    \"nunit.framework.dll\"\n" + 
                       "  ],\n" +
                       "  \"defineConstraints\": [\n" +
                       "    \"UNITY_INCLUDE_TESTS\"\n" +
                       "  ]\n" +
                       "}";
               }
               
           }
           
           
           return assemblyDefinitionContent;
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