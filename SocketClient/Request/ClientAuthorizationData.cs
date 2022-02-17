using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Request {
    public class ClientAuthorizationData {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAuthorizationElement> authContentList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAuthorizationElement> multipleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAuthorizationElement> singleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ClientAuthorizationElement> nameValuePairList { get; set; }
    }

    public class AuthorizationDataReq {

        public ClientAuthorizationData authorizationData { get; set; }
    }
}
