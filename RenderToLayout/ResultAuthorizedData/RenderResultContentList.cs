using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PluginICAOClientSDK.Request;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultContentList {
        #region VARIABLE
        private GroupBox groupBoxContentList;
        public ListView listViewContentList;
        private TextBlock textBlockDescription;
        private TextBlock textBlockContent;
        #endregion

        #region RENDER
        public void renderResultContentList(AuthorizationElement elementContentList, ListView lvAll, ScrollViewer scvAll) {
            if(null != elementContentList) {
                //Get Header Group
                string headerGr = elementContentList.title;
                //Get Description
                string description = elementContentList.label;
                //Get Text Content
                string textContent = elementContentList.text;
                //Create Group Box
                groupBoxContentList = new GroupBox();
                groupBoxContentList.Margin = new System.Windows.Thickness(5, 5, 5, 10);
                groupBoxContentList.Header = headerGr;
                //Create List View
                listViewContentList = new ListView();
                //Create Text Block Description
                textBlockDescription = new TextBlock();
                textBlockDescription.Text = description;
                textBlockDescription.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockDescription.TextWrapping = TextWrapping.Wrap;
                //Create Text Block Description
                textBlockContent = new TextBlock();
                textBlockContent.Text = textContent;
                textBlockContent.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockContent.TextWrapping = TextWrapping.Wrap;
                //Render To Layout
                listViewContentList.Items.Add(textBlockDescription);
                listViewContentList.Items.Add(textBlockContent);
                groupBoxContentList.Content = listViewContentList;
                lvAll.Items.Add(groupBoxContentList);
                scvAll.Content = lvAll;
            }
        }
        #endregion
    }
}
