using PluginICAOClientSDK;
using System;

namespace ClientInspectionSystem.SocketClient.Response {
    public class Refresh {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private int timeOutInterval;

        public Refresh(bool deviceDetailsEnabled, bool presenceEnabled,
                       int timeOutInterval, ISPluginClient pluginClient) {
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeOutInterval = timeOutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp refreshReader() {
            try {
                return pluginClient.refreshReader(deviceDetailsEnabled, presenceEnabled, timeOutInterval);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
