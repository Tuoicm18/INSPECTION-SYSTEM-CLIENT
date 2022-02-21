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
