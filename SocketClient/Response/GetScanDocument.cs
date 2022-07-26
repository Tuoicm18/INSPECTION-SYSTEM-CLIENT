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
        private int timeoutInterval;

        public GetScanDocument(ScanType scanType, bool saveEnabled, 
                               int timeoutInterval, ISPluginClient pluginClient) {
            this.scanType = scanType;
            this.saveEnabled = saveEnabled;
            this.timeoutInterval = timeoutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.ScanDocument.ScanDocumentResp scanDocumentResp() {
            try {
                return pluginClient.scanDocument(scanType, saveEnabled, timeoutInterval);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
