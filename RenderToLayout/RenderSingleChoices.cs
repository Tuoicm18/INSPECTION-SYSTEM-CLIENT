using ClientInspectionSystem.RenderToLayout.Models;
using ClientInspectionSystem.SocketClient.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout {
    public class RenderSingleChoices {
        #region VARIABLE
        private RadioButton radioButtonSingle;
        private GroupBox groupBoxSingle;
        private TextBlock textBlockSingleDesc;
        public ListView listViewSingle;
        private List<GroupBox> groupBoxesSingle = new List<GroupBox>();
        public List<GroupBox> GroupBoxessingle {
            get { return groupBoxesSingle; }
        }

        private List<string> getTitlesSingle = new List<string>();
        private List<SingleSelectModel> singleSelectModels = new List<SingleSelectModel>();
        public bool radioSameGroup { get; set; }
        #endregion

        #region RENDER
        public void renderSingleChoices(string contentRadio, string description,
                                        string headerGroup, ListView lvAll,
                                        ScrollViewer scvAll, int ordinaryInput) {
            try {
                radioSameGroup = checkRadioHasSameGroup(groupBoxesSingle, headerGroup);
                if (radioSameGroup) {
                    //Radio Button Single
                    radioButtonSingle = new RadioButton();
                    radioButtonSingle.Foreground = Brushes.White;
                    radioButtonSingle.Content = contentRadio;
                    radioButtonSingle.GroupName = headerGroup;
                    //Textblock Description
                    textBlockSingleDesc.TextWrapping = TextWrapping.Wrap;
                    textBlockSingleDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockSingleDesc.Text = description;
                    //Add To List Check Box For get Data & Validtion
                    getTitlesSingle.Add(headerGroup);
                    //Render
                    listViewSingle.Items.Add(radioButtonSingle);
                }
                else {
                    //List View Single
                    listViewSingle = new ListView();
                    //Groupbox Single
                    groupBoxSingle = new GroupBox();
                    groupBoxSingle.Margin = new Thickness(5, 5, 5, 10);
                    groupBoxSingle.Header = headerGroup;
                    //Add Groub box to List for get data
                    groupBoxesSingle.Add(groupBoxSingle);

                    //Textblock Description
                    textBlockSingleDesc = new TextBlock();
                    textBlockSingleDesc.TextWrapping = TextWrapping.Wrap;
                    textBlockSingleDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockSingleDesc.Text = description;
                    //Radio Button Single
                    radioButtonSingle = new RadioButton();
                    radioButtonSingle.Foreground = Brushes.White;
                    radioButtonSingle.Content = contentRadio;
                    radioButtonSingle.GroupName = headerGroup;
                    //Render
                    listViewSingle.Items.Add(textBlockSingleDesc);
                    listViewSingle.Items.Add(radioButtonSingle);
                    groupBoxSingle.Content = listViewSingle;
                    //Add To List Check Box For get Data & Validtion
                    getTitlesSingle.Add(headerGroup);
                }
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxSingle);
                }
                lvAll.Items.Add(groupBoxSingle);
                scvAll.Content = lvAll;
            }
            catch (Exception eSingle) {
                Logmanager.Instance.writeLog("RENDER LAYOUT SINGLE CHOICE ERROR " + eSingle.ToString());
            } finally {
                //Add To List For Get Data
                singleSelectModels.Add(new SingleSelectModel {
                    ordinarySingle = ordinaryInput,
                    description = textBlockSingleDesc,
                    title = headerGroup,
                    radioButton = radioButtonSingle,
                    groupBox = groupBoxSingle
                });
            }
        }

        private bool checkRadioHasSameGroup(List<GroupBox> groupBoxes, string txtGroup) {
            if (null != groupBoxes) {
                for (int g = 0; g < groupBoxes.Count; g++) {
                    if (groupBoxes[g].Header.ToString().ToUpper().Equals(txtGroup.ToUpper())) {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region GET DATA FROM LAYOUT SINGLE CHOICES
        public List<AuthorizationElement> getDataSingleChoices() {
            List<AuthorizationElement> authorizationElementsSingle = new List<AuthorizationElement>();
            Dictionary<string, AuthorizationElement> dictAuthElement = new Dictionary<string, AuthorizationElement>();

            if (null != singleSelectModels) {
                for (int models = 0; models < singleSelectModels.Count; models++) {
                    AuthorizationElement val;
                    //Ordinary
                    int ordinarySingle = singleSelectModels[models].ordinarySingle;
                    //Description
                    TextBlock des = singleSelectModels[models].description;
                    //Title
                    string title = singleSelectModels[models].title;
                    //Radio Button 
                    RadioButton radioButton = singleSelectModels[models].radioButton;
                    string grNameRadio = radioButton.GroupName;
                    bool isCheckedRadio = (bool)radioButton.IsChecked;
                    string contentRadio = radioButton.Content.ToString();
                    //Group Box
                    GroupBox groupBox = singleSelectModels[models].groupBox;
                    string headerGroup = groupBox.Header.ToString();
                    if (grNameRadio.ToLower().Equals(headerGroup.ToLower())) {
                        if (dictAuthElement.ContainsKey(headerGroup)) {
                            dictAuthElement.TryGetValue(headerGroup, out val);
                        }
                        else {
                            val = new AuthorizationElement();
                            val.ordinary = ordinarySingle;
                            val.description = des.Text;
                            val.title = headerGroup;
                            val.singleSelect = new List<KeyValuePair<string, object>>();
                            dictAuthElement.Add(headerGroup, val);
                        }
                        val.singleSelect.Add(new KeyValuePair<string, object>(contentRadio, isCheckedRadio));
                    }
                    foreach (var eml in dictAuthElement) {
                        authorizationElementsSingle.Add(eml.Value);
                    }
                }
            }
            authorizationElementsSingle = authorizationElementsSingle.GroupBy(g => g.title).Select(s => s.First()).ToList();
            return authorizationElementsSingle;
        }
        #endregion 
    }
}
