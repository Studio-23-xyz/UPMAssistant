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
                    APIHandler.FetchOnlineGitLicenses();
                    if (_instance == null) Debug.LogError("SharedGUIContent asset not found in Resources folder!");
                }

                return _instance;
            }
        }
        public void DrawGUIContent()
        {
          
            #region Licenses

            if (APIHandler.GitHubLicense != null && APIHandler.GitHubLicense.Count > 0)
            {
                List<string> licenseNames = new List<string>();
                foreach (var license in APIHandler.GitHubLicense)  licenseNames.Add(license.Name);

                APIHandler.SelectedLicenceIndex = EditorGUILayout.Popup("Select License", APIHandler.SelectedLicenceIndex, licenseNames.ToArray());
                
                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(APIHandler.IsDownloading);
                if (GUILayout.Button("Configure"))
                {
                    var licenseFilePath =
                        Path.Combine(DataHandler.Root + DataHandler.GetSavedPackagedName(), DataHandler.LicenseMD);
                    if (File.Exists(licenseFilePath))
                        FileEditorWindowController.ShowWindow(licenseFilePath);
                    else  Debug.LogError("License file not found!");
                }
               
                if(GUILayout.Button("Download"))
                {
                    DataHandler.SaveLicenseURLData(APIHandler.GetLicenseURL());
                    APIHandler.DownloadLicence();
                    
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
