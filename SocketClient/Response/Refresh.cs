using PluginICAOClientSDK;
using System;

namespace ClientInspectionSystem.SocketClient.Response {
    public class Refresh {
        private ISPluginClient pluginClient;
        private bool deviceDetailsEnabled;
        private bool presenceEnabled;
        private long timeoutMilisec;
        private int timeOutInterval;

        public Refresh(bool deviceDetailsEnabled, bool presenceEnabled,
                       long timeoutMilisec, int timeOutInterval, 
                       ISPluginClient pluginClient) {
            this.deviceDetailsEnabled = deviceDetailsEnabled;
            this.presenceEnabled = presenceEnabled;
            this.timeoutMilisec = timeoutMilisec;
            this.timeOutInterval = timeOutInterval;
            this.pluginClient = pluginClient;
        }

        public PluginICAOClientSDK.Response.DeviceDetails.DeviceDetailsResp refreshReader() {
            try {
                return pluginClient.refreshReader(deviceDetailsEnabled, presenceEnabled, timeoutMilisec, timeOutInterval);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
