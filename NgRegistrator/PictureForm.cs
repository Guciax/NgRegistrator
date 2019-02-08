using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace NgRegistrator
{
    public partial class PictureForm : Form
    {
        private readonly string deviceMonikerString;
        private readonly string buttonPressed;
        FilterInfoCollection CaptureDevice;
        VideoCaptureDevice FinalFrame;
        Bitmap bitmap;
        public List<Image> ngPicturesList = new List<Image>();
        private bool finalFramePause = false;

        public PictureForm(string DeviceMonikerString, string buttonPressed)
        {
            InitializeComponent();
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalFrame = new VideoCaptureDevice();
            FinalFrame = new VideoCaptureDevice(DeviceMonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.NewFrame -= Handle_New_Frame;
            Thread.Sleep(1000);
            FinalFrame.Start();
            this.buttonPressed = buttonPressed;
        }

        private class MyPicBox: PictureBox
        {
            private Image _storedImg;
            private int _idx;
            public Image storedImage
            {
                get { return _storedImg; }
                set { _storedImg = value; }
            }
            public int idx
            {
                get { return _idx; }
                set { _idx = value; }
            }
        }

        private void Handle_New_Frame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!finalFramePause)
            {
                if (bitmap != null)
                    bitmap.Dispose();
                bitmap = new Bitmap(eventArgs.Frame);

                if (pictureBox1.Image != null)
                    this.Invoke(new MethodInvoker(delegate () { pictureBox1.Image.Dispose(); }));
                pictureBox1.Image = bitmap;
            }
        }

        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!finalFramePause)
            {
                bitmap = (Bitmap)eventArgs.Frame.Clone();
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                pictureBox1.Image = bitmap;
            }
        }

        private void PictureForm_Load(object sender, EventArgs e)
        {
            this.BringToFront();
        }

        private void PictureForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FinalFrame.Stop();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count < 11)
            {
                Image img = pictureBox1.Image;
                img.Tag = buttonPressed;

                MyPicBox picBx = new MyPicBox();
                picBx.idx = flowLayoutPanel1.Controls.Count;
                picBx.Name = flowLayoutPanel1.Controls.Count.ToString();
                picBx.Margin = new Padding(2);
                picBx.Height = flowLayoutPanel1.Height - 4;
                picBx.Width = img.Width / img.Height * picBx.Height;
                picBx.Image = img;
                picBx.storedImage = img;
                picBx.SizeMode = PictureBoxSizeMode.Zoom;
                picBx.MouseEnter += picBx_MouseEnter;
                picBx.MouseLeave += picBx_MouseLeave;
                picBx.MouseClick += picBx_MouseClick;
                picBx.BorderStyle = BorderStyle.FixedSingle;

                flowLayoutPanel1.Controls.Add(picBx);
                button1.Visible = true;
            }
            else
            {
                MessageBox.Show("Max 10 zdjęć. Skasuj zdjęcia aby dodać nowe.");
            }
        }

        private void picBx_MouseClick(object sender, MouseEventArgs e)
        {
            MyPicBox pb = (MyPicBox)sender;
            flowLayoutPanel1.Controls.Remove(pb);
           
        }

        private void picBx_MouseLeave(object sender, EventArgs e)
        {
            finalFramePause = false;
            MyPicBox pb = (MyPicBox)sender;
            pb.Image = pb.storedImage;
        }

        private void picBx_MouseEnter(object sender, EventArgs e)
        {
            finalFramePause = true;
            MyPicBox pb = (MyPicBox)sender;
            pb.Image = NgRegistrator.Properties.Resources.delete;
            pictureBox1.Image = pb.storedImage;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var pb in flowLayoutPanel1.Controls)
            {
                MyPicBox pBox = (MyPicBox)pb;
                ngPicturesList.Add(pBox.storedImage);
            }
            this.DialogResult = DialogResult.OK;
        }

        private void flowLayoutPanel1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count > 0)
            {
                button1.Enabled = true;
            }
        }

        private void flowLayoutPanel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (flowLayoutPanel1.Controls.Count == 0)
            {
                button1.Enabled = false;
            }
        }
    }
}
