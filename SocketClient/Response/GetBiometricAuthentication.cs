using System;
using System.Collections.Generic;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK;
using PluginICAOClientSDK.Models;

namespace ClientInspectionSystem.SocketClient.Response {
    public class GetBiometricAuthentication {
        private ISPluginClient clientPlugin;
        private BiometricType biometricType;
        private object challenge;
        private int timeoutInterval;
        private ChallengeType challengeType;
        private bool livenessEnabled;
        private string cardNo;
        private bool biometricEvidence;

        public GetBiometricAuthentication(BiometricType biometricType, object challenge,
                                          ChallengeType challengeType, bool livenessEnabled,
                                          string cardNo, int timeoutInterval,
                                          bool biometricEvidence, ISPluginClient clientPlugin) {
            this.biometricType = biometricType;
            this.challenge = challenge;
            this.challengeType = challengeType;
            this.livenessEnabled = livenessEnabled;
            this.cardNo = cardNo;
            this.timeoutInterval = timeoutInterval;
            this.biometricEvidence = biometricEvidence;
            this.clientPlugin = clientPlugin;
        }

        public BiometricAuthResp getResultBiometricAuth() {
            return clientPlugin.biometricAuthentication(biometricType, challenge, 
                                                        challengeType, livenessEnabled,
                                                        cardNo, timeoutInterval,
                                                        biometricEvidence);
        }
    }
}
