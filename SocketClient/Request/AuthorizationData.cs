﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Request {
    public class AuthorizationData {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> authContentList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> multipleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> singleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> nameValuePairList { get; set; }
    }

    public class AuthorizationDataReq {

        public AuthorizationData authorizationData { get; set; }
    }
}
