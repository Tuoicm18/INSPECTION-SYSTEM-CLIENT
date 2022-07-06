using PluginICAOClientSDK;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using System;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDocumentDetails {
        private ISPluginClient pluginClient;
        private bool mrzEnabled;
        private bool imageEnabled;
        private bool dataGroupEnabled;
        private bool optionalDetailsEnabled;
        private TimeSpan timeOutResp;
        private ISPluginClient.DocumentDetailsListener documentDetailsListener;
        private int timeOutInterVal;
        private string canValue;
        private string challenge;
        private bool caEnabled;
        private bool taEnabled;

        public GetDocumentDetails(ISPluginClient pluginClient, bool mrzEnabled,
                                  bool imageEnabled, bool dataGroupEnabled, 
                                  bool optionalDetailsEnabled, TimeSpan timeoutResp,
                                  ISPluginClient.DocumentDetailsListener documentDetailsListener,
                                  int timeOutInterVal, string canValue,
                                  string challenge, bool caEnabled,
                                  bool taEnabled) {
            this.pluginClient = pluginClient;
            this.mrzEnabled = mrzEnabled;
            this.imageEnabled = imageEnabled;
            this.dataGroupEnabled = dataGroupEnabled;
            this.optionalDetailsEnabled = optionalDetailsEnabled;
            this.timeOutResp = timeoutResp;
            this.documentDetailsListener = documentDetailsListener;
            this.timeOutInterVal = timeOutInterVal;
            this.canValue = canValue;
            this.challenge = challenge;
            this.caEnabled = caEnabled;
            this.taEnabled = taEnabled;
        }

        public BaseDocumentDetailsResp getDocumentDetails() {
            return pluginClient.getDocumentDetails(mrzEnabled, imageEnabled, 
                                                   dataGroupEnabled, optionalDetailsEnabled, 
                                                   timeOutResp, documentDetailsListener,
                                                   timeOutInterVal, canValue,
                                                   challenge, caEnabled,
                                                   taEnabled);
        }
    }
}
