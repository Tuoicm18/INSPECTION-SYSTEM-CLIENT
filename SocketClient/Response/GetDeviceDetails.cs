using PluginICAOClientSDK;
using PluginICAOClientSDK.Response;
using System;
using System.Collections.Generic;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetDeviceDetails {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private TimeSpan timeOutResp;

        public GetDeviceDetails(ISPluginClient pluginClient, bool deviceDetailsEnabled, 
                                    bool presenceEnabled, TimeSpan timeOutResp) {
            this.pluginClient = pluginClient;
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeOutResp = timeOutResp;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp getDeviceDetails() {
            return pluginClient.getDeviceDetails(deviceDetailsEnabled, presenceEnabled, timeOutResp);
        }
    }
}
