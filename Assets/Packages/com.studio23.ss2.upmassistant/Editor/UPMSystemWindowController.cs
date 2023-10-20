using System.IO;
using System.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class UPMSystemWindowController: EditorWindow
    {
        
        private static string _packageName;
        
        // warning message
        private string _warningMessage = ""; // Warning message to display
        private const float WarningDuration = 3f; // Duration to display the warning in seconds
        private float _warningStartTime; // Time when the warning started
        private MessageType _messageType = MessageType.Info;
        
        [MenuItem("Studio-23/UPM Package Template Generator/Wizard", priority = 0)]
        public static void ShowWindow()
        {
            UPMSystemWindowController window =GetWindow<UPMSystemWindowController>();
            window.minSize = new Vector2(600, 600); 
            window.titleContent = new GUIContent("Wizard - UPM Package Template Generator");
             
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
            if (string.IsNullOrEmpty(_packageName)) _packageName = DataHandler.GetSavedPackagedName(); 
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
                    var path = Path.Combine(DataHandler.Root, _packageName, entryKey);
                    DataHandler.FolderAndFilesList[entry.Key] = File.Exists(path);
                }
                else
                {
                    // For folders, check if the folder exists
                    var path = Path.Combine(DataHandler.Root, _packageName, entry.Key);
                    DataHandler.FolderAndFilesList[entry.Key] = AssetDatabase.IsValidFolder(path);
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
              
                if (DataHandler.GetSavedPackagedName() != null)
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

                if (entry.Key == DataHandler.PackageJson)
                {
                    if (entry.Value)
                    { 
                        var filePath = Path.Combine(DataHandler.Root , _packageName, entry.Key);
                        var isThisFileAlreadyCreated = File.Exists(filePath);

                        if (isThisFileAlreadyCreated)
                        {
                            if (GUILayout.Button("Configure"))
                            {
                                PackageJsonWindowController.CreateWizard();
                            }
                        }
                        
                    }
                      
                } 
                else if (entry.Key == DataHandler.ReadmeMD || entry.Key == DataHandler.ChangelogMD || entry.Key == DataHandler.ThirdPartyNoticesMD)
                {
                    if (entry.Value)
                    { 
                        var fileExist =
                            Path.Combine(DataHandler.Root, _packageName, entry.Key);
                            
                        if (File.Exists(fileExist))
                        {
                            if (GUILayout.Button("Configure"))
                            {
                                FileEditorWindowController.Instance.ShowWindow(fileExist);
                            }
                        }
                           
                    }
                      
                } 
                else if (entry.Key == DataHandler.LicenseMD)
                {
                    if (entry.Value)
                    { 
                        var filePath = Path.Combine(DataHandler.Root, _packageName, entry.Key);
                        var isThisFileAlreadyCreated = File.Exists(filePath);

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
            if (GUILayout.Button("Generate", GUILayout.Height(40)))
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

        private void CreateFolderStructure()
       {
           var packageDirectory = Path.Combine(DataHandler.Root, DataHandler.GetSavedPackagedName());
           if (!Directory.Exists(packageDirectory))
           {
               Directory.CreateDirectory(packageDirectory);
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
           string filePath = Path.Combine(packageDirectory, $"{fileName.Replace("[[packagename]]",$"{DataHandler.GetSavedPackagedName()}")}");
           
           var assemblyDefinitionContent = DataHandler.GetAssemblyDefinitionContent(fileName);
           
           string directory = Path.GetDirectoryName(filePath); 
           // Create directory if it doesn't exist
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
               Directory.CreateDirectory(filePath);
               Debug.Log($"Folder created: {filePath}");
           }
           else{
               Debug.Log($"Folder already exists: {filePath}");
           }
           
       }
    }
}