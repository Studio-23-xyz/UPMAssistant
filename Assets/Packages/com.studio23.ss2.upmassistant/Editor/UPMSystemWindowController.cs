﻿using System.IO;
using System.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMSystemWindowController: EditorWindow
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
            UPMSystemWindowController window =GetWindow<UPMSystemWindowController>("PackageJsonWindowController Window");
            window.minSize = new Vector2(600, 600); 
            window.titleContent = new GUIContent("UPM System Generator Window");
             
        }
        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            AssetDatabase.Refresh();
            DataHandler.Initialized();
            LoadPackageName();
            LoadExistenceFileFolderStructure();
           
        }
        private static void RestartWindow()
        {
            UPMSystemWindowController window = GetWindow<UPMSystemWindowController>();
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
            if (string.IsNullOrEmpty(_packageName)) _packageName = DataHandler.LoadPackageNameData(); 
        }
        private void LoadExistenceFileFolderStructure()
        {
            // Initialize the dictionary with file/folder existence check
            foreach (var entry in DataHandler.FolderAndFilesList.ToList())
            {
                if (entry.Key.Contains("."))
                {
                    // For files, check if the file exists
                    var entryKey = $"{entry.Key.Replace("[[packagename]]", $"{_packageName}")}";
                    DataHandler.FolderAndFilesList[entry.Key] = File.Exists(DataHandler.ROOT + _packageName + "/" + entryKey);
                }
                else
                {
                    // For folders, check if the folder exists
                    DataHandler.FolderAndFilesList[entry.Key] = AssetDatabase.IsValidFolder(DataHandler.ROOT + _packageName + "/" + entry.Key);
                }
            }
        }
       
        
        private void OnGUI()
        {

            #region TopMenu

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Check All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in DataHandler.FolderAndFilesList.ToList())
                {
                    DataHandler.FolderAndFilesList[entry.Key] = true;
                }
            }
            if (GUILayout.Button("Uncheck All", GUILayout.Width(position.width / 4 - 5)))
            {
                foreach (var entry in DataHandler.FolderAndFilesList.ToList())
                {
                    DataHandler.FolderAndFilesList[entry.Key] = false;
                }
            }
            if (GUILayout.Button("Refresh", GUILayout.Width(position.width / 4 - 5)))
            {
                if(_packageName != "") 
                    DataHandler.SavePackageNameData(_packageName);
                Refresh();
            }
            if (GUILayout.Button("Delete System", GUILayout.Width(position.width / 4 - 5)))
            {
              
                if (DataHandler.LoadPackageNameData() != null)
                {
                    bool userConfirmed = EditorUtility.DisplayDialog("Delete Folder",
                        "Are you sure you want to delete the folder and its contents?",
                        "Yes", "No");

                    if (userConfirmed)
                    {
                        //DeleteFolder(Root + packageName);
                        //PlayerPrefs.DeleteKey("packageName");
                        DataHandler.DeletedSaveData();
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
            
         

            bool isShowNestedAssemblyDefOption = false;
            foreach (var entry in DataHandler.FolderAndFilesList.ToList())
            {
               
                string extension = entry.Key.Split(".").LastOrDefault();

                if (extension == "asmdef")
                {
                    if (isShowNestedAssemblyDefOption)
                    {
                        var entryKey = $"{entry.Key.Replace("[[packagename]]", $"{_packageName}")}";
                        DataHandler.FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entryKey, entry.Value, GUILayout.Width(position.width - 20));
                    }
                }
                else  DataHandler.FolderAndFilesList[entry.Key] = EditorGUILayout.ToggleLeft(entry.Key, entry.Value);



                #region Configure

                if (entry.Key == DataHandler.PACKAGE_JSON)
                {
                    if (entry.Value)
                    { 
                        var isThisFileAlreadyCreated = File.Exists(DataHandler.ROOT + _packageName + "/" + entry.Key);

                        if (isThisFileAlreadyCreated)
                        {
                            if (GUILayout.Button("Configure"))
                            {
                                PackageJsonWindowController.CreateWizard();
                            }
                        }
                        
                    }
                      
                } 
                else if (entry.Key == DataHandler.README_MD || entry.Key == DataHandler.CHANGELOG_MD || entry.Key == DataHandler.THIRDPARTYNOTICES_MD)
                {
                    if (entry.Value)
                    { 
                        var fileExist =
                            Path.Combine(DataHandler.ROOT + DataHandler.LoadPackageNameData(), entry.Key);

                        if (File.Exists(fileExist))
                        {
                            if (GUILayout.Button("Configure"))
                            {
                                FileEditorWindowController.ShowWindow(fileExist);
                            }
                        }
                           
                    }
                      
                } 
                else if (entry.Key == DataHandler.LICENSE_MD)
                {
                    if (entry.Value)
                    { 
                        var isThisFileAlreadyCreated = File.Exists(DataHandler.ROOT + _packageName + "/" + entry.Key);

                        if (isThisFileAlreadyCreated)
                        {
                            
                            #region Licenses
                            SharedGUIContent.Instance.DrawGUIContent();
                            #endregion
                        }
                           
                    }
                      
                } 
                else  if (entry.Key == "Editor") isShowNestedAssemblyDefOption = entry.Value; 
                else  if (entry.Key == "Runtime") isShowNestedAssemblyDefOption = entry.Value; 
                else  if (entry.Key == "Tests/EditMode") isShowNestedAssemblyDefOption = entry.Value; 
                else  if (entry.Key == "Tests/PlayMode") isShowNestedAssemblyDefOption = entry.Value; 

                #endregion
                    
            }

            #region Notification

            if (!string.IsNullOrEmpty(_warningMessage) && Time.realtimeSinceStartup - _warningStartTime < WarningDuration)
            {
                EditorGUILayout.HelpBox(_warningMessage, _messageType);
            }

            #endregion
            
            GUILayout.Space(10);

            #region SubmitOption

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate UPM System", GUILayout.Height(40)))
            {
                if (_packageName == "")
                {
                    ShowNotification("Please enter a package name", MessageType.Error);
                    return;
                }
                DataHandler.SavePackageNameData(_packageName);
                CreateFolderStructure();
            }

            #endregion
             
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
           
           var packageDirectory = DataHandler.ROOT + DataHandler.LoadPackageNameData();
           if (!Directory.Exists(packageDirectory))
           {
               Directory.CreateDirectory(packageDirectory);
               Debug.Log("Folder structure created: " + packageDirectory);
           }
           

           foreach (var entry in DataHandler.FolderAndFilesList)
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

       private void CreateFile(string packageDirectory, string fileName)
       {
           string extension = fileName.Split(".").LastOrDefault();
           
           string filePath = Path.Combine(packageDirectory, fileName);
           if (!File.Exists(filePath) && extension == "asmdef")
           {
               CreateFileAsmdef(packageDirectory, fileName);
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
       void CreateFileAsmdef(string packageDirectory, string fileName)
       {
           string filePath = Path.Combine(packageDirectory, $"{fileName.Replace("[[packagename]]",$"{DataHandler.LoadPackageNameData()}")}");
           
           var assemblyDefinitionContent = DataHandler.GetAssemblyDefinitionContent(fileName);
           
           string directory = Path.GetDirectoryName(filePath); //string fileName = Path.GetFileName(filePath);
           
           if (!Directory.Exists(directory))
           {
               if (directory != null) Directory.CreateDirectory(directory);
           }
           
           if (assemblyDefinitionContent != "")
           {
               File.WriteAllText(filePath, assemblyDefinitionContent);
           }
           else  Debug.LogError("Assembly Definition is not created!");
          
       }

       private static void CreateFolder(string filePath)
       {
           if (!AssetDatabase.IsValidFolder(filePath))
           {
               AssetDatabase.CreateFolder(Path.GetDirectoryName(filePath), Path.GetFileName(filePath));
               Debug.Log("Folder created: " + filePath);
           }
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