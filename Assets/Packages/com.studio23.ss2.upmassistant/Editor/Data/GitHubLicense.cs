using Newtonsoft.Json;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GitHubLicense
    {
        [JsonProperty("key")]
        public string Key { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("url")]
        public string URL { get; set; }
    }
}