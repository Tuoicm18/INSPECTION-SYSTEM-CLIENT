﻿using PluginICAOClientSDK;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using System;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDocumentDetails {
        private ISPluginClient pluginClient;
        private bool mrzEnabled;
        private bool imageEnabled;
        private bool dataGroupEnabled;
        private bool optionalDetailsEnabled;
        private long timeoutMilisec;
        private int timeoutInterval;
        private string canValue;
        private string challenge;
        private bool caEnabled;
        private bool taEnabled;

        public GetDocumentDetails(bool mrzEnabled, bool imageEnabled, 
                                  bool dataGroupEnabled, bool optionalDetailsEnabled,
                                  string canValue, string challenge,
                                  bool caEnabled,bool taEnabled,
                                  long timeoutMilisec, int timeoutInterval,
                                  ISPluginClient pluginClient) {
            this.mrzEnabled = mrzEnabled;
            this.imageEnabled = imageEnabled;
            this.dataGroupEnabled = dataGroupEnabled;
            this.optionalDetailsEnabled = optionalDetailsEnabled;
            this.canValue = canValue;
            this.challenge = challenge;
            this.caEnabled = caEnabled;
            this.taEnabled = taEnabled;
            this.timeoutMilisec = timeoutMilisec;
            this.timeoutInterval = timeoutInterval;
            this.pluginClient = pluginClient;
        }

        public DocumentDetailsResp getDocumentDetails() {
            return pluginClient.getDocumentDetails(mrzEnabled, imageEnabled,
                                                   dataGroupEnabled, optionalDetailsEnabled,
                                                   canValue, challenge,
                                                   caEnabled, taEnabled,
                                                   timeoutMilisec, timeoutInterval);
        }
    }
}
