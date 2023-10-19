using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Studio23.SS2.UPMAssistant.Editor.Data
{
    [System.Serializable]
    public class License
    {
        public string key;
        public string name;
        public string spdx_id;
        public string url;
        public string node_id;
        public string html_url;
        public string description;
        public string implementation;
        public List<string> permissions;
        public List<string> conditions;
        public List<string> limitations;
        public string body;
        public bool featured;
    }

}