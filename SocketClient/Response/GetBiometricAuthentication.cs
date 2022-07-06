using System;
using System.Collections.Generic;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetBiometricAuthentication {
        private ISPluginClient clientPlugin;
        private string biometricType;
        private object challenge;
        private int timeOutInterVal;
        private string challengeType;
        private bool livenessEnabled;
        private string cardNo;
        public TimeSpan timeOutResp;

        public GetBiometricAuthentication(ISPluginClient pluginClient, string biometricType,
                                          object challenge, TimeSpan timeOutResp,
                                          int timeOutInterVal, string challengeType,
                                          bool livenessEnabled, string cardNo) {
            this.clientPlugin = pluginClient;
            this.biometricType = biometricType;
            this.challenge = challenge;
            this.timeOutResp = timeOutResp;
            this.timeOutInterVal = timeOutInterVal;
            this.challengeType = challengeType;
            this.livenessEnabled = livenessEnabled;
            this.cardNo = cardNo;
        }

        public BaseBiometricAuthResp getResultBiometricAuth() {
            return clientPlugin.biometricAuthentication(biometricType, challenge, 
                                                        timeOutResp, timeOutInterVal, 
                                                        challengeType, livenessEnabled,
                                                        cardNo);
        }
    }
}
