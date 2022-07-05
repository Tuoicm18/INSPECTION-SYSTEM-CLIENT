using PluginICAOClientSDK;
using System;

namespace ClientInspectionSystem.SocketClient.Response {
    public class Refresh {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private TimeSpan timeOutResp;
        private int timeOutInterVal;

        public Refresh(ISPluginClient pluginClient, bool deviceDetailsEnabled,
                       bool presenceEnabled, TimeSpan timeOutResp,
                       int timeOutInterVal) {
            this.pluginClient = pluginClient;
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeOutResp = timeOutResp;
            this.timeOutInterVal = timeOutInterVal;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.BaseDeviceDetailsResp refreshReader() {
            try {
                return pluginClient.refresh(deviceDetailsEnabled, presenceEnabled, timeOutResp, timeOutInterVal);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
