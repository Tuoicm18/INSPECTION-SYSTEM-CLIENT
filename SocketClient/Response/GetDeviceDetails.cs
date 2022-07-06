using PluginICAOClientSDK;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDeviceDetails {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private TimeSpan timeOutResp;
        private int timeOutInterVal;

        public GetDeviceDetails(ISPluginClient pluginClient, bool deviceDetailsEnabled, 
                                bool presenceEnabled, TimeSpan timeOutResp,
                                int timeOutInterVal) {
            this.pluginClient = pluginClient;
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeOutResp = timeOutResp;
            this.timeOutInterVal = timeOutInterVal;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp getDeviceDetails() {
            return pluginClient.getDeviceDetails(deviceDetailsEnabled, presenceEnabled, timeOutResp, timeOutInterVal);
        }
    }
}

