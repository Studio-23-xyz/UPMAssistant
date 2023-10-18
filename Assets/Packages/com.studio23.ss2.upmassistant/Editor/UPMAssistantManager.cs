using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMAssistantManager: EditorWindow
    {
       
        // private static string _root;
        private static string _packageName;
        
        // warning message
        private string _warningMessage = ""; // Warning message to display
        private const float WarningDuration = 3f; // Duration to display the warning in seconds
        private float _warningStartTime; // Time when the warning started
        private MessageType _messageType = MessageType.Info;
        
        [MenuItem("Studio-23/UPM Assistant/Generator", priority = 0)]
        public static void ShowWindow()
        {
            UPMAssistantManager window =GetWindow<UPMAssistantManager>("PackageJsonController Window");
            window.titleContent = new GUIContent("UPM System Generator Window");
             
        }
        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            AssetDatabase.Refresh();
            DataManager.Initialized();
            LoadPackageName();
            LoadExistenceFileFolderStructure();
           
        }
        private static void RestartWindow()
        {
            UPMAssistantManager window = GetWindow<UPMAssistantManager>();
            if (window != null)
            {
                window.Close();
                ShowWindow();
            }
            else
            {
                Debug.LogError("My Custom Editor Window not found.");
            }
        }
         private void LoadPackageName()
         {
            _packageName = "";
            if (string.IsNullOrEmpty(_packageName)) _packageName = DataManager.LoadPackageNameData(); 
        }
        private void LoadExistenceFileFolderStructure()
        {
            // Initialize the dictionary with file/folder existence check
            foreach (var entry in DataManager.FolderAndFilesList.ToList())
            {
               // Debug.Log(_root + _packageName + "/" + entry.Key);
                if (entry.Key.Contains("."))
                {
                    // For files, check if the file exists
                    DataManager.FolderAndFilesList[entry.Key] = File.Exists(DataManager.ROOT + _packageName + "/" + entry.Key);
                }
                else
                {
                    // For folders, check if the folder exists
                    DataManager.FolderAndFilesList[entry.Key] = AssetDatabase.IsValidFolder(DataManager.ROOT + _packageName + "/" + entry.Key);
                }
            }
        }
       
        
        private void OnGUI()
        {

            #region TopMenu

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in DataManager.FolderAndFilesList.ToList())
                {
                    DataManager.FolderAndFilesList[entry.Key] = true;
                }
            }
            if (GUILayout.Button("Uncheck All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in DataManager.FolderAndFilesList.ToList())
                {
                    DataManager.FolderAndFilesList[entry.Key] = false;
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(position.width / 4 - 5)))
            {
                if(_packageName != "") 
                    DataManager.SavePackageNameData(_packageName);
                Refresh();
            }
            if (GUILayout.Button("Delete System", GUILayout.Width(position.width / 4 - 5)))
            {
              
                if (DataManager.LoadPackageNameData() != null)
                {
                    bool userConfirmed = EditorUtility.DisplayDialog("Delete Folder",
                        "Are you sure you want to delete the folder and its contents?",
                        "Yes", "No");

                    if (userConfirmed)
                    {
                        //DeleteFolder(Root + packageName);
                        //PlayerPrefs.DeleteKey("packageName");
                        DataManager.DeletedSaveData();
                        RestartWindow();
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
           

             
            _packageName = EditorGUILayout.TextField("Assets/Packages/", _packageName, GUILayout.Width(position.width - 20));
            
         

            bool _isCreateAsseblyDefinition_Editor = false;
            foreach (var entry in DataManager.FolderAndFilesList.ToList())
            {
               
                string extension = entry.Key.Split(".").LastOrDefault();

                if (extension == "asmdef")
                {
                    if (_isCreateAsseblyDefinition_Editor)
                    {
                        
                        var entryKey = $"{entry.Key.Replace("[[packagename]]", $"{_packageName}")}";
                        DataManager.FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entryKey, entry.Value,
                            GUILayout.Width(position.width - 20));
                    }
                }
                else  DataManager.FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value);
                
                
                if (entry.Key == DataManager.PACKAGE_JSON)
                {
                    if (entry.Value)
                    { 
                       var isThisFileAlreadyCreated = File.Exists(DataManager.ROOT + _packageName + "/" + entry.Key);
                       
                       if(isThisFileAlreadyCreated)
                        if (GUILayout.Button("Configure"))
                        {
                            PackageJsonController.CreateWizard();
                        }
                    }
                      
                } 
                else if (entry.Key == DataManager.LICENSE_MD)
                {
                    if (entry.Value)
                    { 
                        var isThisFileAlreadyCreated = File.Exists(DataManager.ROOT + _packageName + "/" + entry.Key);

                        if (isThisFileAlreadyCreated)
                        {
                            
                            #region Licenses
                            SharedGUIContent.Instance.DrawGUIContent();
                            /*if (gitHubLicense != null && gitHubLicense.Count > 0)
                            {
                                List<string> licenseNames = new List<string>();
                                foreach (var license in gitHubLicense)  licenseNames.Add(license.name);

                                SelectedLicenceIndex = EditorGUILayout.Popup("Select License", SelectedLicenceIndex, licenseNames.ToArray());
                            }
                            else
                            {
                                GUILayout.Label("Online licenses are not found! Loading...");
             
                            }
                          
                            
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Configure"))
                            {
                                PackageJsonController.CreateWizard();
                            }
                            if(GUILayout.Button("Download"))
                            {
                                Application.OpenURL("https://choosealicense.com/");
                            }
                            GUILayout.EndHorizontal();*/
                            #endregion
                        }
                           
                    }
                      
                } 
                else  if (entry.Key == "Editor") _isCreateAsseblyDefinition_Editor = entry.Value; 
                    
            }

            if (!string.IsNullOrEmpty(_warningMessage) && Time.realtimeSinceStartup - _warningStartTime < WarningDuration)
            {
                EditorGUILayout.HelpBox(_warningMessage, _messageType);
            }
            
            GUILayout.Space(10);
           
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate UPM System", GUILayout.Height(40)))
            {
                if (_packageName == "")
                {
                    ShowNotification("Please enter a package name", MessageType.Error);
                    return;
                }
                DataManager.SavePackageNameData(_packageName);
                CreateFolderStructure();
            }
             
            GUI.backgroundColor = Color.white; // Reset the background color
        }

       
        private void ShowNotification(string message, MessageType msgType = MessageType.Info)
        {
            _messageType = msgType;
            _warningMessage = message;
            _warningStartTime = Time.realtimeSinceStartup;
        }
       public void CreateFolderStructure()
       {
           
           var packageDirectory = DataManager.ROOT + DataManager.LoadPackageNameData();
           if (!Directory.Exists(packageDirectory))
           {
               Directory.CreateDirectory(packageDirectory);
               Debug.Log("Folder structure created: " + packageDirectory);
           }
           
           /*string rootPath = _root + packageName;
           if (!AssetDatabase.IsValidFolder(rootPath))
           {
               AssetDatabase.CreateFolder("Assets/Packages", packageName);
               Debug.Log("Root folder created: " + rootPath);
           }
           else
           {
               Debug.Log("Root folder already exists: " + rootPath);
           }*/

           foreach (var entry in DataManager. FolderAndFilesList)
           {
               if (entry.Value)
               {
                   if (entry.Key.Contains("."))
                   {
                       CreateFile(packageDirectory, entry.Key);
                   }
                   else
                   {
                       CreateFolder(packageDirectory + "/" + entry.Key);
                   }
               }
           }

           Refresh();
           ShowNotification($"Folder structure created successfully.");
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
       private void CreateFile(string path, string fileName)
       {
           
           string extension = fileName.Split(".").LastOrDefault();
           
           string filePath = Path.Combine(path, fileName);
           if (!File.Exists(filePath) && extension == "asmdef")
           {
               CreateFileAsmdef( path,  fileName);
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
       void CreateFileAsmdef(string path, string fileName)
       {
           string filePath = Path.Combine(path,$"{fileName.Replace("[[packagename]]",$"{_packageName}")}");
           // Define the content of the assembly definition file
           string assemblyDefinitionContent = DataManager.GetAssemblyDefinitionContent(fileName);
           //if(string.IsNullOrEmpty(assemblyDefinitionContent)) 
               File.WriteAllText(filePath, assemblyDefinitionContent);
          
       }
       
       /*private void DeleteFolder(string folderPath)
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
       }*/
    }
}