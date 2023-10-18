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
                    GitHubLicenseAPIController.FetchOnlineGitLicenses();
                    if (_instance == null) Debug.LogError("SharedGUIContent asset not found in Resources folder!");
                }

                return _instance;
            }
        }
        public void DrawGUIContent()
        {
          
            #region Licenses

            if (GitHubLicenseAPIController.gitHubLicense != null && GitHubLicenseAPIController.gitHubLicense.Count > 0)
            {
                List<string> licenseNames = new List<string>();
                foreach (var license in GitHubLicenseAPIController.gitHubLicense)  licenseNames.Add(license.name);

                GitHubLicenseAPIController.SelectedLicenceIndex = EditorGUILayout.Popup("Select License", GitHubLicenseAPIController.SelectedLicenceIndex, licenseNames.ToArray());
                
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(GitHubLicenseAPIController.IsDownloading);
                if (GUILayout.Button("Configure"))
                {
                    var licenseFilePath =
                        Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.LICENSE_MD);
                    if (File.Exists(licenseFilePath))
                        UPMFileEditorWindowController.ShowWindow(licenseFilePath);
                    else  Debug.LogError("License file not found!");
                }
               
                if(GUILayout.Button("Download"))
                {
                    DataManager.SaveLicenseURLData(GitHubLicenseAPIController.GetLicenseURL());
                    GitHubLicenseAPIController.DownloadLicence();
                    
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
