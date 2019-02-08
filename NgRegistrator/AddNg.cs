using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using MST.MES;

namespace NgRegistrator
{
    public partial class AddNg : Form
    {
        string[] ngButtons = new string[] { "ngBrakLutowia", "ngBrakDiodyLed", "ngBrakResConn", "ngPrzesuniecieLed", "ngPrzesuniecieResConn", "ngZabrudzenieLed", "ngUszkodzenieMechaniczneLed", "ngUszkodzenieConn", "ngWadaFabrycznaDiody", "ngUszkodzonePcb", "ngWadaNaklejki", "ngSpalonyConn", "ngInne" };
        string[] scrapButtons = new string[] { "scrapBrakLutowia", "scrapBrakDiodyLed", "scrapBrakResConn", "scrapPrzesuniecieLed", "scrapPrzesuniecieResConn", "scrapZabrudzenieLed", "scrapUszkodzenieMechaniczneLed", "scrapUszkodzenieConn", "scrapWadaFabrycznaDiody", "scrapUszkodzonePcb", "scrapWadaNaklejki", "scrapSpalonyConn", "scrapInne" };
        string selectedButton = "";
        Button previousButton = null;
        List<Image> ImagesList = new List<Image>();
        private readonly string deviceMonikerString;

        public AddNg(string deviceMonikerString)
        {
            InitializeComponent();
            foreach (var ng in ngButtons)
            {
                Button ngButton = new Button();
                ngButton.Text = ng;
                ngButton.Tag = ng.Replace("ng", "");
                ngButton.BackColor = Color.Red;
                ngButton.ForeColor = Color.White;
                ngButton.Size = new Size(flowNgPanel.Width, 40);

                flowNgPanel.Controls.Add(ngButton);
                this.TopMost = true;
                ngButton.MouseClick += Button_MouseClick;
            }

            foreach (var scrap in scrapButtons)
            {
                Button scrapButton = new Button();
                scrapButton.Text = scrap;
                scrapButton.Tag = scrap.Replace("scrap", "");
                scrapButton.BackColor = Color.Black;
                scrapButton.ForeColor = Color.White;
                scrapButton.Size = new Size(flowScrapPanel.Width, 40);
                scrapButton.MouseClick += Button_MouseClick;

                flowScrapPanel.Controls.Add(scrapButton);
            }

            this.deviceMonikerString = deviceMonikerString;
        }

        private void Button_MouseClick(object sender, MouseEventArgs e)
        {
            if (previousButton != null)
            {
                previousButton.ForeColor = Color.White;
                if (previousButton.Text.ToUpper().StartsWith("NG"))
                {
                    previousButton.BackColor = Color.Red;
                }
                else
                {
                    previousButton.BackColor = Color.Black;
                }
            }

            Button but = (Button)sender;
            but.BackColor = Color.White;
            but.ForeColor = Color.Black;
            selectedButton = but.Text;
            previousButton = but;

            foreach (var img in ImagesList)
            {
                if (img.Tag.ToString() == but.Tag.ToString())
                {
                    pictureBox1.Image = img;
                }
            }
        }

        private void AddNg_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
            //this.TopMost = true;

            string dirString = @"Zdjecia";
            if (Directory.Exists(@"Y:\APPS\Produkcja LGI\Kontrola Wzrokowa Karta Pracy 2.0\Zdjecia")) dirString= @"Y:\APPS\Produkcja LGI\Kontrola Wzrokowa Karta Pracy 2.0\Zdjecia";

            DirectoryInfo dir = new DirectoryInfo(dirString);
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles();
                foreach (var file in files)
                {
                    if (file.Extension.ToUpper() == ".PNG")
                    {
                        Image img = Image.FromFile(file.FullName);
                        img.Tag = file.Name.Split('.')[0];
                        ImagesList.Add(img);
                    }
                }
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            this.ActiveControl = textBox1;
        }

        internal static string ShortenPcbSerial(string inputId)
        {
            if (!inputId.Contains("_")) return inputId;
            if (inputId.Length <= 50) return inputId;

            string[] split = inputId.Split('_');
            return $"{split[split.Length - 2]}_{split[split.Length - 1]}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (selectedButton != "") 
            {
                if (textBox1.Text.Trim()!="")
                {
                    if (textBox1.Text.Split('_').Length == 3)
                    {
                        string result = "NG";
                        if (!selectedButton.StartsWith("ng")) { result = "SCR"; }
                        string serial = ShortenPcbSerial(textBox1.Text);
                        //SqlOperations.RegisterNgPcbToMes(serial, selectedButton); old
                        if (!SqlOperations.CheckIfSerialIsInNgTable(serial))
                        {
                            using (PictureForm camForm = new PictureForm(deviceMonikerString, selectedButton))
                            {
                                if (camForm.ShowDialog() == DialogResult.OK)
                                {
                                    var ngPicList = camForm.ngPicturesList;
                                    string orderNo = "";
                                    string[] serialSplitted = serial.Split('_');
                                    if (serialSplitted.Length == 3)
                                    {
                                        orderNo = serialSplitted[1];
                                    }
                                    SavePics.SaveImagesToFiles(ngPicList, DateTime.Now.ToString("dd-MM-yyyy"), orderNo, serial);
                                    SqlOperations.InsertPcbToNgTable(serial, result, selectedButton, orderNo);
                                }
                            }

                        }
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(textBox1.Text + Environment.NewLine + "Nieprawidłowy format kodu QR");
                        textBox1.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Zeskanuj PCB.");
                }
            }
            else
            {
                MessageBox.Show("Wybierz rodzaj wady.");
            }
        }
    }
}
