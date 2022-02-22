using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ClientInspectionSystem.RenderToLayout;
using ClientInspectionSystem.SocketClient.Request;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem {
    /// <summary>
    /// Interaction logic for FormAuthenticationDataNew.xaml
    /// </summary>
    public partial class FormAuthenticationDataNew : MetroWindow {
        #region VARIABLE
        private int ordinaryClick = 0;
        private int checkButtonClick = 0;
        private readonly int BTN_ADD_TEXT = 1;
        private readonly int BTN_MULTIPLE_CHOICES = 2;
        private readonly int BTN_SINGLE_CHOICES = 3;
        private readonly int BTN_ADD_NVP = 4;

        //Plain Text
        private RenderPlainText renderLayoutPlainText = new RenderPlainText();
        //Multiple Choices
        private RenderMultipleChoices renderLayoutMultiple = new RenderMultipleChoices();
        //Single Choices
        private RenderSingleChoices renderLayoutSingleChoices = new RenderSingleChoices();
        //Name Value Pairs (NVP)
        private RenderNameValuePairs renderLayoutNVP = new RenderNameValuePairs();
        #endregion

        #region MAIN
        public FormAuthenticationDataNew() {
            InitializeComponent();
            // Set the window theme to Dark Mode
            ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
            btnSubmitAdd.IsEnabled = false;
            initControlTitleInput();
        }
        #endregion

        #region INIT USER CONTROL
        private void initControlTitleInput() {
            inputTitleFormControl.formAuthDataNew = this;
            disableButtonForInitTitleForm();
            inputTitleFormControl.Visibility = Visibility.Visible;
        }
        #endregion

        #region EVENT CLICK BUTTON
        //Plaint Text
        private void btnAddText_Click(object sender, RoutedEventArgs e) {
            checkButtonClick = BTN_ADD_TEXT;
            changeLabelTextboxForNVP(checkButtonClick);
            renderTextBox(checkButtonClick);
        }

        //Multiple
        private void btnAddMultiChoices_Click(object sender, RoutedEventArgs e) {
            checkButtonClick = BTN_MULTIPLE_CHOICES;
            changeLabelTextboxForNVP(checkButtonClick);
            renderTextBox(checkButtonClick);
        }
        //Single
        private void btnAddSingleChoices_Click(object sender, RoutedEventArgs e) {
            checkButtonClick = BTN_SINGLE_CHOICES;
            changeLabelTextboxForNVP(checkButtonClick);
            renderTextBox(checkButtonClick);
        }

        //Name Value Pairs
        private void btnAddNVP_Click(object sender, RoutedEventArgs e) {
            checkButtonClick = BTN_ADD_NVP;
            changeLabelTextboxForNVP(checkButtonClick);
            renderTextBox(checkButtonClick);
        }
        //Submit
        private void btnSubmit_Click(object sender, RoutedEventArgs e) {
            ordinaryClick = 0;
            DialogResult = true;
        }

        //Button Copy Json
        private void btnCopyJson_Click(object sender, RoutedEventArgs e) {
            try {
                AuthorizationData authorizationData = new AuthorizationData();
                //Title Form
                authorizationData.authorizationTitle = getTitleForm();
                //Content List
                List<AuthorizationElement> authorizationsContentList = renderLayoutPlainText.getDataContentListFromLayout();
                if (null != authorizationsContentList && authorizationsContentList.Count > 0) {
                    authorizationData.authContentList = authorizationsContentList;
                }
                //Multiple
                List<AuthorizationElement> authorizationsMultiple = renderLayoutMultiple.getDataMultipleChoices();

                if (null != authorizationsMultiple && authorizationsMultiple.Count > 0) {
                    authorizationData.multipleSelectList = authorizationsMultiple;
                }
                //Single
                List<AuthorizationElement> authorizationsSingle = renderLayoutSingleChoices.getDataSingleChoices();
                if (null != authorizationsSingle && authorizationsSingle.Count > 0) {
                    authorizationData.singleSelectList = authorizationsSingle;
                }
                //Namve Value Pairs
                List<AuthorizationElement> authorizationsNVP = renderLayoutNVP.getDataNVP();
                if (null != authorizationsNVP && authorizationsNVP.Count > 0) {
                    authorizationData.nameValuePairList = authorizationsNVP;
                }

                //Json String
                string strAuthorizationDataReq = JsonConvert.SerializeObject(authorizationData, Formatting.Indented, ClientExtentions.settingsJsonDuplicateDic);
                Clipboard.SetText(strAuthorizationDataReq);
            }
            catch (Exception eSubmit) {
                Logmanager.Instance.writeLog("BUTTON <COPY JSON> ERROR " + eSubmit.ToString());
            } finally {
                ordinaryClick = 0;
            }
        }
        //Add Group Box
        private void btnSubmitAdd_Click(object sender, RoutedEventArgs e) {
            if (checkButtonClick == BTN_ADD_TEXT) {
                //Render layout plain text
                renderLayoutPlainText.renderPlaintText(scvAll, lvAll, txtGroupHeader.Text,
                                                       txtAddDescription.Text, txtStringAndVKeyNVP.Text,
                                                       lbValidationGruop, btnSubmitAdd, ordinaryClick);
                //For ordinary
                ordinaryClick++;
            }
            else if (checkButtonClick == BTN_MULTIPLE_CHOICES) {
                //Render layout mutiple
                renderLayoutMultiple.renderMultipleChoices(txtGroupHeader.Text, txtStringAndVKeyNVP.Text,
                                                           txtAddDescription.Text, lvAll,
                                                           scvAll, lbValidationContent,
                                                           btnSubmitAdd, ordinaryClick);
                //For ordinary
                if (renderLayoutMultiple.cbHasSameGroup == false) {
                    ordinaryClick++;
                }
                //Remove Hover Listview multiple
                ClientExtentions.removeHoverListView(renderLayoutMultiple.listViewMultiple, this);
            }
            else if (checkButtonClick == BTN_SINGLE_CHOICES) {
                renderLayoutSingleChoices.renderSingleChoices(txtStringAndVKeyNVP.Text, txtAddDescription.Text,
                                                              txtGroupHeader.Text, lvAll,
                                                              scvAll, ordinaryClick,
                                                              lbValidationContent, btnSubmitAdd);
                //For Ordinary
                if (renderLayoutSingleChoices.radioSameGroup == false) {
                    ordinaryClick++;
                }
                //Remove Hover ListView single
                ClientExtentions.removeHoverListView(renderLayoutSingleChoices.listViewSingle, this);
            }
            else {
                renderLayoutNVP.renderNVP(txtStringAndVKeyNVP.Text, txtStringValueNVP.Text,
                                          txtGroupHeader.Text, txtAddDescription.Text,
                                          lvAll, scvAll, ordinaryClick,
                                          lbValidationKey, btnSubmitAdd);
                //For Ordinary
                if (renderLayoutNVP.checkDataGridSame == false) {
                    ordinaryClick++;
                }
                //Remove Hover Listview NVP
                ClientExtentions.removeHoverListView(renderLayoutNVP.listViewNVP, this);
            }
            //Scroll Viewer
            scvAll.ScrollToEnd();
        }
        #endregion

        //Event Scroll
        private void scvAll_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #region EVENT TEXT CHANGED TEXT BOX
        //Header (Title)
        private void txtGroupHeader_TextChanged(object sender, TextChangedEventArgs e) {
            if(checkButtonClick != BTN_ADD_TEXT) {
                if (txtGroupHeader.Text.Equals(string.Empty) || txtStringAndVKeyNVP.Text.Equals(string.Empty) || txtAddDescription.Text.Equals(string.Empty)) {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = false;
                    }
                }
                else {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = true;
                    }
                }
            } else {

            }
            //Validation Content Add Plain Text
            //renderLayoutPlainText.checkDuplicateGroupPlaintText(renderLayoutPlainText.GroubBoxesContentList, txtGroupHeader.Text,
            //                                                   lbValidationGruop, btnSubmitAdd);


            //Validation Multiple Content Checkbox
            if (txtGroupHeader.Text.Equals(string.Empty) == false) {
                if (checkButtonClick == BTN_MULTIPLE_CHOICES) {
                    renderLayoutMultiple.CheckBoxesMultiple.Clear();
                    lbValidationContent.Visibility = Visibility.Collapsed;
                }
                else if (checkButtonClick == BTN_SINGLE_CHOICES) {
                    renderLayoutSingleChoices.RadioButtonList.Clear();
                    lbValidationContent.Visibility = Visibility.Collapsed;
                }
                else {
                    renderLayoutNVP.DataGridList.Clear();
                    lbValidationKey.Visibility = Visibility.Collapsed;
                }
            }
        }
        //Plaint Text & Key
        private void txtStringAndVKeyNVP_TextChanged(object sender, TextChangedEventArgs e) {
            if(checkButtonClick != BTN_ADD_TEXT) {
                if (txtGroupHeader.Text.Equals(string.Empty) || txtStringAndVKeyNVP.Text.Equals(string.Empty) || txtAddDescription.Text.Equals(string.Empty)) {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = false;
                    }
                }
                else {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = true;
                    }
                }
            } else {
                if (txtGroupHeader.Text.Equals(string.Empty) || txtStringAndVKeyNVP.Text.Equals(string.Empty)) {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = false;
                    }
                }
                else {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = true;
                    }
                }
            }
            //Validation Multiple Content Checkbox
            if (txtStringAndVKeyNVP.Text.Equals(string.Empty) == false) {
                if (checkButtonClick == BTN_MULTIPLE_CHOICES) {
                    renderLayoutMultiple.checkDuplicateContent(lbValidationContent, btnSubmitAdd,
                                                               txtStringAndVKeyNVP.Text, renderLayoutMultiple.CheckBoxesMultiple,
                                                               renderLayoutMultiple.GroupBoxesMultiple, txtGroupHeader.Text);
                }
                else if (checkButtonClick == BTN_SINGLE_CHOICES) {
                    renderLayoutSingleChoices.checkDuplicateContent(lbValidationContent, btnSubmitAdd,
                                                                    txtStringAndVKeyNVP.Text, renderLayoutSingleChoices.RadioButtonList,
                                                                    renderLayoutSingleChoices.GroupBoxessingle, txtGroupHeader.Text);
                } else if(checkButtonClick == BTN_ADD_NVP){
                    renderLayoutNVP.checkDuplicateKey(lbValidationKey, btnSubmitAdd,
                                                      txtStringAndVKeyNVP.Text, renderLayoutNVP.DataGridList,
                                                      renderLayoutNVP.GroupBoxesNVP, txtGroupHeader.Text);
                }
            }
        }
        //Value NVP
        private void txtStringValueNVP_TextChanged(object sender, TextChangedEventArgs e) {
            if (txtStringValueNVP.Text.Equals(string.Empty)) {
                if (btnSubmitAdd != null) {
                    btnSubmitAdd.IsEnabled = false;
                }
            }
            else {
                if (txtGroupHeader.Text.Equals(string.Empty) || txtStringAndVKeyNVP.Text.Equals(string.Empty)
                    || txtAddDescription.Text.Equals(string.Empty)) {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = false;
                    }
                }
                else {
                    if (btnSubmitAdd != null) {
                        btnSubmitAdd.IsEnabled = true;
                    }
                }
            }
        }

        //Description 
        private void txtAddDescription_TextChanged(object sender, TextChangedEventArgs e) {
            txtStringAndVKeyNVP.Text = string.Empty;
            if (txtGroupHeader.Text.Equals(string.Empty) || txtStringAndVKeyNVP.Text.Equals(string.Empty) || txtAddDescription.Text.Equals(string.Empty)) {
                if (btnSubmitAdd != null) {
                    btnSubmitAdd.IsEnabled = false;
                }
            }
            else {
                if (btnSubmitAdd != null) {
                    btnSubmitAdd.IsEnabled = true;
                }
            }
        }
        #endregion

        #region RENDER LAYOUT NEED FOR MAIN

        #region CHANGE LABEL & TEXT BOX FOR ADD NAME VALUE PAIR
        private void changeLabelTextboxForNVP(int checkButton) {
            try {
                if (checkButton == BTN_ADD_TEXT || checkButton == BTN_MULTIPLE_CHOICES || checkButton == BTN_SINGLE_CHOICES) {
                    lbAddTextString.Content = ClientContants.LABEL_ADD_TEXT;
                    lbAddStringValueNVP.Visibility = Visibility.Collapsed;
                    txtStringValueNVP.Visibility = Visibility.Collapsed;
                }
                else {
                    lbAddTextString.Content = ClientContants.LABEL_ADD_VALUE;
                    lbAddStringValueNVP.Visibility = Visibility.Visible;
                    txtStringValueNVP.Visibility = Visibility.Visible;
                }
                txtGroupHeader.Text = string.Empty;
                txtStringAndVKeyNVP.Text = string.Empty;
                txtAddDescription.Text = string.Empty;
                txtStringValueNVP.Text = string.Empty;
                //Validation Label
                lbValidationContent.Visibility = Visibility.Collapsed;
                lbValidationGruop.Visibility = Visibility.Collapsed;
                lbValidationKey.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex) {
                Logmanager.Instance.writeLog("CHANGE LABEL FOR NVP ERROR " + ex.ToString());
            }
        }
        #endregion

        #region TEXT BOX
        public void renderTextBox(int checkButton) {
            try {
                if (checkButton == BTN_ADD_TEXT) {
                    // Textbox Description
                    txtAddDescription.IsEnabled = false;
                    //Button Add Text
                    btnAddText.FontWeight = FontWeights.Bold;
                    btnAddText.FontSize = 15;
                    btnAddText.Background = Brushes.Gray;
                    //Button Multiple Choices
                    btnAddMultiChoices.FontWeight = FontWeights.Normal;
                    btnAddMultiChoices.FontSize = 12;
                    btnAddMultiChoices.Background = Brushes.Transparent;
                    //Button Single Choice
                    btnAddSingleChoices.FontWeight = FontWeights.Normal;
                    btnAddSingleChoices.FontSize = 12;
                    btnAddSingleChoices.Background = Brushes.Transparent;
                    //Button NVP
                    btnAddNVP.FontWeight = FontWeights.Normal;
                    btnAddNVP.FontSize = 12;
                    btnAddNVP.Background = Brushes.Transparent;
                }
                else if (checkButton == BTN_MULTIPLE_CHOICES) {
                    // Textbox Description
                    txtAddDescription.IsEnabled = true;
                    //Button Multiple
                    btnAddMultiChoices.FontWeight = FontWeights.Bold;
                    btnAddMultiChoices.FontSize = 15;
                    btnAddMultiChoices.Background = Brushes.Gray;
                    //Button Add Text
                    btnAddText.FontWeight = FontWeights.Normal;
                    btnAddText.FontSize = 12;
                    btnAddText.Background = Brushes.Transparent;
                    //Button Single Choice
                    btnAddSingleChoices.FontWeight = FontWeights.Normal;
                    btnAddSingleChoices.FontSize = 12;
                    btnAddSingleChoices.Background = Brushes.Transparent;
                    //Button NVP
                    btnAddNVP.FontWeight = FontWeights.Normal;
                    btnAddNVP.FontSize = 12;
                    btnAddNVP.Background = Brushes.Transparent;
                }
                else if (checkButton == BTN_SINGLE_CHOICES) {
                    // Textbox Description
                    txtAddDescription.IsEnabled = true;
                    //Button Single Choice
                    btnAddSingleChoices.FontWeight = FontWeights.Bold;
                    btnAddSingleChoices.FontSize = 15;
                    btnAddSingleChoices.Background = Brushes.Gray;
                    //Button Multiple
                    btnAddMultiChoices.FontWeight = FontWeights.Normal;
                    btnAddMultiChoices.FontSize = 12;
                    btnAddMultiChoices.Background = Brushes.Transparent;
                    //Button Add Text
                    btnAddText.FontWeight = FontWeights.Normal;
                    btnAddText.FontSize = 12;
                    btnAddText.Background = Brushes.Transparent;
                    //Button NVP
                    btnAddNVP.FontWeight = FontWeights.Normal;
                    btnAddNVP.FontSize = 12;
                    btnAddNVP.Background = Brushes.Transparent;
                }
                else {
                    // Textbox Description
                    txtAddDescription.IsEnabled = true;
                    //Button NVP
                    btnAddNVP.FontWeight = FontWeights.Bold;
                    btnAddNVP.FontSize = 15;
                    btnAddNVP.Background = Brushes.Gray;
                    //Button Single Choice
                    btnAddSingleChoices.FontWeight = FontWeights.Normal;
                    btnAddSingleChoices.FontSize = 12;
                    btnAddSingleChoices.Background = Brushes.Transparent;
                    //Button Multiple
                    btnAddMultiChoices.FontWeight = FontWeights.Normal;
                    btnAddMultiChoices.FontSize = 12;
                    btnAddMultiChoices.Background = Brushes.Transparent;
                    //Button Add Text
                    btnAddText.FontWeight = FontWeights.Normal;
                    btnAddText.FontSize = 12;
                    btnAddText.Background = Brushes.Transparent;
                }
                this.Height = 700;
                this.Top = 100;
                groubBoxResult.Height = 380;
                groubBoxResult.Margin = new Thickness(5, 245, 5, 5);
                borderGridFormAddValue.Visibility = Visibility.Visible;
            }
            catch (Exception eTxt) {
                Logmanager.Instance.writeLog("RENDER TEXT BOX ERROR " + eTxt.ToString());
            }
        }
        #endregion

        #endregion

        #region VALIDATION
        #endregion

        #region GET DATA FROM LAYOUT
        //Title Form
        public string getTitleForm() {
            return this.Title;
        }
        //Plain Text
        public List<AuthorizationElement> getDataContentList() {
            List<AuthorizationElement> elementsContentList = null;
            this.Dispatcher.Invoke(() => {
                elementsContentList = renderLayoutPlainText.getDataContentListFromLayout();
            });
            return elementsContentList;
        }
        //Multiple Choices
        public List<AuthorizationElement> getDataMultipleChoices() {
            List<AuthorizationElement> elementsMultiple = null;
            this.Dispatcher.Invoke(() => {
                elementsMultiple = renderLayoutMultiple.getDataMultipleChoices();
            });
            return elementsMultiple;
        }
        //Single Choices
        public List<AuthorizationElement> getDataSingleChoices() {
            List<AuthorizationElement> elementSingle = null;
            this.Dispatcher.Invoke(() => {
                elementSingle = renderLayoutSingleChoices.getDataSingleChoices();
            });
            return elementSingle;
        }
        //Name Value Pairs
        public List<AuthorizationElement> getDataNVP() {
            List<AuthorizationElement> elementNVP = null;
            this.Dispatcher.Invoke(() => {
                elementNVP = renderLayoutNVP.getDataNVP();
            });
            return elementNVP;
        }
        #endregion

        #region DISABLE/ENABLE BUTTON
        public void disableButtonForInitTitleForm() {
            btnAddText.IsEnabled = false;
            btnAddMultiChoices.IsEnabled = false;
            btnAddSingleChoices.IsEnabled = false;
            btnAddNVP.IsEnabled = false;
            btnSubmitAdd.IsEnabled = false;
            btnCopyJson.IsEnabled = false;
        }

        public void enableButtonForInitTitleForm() {
            btnAddText.IsEnabled = true;
            btnAddMultiChoices.IsEnabled = true;
            btnAddSingleChoices.IsEnabled = true;
            btnAddNVP.IsEnabled = true;
            btnSubmitAdd.IsEnabled = true;
            btnCopyJson.IsEnabled = true;
        }
        #endregion
    }
}