using ClientInspectionSystem.SocketClient.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout {
    public class RenderPlainText {
        #region VARIABEL
        private GroupBox groupBoxContentList;
        private TextBlock textBlockContentList;
        //private TextBlock textBlockContentListDesc;
        private ListView listViewContentList;
        private List<GroupBox> groupBoxesContentList = new List<GroupBox>();
        public List<GroupBox> GroubBoxesContentList {
            get { return this.groupBoxesContentList; }
        }
        private List<AuthorizationElement> authorizationsContentList = new List<AuthorizationElement>();
        #endregion

        #region RENDER
        public void renderPlaintText(ScrollViewer scvAll, ListView lvAll,
                                      string headerGroup, string description,
                                      string content, Label lbValidationGruop,
                                      Button btnSubmitAdd, int ordinaryInput) {
            try {
                //Group
                groupBoxContentList = new GroupBox();
                groupBoxContentList.Margin = new Thickness(5, 5, 5, 10);
                groupBoxContentList.Header = headerGroup;
                //Description
                //textBlockContentListDesc = new TextBlock();
                //textBlockContentListDesc.TextWrapping = TextWrapping.Wrap;
                //textBlockContentListDesc.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                //textBlockContentListDesc.Text = description;
                //Content
                textBlockContentList = new TextBlock();
                textBlockContentList.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                textBlockContentList.TextWrapping = TextWrapping.Wrap;
                textBlockContentList.Text = content;
                // ADD TO List View
                listViewContentList = new ListView();
                listViewContentList.ForceCursor = false;
                listViewContentList.IsHitTestVisible = false;
                //listViewContentList.Items.Add(textBlockContentListDesc);
                listViewContentList.Items.Add(textBlockContentList);
                groupBoxContentList.Content = listViewContentList;
                //Add group box to list for get data
                groupBoxesContentList.Add(groupBoxContentList);
                //Validation
                //checkDuplicateGroupPlaintText(groupBoxesContentList, headerGroup, lbValidationGruop, btnSubmitAdd);
                //Render
                lvAll.Items.Add(groupBoxContentList);
                scvAll.Content = lvAll;
                //For Get Data
                authorizationsContentList.Add(new AuthorizationElement {
                    ordinary = ordinaryInput,
                    //label = description,
                    title = headerGroup,
                    text = content
                });
            }
            catch (Exception ePlaintText) {
                Logmanager.Instance.writeLog("RENDER PLAINT TEXT ERROR " + ePlaintText.ToString());
            }
        }
        #endregion

        #region VALIDATION
        public void checkDuplicateGroupPlaintText(List<GroupBox> groupBoxes, string headerText,
                                                  Label lbValidationGruop, Button btnSubmitAdd) {
            if (null != groupBoxes) {
                for (int i = 0; i < groupBoxes.Count; i++) {
                    if (groupBoxes[i].Header.ToString().ToLower().Equals(headerText.ToLower())) {
                        lbValidationGruop.Content = ClientContants.LABEL_VALIDATION_ADD_GROUP;
                        lbValidationGruop.Visibility = Visibility.Visible;
                        if (btnSubmitAdd != null) {
                            btnSubmitAdd.IsEnabled = false;
                        }
                        break;
                    }
                    else {
                        lbValidationGruop.Visibility = Visibility.Collapsed;
                        if (btnSubmitAdd != null) {
                            btnSubmitAdd.IsEnabled = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region GET DATA FROM LAYOUT
        public List<AuthorizationElement> getDataContentListFromLayout() {
            if(null != this.authorizationsContentList) {
                return this.authorizationsContentList;
            } else {
                return null;
            }
        }
        #endregion
    }
}
