using ClientInspectionSystem.RenderToLayout.Models;
using ClientInspectionSystem.SocketClient.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PluginICAOClientSDK.Request;
using log4net;

namespace ClientInspectionSystem.RenderToLayout {
    public class RenderNameValuePairs {

        #region VARIABLE
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BrushConverter bc = new BrushConverter();
        private GroupBox groupBoxNameValuePair;
        private DataGrid dataGridNameValuePair;
        private DataGridTextColumn dataGridBoundColumnKey;
        private DataGridTextColumn dataGridBoundColumnValue;
        private TextBlock textBlockDescriptionNVP;
        public ListView listViewNVP;
        private List<GroupBox> groupBoxesNVP = new List<GroupBox>();
        public List<GroupBox> GroupBoxesNVP {
            get { return this.groupBoxesNVP; }
        }
        private List<string> descriptionList = new List<string>();
        public List<string> DescriptionList {
            get { return this.descriptionList; }
        }

        private List<string> getTitleNVP = new List<string>();
        private List<NameValuePairsModel> NVPModels = new List<NameValuePairsModel>();
        public bool checkDataGridSame { get; set; }
        //For Validation
        private List<DataGrid> dataGridList = new List<DataGrid>();
        public List<DataGrid> DataGridList {
            get { return this.dataGridList; }
        }

        #endregion

