using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ControlzEx.Theming;
using log4net;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormBiometricAuth.xaml
    /// </summary>
    public partial class FormBiometricAuth : MetroWindow {
        private MainWindow mainWindow = new MainWindow();
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FormBiometricAuth() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }

        //public void setImageSource(string path) {
        //    imgViewJWT.Source = new BitmapImage(new Uri(path, UriKind.Relative));
        //}

        public void setContenLabelResult(string content) {
            lbResult.Content = content;
        }

        public void setContentLabelType(string content) {
            lbTypeResult.Content = content;
        }

        public void setContentLabelScore(string score) {
            lbScoreResult.Content = score;
        }

        public void setContentLabelResponseCode(string content) {
            lbResultResponseCode.Content = content;
        }

        public void setContentLabelResponseMsg(string msg) {
            lbResultResponseMsg.Content = Content;
        }

        public void setTitleForm(string content) {
            this.Dispatcher.Invoke(() => this.Title = content);
        }

        public void StartCloseTimer(double timeout) {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(timeout);
            timer.Tick += TimerTick;
            timer.Start();
        }

        public void TimerTick(object sender, EventArgs e) {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            this.Close();
            this.Topmost = false;
            //uacISAPP.Hide();
            mainWindow.IsEnabled = true;
        }

        private void btnOK_Click(object sender, System.Windows.RoutedEventArgs e) {
            DialogResult = true;
        }

        public void hideLabelForDeniedAuth() {
            //Show
            lbResponseCode.Visibility = System.Windows.Visibility.Visible;
            lbResponseMsg.Visibility = System.Windows.Visibility.Visible;
            lbResultResponseCode.Visibility = System.Windows.Visibility.Visible;
            lbResultResponseMsg.Visibility = System.Windows.Visibility.Visible;
            //Hide
            lbType.Visibility = System.Windows.Visibility.Collapsed;
            lbTypeResult.Visibility = System.Windows.Visibility.Collapsed;
            lbTitleResult.Visibility = System.Windows.Visibility.Collapsed;
            lbResult.Visibility = System.Windows.Visibility.Collapsed;
            lbScore.Visibility = System.Windows.Visibility.Collapsed;
            lbScoreResult.Visibility = System.Windows.Visibility.Collapsed;
            lbIssueDetailCode.Visibility = System.Windows.Visibility.Collapsed;
            lbResultIssueCode.Visibility = System.Windows.Visibility.Collapsed;
            lbIssueDetailMessage.Visibility = System.Windows.Visibility.Collapsed;
            lbResultIssueMessage.Visibility = System.Windows.Visibility.Collapsed;
            lbJWT.Visibility = System.Windows.Visibility.Collapsed;
            btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
            //Set height Form
            //this.Height = 130;
        }

        //Render Result Biometric Auth
        public void renderResultBiometricAuht(BaseBiometricAuthResp baseBiometricAuthResp) {
            string biometricType = baseBiometricAuthResp.data.biometricType;
            bool biometricResult = baseBiometricAuthResp.data.result;
            int biometricScore = baseBiometricAuthResp.data.score;
            int issueDetailCode = baseBiometricAuthResp.data.issueDetailCode;
            string issueDetailMsg = baseBiometricAuthResp.data.issueDetailMessage;
            int responseCode = baseBiometricAuthResp.errorCode;
            string responseMessage = baseBiometricAuthResp.errorMessage;
            string jwt = baseBiometricAuthResp.data.jwt;

            lbResultResponseCode.Content = responseCode.ToString();
            lbResultResponseMsg.Content = responseMessage;
            lbResultIssueCode.Content = issueDetailCode.ToString();
            lbResultIssueMessage.Content = issueDetailMsg;

            if (responseCode == 0) {
                if (biometricType.Equals(BiometricType.TYPE_FINGER_LEFT) || biometricType.Equals(BiometricType.TYPE_FINGER_RIGHT)) {
                    lbTypeResult.Content = biometricType;
                    lbResult.Content = biometricResult.ToString().ToLower();
                    lbScoreResult.Content = biometricScore.ToString();
                    //Check Result Show/Hide JWT
                    if(biometricResult) {
                        textBlockJWT.Text = jwt;
                    } else {
                        lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                        btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                        //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //Hide Label Issue Details
                    //lbIssueDetailCode.Visibility = System.Windows.Visibility.Collapsed;
                    //lbResultIssueCode.Visibility = System.Windows.Visibility.Collapsed;
                    //lbIssueDetailMessage.Visibility = System.Windows.Visibility.Collapsed;
                    //lbResultIssueMessage.Visibility = System.Windows.Visibility.Collapsed;
                }
                else {
                    lbTypeResult.Content = biometricType;
                    lbResult.Content = biometricResult.ToString().ToLower();
                    lbScoreResult.Content = biometricScore.ToString();

                    if (biometricResult) {
                        textBlockJWT.Text = jwt;
                    }
                    else {
                        lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                        btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                        //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    //if (!biometricResult) {
                    //    if (!string.IsNullOrEmpty(issueDetails)) {
                    //        //Show Label Issue Details
                    //        //lbIssueDetailsResult.Content = issueDetails;
                    //        //lbIssueDetails.Visibility = System.Windows.Visibility.Visible;
                    //        //lbIssueDetailsResult.Visibility = System.Windows.Visibility.Visible;
                    //        lbJWT.Content = lbIssueDetails.Content;
                    //        textBlockJWT.Text = issueDetails;
                    //        textBlockJWT.Visibility = System.Windows.Visibility.Visible;
                    //        btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                    //    }
                    //    //Hide Label JWT
                    //    //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                    //    //lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                    //    //btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                    //}
                    //else {
                    //    //Hide Label Issue Details
                    //    //lbIssueDetails.Visibility = System.Windows.Visibility.Collapsed;
                    //    //lbIssueDetailsResult.Visibility = System.Windows.Visibility.Collapsed;

                    //    //Show Label JWT
                    //    lbJWT.Content = "JWT";
                    //    textBlockJWT.Text = jwt;
                    //    //scvJWT.Visibility = System.Windows.Visibility.Visible;
                    //    lbJWT.Visibility = System.Windows.Visibility.Visible;
                    //    btnViewJWT.Visibility = System.Windows.Visibility.Visible;
                    //}
                }
            }
            else {
                if (responseCode == ClientContants.SOCKET_RESP_CODE_BIO_AUTH_DENIED) {
                    lbResponseCode.Visibility = System.Windows.Visibility.Visible;
                    lbResponseMsg.Visibility = System.Windows.Visibility.Visible;
                    lbResultResponseCode.Content = responseCode.ToString();
                    lbResultResponseMsg.Content = responseMessage;
                }
                lbType.Visibility = System.Windows.Visibility.Collapsed;
                lbTitleResult.Visibility = System.Windows.Visibility.Collapsed;
                //lbIssueDetails.Visibility = System.Windows.Visibility.Collapsed;
                //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        #region MOUSE WHELL SCROLL VIEWER
        private void lvAllLabel_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        #endregion

        private void btnViewJWT_Click(object sender, System.Windows.RoutedEventArgs e) {
            try {
                Image imageButton = (Image)((Button)sender).Content;
                if(imageButton.Name.Equals(ClientContants.CONTENT_BTN_VIEW_JWT)) {
                    hideShowLayoutViewJWT(imageButton.Name);
                    btnViewJWT.BorderThickness = new System.Windows.Thickness(0, 0, 0, 0);
                    imageButton.Name = ClientContants.CONTENT_BTN_VIEW_JWT_CANCEL;
                    imageButton.Source = new BitmapImage(new Uri("/Resource/icons8-hidden-24.png", UriKind.Relative));
                } else {
                    hideShowLayoutViewJWT(imageButton.Name);
                    btnViewJWT.BorderThickness = new System.Windows.Thickness(0, 0, 0, 0);
                    imageButton.Name = ClientContants.CONTENT_BTN_VIEW_JWT;
                    imageButton.Source = new BitmapImage(new Uri("/Resource/eye-24.ico", UriKind.Relative));
                }
            }
            catch (Exception exBtnViewJWT) {
                logger.Error(exBtnViewJWT);
            }
        }

        //Hide/Show For View JWT
        private void hideShowLayoutViewJWT(string imageName) {
            if(imageName.Equals(ClientContants.CONTENT_BTN_VIEW_JWT)) {
                gridForViewJWT.Visibility = System.Windows.Visibility.Visible;
                txtViewJWT.Text = textBlockJWT.Text;
                //Hide Label
                lbResponseCode.Visibility = System.Windows.Visibility.Collapsed;
                lbResponseMsg.Visibility = System.Windows.Visibility.Collapsed;
                lbResultResponseCode.Visibility = System.Windows.Visibility.Collapsed;
                lbResultResponseMsg.Visibility = System.Windows.Visibility.Collapsed;
                lbType.Visibility = System.Windows.Visibility.Collapsed;
                lbTypeResult.Visibility = System.Windows.Visibility.Collapsed;
                lbTitleResult.Visibility = System.Windows.Visibility.Collapsed;
                lbResult.Visibility = System.Windows.Visibility.Collapsed;
                lbScore.Visibility = System.Windows.Visibility.Collapsed;
                lbScoreResult.Visibility = System.Windows.Visibility.Collapsed;
                lbIssueDetailCode.Visibility = System.Windows.Visibility.Collapsed;
                lbResultIssueCode.Visibility = System.Windows.Visibility.Collapsed;
                lbIssueDetailMessage.Visibility = System.Windows.Visibility.Collapsed;
                lbResultIssueMessage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else {
                gridForViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                txtViewJWT.Text = string.Empty;
                //Show Label
                lbResponseCode.Visibility = System.Windows.Visibility.Visible;
                lbResponseMsg.Visibility = System.Windows.Visibility.Visible;
                lbResultResponseCode.Visibility = System.Windows.Visibility.Visible;
                lbResultResponseMsg.Visibility = System.Windows.Visibility.Visible;
                lbType.Visibility = System.Windows.Visibility.Visible;
                lbTypeResult.Visibility = System.Windows.Visibility.Visible;
                lbTitleResult.Visibility = System.Windows.Visibility.Visible;
                lbResult.Visibility = System.Windows.Visibility.Visible;
                lbScore.Visibility = System.Windows.Visibility.Visible;
                lbScoreResult.Visibility = System.Windows.Visibility.Visible;
                lbIssueDetailCode.Visibility = System.Windows.Visibility.Visible;
                lbResultIssueCode.Visibility = System.Windows.Visibility.Visible;
                lbIssueDetailMessage.Visibility = System.Windows.Visibility.Visible;
                lbResultIssueMessage.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
