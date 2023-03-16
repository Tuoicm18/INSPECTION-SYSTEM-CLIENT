using PluginICAOClientSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetEnrollFingerprint {
        private ISPluginClient pluginClient;
        private string carNo;
        private int timeoutInterval;

        public GetEnrollFingerprint(ISPluginClient pluginClientInput, string carNoInput, int timeoutIntervalInput) {
            this.pluginClient = pluginClientInput;
            this.carNo = carNoInput;
            this.timeoutInterval = timeoutIntervalInput;
        }

        public PluginICAOClientSDK.Response.EnrollFingerprint.EnrollFingerprintResp enrollFingerprintResp() {
            try {
                return pluginClient.enrollFIngerprint(carNo, timeoutInterval);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
