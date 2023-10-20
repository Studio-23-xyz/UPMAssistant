using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public static class APIHandler
    {
        private const string ApiURL = "https://api.github.com/licenses";
        public static int SelectedLicenceIndex;
        public static List<GitHubLicense> GitHubLicense;
        public static bool IsDownloading;

        private static string _selectedLicenceURL;

        public static void FetchOnlineGitLicenses()
        {
            _selectedLicenceURL = DataHandler.GetSavedLicenseURL();
            var www = UnityWebRequest.Get(ApiURL);
            www.SendWebRequest().completed += FetchLicensesCallback;
        }

        private static void FetchLicensesCallback(AsyncOperation operation)
        {
            var www = ((UnityWebRequestAsyncOperation) operation).webRequest;

            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("ConnectionError: " + www.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("ProtocolError: " + www.error);
                    break;
                default:
                {
                    var jsonResponse = www.downloadHandler.text;
                    GitHubLicense = JsonConvert.DeserializeObject<List<GitHubLicense>>(jsonResponse);
                    SelectedLicenceIndex = GetLicenseIndex(_selectedLicenceURL);
                    break;
                }
            }
        }

        public static void DownloadLicence()
        {
            IsDownloading = true;
            var www = UnityWebRequest.Get(DataHandler.GetSavedLicenseURL());
            www.SendWebRequest().completed += FetchLicensesCallback2;
        }

        private static void FetchLicensesCallback2(AsyncOperation operation)
        {
            var www = ((UnityWebRequestAsyncOperation) operation).webRequest;

            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("ConnectionError: " + www.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("ProtocolError: " + www.error);
                    break;
                default:
                {
                    var jsonResponse = www.downloadHandler.text;
                    var license = JsonConvert.DeserializeObject<License>(jsonResponse);
                    var licenseFilePath = Path.Combine(DataHandler.Root, DataHandler.GetSavedPackagedName(),
                        DataHandler.LicenseMD);
                    if (!File.Exists(licenseFilePath))
                    {
                        Debug.LogError("License file not found!");
                        return;
                    }

                    File.WriteAllTextAsync(licenseFilePath, string.Empty);
                    File.WriteAllTextAsync(licenseFilePath, license.Body);
                    AssetDatabase.Refresh();
                    break;
                }
            }

            IsDownloading = false;
        }

        public static int GetLicenseIndex(string url)
        {
            var currentSelectedLicence = 0;
            if (GitHubLicense is {Count: > 0})
                currentSelectedLicence = GitHubLicense.IndexOf(GitHubLicense.Find(x => x.URL == url));
            return currentSelectedLicence;
        }

        public static string GetLicenseURL()
        {
            var currentSelectedLicence = DataHandler.DefaultLicenceURL;
            if (GitHubLicense is {Count: > 0}) currentSelectedLicence = GitHubLicense[SelectedLicenceIndex].URL;
            return currentSelectedLicence;
        }
    }
}