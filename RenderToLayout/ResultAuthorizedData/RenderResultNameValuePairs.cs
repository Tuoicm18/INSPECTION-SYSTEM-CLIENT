using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultNameValuePairs {
        #region VARIABLE
        private BrushConverter bc = new BrushConverter();
        private GroupBox groupBoxNVP;
        public ListView listViewNVP;
        private TextBlock textBlockDescriptionNVP;
        private DataGrid dataGridNVP;
        private DataGridTextColumn dataGridBoundColumnKey;
        private DataGridTextColumn dataGridBoundColumnValue;
        #endregion

        #region RENDER
        public void renderResultNVP(AuthorizationElement elementNVP, ListView lvALl, ScrollViewer scvAll) {
            if(null != elementNVP) {
                //Get Header Group
                string headerGr = elementNVP.title;
                //Get Description
                string description = elementNVP.label;
                //Create Group Box
                groupBoxNVP = new GroupBox();
                groupBoxNVP.Margin = new Thickness(5, 5, 5, 10);
                groupBoxNVP.Header = headerGr;
                //Create List View
                listViewNVP = new ListView();
                //Create Text Block Description
                textBlockDescriptionNVP = new TextBlock();
                textBlockDescriptionNVP.Text = description;
                textBlockDescriptionNVP.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockDescriptionNVP.TextWrapping = TextWrapping.Wrap;
                //Create Data Grid
                dataGridNVP = new DataGrid();
                dataGridNVP.Background = Brushes.Black;
                dataGridNVP.RowBackground = (Brush)bc.ConvertFrom(ClientContants.CODE_COLOR_ROW_DATA_GRID);
                dataGridNVP.AlternatingRowBackground = (Brush)bc.ConvertFrom(ClientContants.CODE_COLOR_ALTERNATING_ROW_DATA_GRID);
                //Bind data to Data Grid
                Dictionary<string, string> nvpRequest;
                if (null != elementNVP.nameValuePair && elementNVP.nameValuePair.Count > 0) {
                    nvpRequest = elementNVP.nameValuePair;
                    foreach (var kv in nvpRequest) {
                        dataGridNVP.Items.Add(new ModelBindDataNVP { key = kv.Key, value = kv.Value });
                    }
                }
                //Key Cloumn
                dataGridBoundColumnKey = new DataGridTextColumn();
                styleDataGridTextColumn(dataGridBoundColumnKey, true);
                //Binding Data KEY
                dataGridBoundColumnKey.Binding = new Binding("key");
                dataGridNVP.Columns.Add(dataGridBoundColumnKey);

                //Value Cloumn
                dataGridBoundColumnValue = new DataGridTextColumn();
                styleDataGridTextColumn(dataGridBoundColumnValue, false);
                //Binding Data VALUE
                dataGridBoundColumnValue.Binding = new Binding("value");
                dataGridNVP.Columns.Add(dataGridBoundColumnValue);

                //Render
                listViewNVP.Items.Add(textBlockDescriptionNVP);
                listViewNVP.Items.Add(dataGridNVP);
                groupBoxNVP.Content = listViewNVP;
                lvALl.Items.Add(groupBoxNVP);
                scvAll.Content = lvALl;
            }
        }

        //Style Cell Data Grid
        private void styleDataGridTextColumn(DataGridTextColumn dataGridTextColumn, bool isKey) {
            if (isKey) {
                //KEY COLUMN
                dataGridTextColumn.Header = ClientContants.HEADER_KEY_COLUMN;
                dataGridTextColumn.CanUserResize = false;
                dataGridTextColumn.Width = ClientContants.WIDTH_KEY_COLUMN;
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
                dataGridTextColumn.Header = ClientContants.HEADER_VALUE_COLUMN;
                dataGridTextColumn.CanUserResize = false;
                dataGridTextColumn.FontWeight = FontWeights.Bold;
                dataGridTextColumn.Width = ClientContants.WIDTH_VALUE_COLUMN;
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
