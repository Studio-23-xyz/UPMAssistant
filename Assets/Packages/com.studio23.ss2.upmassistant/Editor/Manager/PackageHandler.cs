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
                return JsonConvert.DeserializeObject<PackageJsonData>(jsonString);
            }

            Debug.Log($"Default data loaded");
            return LoadDefaultPackageJsonData();
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

            if (!jsonObject.HasValues)
            {
                Debug.LogError($"Data not found!");
                return;
            }

            File.WriteAllTextAsync(FilePath, jsonString);
            Debug.Log($"File created with data: {FilePath}");
            AssetDatabase.Refresh();
        }

        private static bool UpdateProperties(PackageJsonData jsonData)
        {
            var haveUpdate = false;

            haveUpdate |= UpdateField(jsonData, "name", DataHandler.GetSavedPackagedName());
            haveUpdate |= UpdateField(jsonData, "version", Application.version);
            haveUpdate |= UpdateField(jsonData, "displayName", Application.productName);
            haveUpdate |= UpdateField(jsonData, "licensesUrl", DataHandler.GetSavedLicenseURL());
            haveUpdate |= UpdateField(jsonData, "author.name", Application.companyName);

            return haveUpdate;
        }

        // Check is there any update on unity PlayerSettings or github license
        private static bool UpdateField<T>(PackageJsonData jsonData, string fieldName, T newValue)
        {
            var field = jsonData.GetType().GetField(fieldName);
            if (field != null)
            {
                var currentValue = field.GetValue(jsonData);
                if (!EqualityComparer<T>.Default.Equals((T) currentValue, newValue))
                {
                    field.SetValue(jsonData, newValue);
                    return true;
                }
            }

            return false;
        }

        private static PackageJsonData LoadDefaultPackageJsonData()
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("DefaultPackageJsonData");
            PackageJsonData defaultJsonData = null;
            if (jsonFile != null)
            {
                defaultJsonData = JsonConvert.DeserializeObject<PackageJsonData>(jsonFile.text);
            }

            PackageJsonData data = new PackageJsonData();
            data.Name = DataHandler.GetSavedPackagedName();
            data.Version = Application.version;
            data.DisplayName = Application.productName;
            if (defaultJsonData != null) data.Description = defaultJsonData.Description;
            string[] unityVersion = Application.unityVersion.Split(".");
            data.Unity = $"{unityVersion[0]}.{unityVersion[1]}";
            data.UnityRelease = unityVersion[2];
            data.DocumentationUrl = $"https://openupm.com/packages/{DataHandler.GetSavedPackagedName()}";
            data.ChangelogUrl = $"https://openupm.com/packages/{DataHandler.GetSavedPackagedName()}";
            data.LicensesUrl = $"{DataHandler.GetSavedLicenseURL()}";
            if (defaultJsonData != null)
            {
                ScopedRegistry scopedRegistry = defaultJsonData.ScopedRegistries[0];
                data.ScopedRegistries = new List<ScopedRegistry> {scopedRegistry,};
            }

            if (defaultJsonData != null) data.Dependencies = defaultJsonData.Dependencies;
            data.Keywords = Application.productName.Split(" ").ToList();
            ;
            data.Author.Name = Application.companyName;
            if (defaultJsonData != null) data.Author.Email = defaultJsonData.Author.Email;
            if (defaultJsonData != null) data.Author.URL = defaultJsonData.Author.URL;

            return data;
        }
    }
}