        #region RENDER
        public void renderNVP(string keyContent, string valueContent,
                              string headerGroup, string description,
                              ListView lvAll, ScrollViewer scvAll,
                              int ordinaryInput, Label lbValidationKey,
                              Button btnSubmitAdd) {
            try {
                checkDataGridSame = checkDataGridSameGroup(groupBoxesNVP, headerGroup);
                //UUID
                string uuid = string.Empty;
                uuid = "G_" + Guid.NewGuid().ToString("N");
                if (checkDataGridSame) {
                    //Textblock Description
                    textBlockDescriptionNVP.TextWrapping = TextWrapping.Wrap;
                    textBlockDescriptionNVP.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockDescriptionNVP.Text = description;
                    //Data Grid NVP
                    dataGridNameValuePair.Items.Add(new ModelBindingDataNVP { key = keyContent, value = " " + valueContent });
                    //Add to list for get Data
                    getTitleNVP.Add(headerGroup + uuid);
                }
                else {
                    //List View NVP
                    listViewNVP = new ListView();
                    //Groupbox NVP
                    groupBoxNameValuePair = new GroupBox();
                    groupBoxNameValuePair.Header = headerGroup;
                    //Add to List for get data
                    groupBoxesNVP.Add(groupBoxNameValuePair);
                    descriptionList.Add(description);
                    //Textblock Description
                    textBlockDescriptionNVP = new TextBlock();
                    textBlockDescriptionNVP.TextWrapping = TextWrapping.Wrap;
                    textBlockDescriptionNVP.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                    textBlockDescriptionNVP.Text = description;
                    //Data Grid NVP
                    dataGridNameValuePair = new DataGrid();
                    dataGridNameValuePair.Name = uuid;
                    dataGridNameValuePair.Background = Brushes.Black;
                    dataGridNameValuePair.RowBackground = (Brush)bc.ConvertFrom("#111111");
                    dataGridNameValuePair.AlternatingRowBackground = (Brush)bc.ConvertFrom("#282828");
                    dataGridNameValuePair.MaxWidth = ClientContants.DATA_GRID_MAX_WIDTH;
                    dataGridNameValuePair.Items.Add(new ModelBindingDataNVP { key = keyContent, value = " " + valueContent });
                    //For Validation
                    dataGridList.Add(dataGridNameValuePair);

                    //KEY COLUMN
                    dataGridBoundColumnKey = new DataGridTextColumn();
                    styleDataGridTextColumn(dataGridBoundColumnKey, true); // Style
                    dataGridBoundColumnKey.Binding = new Binding("key"); // Binding data
                    dataGridNameValuePair.Columns.Add(dataGridBoundColumnKey);
                    //VALUE COLUMN
                    dataGridBoundColumnValue = new DataGridTextColumn();
                    styleDataGridTextColumn(dataGridBoundColumnValue, false);
                    dataGridBoundColumnValue.Binding = new Binding("value");
                    dataGridNameValuePair.Columns.Add(dataGridBoundColumnValue);
                    //Render
                    listViewNVP.Items.Add(textBlockDescriptionNVP);
                    listViewNVP.Items.Add(dataGridNameValuePair);
                    groupBoxNameValuePair.Content = listViewNVP;
                    //Add to list for get data
                    getTitleNVP.Add(headerGroup + uuid);
                }
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxNameValuePair);
                }
                lvAll.Items.Add(groupBoxNameValuePair);
                scvAll.Content = lvAll;
                //Validation
                checkDuplicateKey(lbValidationKey, btnSubmitAdd, keyContent, dataGridList, groupBoxesNVP, headerGroup);
            }
            catch (Exception eNVP) {
                logger.Error(eNVP);
            } finally {
                //Add To List For Get Data
                NVPModels.Add(new NameValuePairsModel {
                    ordinaryNVP = ordinaryInput,
                    description = textBlockDescriptionNVP,
                    title = headerGroup,
                    dataGrid = dataGridNameValuePair,
                    keyData = keyContent,
                    valueData = valueContent,
                    groupBox = groupBoxNameValuePair
                });
            }
        }
        //Check Same Group Data Grid
        private bool checkDataGridSameGroup(List<GroupBox> groupBoxes, string txtHeader) {
            if (null != groupBoxes) {
                for (int g = 0; g < groupBoxes.Count; g++) {
                    if (groupBoxes[g].Header.ToString().ToLower().Equals(txtHeader.ToLower())) {
                        return true;
                    }
                }
            }
            return false;
        }
        //Check Same Group Description
        private bool checkDescriptionSameGroup(List<string> des, string txtDes) {
            if (null != des) {
                des = des.GroupBy(g => g).Select(s => s.First()).ToList();
                for (int d = 0; d < des.Count; d++) {
                    if (des[d].ToLower().Equals(txtDes.ToLower())) {
                        return true;
                    }
                }
            }
            return false;
        }
        //Style Cell Data Grid
        private void styleDataGridTextColumn(DataGridTextColumn dataGridTextColumn, bool isKey) {
            if (isKey) {
                //KEY COLUMN
                dataGridTextColumn.Header = "KEY";
                dataGridTextColumn.CanUserResize = false;
                dataGridTextColumn.Width = 200;
                //Style Header
                Style headerStyleKey = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                headerStyleKey.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.BackgroundProperty, Brushes.Black));
                headerStyleKey.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorVisibilityProperty, Visibility.Collapsed));
                headerStyleKey.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 0, 0)));
                headerStyleKey.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalAlignment.Right));
                dataGridTextColumn.HeaderStyle = headerStyleKey;

                //Wrapping text in rows
                Style elementStyleKey = new Style(typeof(TextBlock));
                elementStyleKey.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                elementStyleKey.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));
                dataGridTextColumn.ElementStyle = elementStyleKey;
            }
            else {
                //VALUE COLUMN
                dataGridTextColumn.Header = "     VALUE";
                dataGridTextColumn.CanUserResize = false;
                dataGridTextColumn.FontWeight = FontWeights.Bold;
                dataGridTextColumn.Width = 540;
                Style headerStyleValue = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));
                headerStyleValue.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.BackgroundProperty, Brushes.Black));
                headerStyleValue.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.SeparatorVisibilityProperty, Visibility.Collapsed));
                headerStyleValue.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.BorderThicknessProperty, new Thickness(0, 0, 0, 0)));
                dataGridTextColumn.HeaderStyle = headerStyleValue;

                //Text Wrapping Rows
                Style elementStyleValue = new Style(typeof(TextBlock));
                elementStyleValue.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
                elementStyleValue.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
                elementStyleValue.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(15, 0, 0, 0)));
                dataGridTextColumn.ElementStyle = elementStyleValue;
            }
        }
        #endregion

        #region GET DATA FROM LAYOUT NVP
        public List<AuthorizationElement> getDataNVP() {
            List<AuthorizationElement> authorizationElementsNVP = new List<AuthorizationElement>();
            Dictionary<string, AuthorizationElement> dictAuthElement = new Dictionary<string, AuthorizationElement>();

            if (null != NVPModels) {
                for (int nvp = 0; nvp < NVPModels.Count; nvp++) {
                    //Ordinary 
                    int ordinary = NVPModels[nvp].ordinaryNVP;
                    //Description
                    TextBlock des = NVPModels[nvp].description;
                    //Title
                    string title = NVPModels[nvp].title;
                    //Data grid
                    DataGrid dataGrid = NVPModels[nvp].dataGrid;
                    //Data key
                    string keyData = NVPModels[nvp].keyData;
                    //Value key
                    string valueData = NVPModels[nvp].valueData;
                    //Group Box
                    GroupBox groupBox = NVPModels[nvp].groupBox;
                    for (int t = 0; t < getTitleNVP.Count; t++) {
                        AuthorizationElement val;
                        string uuidDataGrid = ClientExtentions.subStringRight(getTitleNVP[t], dataGrid.Name.Length);
                        string uuidTitleDataGrid = getTitleNVP[t];
                        if (uuidDataGrid.ToLower().Equals(dataGrid.Name.ToLower())) {
                            string titleGroupBoxDataGird = groupBox.Header.ToString();
                            if (uuidTitleDataGrid.ToLower().Contains(titleGroupBoxDataGird.ToLower())) {
                                if (dictAuthElement.ContainsKey(titleGroupBoxDataGird)) {
                                    dictAuthElement.TryGetValue(titleGroupBoxDataGird, out val);
                                }
                                else {
                                    val = new AuthorizationElement();
                                    val.ordinary = ordinary;
                                    val.label = des.Text;
                                    val.title = titleGroupBoxDataGird;
                                    val.nameValuePair = new Dictionary<string, string>();
                                    dictAuthElement.Add(titleGroupBoxDataGird, val);
                                }
                                val.nameValuePair.Add(keyData, valueData);
                            }
                        }
                        foreach (var eml in dictAuthElement) {
                            authorizationElementsNVP.Add(eml.Value);
                        }
                    }
                }
            }
            authorizationElementsNVP = authorizationElementsNVP.GroupBy(g => g.title).Select(s => s.First()).ToList();
            return authorizationElementsNVP;
        }
        #endregion

        #region VALIDATION 
        public void checkDuplicateKey(Label lbValidationKey, Button btnSubmitAdd,
                                      string keyInput, List<DataGrid> dataGrids,
                                      List<GroupBox> groupBoxes, string headerGroupBox) {
            if (null != groupBoxes) {
                ModelBindingDataNVP modelBindingDataNVP;
                for (int gb = 0; gb < groupBoxes.Count; gb++) {
                    if (groupBoxes[gb].Header.ToString().ToUpper().Equals(headerGroupBox.ToUpper())) {
                        if (null != dataGrids) {
                            for (int dt = 0; dt < dataGrids.Count; dt++) {
                                for (int item = 0; item < dataGrids[dt].Items.Count; item++) {
                                    modelBindingDataNVP = (ModelBindingDataNVP)dataGrids[dt].Items[item];
                                    if (modelBindingDataNVP.key.ToLower().Equals(keyInput.ToLower())) {
                                        lbValidationKey.Content = ClientContants.LABEL_VALIDATION_ADD_KEY;
                                        lbValidationKey.Visibility = Visibility.Visible;
                                        if (null != btnSubmitAdd) {
                                            btnSubmitAdd.IsEnabled = false;
                                        }
                                        break;
                                    }
                                    else {
                                        lbValidationKey.Visibility = Visibility.Collapsed;
                                        if (null != btnSubmitAdd) {
                                            btnSubmitAdd.IsEnabled = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        lbValidationKey.Visibility = Visibility.Collapsed;
                        if (null != btnSubmitAdd) {
                            btnSubmitAdd.IsEnabled = true;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
