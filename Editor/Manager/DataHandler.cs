using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading.Tasks;
using Studio23.SS2.UPMAssistant.Editor.Data;
using Newtonsoft.Json;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public static class DataHandler
    {
        public const string Root = "Assets/Packages";
        private const string DataPath = "Assets/UPMAssistant/Editor/Data";
        private const string LicenseURL = "licenseURLData.json";
        private const string PackageName = "packageNameData.json";

        public const string DefaultPackageName = "com.[companyname].[project].[packagename]";

        public const string
            DefaultLicenceURL = "https://api.github.com/licenses/mit"; //"https://api.github.com/licenses/lgpl-2.1";

        public const string PackageJson = "package.json";
        public const string ReadmeMD = "README.md";
        public const string ChangelogMD = "CHANGELOG.md";
        public const string LicenseMD = "LICENSE.md";
        public const string ThirdPartyNoticesMD = "ThirdPartyNotices.md";

        private const string ProjectName = "ss2";

        public static Dictionary<string, bool> FolderAndFilesList = new Dictionary<string, bool>
        {
            {PackageJson, false},
            {ReadmeMD, false},
            {ChangelogMD, false},
            {LicenseMD, false},
            {ThirdPartyNoticesMD, false},
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
            if (!Directory.Exists(Root))
            {
                Directory.CreateDirectory(Root);
                Debug.Log($"Folder structure created: {Root}");
            }

            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
                Debug.Log("Folder structure created: " + DataPath);
            }

            var licensePath = Path.Combine(DataPath, LicenseURL);
            if (!File.Exists(licensePath))
            {
                LicenseURLData data = new LicenseURLData {LicenseURL = DefaultLicenceURL};
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllTextAsync(licensePath, jsonData);
                // File.Create(licensePath).Close();
            }

            var packageNamePath = Path.Combine(DataPath, PackageName);
            if (!File.Exists(packageNamePath))
            {
                string updatedPackageName = DefaultPackageName;
                updatedPackageName = updatedPackageName.Replace("[companyname]", PlayerSettings.companyName);
                updatedPackageName = updatedPackageName.Replace("[project]", ProjectName); //Application.productName
                updatedPackageName = updatedPackageName.Replace("[packagename]", PlayerSettings.productName);
                updatedPackageName = updatedPackageName.Replace(" ", "").ToLower();
                PackageNameData data = new PackageNameData {PackageName = updatedPackageName};
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllTextAsync(packageNamePath, jsonData);
            }
        }

        public static void SaveLicenseURLData(string licenseURL)
        {
            var licensePath = Path.Combine(DataPath, LicenseURL);

            LicenseURLData data = new LicenseURLData {LicenseURL = licenseURL};
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllTextAsync(licensePath, jsonData);
        }

        public static string GetSavedLicenseURL()
        {
            var licensePath = Path.Combine(DataPath, LicenseURL);

            if (File.Exists(licensePath))
            {
                Task<string> jsonData = File.ReadAllTextAsync(licensePath);
                LicenseURLData data = JsonConvert.DeserializeObject<LicenseURLData>(jsonData.Result);
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
            var packageNamePath = Path.Combine(DataPath, PackageName);

            PackageNameData data = new PackageNameData {PackageName = packageName};
            string jsonData = JsonConvert.SerializeObject(data);

            File.WriteAllTextAsync(packageNamePath, jsonData);
        }

        public static string GetSavedPackagedName()
        {
            var packageNamePath = Path.Combine(DataPath, PackageName);

            if (File.Exists(packageNamePath))
            {
                Task<string> jsonData = File.ReadAllTextAsync(packageNamePath);
                PackageNameData data = JsonConvert.DeserializeObject<PackageNameData>(jsonData.Result);
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
            var packageNamePath = Path.Combine(Root, GetSavedPackagedName());
            if (Directory.Exists(packageNamePath))
            {
                Directory.Delete(packageNamePath, true);
                Debug.Log("All initialized folder & files are deleted successfully.");
            }
            else
            {
                Debug.LogWarning("Nothing found! No folder & files are deleted.");
            }

            if (Directory.Exists("Assets/UPMAssistant"))
            {
                Directory.Delete("Assets/UPMAssistant", true);
                Debug.Log("All initialized folder & files are deleted successfully.");
            }
            else
            {
                Debug.LogWarning("Nothing found! No folder & files are deleted.");
            }
        }

        public static string GetAssemblyDefinitionContent(string fileName)
        {
            string assemblyDefinitionContent = "";

            if (fileName.Contains(".editor"))
            {
                assemblyDefinitionContent = "{\n" + $"  \"name\": \"{GetSavedPackagedName()}.editor\",\n" +
                                            "  \"references\": [],\n" + "  \"optionalUnityReferences\": [],\n" +
                                            "  \"includePlatforms\": [\"Editor\"],\n" +
                                            "  \"excludePlatforms\": [],\n" + "  \"allowUnsafeCode\": false,\n" +
                                            "  \"overrideReferences\": false,\n" +
                                            "  \"precompiledReferences\": [],\n" + "  \"autoReferenced\": true,\n" +
                                            "  \"defineConstraints\": []\n" + "}";
            }
            else if (fileName.Contains(".tests"))
            {
                if (fileName.Contains("editmode"))
                {
                    assemblyDefinitionContent = "{\n" + $"  \"name\": \"editmode.tests\",\n" +
                                                "  \"allowUnsafeCode\": false,\n" + "  \"autoReferenced\": false,\n" +
                                                "  \"overrideReferences\": true,\n" + "  \"references\": [\n" +
                                                "    \"UnityEngine.TestRunner\",\n" +
                                                "    \"UnityEditor.TestRunner\"\n" + "  ],\n" +
                                                "  \"optionalUnityReferences\": [],\n" +
                                                "  \"includePlatforms\": [\"Editor\"],\n" +
                                                "  \"excludePlatforms\": [],\n" + "  \"precompiledReferences\": [\n" +
                                                "    \"nunit.framework.dll\"\n" + "  ],\n" +
                                                "  \"defineConstraints\": [\n" + "    \"UNITY_INCLUDE_TESTS\"\n" +
                                                "  ]\n" + "}";
                }
                else if (fileName.Contains("playmode"))
                {
                    assemblyDefinitionContent = "{\n" + $"  \"name\": \"playmode.tests\",\n" +
                                                "  \"allowUnsafeCode\": false,\n" + "  \"autoReferenced\": false,\n" +
                                                "  \"overrideReferences\": true,\n" + "  \"references\": [\n" +
                                                "    \"UnityEngine.TestRunner\",\n" +
                                                "    \"UnityEditor.TestRunner\"\n" + "  ],\n" +
                                                "  \"optionalUnityReferences\": [],\n" +
                                                "  \"includePlatforms\": [],\n" + "  \"excludePlatforms\": [],\n" +
                                                "  \"precompiledReferences\": [\n" + "    \"nunit.framework.dll\"\n" +
                                                "  ],\n" + "  \"defineConstraints\": [\n" +
                                                "    \"UNITY_INCLUDE_TESTS\"\n" + "  ]\n" + "}";
                }
            }
            else if (fileName.Contains("Runtime/"))
            {
                assemblyDefinitionContent = "{\n" + $"  \"name\": \"{GetSavedPackagedName()}\",\n" +
                                            "  \"references\": [],\n" + "  \"optionalUnityReferences\": [],\n" +
                                            "  \"includePlatforms\": [],\n" + "  \"excludePlatforms\": [],\n" +
                                            "  \"allowUnsafeCode\": false,\n" + "  \"overrideReferences\": false,\n" +
                                            "  \"precompiledReferences\": [],\n" + "  \"autoReferenced\": true,\n" +
                                            "  \"defineConstraints\": []\n" + "}";
            }
            else
            {
                Debug.LogError("Assembly Definition file not found!");
            }

            return assemblyDefinitionContent;
        }
    }
}