using System.Collections.Generic;
using Newtonsoft.Json; // Make sure to include the Newtonsoft.Json namespace for JsonProperty attribute

public class License
{
    [JsonProperty("key")]
    public string Key;
    
    [JsonProperty("name")]
    public string Name;
    
    [JsonProperty("spdx_id")]
    public string SPDXId;
    
    [JsonProperty("url")]
    public string URL;
    
    [JsonProperty("node_id")]
    public string NodeId;
    
    [JsonProperty("html_url")]
    public string HtmlURL;
    
    [JsonProperty("description")]
    public string Description;
    
    [JsonProperty("implementation")]
    public string Implementation;
    
    [JsonProperty("permissions")]
    public List<string> Permissions;
    
    [JsonProperty("conditions")]
    public List<string> Conditions;
    
    [JsonProperty("limitations")]
    public List<string> Limitations;
    
    [JsonProperty("body")]
    public string Body;
    
    [JsonProperty("featured")]
    public bool Featured;
}