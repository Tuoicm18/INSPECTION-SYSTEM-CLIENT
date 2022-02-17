using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientInspectionSystem.RenderToLayout.Models {
    public class NameValuePairsModel {
        public int ordinaryNVP { get; set; }
        public TextBlock description { get; set; }
        public string title { get; set; }
        public DataGrid dataGrid { get; set; }
        public string keyData { get; set; }
        public string valueData { get; set; }
        public GroupBox groupBox { get; set; }
    }
}
