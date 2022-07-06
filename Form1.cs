using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientInspectionSystem {
    public partial class Form1 : Form {


        public Form1() {
            InitializeComponent();
            axAcroPDF1.src = @"D:\ICAO\Scanner.pdf";
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
    }
}
