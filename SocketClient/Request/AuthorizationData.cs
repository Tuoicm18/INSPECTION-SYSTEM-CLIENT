using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Request {
    public class AuthorizationData {
        public List<AuthorizationElement> authContentList { get; set; }
        public List<AuthorizationElement> multipleSelectList { get; set; }
        public List<AuthorizationElement> singleSelectList { get; set; }
        public List<AuthorizationElement> nameValuePairList { get; set; }
    }

    public class AuthorizationDataReq {
        public AuthorizationData authorizationData { get; set; }
    }
}
