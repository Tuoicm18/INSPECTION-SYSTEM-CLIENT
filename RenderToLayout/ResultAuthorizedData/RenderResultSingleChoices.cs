using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultSingleChoices {
        #region VARIABLE
        private GroupBox groupBoxSingle;
        public ListView listViewSingle;
        private TextBlock textBlockDescriptionSingle;
        private RadioButton radioButtonSingle;
        //For Check Radio Button Same Group
        private List<GroupBox> groupBoxesSingle = new List<GroupBox>();
        public bool radioSameGroup { get; set; }
        #endregion

        #region RENDER
        public void renderResultSingleChoices(AuthorizationElement elementSingleChoices, ListView lvAll, ScrollViewer scvAll) {
            if(null != elementSingleChoices) {
                //Get Header Group
                string headerGr = elementSingleChoices.title;
                //Get Description
                string description = elementSingleChoices.label;
                //Check Same Group
                radioSameGroup = checkRadioHasSameGroup(groupBoxesSingle, headerGr);
                if (radioSameGroup) {
                    //Init Radio Button
                    Dictionary<string, bool> singleChoicesRequestSame;
                    if (null != elementSingleChoices.singleSelect && elementSingleChoices.singleSelect.Count > 0) {
                        singleChoicesRequestSame = elementSingleChoices.singleSelect;
                        foreach (var kv in singleChoicesRequestSame) {
                            //Create Radio Button
                            radioButtonSingle = new RadioButton();
                            radioButtonSingle.Content = kv.Key;
                            radioButtonSingle.IsChecked = (bool)kv.Value;
                        }
                    }
                }
                else {
                    //Create Group Box
                    groupBoxSingle = new GroupBox();
                    groupBoxSingle.Margin = new Thickness(5, 5, 5, 10);
                    groupBoxSingle.Header = headerGr;
                    groupBoxSingle.IsEnabled = false;

                    groupBoxesSingle.Add(groupBoxSingle);

                    //Create List View
                    listViewSingle = new ListView();

                    //Create Text Block Description
                    textBlockDescriptionSingle = new TextBlock();
                    textBlockDescriptionSingle.Text = description;
                    textBlockDescriptionSingle.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                    textBlockDescriptionSingle.TextWrapping = TextWrapping.Wrap;
                    listViewSingle.Items.Add(textBlockDescriptionSingle);
                    //Init Radio Button
                    Dictionary<string, bool> singleChoicesRequest;
                    if (null != elementSingleChoices.singleSelect && elementSingleChoices.singleSelect.Count > 0) {
                        singleChoicesRequest = elementSingleChoices.singleSelect;
                        foreach (var kv in singleChoicesRequest) {
                            //Create Radio Button
                            radioButtonSingle = new RadioButton();
                            radioButtonSingle.Content = kv.Key;
                            radioButtonSingle.IsChecked = (bool)kv.Value;
                            //Add To List View & Gr Box
                            listViewSingle.Items.Add(radioButtonSingle);
                        }
                    }
                }

                //Render
                groupBoxSingle.Content = listViewSingle;
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxSingle);
                }
                lvAll.Items.Add(groupBoxSingle);
                scvAll.Content = lvAll;
            }
        }

        //Check the Radio Button same group
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
    }
}
