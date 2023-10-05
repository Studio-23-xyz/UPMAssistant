using System.Collections.Generic;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class PackageJsonData
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unityVersion;
        public string unityRelease;
        public string documentationUrl;
        public string changelogUrl;
        public string licensesUrl;
        public string scopedRegistryName;
        public string scopedRegistryUrl;
        public List<string> scopedRegistryScopes = new List<string>();
        public List<PackageDependency> dependencies = new List<PackageDependency>();
        public List<string> keywords = new List<string>();
        public string authorName;
        public string authorEmail;
        public string authorUrl;
    }
}