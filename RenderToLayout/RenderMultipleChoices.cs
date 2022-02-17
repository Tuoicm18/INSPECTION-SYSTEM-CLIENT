using ClientInspectionSystem.RenderToLayout.Models;
using ClientInspectionSystem.SocketClient.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientInspectionSystem.RenderToLayout {
    public class RenderMultipleChoices {
        #region VARIABEL
        private GroupBox groupBoxMultiple;
        private CheckBox checkBoxMultiple;
        private TextBlock textBlockMultipleDesc;
        public ListView listViewMultiple;
        private List<GroupBox> groupBoxesMultiple = new List<GroupBox>();
        public List<GroupBox> GroupBoxesMultiple {
            get { return this.groupBoxesMultiple; }
        }
        private List<CheckBox> checkBoxesMultiple = new List<CheckBox>();
        public List<CheckBox> CheckBoxesMultiple {
            get { return this.checkBoxesMultiple; }
        }

        private List<string> getTitleMultiple = new List<string>();
        private List<MultipleSelectModel> multipleSelectModels = new List<MultipleSelectModel>();

        public bool cbHasSameGroup { get; set; }
        #endregion

        #region RENDER
        public void renderMultipleChoices(string headerGroup, string contentCheckBox,
                                           string description, ListView lvAll,
                                           ScrollViewer scvAll, Label lbValidationConetn,
                                           Button btnSubmitAdd, int ordinaryInput) {
            cbHasSameGroup = checkCbHasSameGroup(groupBoxesMultiple, headerGroup);
            try {          
                //UUID
                string uuid = string.Empty;
                uuid = "G_" + Guid.NewGuid().ToString("N");
                if (cbHasSameGroup) {
                    //Textblock Description
                    textBlockMultipleDesc.TextWrapping = TextWrapping.Wrap;
                    textBlockMultipleDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockMultipleDesc.Text = description;
                    //Check Box Multiple
                    checkBoxMultiple = new CheckBox();
                    checkBoxMultiple.Name = uuid;
                    checkBoxMultiple.Foreground = Brushes.White;
                    checkBoxMultiple.Content = contentCheckBox;
                    //Add To List Check Box For get Data & Validtion
                    checkBoxesMultiple.Add(checkBoxMultiple);
                    getTitleMultiple.Add(headerGroup + uuid);
                    listViewMultiple.Items.Add(checkBoxMultiple);
                }
                else {
                    //List View Multiple
                    listViewMultiple = new ListView();
                    //Groupbox Multiple
                    groupBoxMultiple = new GroupBox();
                    groupBoxMultiple.Margin = new Thickness(5, 5, 5, 10);
                    groupBoxMultiple.Header = headerGroup;
                    //Add Groub box to List for get data
                    groupBoxesMultiple.Add(groupBoxMultiple);

                    //Textblock Description
                    textBlockMultipleDesc = new TextBlock();
                    textBlockMultipleDesc.TextWrapping = TextWrapping.Wrap;
                    textBlockMultipleDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockMultipleDesc.Text = description;
                    //Check Box Multiple
                    checkBoxMultiple = new CheckBox();
                    checkBoxMultiple.Name = uuid;
                    checkBoxMultiple.Foreground = Brushes.White;
                    checkBoxMultiple.Content = contentCheckBox;
                    //Render
                    listViewMultiple.Items.Add(textBlockMultipleDesc);
                    listViewMultiple.Items.Add(checkBoxMultiple);
                    groupBoxMultiple.Content = listViewMultiple;
                    //Add To List Check Box For get Data & Validtion
                    checkBoxesMultiple.Add(checkBoxMultiple);
                    getTitleMultiple.Add(headerGroup + uuid);
                }
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxMultiple);
                }
                lvAll.Items.Add(groupBoxMultiple);
                scvAll.Content = lvAll;
            }
            catch (Exception eMulti) {
                Logmanager.Instance.writeLog("RENDER MULTIPLE CHOICES ERROR " + eMulti.ToString());
            } finally {
                //Add To List For Get Data
                multipleSelectModels.Add(new MultipleSelectModel {
                    ordinaryMultipleSelect = ordinaryInput,
                    description = textBlockMultipleDesc,
                    title = headerGroup,
                    checkBox = checkBoxMultiple,
                    groupBox = groupBoxMultiple
                });
            }
        }
        //Check the checkbox has the same group 
        private bool checkCbHasSameGroup(List<GroupBox> groupBoxes, string txtGroup) {
            if (null != groupBoxes) {
                for (int g = 0; g < groupBoxes.Count; g++) {
                    if (groupBoxes[g].Header.ToString().ToLower().Equals(txtGroup.ToLower())) {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region VALIDATION
        public void checkDuplicateContent(Label lbValidationContent, Button btnSubmitAdd,
                                          string contentCheckBox, List<CheckBox> checkBoxes,
                                          List<GroupBox> groupBoxes, string headerGroupBox) {
            if (null != groupBoxes) {
                for (int gb = 0; gb < groupBoxes.Count; gb++) {
                    if (groupBoxes[gb].Header.ToString().ToUpper().Equals(headerGroupBox.ToUpper())) {
                        if (null != checkBoxes) {
                            for (int cb = 0; cb < checkBoxes.Count; cb++) {
                                if (checkBoxes[cb].Content.ToString().ToLower().Equals(contentCheckBox.ToLower())) {
                                    lbValidationContent.Content = ClientContants.LABEL_VALIDATION_ADD_CONTENT;
                                    lbValidationContent.Visibility = Visibility.Visible;
                                    if (null != btnSubmitAdd) {
                                        btnSubmitAdd.IsEnabled = false;
                                    }
                                    break;
                                }
                                else {
                                    lbValidationContent.Visibility = Visibility.Collapsed;
                                    if (null != btnSubmitAdd) {
                                        btnSubmitAdd.IsEnabled = true;
                                    }
                                }
                            }
                        }
                    }
                    else {
                        lbValidationContent.Visibility = Visibility.Collapsed;
                        if (null != btnSubmitAdd) {
                            btnSubmitAdd.IsEnabled = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region GET DATA FROM LAYOUT MULTIPLE CHOICES
        public List<AuthorizationElement> getDataMultipleChoices() {
            List<AuthorizationElement> authorizationElementsMultiple = new List<AuthorizationElement>();
            Dictionary<string, AuthorizationElement> dictAuthElement = new Dictionary<string, AuthorizationElement>();
            string uuidCheckBox;
            string uuidTitleGroupBox;
             if (null != multipleSelectModels) {
                for (int models = 0; models < multipleSelectModels.Count; models++) {
                    AuthorizationElement val;
                    //Ordinary
                    int ordianry = multipleSelectModels[models].ordinaryMultipleSelect;
                    //Description
                    TextBlock des = multipleSelectModels[models].description;
                    //Title
                    string title = multipleSelectModels[models].title;
                    //Check Box
                    CheckBox getCheckBox = multipleSelectModels[models].checkBox;
                    string contentCheckBox = getCheckBox.Content.ToString();
                    bool isCheckedCheckBox = (bool)getCheckBox.IsChecked;
                    //Group Box
                    GroupBox getGroupBox = multipleSelectModels[models].groupBox;

                    //Loop List title
                    for (int t = 0; t < getTitleMultiple.Count; t++) {
                        uuidCheckBox = ClientExtentions.subStringRight(getTitleMultiple[t], getCheckBox.Name.Length);
                        uuidTitleGroupBox = getTitleMultiple[t];
                        if (uuidCheckBox.Equals(getCheckBox.Name)) {
                            string titleGroupBox = getGroupBox.Header.ToString();
                            if (uuidTitleGroupBox.Contains(titleGroupBox)) {
                                if (dictAuthElement.ContainsKey(titleGroupBox)) {
                                    dictAuthElement.TryGetValue(titleGroupBox, out val);
                                }
                                else {
                                    val = new AuthorizationElement();
                                    val.ordinary = ordianry;
                                    val.description = des.Text;
                                    val.title = titleGroupBox;
                                    val.multipleSelect = new List<KeyValuePair<string, object>>();
                                    dictAuthElement.Add(titleGroupBox, val);
                                }
                                val.multipleSelect.Add(new KeyValuePair<string, object> (contentCheckBox, isCheckedCheckBox));
                            }
                        }
                    }
                }
                foreach (var eml in dictAuthElement) {
                    authorizationElementsMultiple.Add(eml.Value);
                }
            }
            authorizationElementsMultiple = authorizationElementsMultiple.GroupBy(g => g.title).Select(s => s.First()).ToList();
            return authorizationElementsMultiple;
        }
        #endregion
    }
}