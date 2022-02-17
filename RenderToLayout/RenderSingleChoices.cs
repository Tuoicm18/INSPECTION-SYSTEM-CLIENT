using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private List<RadioButton> radioButtonsSingle = new List<RadioButton>();
        public List<RadioButton> RadioButtonsSingle {
            get { return this.radioButtonsSingle; }
        }
        #endregion

        #region RENDER
        public void renderSingleChoices(string contentRadio, string description,
                                        string headerGroup, ListView lvAll,
                                        ScrollViewer scvAll) {
            try {
                bool radioSameGroup = checkRadioHasSameGroup(groupBoxesSingle, headerGroup);
                if(radioSameGroup) {
                    //Radio Button Single
                    radioButtonSingle = new RadioButton();
                    radioButtonSingle.Foreground = Brushes.White;
                    radioButtonSingle.Content = contentRadio;
                    //Textblock Description
                    textBlockSingleDesc = new TextBlock();
                    textBlockSingleDesc.TextWrapping = TextWrapping.Wrap;
                    textBlockSingleDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockSingleDesc.Text = description;
                    //Render
                    listViewSingle.Items.Add(textBlockSingleDesc);
                    listViewSingle.Items.Add(radioButtonSingle);
                    groupBoxSingle.Content = listViewSingle;
                    //Add To List Check Box For get Data & Validtion
                    radioButtonsSingle.Add(radioButtonSingle);
                } else {
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
                    //Render
                    listViewSingle.Items.Add(textBlockSingleDesc);
                    listViewSingle.Items.Add(radioButtonSingle);
                    groupBoxSingle.Content = listViewSingle;
                    //Add To List Check Box For get Data & Validtion
                    radioButtonsSingle.Add(radioButtonSingle);
                }
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxSingle);
                }
                lvAll.Items.Add(groupBoxSingle);
                scvAll.Content = lvAll;
            } catch(Exception eSingle) {
                Logmanager.Instance.writeLog("RENDER LAYOUT SINGLE CHOICE ERROR " + eSingle.ToString());
            }
        }

        private bool checkRadioHasSameGroup(List<GroupBox> groupBoxes, string txtGroup) {
            if(null != groupBoxes) {
                for(int g  = 0; g < groupBoxes.Count; g++) {
                    if(groupBoxes[g].Header.ToString().ToUpper().Equals(txtGroup.ToUpper())) {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
