using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NgRegistrator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
        }
        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            this.TopLevel = true;
            this.TopMost = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FilterInfoCollection CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            List<string> devices = new List<string>();
            foreach (FilterInfo dev in CaptureDevice)
            {
                devices.Add(dev.MonikerString);
            }
            if (devices.Count > 0)
            {
                using (AddNg addForm = new AddNg(devices[0]))
                {
                    addForm.ShowDialog();
                }
                    
            }
            else
            {
                MessageBox.Show("Kamera nie podłączona!");
            }
        }
    }
}
