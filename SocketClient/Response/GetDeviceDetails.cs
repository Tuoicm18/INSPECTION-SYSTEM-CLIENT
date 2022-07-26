using PluginICAOClientSDK;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDeviceDetails {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private int timeoutInterval;

        public GetDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled, 
                                int timeoutInterval, ISPluginClient pluginClient) {
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeoutInterval = timeoutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp getDeviceDetails() {
            return pluginClient.getDeviceDetails(deviceDetailsEnabled, presenceEnabled, timeoutInterval);
        }
    }
}

