using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Codice.Client.BaseCommands.Import;

namespace Studio23.SS2.UPMAssistant.Editor
{
    
 
public static class DataManager
{
   public static readonly string ROOT = "Assets/Packages/";
    public static readonly string DATA_PATH = "Assets/UPMAssistant/Editor/Data";
    public static readonly string LICENSE_URL = "licenseURLData.json";
    public static readonly string PACKAGE_NAME = "packageNameData.json";
   
    public const string DefaultPackageName = "com.[companyname].[project].[packagename]";
    public const string DefaultLicenceURL = "https://api.github.com/licenses/mit";//"https://api.github.com/licenses/lgpl-2.1";
    
    public static readonly string PACKAGE_JSON = "package.json";
    public static readonly string README_MD = "README.md";
    public static readonly string CHANGELOG_MD = "CHANGELOG.md";
    public static readonly string LICENSE_MD = "LICENSE.md";
    public static readonly string THIRDPARTYNOTICES_MD = "ThirdPartyNotices.md";
     
    
    public static Dictionary<string, bool> FolderAndFilesList = new Dictionary<string, bool>
    {
        {PACKAGE_JSON, false},
        {README_MD, false},
        {CHANGELOG_MD, false},
        {LICENSE_MD, false},
        {THIRDPARTYNOTICES_MD, false},
        {"Editor", false},
        {$"Editor/[[packagename]].editor.asmdef", false},
        {"Runtime", false},
        {$"Runtime/[[packagename]].asmdef", false},
        {"Runtime/Data", false},
        {"Runtime/Core", false},
        {"Tests", false},
        {"Tests/EditMode", false},
        {$"Tests/EditMode/editmode.tests.asmdef", false},
        {"Tests/PlayMode", false},
        {$"Tests/PlayMode/playmode.tests.asmdef", false},
        {"Samples", false},
        {"Documentation", false},
           
    };
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
            string updatedPackageName = DefaultPackageName;
            updatedPackageName = updatedPackageName.Replace("[companyname]", PlayerSettings.companyName);
            updatedPackageName = updatedPackageName.Replace("[project]", "ss2"); //Application.productName
            updatedPackageName = updatedPackageName.Replace("[packagename]", PlayerSettings.productName);
            updatedPackageName = updatedPackageName.Replace(" ", "").ToLower();
            PackageNameData data = new PackageNameData { PackageName = updatedPackageName };
            string jsonData = JsonUtility.ToJson(data);
            File.WriteAllText(packageNamePath, jsonData);
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
            Debug.LogError("License URL data file not found! returning default license URL.");
            return DefaultLicenceURL;
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

    public static void DeletedSaveData()
    {
        var packageNamePath = ROOT + LoadPackageNameData();
        if (Directory.Exists(packageNamePath))
        {
            Directory.Delete(packageNamePath, true);
            Debug.Log("All initialized folder & files are deleted successfully.");
        }else
        {
            Debug.LogWarning("Nothing found! No folder & files are deleted.");
        }

        if (Directory.Exists("Assets/UPMAssistant"))
        {
            Directory.Delete("Assets/UPMAssistant", true);
            Debug.Log("All initialized folder & files are deleted successfully.");
        }else
        {
            Debug.LogWarning("Nothing found! No folder & files are deleted.");
        }
    }

     public static string GetAssemblyDefinitionContent(string fileName)
       {
           string assemblyDefinitionContent = "";

           if (fileName.Contains(".editor"))
           {
               assemblyDefinitionContent = 
                   "{\n" +
                   $"  \"name\": \"{LoadPackageNameData()}.editor\",\n" +
                   "  \"references\": [],\n" +
                   "  \"optionalUnityReferences\": [],\n" +
                   "  \"includePlatforms\": [\"Editor\"],\n" +
                   "  \"excludePlatforms\": [],\n" +
                   "  \"allowUnsafeCode\": false,\n" +
                   "  \"overrideReferences\": false,\n" +
                   "  \"precompiledReferences\": [],\n" +
                   "  \"autoReferenced\": true,\n" +
                   "  \"defineConstraints\": []\n" +
                   "}";
           }else if (fileName.Contains(".tests"))
           {
               if (fileName.Contains("editmode"))
               {
                    assemblyDefinitionContent =
                       "{\n" +
                       $"  \"name\": \"editmode.tests\",\n" +
                       "  \"allowUnsafeCode\": false,\n" +
                       "  \"autoReferenced\": false,\n" +
                       "  \"overrideReferences\": true,\n" +
                       "  \"references\": [\n" +
                       "    \"UnityEngine.TestRunner\",\n" +
                       "    \"UnityEditor.TestRunner\"\n" +
                       "  ],\n" +
                       "  \"optionalUnityReferences\": [],\n" +
                       "  \"includePlatforms\": [\"Editor\"],\n" +
                       "  \"excludePlatforms\": [],\n" +
                       "  \"precompiledReferences\": [\n" +
                       "    \"nunit.framework.dll\"\n" + 
                       "  ],\n" +
                       "  \"defineConstraints\": [\n" +
                       "    \"UNITY_INCLUDE_TESTS\"\n" +
                       "  ]\n" +
                       "}";
               }
               else if (fileName.Contains("playmode"))
               {
                   assemblyDefinitionContent =
                       "{\n" +
                       $"  \"name\": \"playmode.tests\",\n" +
                       "  \"allowUnsafeCode\": false,\n" +
                       "  \"autoReferenced\": false,\n" +
                       "  \"overrideReferences\": true,\n" +
                       "  \"references\": [\n" +
                       "    \"UnityEngine.TestRunner\",\n" +
                       "    \"UnityEditor.TestRunner\"\n" +
                       "  ],\n" +
                       "  \"optionalUnityReferences\": [],\n" +
                       "  \"includePlatforms\": [],\n" +
                       "  \"excludePlatforms\": [],\n" +
                       "  \"precompiledReferences\": [\n" +
                       "    \"nunit.framework.dll\"\n" + 
                       "  ],\n" +
                       "  \"defineConstraints\": [\n" +
                       "    \"UNITY_INCLUDE_TESTS\"\n" +
                       "  ]\n" +
                       "}";
               }
               
           } else if (fileName.Contains("Runtime/"))
           {
               assemblyDefinitionContent = 
                   "{\n" +
                   $"  \"name\": \"{LoadPackageNameData()}\",\n" +
                   "  \"references\": [],\n" +
                   "  \"optionalUnityReferences\": [],\n" +
                   "  \"includePlatforms\": [],\n" +
                   "  \"excludePlatforms\": [],\n" +
                   "  \"allowUnsafeCode\": false,\n" +
                   "  \"overrideReferences\": false,\n" +
                   "  \"precompiledReferences\": [],\n" +
                   "  \"autoReferenced\": true,\n" +
                   "  \"defineConstraints\": []\n" +
                   "}";
           }
           else
           {
               Debug.LogError("Assembly Definition file not found!");
           }
           
           
           return assemblyDefinitionContent;
       }
   
}
}