using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Studio23.SS2.UPMAssistant.Editor.Data;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor
{
    
    public static class PackageHandler
    {
        public static string FilePath;
        public static PackageJsonData LoadData()
        {


            if (File.Exists(FilePath))
            {
                var jsonString = File.ReadAllText(FilePath);
                
                if (string.IsNullOrWhiteSpace(jsonString))
                {
                    Debug.Log($"package.json is empty. Default data is loaded");
                     return LoadDefaultPackageJsonData();
                }
                Debug.Log($"Previous data is loaded");
               return  JsonConvert.DeserializeObject<PackageJsonData>(jsonString);
            }

            Debug.Log($"Default data loaded");
            return  LoadDefaultPackageJsonData();
        }

        public static PackageJsonData CheckUpdateOnDisabledProperties(PackageJsonData jsonData)
        {
            var haveUpdate = UpdateProperties(jsonData);
            if (haveUpdate)
            {
                SaveData(jsonData);
            }

            return jsonData;
        }

        public static void SaveData(PackageJsonData newJsonData)
        {
            string jsonString = JsonConvert.SerializeObject(newJsonData, Formatting.Indented);
            JObject jsonObject = JObject.Parse(jsonString);

            if (jsonObject == null || !jsonObject.HasValues)
            {
                Debug.LogError($"Data not found!");
                return;
            }

            File.WriteAllText(FilePath, jsonString);
            Debug.Log($"File created with data: {FilePath}");
            AssetDatabase.Refresh();
        }
        
        private static bool UpdateProperties(PackageJsonData jsonData)
        {
            var haveUpdate = false;

            haveUpdate |= UpdateField(jsonData, "name", DataHandler.LoadPackageNameData());
            haveUpdate |= UpdateField(jsonData, "version", Application.version);
            haveUpdate |= UpdateField(jsonData, "displayName", Application.productName);
            haveUpdate |= UpdateField(jsonData, "licensesUrl", DataHandler.LoadLicenseURLData());
            haveUpdate |= UpdateField(jsonData.author, "name", Application.companyName);

            return haveUpdate;
        }
        private static bool UpdateField<T>(object obj, string fieldName, T newValue)
        {
            var field = obj.GetType().GetField(fieldName);
            if (field != null)
            {
                var currentValue = field.GetValue(obj);
                if (!EqualityComparer<T>.Default.Equals((T)currentValue, newValue))
                {
                    field.SetValue(obj, newValue);
                    return true;
                }
            }

            return false;
        }
        
        private static PackageJsonData LoadDefaultPackageJsonData()
        {
            PackageJsonData data = new PackageJsonData();
            data.name = DataHandler.LoadPackageNameData();
            data.version = Application.version; // 1.0.1
            data.displayName = Application.productName; // UPM Assistant
            data.description = "The UPM Assistant is an editor extension tool designed to simplify the process of creating folder structures required for Unity packages that are published on  openupm.com";
            string[] unityVersion = Application.unityVersion.Split(".");
            data.unity = unityVersion[0] +"."+ unityVersion[1];//"2022.3";
            data.unityRelease = unityVersion[2]; //"9f1";
            data.documentationUrl = $"https://openupm.com/packages/{DataHandler.LoadPackageNameData()}";
            data.changelogUrl = $"https://openupm.com/packages/{DataHandler.LoadPackageNameData()}";
            data.licensesUrl = $"{DataHandler.LoadLicenseURLData()}";//"https://api.github.com/licenses/mit";

            ScopedRegistry scopedRegistry = new ScopedRegistry()
            {
                name = "Studio 23",
                url = "https://package.openupm.com",
                scopes = new List<string> { "com.studio23.ss2"}
            };
            data.scopedRegistries = new List<ScopedRegistry>
            {
                scopedRegistry,
            };
           
           
            data.dependencies = new Dictionary<string, string>(){
                {"com.unity.nuget.newtonsoft-json", "3.2.1"},
                
            };
            

            data.keywords = Application.productName.Split(" ").ToList();; //new List<string> { "UPM", "Assistant", "System" };
            data.author.name = Application.companyName;// Studio 23
            data.author.email = "contact@studio-23.xyz";
            data.author.url = "https://studio-23.xyz";

            return data;
        } 
    }
}