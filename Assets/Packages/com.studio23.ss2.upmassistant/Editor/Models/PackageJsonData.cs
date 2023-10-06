using System.Collections.Generic;

namespace Studio23.SS2.UPMAssistant.Editor
{
    public class PackageJsonData
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        public string documentationUrl;
        public string changelogUrl;
        public string licensesUrl;
        public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();
        public List<string> keywords = new List<string>();
        
       
        /*public string authorName;
        public string authorEmail;
        public string authorUrl;*/
        public Author author = new Author();
    }

    public class Author
    {
        public string name;
        public string email;
        public string url;
    }
}