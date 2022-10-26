using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using ClientInspectionSystem.Models;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using PluginICAOClientSDK.Request;
using ClientInspectionSystem.RenderToLayout.ResultAuthorizedData;
using PluginICAOClientSDK.Response.BiometricAuth;
using System.Windows.Media.Imaging;
using log4net;
using PluginICAOClientSDK.Models;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormResultAuthorizationData.xaml
    /// </summary>
    public partial class FormResultAuthorizationData : MetroWindow {

        #region VARIABLE
        //Content List
        private RenderResultContentList renderResultContentList = new RenderResultContentList();
        //Multiple Choices
        private RenderResultMultipleChoices renderResultMultipleChoices = new RenderResultMultipleChoices();
        //Single Choices
        private RenderResultSingleChoices renderResultSingleChoices = new RenderResultSingleChoices();
        //NVP
        private RenderResultNameValuePairs renderResultNVP = new RenderResultNameValuePairs();
        //Document Digest
        private RenderResultDocumentDigest renderResultDocumentDigest = new RenderResultDocumentDigest();
        private RenderResultDocumentDigestTable renderResultDocumentDigestTable = new RenderResultDocumentDigestTable();
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region MAIN
        public FormResultAuthorizationData() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
        }
        #endregion

        #region EVENT BUTTON CLICK
        private void btnSubmitOk_Click(object sender, System.Windows.RoutedEventArgs e) {
            this.Topmost = false;
            DialogResult = true;
        }

        private void btnViewJWT_Click(object sender, System.Windows.RoutedEventArgs e) {
            try {
                Image imageButton = (Image)((Button)sender).Content;
                if (imageButton.Name.Equals(ClientContants.CONTENT_BTN_VIEW_JWT)) {
                    hideShowLayoutViewJWT(imageButton.Name);
                    btnViewJWT.BorderThickness = new System.Windows.Thickness(0, 0, 0, 0);
                    imageButton.Name = ClientContants.CONTENT_BTN_VIEW_JWT_CANCEL;
                    imageButton.Source = new BitmapImage(new Uri("/Resource/icons8-hidden-24.png", UriKind.Relative));
                }
                else {
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
        #endregion

        //Hide/Show For View JWT
        private void hideShowLayoutViewJWT(string imageName) {
            if (imageName.Equals(ClientContants.CONTENT_BTN_VIEW_JWT)) {
                gridForViewJWT.Visibility = System.Windows.Visibility.Visible;
                scvAll.Visibility = System.Windows.Visibility.Collapsed;
                txtViewJWT.Text = textBlockJWT.Text;
            }
            else {
                gridForViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                scvAll.Visibility = System.Windows.Visibility.Visible;
                txtViewJWT.Text = string.Empty;
            }
        }

        #region MOUSE WHELL SCROLL VIEWER
        private void scvAll_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
        #endregion

        #region GET DATA RESULT AUTHOIRZED
        //Title Form
        public void renderTitleForm(string title) {
            this.Title = title;
        }

        //Render Result Biometric Auth
        public void renderResultBiometricAuht(BiometricAuthResp baseBiometricAuthResp) {
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
                if (biometricType.Equals(BiometricType.RIGHT_FINGER) || biometricType.Equals(BiometricType.RIGHT_FINGER)) {
                    lbTypeResult.Content = biometricType;
                    lbResult.Content = biometricResult.ToString().ToLower();
                    lbScoreResult.Content = biometricScore.ToString();
                    //Check Result Show/Hide JWT
                    if (biometricResult) {
                        textBlockJWT.Text = jwt;
                    }
                    else {
                        lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                        btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                        //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //Hide Label Issue Details
                    //lbIssueDetails.Visibility = System.Windows.Visibility.Collapsed;
                    //lbIssueDetailsResult.Visibility = System.Windows.Visibility.Collapsed;
                }
                else {
                    lbTypeResult.Content = biometricType;
                    lbResult.Content = biometricResult.ToString().ToLower();
                    lbScoreResult.Content = biometricScore.ToString();

                    //Check Result Show/Hide JWT
                    if (biometricResult) {
                        textBlockJWT.Text = jwt;
                    }
                    else {
                        lbJWT.Visibility = System.Windows.Visibility.Collapsed;
                        btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                        //scvJWT.Visibility = System.Windows.Visibility.Collapsed;
                    }

                    //if (!biometricResult) {
                    //    //if (!string.IsNullOrEmpty(issueDetails)) {
                    //    //    //Show Label Issue Details
                    //    //    //lbIssueDetailsResult.Content = issueDetails;
                    //    //    //lbIssueDetails.Visibility = System.Windows.Visibility.Visible;
                    //    //    //lbIssueDetailsResult.Visibility = System.Windows.Visibility.Visible;
                    //    //    lbJWT.Content = lbIssueDetails.Content;
                    //    //    textBlockJWT.Text = issueDetails;
                    //    //    textBlockJWT.Visibility = System.Windows.Visibility.Visible;
                    //    //    btnViewJWT.Visibility = System.Windows.Visibility.Collapsed;
                    //    //}
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

        //Content List
        public void renderToLayoutResultContentList(AuthorizationElement elementContentList) {
            try {
                renderResultContentList.renderResultContentList(elementContentList, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultContentList.listViewContentList, this);
            } catch (Exception ex) {
                logger.Error(ex);
            }
        }
        //Multiple Choices
        public void renderToLayoutReslutMultiple(AuthorizationElement elementMultiple) {
            try {
                renderResultMultipleChoices.renderResultMultipleChoices(elementMultiple, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultMultipleChoices.listViewMultiple, this);
            } catch(Exception ex) {
                logger.Error(ex);
            }
        }
        //Single Choices
        public void renderToLayoutResultSingle(AuthorizationElement elementSingle) {
            try {
                renderResultSingleChoices.renderResultSingleChoices(elementSingle, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultSingleChoices.listViewSingle, this);
            }
            catch(Exception ex) {
                logger.Error(ex);
            }
        }
        //NVP
        public void renderToLayoutNVP(AuthorizationElement elementNVP) {
            try {
                renderResultNVP.renderResultNVP(elementNVP, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultNVP.listViewNVP, this);
            }
            catch(Exception ex) {
                logger.Error(ex);
            }
        }

        //Document Digest
        public void renderToLayoutDocDigest(AuthorizationElement elementDocDigest) {
            try {
                renderResultDocumentDigest.renderResultDocumentDigest(elementDocDigest, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultDocumentDigest.listViewDocDigest, this);
            } catch (Exception ex) {
                logger.Error(ex);
            }
        }

        //Document Digest Table
        public void renderToLayoutDocDigestTable(AuthorizationElement elementDocDigest) {
            try {
                renderResultDocumentDigestTable.renderResulDocDigest(elementDocDigest, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultDocumentDigestTable.listViewDocDigest, this);
            } catch (Exception ex) {
                logger.Error(ex);
            }
        }
        #endregion
    }
}
