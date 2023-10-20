using Newtonsoft.Json;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    public class PackageDependency
    {
        [JsonProperty("name")] public string Name;
        [JsonProperty("version")] public string Version;
    }
}