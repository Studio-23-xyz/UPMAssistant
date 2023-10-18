using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;


namespace Studio23.SS2.UPMAssistant.Editor
{ 
    public static class GitHubLicenseAPIController  
{
    
    private const string APIURL = "https://api.github.com/licenses";
    public static int SelectedLicenceIndex;
    public static List<GitHubLicense> gitHubLicense;
    public static bool IsDownloading = false;
 
    private static string _selectedLicenceURL; 
    
    public static void FetchOnlineGitLicenses()
    {
        _selectedLicenceURL = DataManager.LoadLicenseURLData();
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
            SelectedLicenceIndex =  GetLicenseIndex(_selectedLicenceURL);
           
        }
    }
    
   
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
            var license = JsonConvert.DeserializeObject<License>(jsonResponse);
            var licenseFilePath =
                Path.Combine(DataManager.ROOT + DataManager.LoadPackageNameData(), DataManager.LICENSE_MD);
            if (!File.Exists(licenseFilePath))
            {
                Debug.LogError("License file not found!");
                return;
            }
            File.WriteAllText(licenseFilePath, string.Empty);
            File.WriteAllText(licenseFilePath, license.body);
            AssetDatabase.Refresh();
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






}