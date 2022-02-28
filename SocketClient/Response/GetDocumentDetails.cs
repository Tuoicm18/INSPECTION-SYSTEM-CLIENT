﻿using PluginICAOClientSDK;
using PluginICAOClientSDK.Response;
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
        public GetDocumentDetails(ISPluginClient pluginClient, bool mrzEnabled,
                                  bool imageEnabled, bool dataGroupEnabled, 
                                  bool optionalDetailsEnabled, TimeSpan timeoutResp,
                                  ISPluginClient.DocumentDetailsListener documentDetailsListener,
                                  int timeOutInterVal) {
            this.pluginClient = pluginClient;
            this.mrzEnabled = mrzEnabled;
            this.imageEnabled = imageEnabled;
            this.dataGroupEnabled = dataGroupEnabled;
            this.optionalDetailsEnabled = optionalDetailsEnabled;
            this.timeOutResp = timeoutResp;
            this.documentDetailsListener = documentDetailsListener;
            this.timeOutInterVal = timeOutInterVal;
        }

        public BaseDocumentDetailsResp getDocumentDetails() {
            return pluginClient.getDocumentDetails(mrzEnabled, imageEnabled, 
                                                   dataGroupEnabled, optionalDetailsEnabled, 
                                                   timeOutResp, documentDetailsListener,
                                                   timeOutInterVal);
        }
    }
}
