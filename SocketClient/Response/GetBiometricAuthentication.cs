using System;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Request;
using PluginICAOClientSDK.Response.BiometricAuth;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetBiometricAuthentication {
        private ISPluginClient clientPlugin;
        private string biometricType;
        private AuthorizationData authorizationData;
        public TimeSpan timeOutResp;

        public GetBiometricAuthentication(ISPluginClient pluginClient, string biometricType,
                                          AuthorizationData authorizationData, TimeSpan timeOutResp) {
            this.clientPlugin = pluginClient;
            this.biometricType = biometricType;
            this.authorizationData = authorizationData;
            this.timeOutResp = timeOutResp;
        }

        public BaseBiometricAuthResp getResultBiometricAuth() {
            return clientPlugin.biometricAuthentication(biometricType, authorizationData, timeOutResp);
        }
    }
}
