using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientInspectionSystem {
    public partial class FormViewPDF : Form {
        public string fileNamePDF = string.Empty;
        public FormViewPDF() {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.BackColor = System.Drawing.Color.MistyRose;
            Graphics g = this.CreateGraphics();
            Double startingPoint = (this.Width / 2) - (g.MeasureString(this.Text.Trim(), this.Font).Width / 2);
            Double widthOfASpace = g.MeasureString(" ", this.Font).Width;
            String tmp = " ";
            Double tmpWidth = 0;

            while ((tmpWidth + widthOfASpace) < startingPoint) {
                tmp += " ";
                tmpWidth += widthOfASpace;
            }

            this.Text = tmp + this.Text.Trim();
        }

        private void FormViewPDF_FormClosed(object sender, FormClosedEventArgs e) {
            if(File.Exists(fileNamePDF)) {
                File.Delete(fileNamePDF);
            }
        }

        public void loadPDF() {
            axAcroPDF1.src = fileNamePDF;
        }
    }
}
