using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClientInspectionSystem.ClientExtentions;

namespace ClientInspectionSystem.SocketClient.Request {
    public class AuthorizationElement {
        public int ordinary { get; set; }
        public string description { get; set; }

        public string title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<KeyValuePair<string, object>> multipleSelect { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> singleSelect { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> nameValuePair { get; set; }
    }
}
