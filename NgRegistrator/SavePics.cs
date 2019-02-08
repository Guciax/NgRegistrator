using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace NgRegistrator
{
    public class SavePics
    {
        private static void ConnectPDrive()
        {
            Process myProcess = new Process();
            myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess.StartInfo.CreateNoWindow = true;
            myProcess.StartInfo.UseShellExecute = false;
            myProcess.StartInfo.FileName = "cmd.exe";
            myProcess.StartInfo.Arguments = @"/c net use P: \\mstms005\shared /user:eprod plfm!234 /PERSISTENT:NO";
            myProcess.EnableRaisingEvents = true;
            myProcess.Start();
            myProcess.WaitForExit();
        }

        internal static void SaveImagesToFiles(List<Image> imgList, string date, string orderNo, string serialNo)
        {
            var imgFolderPath = Path.Combine(@"P:\Kontrola_Wzrokowa", date, orderNo);
            if (Path.GetPathRoot(imgFolderPath).StartsWith("P"))
            {
                if (!Directory.Exists("P:\\"))
                {
                    ConnectPDrive();
                }
            }
            if (!Directory.Exists(imgFolderPath))
            {
                System.IO.Directory.CreateDirectory(imgFolderPath);
            }

            for (int i = 0; i < imgList.Count; i++)
            {
                var saveBmp = imgList[i];
                var ngReason = saveBmp.Tag.ToString();

                saveBmp.Save($@"{imgFolderPath}\{serialNo}_{ngReason}_{i}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
