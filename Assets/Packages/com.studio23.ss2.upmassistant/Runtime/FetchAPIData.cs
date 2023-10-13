using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

public class GitHubLicenseFetcher : MonoBehaviour
{
    private const string apiUrl = "https://api.github.com/licenses";

    void Start()
    {
        StartCoroutine(FetchLicenses());
    }

    IEnumerator FetchLicenses()
    {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            // Parse JSON response using Newtonsoft.Json
            string jsonResponse = www.downloadHandler.text;
            List<GitHubLicense> licenses = JsonConvert.DeserializeObject<List<GitHubLicense>>(jsonResponse);

            // Handle the licenses data (you can use it as needed)
            foreach (GitHubLicense license in licenses)
            {
                Debug.Log("License Key: " + license.key);
                Debug.Log("License Name: " + license.name);
                Debug.Log("License URL: " + license.url);
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
}