using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class UPMAssistantDataManager
{
    private static readonly string ROOT = "Assets/Packages/";
    private static readonly string DATA_PATH = "Assets/UPMAssistant/Editor/Data";
    private static readonly string LICENSE_URL = "licenseURLData.json";
    private static readonly string PACKAGE_NAME = "packageNameData.json";
   
    public const string DefaultPackageName = "com.[companyname].[project].[packagename]";
    public const string DefaultLicenceURL = "https://api.github.com/licenses/mit";//"https://api.github.com/licenses/lgpl-2.1";
    
    public static void Initialized()
    {
        if (!Directory.Exists(ROOT))
        {
            Directory.CreateDirectory(ROOT);
            Debug.Log("Folder structure created: " + ROOT);
        }
        if (!Directory.Exists(DATA_PATH))
        {
            Directory.CreateDirectory(DATA_PATH);
            Debug.Log("Folder structure created: " + DATA_PATH);
        } 
        
        var licensePath = Path.Combine(DATA_PATH, LICENSE_URL);
        if (!File.Exists(licensePath))
        {
            LicenseURLData data = new LicenseURLData { LicenseURL = DefaultLicenceURL };
            string jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(licensePath, jsonData);
            // File.Create(licensePath).Close();
        }
        
        var packageNamePath = Path.Combine(DATA_PATH, PACKAGE_NAME);
        if (!File.Exists(packageNamePath))
        {
            DefaultPackageName.Replace("[companyname]", PlayerSettings.companyName);
            DefaultPackageName.Replace("[project]", PlayerSettings.productName);
            DefaultPackageName.Replace("[packagename]", PlayerSettings.productName);
            PackageNameData data = new PackageNameData { PackageName = DefaultPackageName.ToLower() };
            string jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(packageNamePath, String.Empty);
        }
    }
    public static void SaveLicenseURLData(string licenseURL)
    {
        var licensePath = Path.Combine(DATA_PATH, LICENSE_URL);
        
        LicenseURLData data = new LicenseURLData { LicenseURL = licenseURL };
        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(licensePath, jsonData);
    }

    public static string LoadLicenseURLData()
    {
        var licensePath = Path.Combine(DATA_PATH, LICENSE_URL);
        
        if (File.Exists(licensePath))
        {
            string jsonData = File.ReadAllText(licensePath);
            LicenseURLData data = JsonUtility.FromJson<LicenseURLData>(jsonData);
            return data.LicenseURL;
        }
        else
        {
            Debug.LogError("License URL data file not found!");
            return null;
        }
    }
    public static void SavePackageNameData(string packageName)
    {
        var packageNamePath = Path.Combine(DATA_PATH, PACKAGE_NAME);
        
        PackageNameData data = new PackageNameData { PackageName = packageName };
        string jsonData = JsonUtility.ToJson(data);
        
        File.WriteAllText(packageNamePath, jsonData);
    }

    public static string LoadPackageNameData()
    {
        var packageNamePath = Path.Combine(DATA_PATH, PACKAGE_NAME);
        
        if (File.Exists(packageNamePath))
        {
            string jsonData = File.ReadAllText(packageNamePath);
            PackageNameData data = JsonUtility.FromJson<PackageNameData>(jsonData);
            return data.PackageName;
        }
        else
        {
            Debug.LogError("Package name data file not found!");
            return null;
        }
    }

   
}