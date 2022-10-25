using log4net;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ClientInspectionSystem.RenderToLayout.ResultAuthorizedData {
    public class RenderResultDocumentDigest {

        #region VARIABLE
        private readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private GroupBox groupBoxDocDigest;
        public ListView listViewDocDigest;
        private TextBlock textBlocDescription;
        private TextBlock textBlockHashAlgo;
        private TextBlock textBlockHashValue;
        #endregion

        #region RENDER
        public void renderResultDocumentDigest(AuthorizationElement elementDocumentDigest, ListView lvAll, ScrollViewer scvAll) {
            if(null != elementDocumentDigest) {
                //Get Header Group
                string headerGr = elementDocumentDigest.title;
                //Get Description
                string description = elementDocumentDigest.label;
                if(!string.IsNullOrEmpty(description)) {
                    textBlocDescription = new TextBlock();
                    textBlocDescription.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                    textBlocDescription.TextWrapping = TextWrapping.Wrap;
                    textBlocDescription.Text = description;
                    listViewDocDigest.Items.Add(textBlocDescription);
                }
                //Create Group Box
                groupBoxDocDigest = new GroupBox();
                groupBoxDocDigest.Margin = new System.Windows.Thickness(5, 5, 5, 10);
                groupBoxDocDigest.Header = headerGr;
                //Create List View
                listViewDocDigest = new ListView();
                //Text block
                textBlockHashAlgo = new TextBlock();
                textBlockHashAlgo.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockHashAlgo.TextWrapping = TextWrapping.Wrap;

                textBlockHashValue = new TextBlock();
                textBlockHashValue.MaxWidth = ClientContants.MAX_WIDTH_TEXT_BLOCK;
                textBlockHashValue.TextWrapping = TextWrapping.Wrap;

                //Get Text Hash Algo
                DocumentDigest documentDigest = elementDocumentDigest.documentDigest;
                if(null != documentDigest) {
                    string hashAlgo = "HASH ALGORITHM\n" + documentDigest.digestAlgo;
                    string hashValue = "HASH VALUE\n" + documentDigest.digestValue;
                    textBlockHashAlgo.Text = hashAlgo;
                    textBlockHashValue.Text = hashValue;
                } else {
                    logger.Warn("DOCUMENT DIGEST RESPONSE IS NULL");
                }

                //Render To Layout
                listViewDocDigest.Items.Add(textBlockHashAlgo);
                listViewDocDigest.Items.Add(textBlockHashValue);
                groupBoxDocDigest.Content = listViewDocDigest;
                lvAll.Items.Add(groupBoxDocDigest);
                scvAll.Content = lvAll;
            }
        }
        #endregion
    }
}
