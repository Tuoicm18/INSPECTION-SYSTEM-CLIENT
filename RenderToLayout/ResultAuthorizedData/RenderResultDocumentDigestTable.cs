using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultDocumentDigestTable {

        #region VARIABLE
        private BrushConverter bc = new BrushConverter();
        private GroupBox groupBox;
        public ListView listViewDocDigest;
        private TextBlock textBlockDescription;
        private DataGrid dataGridDocDigest;
        private DataGridTextColumn dataGridBoundColumnKey;
        private DataGridTextColumn dataGridBoundColumnValue;
        #endregion

        #region RENDER

        public void renderResulDocDigest(AuthorizationElement elementDocDigest, ListView lvALl, ScrollViewer scvAll) {
            if (null != elementDocDigest) {
                //Get Header Group
                string headerGr = elementDocDigest.title;
                //Get Description
                string description = elementDocDigest.label;
                //Create Group Box
                groupBox = new GroupBox();
                groupBox.Margin = new Thickness(5, 5, 5, 10);
                groupBox.Header = headerGr;
                //Create List View
                listViewDocDigest = new ListView();
                //Create Text Block Description
                textBlockDescription = new TextBlock();
                textBlockDescription.Text = description;
                textBlockDescription.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockDescription.TextWrapping = TextWrapping.Wrap;
                //Create Data Grid
                dataGridDocDigest = new DataGrid();
                dataGridDocDigest.Background = Brushes.Black;
                dataGridDocDigest.RowBackground = (Brush)bc.ConvertFrom(ClientContants.CODE_COLOR_ROW_DATA_GRID);
                dataGridDocDigest.AlternatingRowBackground = (Brush)bc.ConvertFrom(ClientContants.CODE_COLOR_ALTERNATING_ROW_DATA_GRID);
                //Bind data to Data Grid
                DocumentDigest documentDigest = elementDocDigest.documentDigest;
                if(null != documentDigest) {
                    dataGridDocDigest.Items.Add(new ModelBindDataNVP { key = documentDigest.digestAlgo, value = documentDigest.digestValue });
                }

                //Key Cloumn
                dataGridBoundColumnKey = new DataGridTextColumn();
                styleDataGridTextColumn(dataGridBoundColumnKey, true);
                //Binding Data KEY
                dataGridBoundColumnKey.Binding = new Binding("key");
                dataGridDocDigest.Columns.Add(dataGridBoundColumnKey);

                //Value Cloumn
                dataGridBoundColumnValue = new DataGridTextColumn();
                styleDataGridTextColumn(dataGridBoundColumnValue, false);
                //Binding Data VALUE
                dataGridBoundColumnValue.Binding = new Binding("value");
                dataGridDocDigest.Columns.Add(dataGridBoundColumnValue);

                //Render
                if(!string.IsNullOrEmpty(description)) {
                    listViewDocDigest.Items.Add(textBlockDescription);
                }
                listViewDocDigest.Items.Add(dataGridDocDigest);
                groupBox.Content = listViewDocDigest;
                lvALl.Items.Add(groupBox);
                scvAll.Content = lvALl;
            }
        }

        //Style Cell Data Grid
        private void styleDataGridTextColumn(DataGridTextColumn dataGridTextColumn, bool isKey) {
            if (isKey) {
                //KEY COLUMN
                dataGridTextColumn.Header = ClientContants.HEADER_KEY_COLUMN_DOC_DIGEST;
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
                dataGridTextColumn.Header = ClientContants.HEADER_VALUE_COLUMN_DOC_DIGEST;
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
            #endregion
        }
    }
}
