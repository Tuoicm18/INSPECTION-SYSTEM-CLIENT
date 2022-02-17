using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ClientInspectionSystem.RenderToLayout {
    public class RenderNameValuePairs {
        #region VARIABLE
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
        #endregion

        #region RENDER
        public void renderNVP(string keyContent, string valueContent,
                              string headerGroup, string description,
                              ListView lvAll, ScrollViewer scvAll) {
            try {
                bool checkDataGridSame = checkDataGridSameGroup(groupBoxesNVP, headerGroup);

                if (checkDataGridSame) {
                    bool checkSameDescription = checkDescriptionSameGroup(descriptionList, description);
                    if(checkSameDescription) {
                        dataGridNameValuePair.Items.Add(new ModelBindingDataNVP { key = keyContent, value = "    " + valueContent });
                        if(null != listViewNVP.Items) {
                            listViewNVP.Items.Remove(dataGridNameValuePair);
                        }
                        listViewNVP.Items.Add(dataGridNameValuePair);
                        //Add to List for get data
                        groupBoxesNVP.Add(groupBoxNameValuePair);
                        descriptionList.Add(description);
                    } else {
                        //Data Grid NVP
                        dataGridNameValuePair = new DataGrid();
                        dataGridNameValuePair.Background = Brushes.Black;
                        dataGridNameValuePair.RowBackground = (Brush)bc.ConvertFrom("#111111");
                        dataGridNameValuePair.AlternatingRowBackground = (Brush)bc.ConvertFrom("#282828");
                        dataGridNameValuePair.MaxWidth = ClientContants.DATA_GRID_MAX_WIDTH;
                        dataGridNameValuePair.Items.Add(new ModelBindingDataNVP { key = keyContent, value = "    " + valueContent });
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
                        //Textblock Description
                        textBlockDescriptionNVP = new TextBlock();
                        textBlockDescriptionNVP.TextWrapping = TextWrapping.Wrap;
                        textBlockDescriptionNVP.MaxWidth = ClientContants.TEXT_BLOCK_DESCRIPTION_MAX_WIDTH;
                        textBlockDescriptionNVP.Text = description;
                        //Add to List for get data
                        groupBoxesNVP.Add(groupBoxNameValuePair);
                        descriptionList.Add(description);
                        //Render
                        listViewNVP.Items.Add(textBlockDescriptionNVP);
                        listViewNVP.Items.Add(dataGridNameValuePair);
                        groupBoxNameValuePair.Content = listViewNVP;
                    }
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
                    dataGridNameValuePair.Background = Brushes.Black;
                    dataGridNameValuePair.RowBackground = (Brush)bc.ConvertFrom("#111111");
                    dataGridNameValuePair.AlternatingRowBackground = (Brush)bc.ConvertFrom("#282828");
                    dataGridNameValuePair.MaxWidth = ClientContants.DATA_GRID_MAX_WIDTH;
                    dataGridNameValuePair.Items.Add(new ModelBindingDataNVP { key = keyContent, value = "    " + valueContent });
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
                }
                if (null != lvAll.Items) {
                    lvAll.Items.Remove(groupBoxNameValuePair);
                }
                lvAll.Items.Add(groupBoxNameValuePair);
                scvAll.Content = lvAll;
            }
            catch (Exception eNVP) {
                Logmanager.Instance.writeLog("RENDER LAYOUT NVP ERROR " + eNVP.ToString());
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
    }
}
