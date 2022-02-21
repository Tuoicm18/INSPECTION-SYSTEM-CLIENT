using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultMultipleChoices {
        #region VARIABLE
        private GroupBox groupBoxMultiple;
        public ListView listViewMultiple;
        private CheckBox checkBoxMultiple;
        private TextBlock textBlockDescriptionMultiple;
        //For Check CheckBox Same Group
        private List<GroupBox> groupBoxesMultiple = new List<GroupBox>();
        public bool checkBoxSameGroup { get; set; }
        #endregion

        #region RENDER
        public void renderResultMultipleChoices(AuthorizationElement elementMultiple, ListView lvAll, ScrollViewer scvAll) {
            if(null != elementMultiple) {
                //Get Header Group
                string headerGr = elementMultiple.title;
                //Get Description
                string description = elementMultiple.description;
                //Check Same Group
                this.checkBoxSameGroup = checkBoxHasSameGroup(groupBoxesMultiple, headerGr);
                if (this.checkBoxSameGroup) {
                    //Get List Multiple Choices
                    Dictionary<string, bool> multipleChoicesRequestSame;
                    if (null != elementMultiple.multipleSelect && elementMultiple.multipleSelect.Count > 0) {
                        multipleChoicesRequestSame = elementMultiple.multipleSelect;
                        foreach (var kv in multipleChoicesRequestSame) {
                            //Create Check Box
                            checkBoxMultiple = new CheckBox();
                            checkBoxMultiple.Content = kv.Key;
                            checkBoxMultiple.IsChecked = (bool)kv.Value;
                        }
                    }
                }
                else {
                    //Create Group Box
                    groupBoxMultiple = new GroupBox();
                    groupBoxMultiple.Margin = new Thickness(5, 5, 5, 10);
                    groupBoxMultiple.Header = headerGr;

                    groupBoxesMultiple.Add(groupBoxMultiple);

                    //Create List View
                    listViewMultiple = new ListView();

                    //Create Text Block Description
                    textBlockDescriptionMultiple = new TextBlock();
                    textBlockDescriptionMultiple.Text = description;
                    textBlockDescriptionMultiple.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                    textBlockDescriptionMultiple.TextWrapping = TextWrapping.Wrap;
                    //Add To List View
                    listViewMultiple.Items.Add(textBlockDescriptionMultiple);
                    //Get List Multiple Choices
                    Dictionary<string, bool> multipleChoicesRequest;
                    if (null != elementMultiple.multipleSelect && elementMultiple.multipleSelect.Count > 0) {
                        multipleChoicesRequest = elementMultiple.multipleSelect;
                        foreach (var kv in multipleChoicesRequest) {
                            //Create Check Box
                            checkBoxMultiple = new CheckBox();
                            checkBoxMultiple.Content = kv.Key;
                            checkBoxMultiple.IsChecked = (bool)kv.Value;
                            //Add To List View
                            listViewMultiple.Items.Add(checkBoxMultiple);
                        }
                    }
                }
                //Add To Group Box
                groupBoxMultiple.Content = listViewMultiple;
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxMultiple);
                }
                //Render
                lvAll.Items.Add(groupBoxMultiple);
                scvAll.Content = lvAll;
            }
        }

        //Check the Checkbox same group
        private bool checkBoxHasSameGroup(List<GroupBox> groupBoxes, string txtGroup) {
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
