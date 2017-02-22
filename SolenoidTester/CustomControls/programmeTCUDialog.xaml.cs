using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SolenoidTester.CustomControls
{

    public delegate void FlashTcuDlgHide();

    public delegate void GetTcuPartNumber(string srt);
    public delegate void FlashTCU(string str);
    public delegate void SendFlashTcuData0(string[] str);
    public delegate void SendFlashTcuData1(string[] str);
    public delegate void SendFlashTcuData2(string[] str);
    public delegate void SendFlashTcuData3(string[] str);
    public delegate void SendFlashTcuData4(string[] str);
    /// <summary>
    /// Interaction logic for programmeTCUDialog.xaml
    /// </summary>
    public partial class programmeTCUDialog : ModernDialog
    {

        public event FlashTcuDlgHide FTDlgHideEvent;
        public event GetTcuPartNumber GetPartNumber;
        public event FlashTCU Flashtcu;
        public event SendFlashTcuData0 SendFlsahData0;
        public event SendFlashTcuData1 SendFlsahData1;
        public event SendFlashTcuData2 SendFlsahData2;
        public event SendFlashTcuData3 SendFlsahData3;
        public event SendFlashTcuData4 SendFlsahData4;
        private delegate void outputDelegate(double precent);
        public System.Timers.Timer progreTimer = new System.Timers.Timer(500); //实例化Timer类，设置间隔时间为10000毫秒；
        /// <summary>
        /// 解压路径
        /// </summary>
        private string UnZipPath = "";
        private string[] UnZipFileName;
        private string flashType = "";
        private DateTime flashBar = DateTime.Now;
        private int flashSecond = 0;
        private bool startTimerFlag = false;
        public programmeTCUDialog()
        {
            InitializeComponent();
            
            btn_FlashTcu.Visibility = Visibility.Visible;
            btn_FlashTcu.IsEnabled = false;
            btn_Flashing.Visibility = Visibility.Collapsed;
            this.Buttons = new Button[] { this.CancelButton };
            this.CancelButton.Content = "退出";
            this.CancelButton.Visibility = System.Windows.Visibility.Collapsed;
        }
        //固件文件
        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
   //         Directory.GetCurrentDirectory() + "/zip/AccessFilmName.txt";
            //var targetPath = @"E:\C#\UnZip\ZipFile";
            var targetPath = @"C:\TCUFirmWare\ZipFile\";
            CleanFiles(targetPath);
            string TargetPath = @"C:\TCUFirmWare\ZipFile\";
            string FilePath;
            //File.Delete(TargetPath);//直接删除其中的文件
            OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "zip文件|*.zip";
            openFileDialog.InitialDirectory = @"C:\TCUFirmWare\";;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                UnZipPath = openFileDialog.FileName;
                TextFileName.Text = UnZipPath;
                try
                {
                    ZipFile.ExtractToDirectory(UnZipPath, TargetPath);
                    btn_FlashTcu.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            /*// Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                TextFileName.Text = filename;
            }*/
        }


        /// <summary>
        /// 解析刷写内容
        /// </summary>
        /// <param name="data">刷写文件名</param>
        private void AnalyFlashData(string path)
        {
            string[] strArrData = new string[] { };
            string[] strArrData0 = new string[] { };
            string[] strArrData1 = new string[] { };
            string[] strArrData2 = new string[] { };
            string[] strArrData3 = new string[] { };
            string[] strArrData4 = new string[] { };
            if (UnZipFileName != null)
            {
                UnZipFileName = null;
            }
            int flashFileNum = 0;
            int startPostion = 0;
            int whereTbl = 1;
            byte[] binchar;
            string TargetPath = @"C:\TCUFirmWare\ZipFile\";
            string[] flashFileName = new string[5] { "", "", "", "", "" };
            if (path.Length == 12)
            {
                if (path.Substring(8, 4).ToUpper() == "BOOT"|| path.Substring(8, 4).ToUpper() == "1905")
                {
                    flashType = "boot";
                    flashFileNum = 2;
                    UnZipFileName = Directory.GetFiles(TargetPath);
                    for (int i = 0; i < UnZipFileName.Length; i++)
                    {
                        if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() == "FIL")
                        {
                            flashFileName[0] = UnZipFileName[i];
                        }
                        if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() == "BIN")
                        {
                            flashFileName[1] = UnZipFileName[i];
                        }

                    }
                    startPostion = 248;
                }

            }
            if (path.Length == 13)
            {
                if (path.Substring(8, 5).ToUpper() == "RESET")
                {
                    flashType = "reset";
                    flashFileNum = 1;
                    UnZipFileName = Directory.GetFiles(TargetPath);
                    for (int i = 0; i < UnZipFileName.Length; i++)
                    {
                        if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() == "FIL")
                        {
                            flashFileName[0] = UnZipFileName[i];
                        }
                    }
                    startPostion = 168;
                }

            }
            if (path.Length == 19)
            {
                if (path.Substring(11,4).ToUpper()=="ZERO")
                {
                    flashType = "zero";
                    flashFileNum = 1;
                    UnZipFileName = Directory.GetFiles(TargetPath);
                    for (int i = 0; i < UnZipFileName.Length; i++)
                    {
                        if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() == "TBL")
                        {
                            flashFileName[0] = UnZipFileName[i];
                        }
                    }
                }
            }
            if (path.Length == 8)
            {
                flashType = "main";
                flashFileNum = 5;
                UnZipFileName = Directory.GetFiles(TargetPath);

                for (int i = 0; i < UnZipFileName.Length - 1; i++)
                {
                    if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() != "TBL")
                    {
                        flashFileName[whereTbl] = UnZipFileName[i];
                        whereTbl++;
                    }
                    if (UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 3, 3).ToUpper() == "TBL")
                    {
                        GetPartNumber(UnZipFileName[i].ToString().Substring(UnZipFileName[i].Length - 12, 8));
                    }
                }
                /*if (UnZipFileName[0].ToString().Substring(UnZipFileName[0].Length - 3, 3) == "tbl")
                {
                    flashFileName[1] = UnZipFileName[1];
                    flashFileName[2] = UnZipFileName[2];
                    flashFileName[3] = UnZipFileName[3];
                    flashFileName[4] = UnZipFileName[4];
                }
                else
                {
                    flashFileName[1] = UnZipFileName[0];
                    flashFileName[2] = UnZipFileName[1];
                    flashFileName[3] = UnZipFileName[2];
                    flashFileName[4] = UnZipFileName[3];
                }*/

                flashFileName[0] = UnZipFileName[5];

                startPostion = 495;
            }
            for (int flashnum = 0; flashnum < flashFileNum; flashnum++)
            {
                FileStream Myfile1 = new FileStream(flashFileName[flashnum], FileMode.Open, FileAccess.Read);
                BinaryReader binreader = new BinaryReader(Myfile1);
                var fileLen = (int)Myfile1.Length; //获取bin文件长度
                if (flashnum > 0)
                {
                    startPostion = 0;
                }
                if (fileLen > 0)
                {
                    var arrStr = new string[fileLen];
                    strArrData = new string[fileLen - startPostion];


                    binchar = binreader.ReadBytes(fileLen);
                    for (int i = 0; i < fileLen; i++)
                    {
                        arrStr[i] = binchar[i].ToString("X2");
                    }
                    for (int j = 0; j < fileLen - startPostion; j++)
                    {
                        strArrData[j] = arrStr[j + startPostion];
                    }
                }
                binreader.Close();
                switch (flashnum)
                {
                    case 0:
                        strArrData0 = new string[fileLen - startPostion];
                        strArrData0 = strArrData;
                        break;
                    case 1:
                        strArrData1 = new string[fileLen];
                        strArrData1 = strArrData;
                        break;
                    case 2:
                        strArrData2 = new string[fileLen];
                        strArrData2 = strArrData;
                        break;
                    case 3:
                        strArrData3 = new string[fileLen];
                        strArrData3 = strArrData;
                        break;
                    case 4:
                        strArrData4 = new string[fileLen];
                        strArrData4 = strArrData;
                        break;
                }
                fileLen = 0;
            }
            SendFlashData(strArrData0, strArrData1, strArrData2, strArrData3, strArrData4);
        }

        /// <summary>
        /// 发送刷写内容
        /// </summary>
        /// <param name="data0"></param>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        /// <param name="data3"></param>
        /// <param name="data4"></param>
        private async void SendFlashData(string[] data0, string[] data1, string[] data2, string[] data3, string[] data4)
        {
            
            if (data0.Length > 0)
            {
                SendFlsahData0(data0);
            }
            if (data1.Length > 0)
            {
                SendFlsahData1(data1);
            }
            if (data2.Length > 0)
            {
                SendFlsahData2(data2);
            }
            if (data3.Length > 0)
            {
                SendFlsahData3(data3);
            }
            if (data4.Length > 0)
            {
                SendFlsahData4(data4);
            }
            Flashtcu?.Invoke(flashType);
            if (flashType=="main")
            {
                flashSecond = 355;
            }
            if (flashType == "boot")
            {
                flashSecond = 22;
            }
            if (flashType == "reset")
            {
                flashSecond = 6;
            }
            if (flashType == "zero")
            {
                flashSecond = 2;
            }
            if (flashType == "main")
            {
                btn_DlgHide.IsEnabled = false;
            }
            outputprogressbar(100);
            startTimerFlag = false;
            flashBar = DateTime.Now.AddSeconds(flashSecond);

            progreTimer.Elapsed += new System.Timers.ElapsedEventHandler(FlashProgressbar); //到达时间的时候执行事件；
            progreTimer.AutoReset = true; //设置是执行一次（false）还是一直执行(true)；
            progreTimer.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
        }

        private TimeSpan flashTime;
        private void FlashProgressbar(object source, System.Timers.ElapsedEventArgs e)
        {
            //var denominator = int.Parse(flashBar.ToString("m"))*60+int.Parse(flashBar.ToString("s"));
            var denominator = float.Parse(flashBar.Minute.ToString())*60+ float.Parse(flashBar.Second.ToString());
            if (DateTime.Now < flashBar)
            {
                var numerator = float.Parse(DateTime.Now.Minute.ToString()) * 60 + float.Parse(DateTime.Now.Second.ToString());
                if (!startTimerFlag)
                {
                    startTimerFlag = true;
                    outputprogressbar(100);
                }
                else
                {
                    flashTime = flashBar - DateTime.Now;
                    outputprogressbar((flashTime.TotalSeconds/ flashSecond)*100);
                }
            }
            else
            {
                startTimerFlag = false;
                progreTimer.Elapsed -= new System.Timers.ElapsedEventHandler(FlashProgressbar);
                progreTimer.Stop();
                Thread.Sleep(10);
                flashSecond = 0;
                outputprogressbar(0);
            }
        }

        private void outputprogressbar(double msg)
        {
            progressbar.Dispatcher.Invoke(new outputDelegate(progressbarAction), msg);
        }

        private void progressbarAction(double msg)
        {
            progressbar.Value = 100-msg;
        }
        private static void CleanFiles(string dir)

        {
            if (!Directory.Exists(dir))
            {
                File.Delete(dir);
                return;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(dir);
                string[] files = Directory.GetFiles(dir);

                if (0 != dirs.Length)
                {
                    foreach (string subDir in dirs)
                    {
                        if (Directory.GetFiles(subDir) == null)
                        {
                            Directory.Delete(subDir);
                            return;
                        }
                        else CleanFiles(subDir);
                    }
                }
                if (0 != files.Length)
                {
                    foreach (string file in files)

                    {
                        File.Delete(file);
                    }
                }
                else Directory.Delete(dir);
            }
        }

        private void btn_DlgHide_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            progreTimer.Stop();
            progressbar.Value = 0;
            btn_FlashTcu.Visibility = Visibility.Visible;
            btn_Flashing.Visibility = Visibility.Collapsed;
            btn_ChooseFlashFile.IsEnabled = true;
            btn_FlashTcu.IsEnabled = false;
            TextFileName.Text = "";
            FTDlgHideEvent?.Invoke();
            ((App)Application.Current).dataPro.StopFlashTcu();

        }

        private void btn_FlashTcu_Click(object sender, RoutedEventArgs e)
        {
            string TargetPath = @"C:\TCUFirmWare\ZipFile\";
            string strFileName;
            btn_FlashTcu.Visibility = Visibility.Collapsed;
            btn_Flashing.Visibility = Visibility.Visible;
            btn_Flashing.IsEnabled = false;
            btn_ChooseFlashFile.IsEnabled = false;
            
            strFileName = UnZipPath.Substring(15, UnZipPath.Length - 19);
            AnalyFlashData(strFileName);
        }
    }
}
