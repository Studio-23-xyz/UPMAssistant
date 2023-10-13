using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine.Serialization;

namespace Studio23.SS2.UPMAssistant.Editor
{
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public class GitHubLicenseHandler : EditorWindow
{
    private const string APIURL = "https://api.github.com/licenses";
    public List<GitHubLicense> gitHubLicense;
    public License license;
    public int SelectedLicenceIndex;
    public const string DefaultLicenceURL = "https://api.github.com/licenses/mit";//"https://api.github.com/licenses/lgpl-2.1";
    public string SelectedLicenceURL = "https://api.github.com/licenses/mit";

    [MenuItem("Studio-23/GitHub Licenses")]
    public static void ShowWindow()
    {
        GetWindow<GitHubLicenseHandler>("GitHub Licenses");
    }

    private void OnEnable()
    {
        FetchLicenses();
    }

    public void FetchLicenses()
    {
        UnityWebRequest www = UnityWebRequest.Get(APIURL);
        www.SendWebRequest().completed += FetchLicensesCallback;
    }

    private void FetchLicensesCallback(AsyncOperation operation)
    {
        UnityWebRequest www = ((UnityWebRequestAsyncOperation)operation).webRequest;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            gitHubLicense = JsonConvert.DeserializeObject<List<GitHubLicense>>(jsonResponse);
            SelectedLicenceIndex =  SelectLicence(SelectedLicenceURL);
            Repaint();
        }
    }
    
    public int SelectLicence(string url)
    {
        int currentSelectedLicence = 0;
        if (gitHubLicense is {Count: > 0})
        {
            currentSelectedLicence = gitHubLicense.IndexOf(gitHubLicense.Find(x => x.url == url));
        }

        return currentSelectedLicence;

    }
    private void OnGUI()
    {
        
        if (gitHubLicense != null && gitHubLicense.Count > 0)
        {
            List<string> licenseNames = new List<string>();
            foreach (var license in gitHubLicense)  licenseNames.Add(license.name);

            SelectedLicenceIndex = EditorGUILayout.Popup("Select License", SelectedLicenceIndex, licenseNames.ToArray());

            if (SelectedLicenceIndex >= 0 && SelectedLicenceIndex < gitHubLicense.Count)
            {
                GUILayout.Space(10);
                var selectedLicenseObj = gitHubLicense[SelectedLicenceIndex];
                GUILayout.Label($"URL: {selectedLicenseObj.url}");
            }
        }
        else
        {
            GUILayout.Label("Loading licenses...");
        }
    }

  
    
    public void SaveLicense(string licenseFilePath)
    {
        if (File.Exists(licenseFilePath))
        {
            LicenseFilePath = licenseFilePath;
            UnityWebRequest www = UnityWebRequest.Get(gitHubLicense[SelectedLicenceIndex].url);
            www.SendWebRequest().completed += FetchLicensesCallback2;
        }
    }

    private string LicenseFilePath;
    private void FetchLicensesCallback2(AsyncOperation operation)
    {
        UnityWebRequest www = ((UnityWebRequestAsyncOperation)operation).webRequest;

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            license = JsonConvert.DeserializeObject<License>(jsonResponse);
            File.WriteAllText(LicenseFilePath, license.body);
        }
    }
    
}

[System.Serializable]
public class GitHubLicense
{
    public string key;
    public string name;
    public string url;
}

[System.Serializable]
public class License
{
    public string key;
    public string name;
    public string spdx_id;
    public string url;
    public string node_id;
    public string html_url;
    public string description;
    public string implementation;
    public List<string> permissions;
    public List<string> conditions;
    public List<string> limitations;
    public string body;
    public bool featured;
}

}