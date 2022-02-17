using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientInspectionSystem.RenderToLayout.Models {
    public class SingleSelectModel {
        public int ordinarySingle { get; set; }
        public TextBlock description { get; set; }
        public string  title { get; set; }
        public RadioButton radioButton { get; set; }
        public GroupBox groupBox { get; set; }
    }
}
