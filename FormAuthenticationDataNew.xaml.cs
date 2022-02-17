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
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Newtonsoft.Json;

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
            try {
                //Logmanager.Instance.writeLog("JSON CONTENT LIST " + JsonConvert.SerializeObject(renderLayoutPlainText.getDataContentListFromLayout()));
                JsonSerializerSettings settings = new JsonSerializerSettings { Converters = new[] { new ClientExtentions.KeyValuePairConverter() } };
                string jsonMultiple = JsonConvert.SerializeObject(renderLayoutMultiple.getDataMultipleChoices(), Formatting.Indented, settings);
                Logmanager.Instance.writeLog("\nJSON MULTIPLE CHOICES \n" + jsonMultiple);
            } catch(Exception eSubmit) {
                Logmanager.Instance.writeLog("BUTTON <OK> ERROR " + eSubmit.ToString());
            } finally {
                ordinaryClick = 0;
                DialogResult = true;
            }
        }
        //Add Group Box
        private void btnSubmitAdd_Click(object sender, RoutedEventArgs e) {
            if (checkButtonClick == BTN_ADD_TEXT) {
                //For ordinary
                ordinaryClick++;
                //Render layout plain text
                renderLayoutPlainText.renderPlaintText(scvAll, lvAll, txtGroupHeader.Text,
                                                       txtAddDescription.Text, txtStringAndVKeyNVP.Text,
                                                       lbValidationGruop, btnSubmitAdd, ordinaryClick);
            }
            else if (checkButtonClick == BTN_MULTIPLE_CHOICES) {
                //For ordinary
                if(renderLayoutMultiple.cbHasSameGroup == false) {
                    ordinaryClick++;
                }
                //Render layout mutiple
                renderLayoutMultiple.renderMultipleChoices(txtGroupHeader.Text, txtStringAndVKeyNVP.Text,
                                                           txtAddDescription.Text, lvAll,
                                                           scvAll, lbValidationContent,
                                                           btnSubmitAdd, ordinaryClick);
                //Remove Hover Listview multiple
                removeHoverListView(renderLayoutMultiple.listViewMultiple);
            }
            else if (checkButtonClick == BTN_SINGLE_CHOICES) {
                renderLayoutSingleChoices.renderSingleChoices(txtStringAndVKeyNVP.Text, txtAddDescription.Text,
                                                              txtGroupHeader.Text, lvAll, scvAll);
                //Remove Hover ListView single
                removeHoverListView(renderLayoutSingleChoices.listViewSingle);
            }
            else {
                renderLayoutNVP.renderNVP(txtStringAndVKeyNVP.Text, txtStringValueNVP.Text,
                                          txtGroupHeader.Text, txtAddDescription.Text,
                                          lvAll, scvAll);
                //Remove Hover Listview NVP
                removeHoverListView(renderLayoutNVP.listViewNVP);
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
            //Validation Content Add Plain Text
            //renderLayoutPlainText.checkDuplicateGroupPlaintText(renderLayoutPlainText.GroubBoxesContentList, txtGroupHeader.Text,
            //                                                   lbValidationGruop, btnSubmitAdd);

        }
        //Plaint Text & Key
        private void txtStringAndVKeyNVP_TextChanged(object sender, TextChangedEventArgs e) {
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
            //Validation Multiple Content Checkbox
            //renderLayoutMultiple.checkDuplicateContent(lbValidationContent, btnSubmitAdd, 
            //                                           txtStringAndVKeyNVP.Text, renderLayoutMultiple.CheckBoxesMultiple,
            //                                           renderLayoutMultiple.GroupBoxesMultiple, txtGroupHeader.Text);
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

        #region REMOVE HOVER LISTVIEW
        private void removeHoverListView(ListView lv) {
            //Remove Hover List View
            lv.ItemContainerStyle = this.Resources["RemoveHoverListView"] as Style;
        }
        #endregion

        #endregion

        #region VALIDATION
        #endregion
    }
}
