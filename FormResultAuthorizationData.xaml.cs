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
        #endregion

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
        public void renderResultBiometricAuht(string biometricType, bool result, float score) {
            if(biometricType.Equals(BiometricType.TYPE_FINGER_LEFT) || biometricType.Equals(BiometricType.TYPE_FINGER_RIGHT)) {
                lbTitleScore.Visibility = System.Windows.Visibility.Visible;
                lbtScore.Visibility = System.Windows.Visibility.Visible;
                if(result) {
                    lbType.Content = biometricType;
                    lbResult.Content = result.ToString();
                    lbtScore.Content = score.ToString();
                } else {
                    //Check Score
                    if(score == 0) {
                        lbType.Content = biometricType;
                        lbResult.Content = ClientContants.LB_NOT_FOUND_FINGER;
                        lbtScore.Content = 0.ToString();
                    } else {
                        lbType.Content = biometricType;
                        lbResult.Content = result.ToString();
                        lbtScore.Content = score.ToString();
                    }
                }
            } else {
                lbTitleScore.Visibility = System.Windows.Visibility.Collapsed;
                lbtScore.Visibility = System.Windows.Visibility.Collapsed;
                if (result) {
                    lbType.Content = biometricType;
                    lbResult.Content = result.ToString();
                }
                else {
                    lbType.Content = biometricType;
                    lbResult.Content = result.ToString();
                }
            }
        }

        //Content List
        public void renderToLayoutResultContentList(AuthorizationElement elementContentList) {
            try {
                renderResultContentList.renderResultContentList(elementContentList, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultContentList.listViewContentList, this);
            } catch (Exception ex) {
                Logmanager.Instance.writeLog("RENDER RESULT CONTENT LIST ERR " + ex.ToString());
            }
        }
        //Multiple Choices
        public void renderToLayoutReslutMultiple(AuthorizationElement elementMultiple) {
            try {
                renderResultMultipleChoices.renderResultMultipleChoices(elementMultiple, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultMultipleChoices.listViewMultiple, this);
            } catch(Exception ex) {
                Logmanager.Instance.writeLog("RENDER RESULT MULTIPLE CHOICES ERR " + ex.ToString());
            }
        }
        //Single Choices
        public void renderToLayoutResultSingle(AuthorizationElement elementSingle) {
            try {
                renderResultSingleChoices.renderResultSingleChoices(elementSingle, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultSingleChoices.listViewSingle, this);
            }
            catch(Exception ex) {
                Logmanager.Instance.writeLog("RENDER RESULT SINGLE CHOICES ERR " + ex.ToString());
            }
        }
        //NVP
        public void renderToLayoutNVP(AuthorizationElement elementNVP) {
            try {
                renderResultNVP.renderResultNVP(elementNVP, lvAll, scvAll);
                ClientExtentions.removeHoverListView(renderResultNVP.listViewNVP, this);
            }
            catch(Exception ex) {
                Logmanager.Instance.writeLog("RENDER RESULT NVP ERR " + ex.ToString());
            }
        }
        #endregion
    }
}
