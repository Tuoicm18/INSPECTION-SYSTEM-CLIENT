using PluginICAOClientSDK;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetScanDocument {
        private ISPluginClient pluginClient;
        private string scanType;
        private bool saveEnabled;
        private TimeSpan timeOutResp;
        private int timeOutInterVal;

        public GetScanDocument(ISPluginClient pluginClient, string scanType,
                            bool saveEnabled, TimeSpan timeOutResp,
                            int timeOutInterVal) {
            this.pluginClient = pluginClient;
            this.scanType = scanType;
            this.saveEnabled = saveEnabled;
            this.timeOutResp = timeOutResp;
            this.timeOutInterVal = timeOutInterVal;
        }

        public PluginICAOClientSDK.Response.ScanDocument.BaseScanDocumentResp scanDocumentResp() {
            try {
                return pluginClient.scanDocument(this.scanType, this.saveEnabled, this.timeOutResp, this.timeOutInterVal);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
