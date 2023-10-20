using System.Collections.Generic;
using Newtonsoft.Json;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    public class PackageJsonData
    {
        public PackageJsonData()
        {
            ScopedRegistries = new List<ScopedRegistry>();
            Dependencies = new Dictionary<string, string>();
            Keywords = new List<string>();
            Author = new Author();
        }

        [JsonProperty("name")] public string Name;
        [JsonProperty("version")] public string Version;
        [JsonProperty("displayName")] public string DisplayName;
        [JsonProperty("description")] public string Description;
        [JsonProperty("unity")] public string Unity;
        [JsonProperty("unityRelease")] public string UnityRelease;
        [JsonProperty("documentationUrl")] public string DocumentationUrl;
        [JsonProperty("changelogUrl")] public string ChangelogUrl;
        [JsonProperty("licensesUrl")] public string LicensesUrl;
        [JsonProperty("scopedRegistries")] public List<ScopedRegistry> ScopedRegistries;
        [JsonProperty("dependencies")] public Dictionary<string, string> Dependencies;
        [JsonProperty("keywords")] public List<string> Keywords;
        [JsonProperty("author")] public Author Author;
    }

    public class Author
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("email")] public string Email;
        [JsonProperty("url")] public string URL;
    }

    public class ScopedRegistry
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("url")] public string URL;
        [JsonProperty("scopes")] public List<string> Scopes;
    }
}