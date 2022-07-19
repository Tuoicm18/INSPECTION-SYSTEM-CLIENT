using PluginICAOClientSDK;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDeviceDetails {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private long timeoutMilisec;
        private int timeoutInterval;

        public GetDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled, 
                                long timeoutMilisec, int timeoutInterval, 
                                ISPluginClient pluginClient) {
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeoutMilisec = timeoutMilisec;
            this.timeoutInterval = timeoutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp getDeviceDetails() {
            return pluginClient.getDeviceDetails(deviceDetailsEnabled, presenceEnabled, timeoutMilisec, timeoutInterval);
        }
    }
}

