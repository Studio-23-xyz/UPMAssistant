using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Studio23.SS2.UPMAssistant.Editor;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    [CreateAssetMenu(fileName = "SharedGUIContent", menuName = "Editor/Shared GUI Content")]
    public class SharedGUIContent : ScriptableObject
    {
        private static SharedGUIContent _instance;

        
        public static SharedGUIContent Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SharedGUIContent>("SharedGUIContent");
                    GitHubLicenseHandler.FetchOnlineGitLicenses();
                    if (_instance == null) Debug.LogError("SharedGUIContent asset not found in Resources folder!");
                }

                return _instance;
            }
        }
        public void DrawGUIContent()
        {
          
            #region Licenses

            if (GitHubLicenseHandler.gitHubLicense != null && GitHubLicenseHandler.gitHubLicense.Count > 0)
            {
                List<string> licenseNames = new List<string>();
                foreach (var license in GitHubLicenseHandler.gitHubLicense)  licenseNames.Add(license.name);

                GitHubLicenseHandler.SelectedLicenceIndex = EditorGUILayout.Popup("Select License", GitHubLicenseHandler.SelectedLicenceIndex, licenseNames.ToArray());
                
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(GitHubLicenseHandler.IsDownloading);
                if (GUILayout.Button("Configure"))
                {
                    var licenseFilePath =
                        Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.LICENSE_MD);
                    if (File.Exists(licenseFilePath))
                        UPMEditorWindow.ShowWindow(licenseFilePath);
                    else  Debug.LogError("License file not found!");
                }
               
                if(GUILayout.Button("Download"))
                {
                    DataManager.SaveLicenseURLData(GitHubLicenseHandler.GetLicenseURL());
                    GitHubLicenseHandler.DownloadLicence();
                    
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.EndHorizontal();
                
            }
            else
            {
                GUILayout.Label("Online licenses are not found! Reload window to try again.");
            }
                          
                            
           
            #endregion
            
        }
    }
}
