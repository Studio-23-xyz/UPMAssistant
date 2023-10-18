
using System.IO;
using System.Threading.Tasks;


namespace Studio23.SS2.UPMAssistant.Editor
{
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;

public static class GitHubLicenseHandler  
{
    
    private const string APIURL = "https://api.github.com/licenses";
    public static List<GitHubLicense> gitHubLicense;
    public static License license;
    public static int SelectedLicenceIndex;
    public static string SelectedLicenceURL = "https://api.github.com/licenses/mit"; // "https://api.github.com/licenses/lgpl-2.1";
   
    /*[MenuItem("Studio-23/GitHub Licenses")]
    public static void ShowWindow()
    {
        GetWindow<GitHubLicenseHandler>("GitHub Licenses");
    }*/

    /*private void OnEnable()
    {
        FetchOnlineGitLicenses();
       
    }*/

    public static void FetchOnlineGitLicenses()
    {
        SelectedLicenceURL = DataManager.LoadLicenseURLData();
        UnityWebRequest www = UnityWebRequest.Get(APIURL);
        www.SendWebRequest().completed += FetchLicensesCallback;
    }

    private static void FetchLicensesCallback(AsyncOperation operation)
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
            SelectedLicenceIndex =  GetLicenseIndex(SelectedLicenceURL);
           
        }
    }
    
   
    /*private void OnGUI()
    {
        sharedContent.DrawGUIContent();
        
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
    }*/

    
    public static bool IsDownloading = false;
    public static void DownloadLicence()
    {
        IsDownloading = true;
        UnityWebRequest www = UnityWebRequest.Get(DataManager.LoadLicenseURLData());
            www.SendWebRequest().completed += FetchLicensesCallback2;
    }

   
    private static void FetchLicensesCallback2(AsyncOperation operation)
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
            var licenseFilePath =
                Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.LICENSE_MD);
            if (!File.Exists(licenseFilePath))
            {
                Debug.LogError("License file not found!");
                return;
            }
            File.WriteAllText(licenseFilePath, string.Empty);
            File.WriteAllText(licenseFilePath, license.body);

        }
        IsDownloading = false;
    }
    public static int GetLicenseIndex(string url)
    {
        int currentSelectedLicence = 0;
        if (gitHubLicense is {Count: > 0})
        {
            currentSelectedLicence = gitHubLicense.IndexOf(gitHubLicense.Find(x => x.url == url));
        }
        return currentSelectedLicence;
    }
    public static string GetLicenseURL()
    {
        var currentSelectedLicence = DataManager.DefaultLicenceURL;
        if (gitHubLicense is {Count: > 0})
        {
            currentSelectedLicence = gitHubLicense[SelectedLicenceIndex].url;
        }
        return currentSelectedLicence;

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