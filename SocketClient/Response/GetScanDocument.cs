using ClientInspectionSystem;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetScanDocument {
        private ISPluginClient pluginClient;
        private ScanType scanType;
        private bool saveEnabled;
        private long timeoutMilisec;
        private int timeoutInterval;

        public GetScanDocument(ScanType scanType, bool saveEnabled, 
                               long timeoutMilisec, int timeoutInterval,
                               ISPluginClient pluginClient) {
            this.scanType = scanType;
            this.saveEnabled = saveEnabled;
            this.timeoutMilisec = timeoutMilisec;
            this.timeoutInterval = timeoutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.ScanDocument.ScanDocumentResp scanDocumentResp() {
            try {
                return pluginClient.scanDocument(scanType, saveEnabled, timeoutMilisec, timeoutInterval);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
