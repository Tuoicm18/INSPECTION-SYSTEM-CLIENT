﻿using log4net;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ClientInspectionSystem {
    public static class ClientExtentions {

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region INNER CLASS
        public class ListWithDuplicates : List<KeyValuePair<object, object>> {
            public void Add(object key, object value) {
                var element = new KeyValuePair<object, object>(key, value);
                this.Add(element);
            }
        }

        //JsonConverter For List<KeyValuePair<string, object>
        public class KeyValuePairConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return objectType == typeof(List<KeyValuePair<string, object>>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                List<KeyValuePair<string, object>> list = value as List<KeyValuePair<string, object>>;
                writer.WriteStartObjectAsync();
                foreach (var item in list) {
                    //writer.WriteStartObject();
                    writer.WritePropertyName(item.Key);
                    writer.WriteValue(item.Value);
                    //writer.WriteEndObject();
                }
                writer.WriteEndObjectAsync();
            }
        }
        #endregion

        //Setting Jsons For KeyValuPair<string, object>
        public static JsonSerializerSettings settingsJsonDuplicateDic = new JsonSerializerSettings { Converters = new[] { new ClientExtentions.KeyValuePairConverter() } };

        #region OTHER

        /// <summary>
        /// Returns the right part of the string instance.
        /// </summary>
        /// <param name="count">Number of characters to return.</param>
        public static string subStringRight(string input, int count) {
            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }

        /// <summary>
        /// Returns the left part of this string instance.
        /// </summary>
        /// <param name="count">Number of characters to return.</param>
        public static string subStringLeft(string input, int count) {
            return input.Substring(0, Math.Min(input.Length, count));
        }

        //Read version of current app
        public static string getCurrentVersion() {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            return version;
        }

        //Base64 to BitmapImage
        public static BitmapImage base64ToBitmapImage(string base64Img) {
            try {
                byte[] imgByte = Convert.FromBase64String(base64Img);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(imgByte);
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();
                return bi;
            }
            catch (Exception e) {
                throw e;
            }
        }

        //Create UUID
        public static string generateUUID() {
            return Guid.NewGuid().ToString();
        }
        #endregion

        #region REMOVE HOVER LISTVIEW
        public static void removeHoverListView(ListView lv, MetroWindow metroWindow) {
            //Remove Hover List View
            lv.ItemContainerStyle = metroWindow.Resources["RemoveHoverListView"] as Style;
        }
        #endregion

        #region GET IP UPDATE 2022.04.29
        public static string GetLocalIPAddress() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        #endregion

        #region 2022.05.24 SHOW PROGRESS DIALOG CONTROLLER
        public static ProgressDialogController progressDialogController { get; set; } = null;
        public static void showProgressDialogController(MetroWindow metroWindow, string title,
                                                        string message, bool isSystemTray,
                                                        int timeout, bool setIndeterminate,
                                                        bool isClose = true) {
            try {
                //logger.Debug("<DIALOG> " + message + " <SYSTEM TRAY> " + isSystemTray);
                if (!isSystemTray) {
                    metroWindow.Dispatcher.Invoke(async () => {
                        progressDialogController = await metroWindow.ShowProgressAsync(title, message);
                        if (setIndeterminate) {
                            progressDialogController.SetIndeterminate();
                        }
                        if (isClose) {
                            await Task.Delay(timeout);
                            await progressDialogController.CloseAsync();
                        }
                    });
                }
            }
            catch (Exception e) {
                logger.Error("SHOW PROGRESS DIALOG", e);
            }
        }

        //Show dialog and change current message
        public static void showProgressDialogControllerAndChangeMsg(MetroWindow metroWindow, string title,
                                                                    string message, string messageChange,
                                                                    int timeout, int timeoutForMsgChange,
                                                                    bool setIndeterminate, bool isSystemTray) {
            try {
                //logger.Debug("<DIALOG> " + message + " <SYSTEM TRAY> " + isSystemTray);
                if (!isSystemTray) {
                    metroWindow.Dispatcher.Invoke(async () => {
                        progressDialogController = await metroWindow.ShowProgressAsync(title, message);
                        if (setIndeterminate) {
                            progressDialogController.SetIndeterminate();
                        }
                        await Task.Delay(timeout);
                        progressDialogController.SetMessage(messageChange);
                        await Task.Delay(timeoutForMsgChange);
                        await progressDialogController.CloseAsync();
                    });
                }
            }
            catch (Exception e) {
                logger.Error("SHOW PROGRESS DIALOG", e);
                return;
            }
        }

        //Update 2023.02.20 For close process dialog controller when in syste, tray
        public static void dlgIsInSystemtray(bool isInSystray) {
            if (isInSystray) {
                if (null != progressDialogController) {
                    if (progressDialogController.IsOpen) {
                        logger.Warn("[CLOSED] DIALOG CONTROLLER CAUSE IS IN SYSTEM TRAY");
                        progressDialogController.CloseAsync();

                    }
                }
            }
        }
        #endregion
    }
}
