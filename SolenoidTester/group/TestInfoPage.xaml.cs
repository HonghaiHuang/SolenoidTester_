using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Collections;//ArrayList
using System.IO;
using System.Threading;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using SolenoidTester.CustomControls;
using System.Data;
using SolenoidTester.DataClass;

namespace SolenoidTester.group
{
    /// <summary>
    /// Interaction logic for TestInfoPage.xaml
    /// </summary>
    public partial class TestInfoPage : UserControl, IContent
    {
        //DataProcess ((App)Application.Current).datapro;
        ///    AccessCom accesscom = new AccessCom();
        ///    

        /// <summary>
        /// 存储故障码
        /// </summary>
        string[] ShowFault;

        /// <summary>
        /// 存储测试信息
        /// </summary>
        private string[] _TestInformation = new string[9];


        private string TestTime = "";



        private delegate void outputDelegate(string msg);

        private delegate void ChangeBtnStats();
        programmeTCUDialog programmeDialog;
        private bool DlgOpen = false;
        public TestInfoPage()
        {
            InitializeComponent();
            InitUpdataUIdata();
            programmeDialog = new programmeTCUDialog();
        }

        private void InitUpdataUIdata()
        {
            Text_ReportNo.Text = System.DateTime.Now.ToString("yyyyMMddHHmm");
            TestTime= System.DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void getpartnumber(string str)
        {
            ((App) Application.Current).dataPro.GetPartNumber(str);
        }

        private void StratFlashTcu(string str)
        {
            ((App)Application.Current).dataPro.FlashTcu(str);
            ///       ((App) Application.Current).datapro.FlashTcu(str);
        }

        private void SendFlashData0(string[] str)
        {
                 ((App) Application.Current).dataPro.SendTcuFlashData0(str);
        }

        private void SendFlashData1(string[] str)
        {
                   ((App) Application.Current).dataPro.SendTcuFlashData1(str);
        }

        private void SendFlashData2(string[] str)
        {
                   ((App) Application.Current).dataPro.SendTcuFlashData2(str);
        }

        private void SendFlashData3(string[] str)
        {
                   ((App) Application.Current).dataPro.SendTcuFlashData3(str);
        }

        private void SendFlashData4(string[] str)
        {
                    ((App) Application.Current).dataPro.SendTcuFlashData4(str);
        }
        int kkk = 0;
        //测试配置文件
        private void fileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FlashInterrupt()
        {
            
            OutputFlashinghed();
            OutputTcuFlashed();
            OutputChooseFlashFile();
            OutputTextFileName();
            Outputprogressbar("0");
            MessageBox.Show("刷写失败，请断开TCU并重试!", "提示");
            FlashTcuDlgHided();
        }

        /// <summary>
        /// 故障码读取窗口关闭
        /// </summary>
        private void FalutCodeDlgHided()
        {
            btn_ReadFC.IsEnabled = true;
            btn_ReadFT.IsEnabled = true;
            DlgOpen = false;
        }

        private void FlashTcuDlgHided()
        {
            OutputTcuFlashed();
            OutputFlashinghed();
            OutputChooseFlashFile();
            OutputTextFileName();
            Outputprogressbar("0");
            outputbtn_ReadFT();
            outputbtn_ReadFC();
            outputbtn_ClearFC();
            outputbtn_ReadTI();
            outputbtn_FlashVIN_Click();
            DlgOpen = false;
        }

        private void outputbtn_ReadFT()
        {
            btn_ReadFT.Dispatcher.Invoke(new ChangeBtnStats(btn_ReadFTAction));
        }

        private void btn_ReadFTAction()
        {
            btn_ReadFT.IsEnabled = true;
        }

        private void outputbtn_ReadFC()
        {
            btn_ReadFC.Dispatcher.Invoke(new ChangeBtnStats(btn_ReadFCAction));
        }

        private void btn_ReadFCAction()
        {
            btn_ReadFC.IsEnabled = true;
        }

        private void outputbtn_ClearFC()
        {
            btn_CleakFC.Dispatcher.Invoke(new ChangeBtnStats(brn_ClearFCAction));
        }

        private void brn_ClearFCAction()
        {
            btn_CleakFC.IsEnabled = true;
        }

        private void outputbtn_ReadTI()
        {
            btn_ReadTI.Dispatcher.Invoke(new ChangeBtnStats(btn_ReadTIAction));
        }

        private void btn_ReadTIAction()
        {
            btn_ReadTI.IsEnabled = true;
        }
        private void outputbtn_FlashVIN_Click()
        {
            btn_FlashVIN.Dispatcher.Invoke(new ChangeBtnStats(btn_FlashVIN_ClickAction));
        }

        private void btn_FlashVIN_ClickAction()
        {
            btn_FlashVIN.IsEnabled = true;
        }

        /// <summary>
        /// 显示TCU信息
        /// </summary>
        /// <param name="TCUMsg"></param>
        private void TCUInfoMSG(string[] TCUMsg)
        {
            var noInfo = false;

            outputText_PartNumber(TCUMsg[0]);
            outputTextNoFlashingSoftware(TCUMsg[1]);
            outputTextCommercialStatus(TCUMsg[2]);
            outputTextManufacturingTrackingCode(TCUMsg[3]);
            outputTextVIN(TCUMsg[4]);
            for(int i = 0; i < TCUMsg.Length; i++)
            {
                if (TCUMsg[i] == "")
                {
                    noInfo = true;
                }
            }
            if (!noInfo)
            {
                outputbtn_ClearFC();
                outputbtn_ReadFC();
                outputbtn_ReadFT();
            }
        }

        #region 显示TCU信息到界面
        /// <summary>
        /// TextCarFrameNumber
        /// </summary>
        /// <param name="msg"></param>
        private void outputText_PartNumber(string msg)
        {
            this.Text_PartNumber.Dispatcher.Invoke(new outputDelegate(Text_PartNumberAction), msg);
        }

        private void Text_PartNumberAction(string msg)
        {
            Text_PartNumber.Text = msg;
        }
        /// <summary>
        /// TextEngineTypeAction
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextNoFlashingSoftware(string msg)
        {
            this.TextNoFlashingSoftware.Dispatcher.Invoke(new outputDelegate(TextNoFlashingSoftwareAction), msg);
        }

        private void TextNoFlashingSoftwareAction(string msg)
        {
            TextNoFlashingSoftware.Text = msg;
        }
        /// <summary>
        /// TextBasicType
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextCommercialStatus(string msg)
        {
            this.TextCommercialStatus.Dispatcher.Invoke(new outputDelegate(TextCommercialStatusAction), msg);
        }

        private void TextCommercialStatusAction(string msg)
        {
            TextCommercialStatus.Text = msg;
        }
        /// <summary>
        /// TextFinalType
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextManufacturingTrackingCode(string msg)
        {
             this.TextManufacturingTrackingCode.Dispatcher.Invoke(new outputDelegate(TextManufacturingTrackingCodeAction), msg);
        }

        private void TextManufacturingTrackingCodeAction(string msg)
        {
            TextManufacturingTrackingCode.Text = msg;
        }
        /// <summary>
        /// TextBroadcastNumber
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextVIN(string msg)
        {
             this.TextVIN.Dispatcher.Invoke(new outputDelegate(TextVINAction), msg);
        }

        private void TextVINAction(string msg)
        {
            TextVIN.Text = msg;
        }
        /// <summary>
        /// TextVersion
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextVersion(string msg)
        {
       ////     this.TextVersion.Dispatcher.Invoke(new outputDelegate(TextVersionAction), msg);
        }

        private void TextVersionAction(string msg)
        {
       ////     TextVersion.Text = msg;
        }
        /// <summary>
        /// TextDiagnosisID
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextDiagnosisID(string msg)
        {
    ////        this.TextDiagnosisID.Dispatcher.Invoke(new outputDelegate(TextDiagnosisIDAction), msg);
        }

        private void TextDiagnosisIDAction(string msg)
        {
    ////        TextDiagnosisID.Text = msg;
        }
        /// <summary>
        /// TextMPCode
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextMPCode(string msg)
        {
  ////          this.TextMPCode.Dispatcher.Invoke(new outputDelegate(TextMPCodeAction), msg);
        }

        private void TextMPCodeAction(string msg)
        {
   ///         TextMPCode.Text = msg;
        }
        /// <summary>
        /// TextMEC
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextMEC(string msg)
        {
     ///       this.TextMEC.Dispatcher.Invoke(new outputDelegate(TextMECAction), msg);
        }

        private void TextMECAction(string msg)
        {
  ///          TextMEC.Text = msg;
        }
        /// <summary>
        /// TextMEC
        /// </summary>
        /// <param name="msg"></param>
        private void outputTextMTC(string msg)
        {
   ///         this.TextMTC.Dispatcher.Invoke(new outputDelegate(TextMTCAction), msg);
        }

        private void TextMTCAction(string msg)
        {
   ///         TextMTC.Text = msg;
        }
        #endregion

        /// <summary>
        /// 初始化用户信息
        /// </summary>
        private void IniAdmin()
        {
            TextOperator.Text = GetAdmin();
            //TextSerial.Text = IDName;
        }
        private string GetAdmin()
        {
            string SavePath = Directory.GetCurrentDirectory() + "/txt文件/Admin.txt";
            string AdminName = "";
            //   StreamReader sr = new StreamReader(SavePath, Encoding.Default);

            //  StreamReader sr = new StreamReader(SavePath, Encoding.Default);
            StreamReader sr = new StreamReader(SavePath, Encoding.UTF8);
            // int[] InitPort = new int[15];
            for (int i = 0; i < 1; i++)
            {
                AdminName = @sr.ReadLine();
            }
            return AdminName;

        }
        /// <summary>
        /// TCU故障码读取与清除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void codeDialog_Click(object sender, RoutedEventArgs e)
        {
            /*if (((App)Application.Current).datapro != null)
            {
                ((App)Application.Current).datapro.SetStructAndAccessUse(false, false);
                ((App)Application.Current).datapro.DisConnectCAN();
                ((App)Application.Current).datapro.ReleaseAllObject();
                ((App)Application.Current).datapro = null;
                Thread.Sleep(600);
            }*/


            //codeDialog.ShowDialog();
            ///     codeDialog.listBoxFaultCode.Items.Clear();
            ///    codeDialog.textFaultCodeExplan.Text = "";
            DlgOpen = true;
            btn_ReadFC.IsEnabled = false;
            btn_ReadFT.IsEnabled = false;
            /*if (codeDialog.DialogResult == true)
            {
                codeDialog.Close();
            }
            if (codeDialog.DialogResult == false)
            {

            }*/
        }


        /// <summary>
        /// 读取故障码
        /// </summary>
        public void ReadFaultCode()
        {
            ///       ((App) Application.Current).datapro.GetFaultCode();
        }

        public void ClearFaultCode()
        {
            ///    ((App) Application.Current).datapro.SendDataToTcu("01 04 00 00 00 00 00 00");
        }

        private void ShowFaultCode(string[] msg)
        {
            /*string str;
            string[] ShowFaultCode;
            string FaultCodeInfo = "";
            str = msg[0];
            if (str == "无故障码" || str == "故障码已清除")
            {
                codeDialog.listBoxFaultCode.Items.Insert(0, str);
            }
            else
            {
                ShowFaultCode = new string[str.Length / 2];
                for (int i = 0; i < str.Length / 2; i++)
                {
                    ShowFaultCode[i] = str.Substring(i * 2, 1) + str.Substring(i * 2 + 1, 1);
                }
                for (int j = 0; j < ShowFaultCode.Length / 4; j++)
                {
                    FaultCodeInfo = "第" + ShowFaultCode[j * 4 + 1] + "个是：" + ShowFaultCode[j * 4 + 2] + ShowFaultCode[j * 4 + 3];
                    codeDialog.listBoxFaultCode.Items.Insert(j, FaultCodeInfo);
                }
            }*/
            outputlistBoxFaultCode(msg[0]);
        }
        private void outputtextFaultCodeExplan(string msg)
        {
            this.textFaultCodeExplan.Dispatcher.Invoke(new outputDelegate(textFaultCodeExplanAction), msg);
        }

        private void textFaultCodeExplanAction(string msg)
        {
            this.textFaultCodeExplan.Text += msg + "\r\n";
        }

        /// <summary>
        /// listBoxFaultCode
        /// </summary>
        /// <param name="msg"></param>
        private void outputlistBoxFaultCode(string msg)
        {
            this.listBoxFaultCode.Dispatcher.Invoke(new outputDelegate(listBoxFaultCodeAction), msg);
        }

        private void listBoxFaultCodeAction(string msg)
        {
            string[] ShowFaultCode;
            string FaultCodeInfo = "";
            string[] faultCode;
            int faultCodeCount = 0;
            if (msg == "无故障码" || msg == "故障码已清除")
            {
                listBoxFaultCode.Items.Insert(0, msg);
            }
            else
            {
                ShowFaultCode = new string[msg.Length / 5];

                for (int i = 0; i < msg.Length / 5; i++)
                {
                    ShowFaultCode[i] = msg.Substring(i * 5, 5);
                }

                ShowFault = ShowFaultCode;
                faultCode = new string[ShowFaultCode.Length];
                for (int j = 0; j < ShowFaultCode.Length; j++)
                {
                    FaultCodeInfo = "第" + j + 1 + "个是：" + ShowFaultCode[j];
                    faultCode[faultCodeCount] = ShowFaultCode[j];
                    faultCodeCount++;
                    listBoxFaultCode.Items.Insert(j, FaultCodeInfo);

                }
                AnalyzeFaultCode(faultCode);
            }
            

        }




        private void AnalyzeFaultCode(string[] arrstr)
        {
            if (arrstr.Length <= 0) return;
            string[] faultCode = new string[arrstr.Length];
            //去掉重复的故障码
            int usefulDateCount = 1;
            faultCode[0] = arrstr[0];
            for (int i = 1; i < arrstr.Length; i++)
            {
                for (int j = 0; j < usefulDateCount; j++)
                {
                    if (faultCode[j] == arrstr[i])
                    {
                        break;
                    }
                    if (j == usefulDateCount - 1)
                    {
                        faultCode[usefulDateCount] = arrstr[i];
                        usefulDateCount++;
                    }
                }
            }
            for (int k = 0; k < faultCode.Length; k++)
            {
                if (faultCode[k] != null)
                {
                    outputtextFaultCodeExplan(faultCodeExplain(faultCode[k]));
                }
                /*if (faultCode[k] == null)
                {
                    outputtextFaultCodeExplan(faultCodeExplain("0741"));
                }*/
            }

        }

        /// <summary>
        /// 对故障码的解释
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string faultCodeExplain(string code)
        {
            string explan = "未知故障码";
            if (code == "P0218")
            {
                explan = "故障码：P0218\r\n" + "故障说明：变速器油温度过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 检查变速器冷却系统是否堵塞或损坏\r\n"
                    + "2. 执行“自动变速器冷却器冲洗和流量测试”以确认合适的变速器油冷却器流量\r\n"
                    + "3. 如果发现故障，必要时，修理或更换损坏的部件\r\n"
                    + "4. 执行“管路压力检查”以确认合适的变速器管路压\r\n"
                    + "5. 如果发现故障，必要时，修理或更换损坏的部件\r\n";
            }
            if (code == "P0562")
            {
                explan = "故障码：P0562\r\n" + "故障说明：系统电压过低\r\n"
                    + "故障对策：\r\n"
                    + "在发动机运转的情况下，用数字式万用表测量和记录蓄电池电压。蓄电池电压应在 12.5-14.5 伏之间。如果电压不在规定值内，或充电指示灯点亮，则转至“充电系统测试”\r\n";
            }
            if (code == "P0563")
            {
                explan = "故障码：P0563\r\n" + "故障说明：系统电压过高\r\n"
                    + "故障对策：\r\n"
                    + "在发动机运转的情况下，用数字式万用表测量和记录蓄电池电压。蓄电池电压应在 12.5-14.5 伏之间。如果电压不在规定值内，或充电指示灯点亮，则转至“充电系统测试”\r\n";
            }
            if (code == "P0601")
            {
                explan = "故障码：P0601\r\n" + "故障说明：变速器控制模块 (TCM) 只读存储器 (ROM)\r\n"
                    + "故障对策：\r\n"
                    + "重新编程变速器控制模块并重新检查故障诊断码。如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0602")
            {
                explan = "故障码：P0602\r\n" + "故障说明：变速器控制模块 (TCM) 未编程\r\n"
                    + "故障对策：\r\n"
                    + "重新编程变速器控制模块并重新检查故障诊断码。如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0603")
            {
                explan = "故障码：P0603\r\n" + "故障说明： 变速器控制模块 (TCM) 长期存储器复位\r\n"
                    + "故障对策：\r\n"
                    + "重新编程变速器控制模块并重新检查故障诊断码。如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0604")
            {
                explan = "故障码：P0604\r\n" + "故障说明：变速器控制模块 (TCM) 随机存取存储器(RAM)\r\n"
                    + "故障对策：\r\n"
                    + "重新编程变速器控制模块并重新检查故障诊断码。如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P062F")
            {
                explan = "故障码：P062F\r\n" + "故障说明：变速器控制模块 (TCM) 长期存储器性能\r\n"
                    + "故障对策：\r\n"
                    + "重新编程变速器控制模块并重新检查故障诊断码。如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0634")
            {
                explan = "故障码：P0634\r\n" + "故障说明：变速器控制模块 (TCM) 温度过高\r\n"
                    + "故障对策：\r\n"
                    + "检测变速器冷却器、变速器油管、发动机冷却系统和变速器油位，并检测在冷却系统空气流道中的任何可能导致过热状况的堵塞物。" +
                    "询问关于客户牵引或极限驾驶的情况。发动机冷却系统或变速器冷却系统的故障可能导致设置该诊断。更换控制电磁阀（带阀体和变速器控制模块）总成之前，" +
                    "执行“控制电磁阀的检查”和“变速器控制模块总成检查”\r\n";
            }
            if (code == "P0658")
            {
                explan = "故障码：P0658\r\n" + "故障说明：压力控制 (PC)/ 换档锁止电磁阀控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆\r\n"
                    + "2. 观察“High Side Driver CKT Status （高电平侧驱动器电路状态）”参数。该参数应显示“OK（正常）\r\n"
                    + "3. 如果参数显示“Open （开路）”、“Short toGround （对搭铁短路）”、“Short to Voltage（对电压短路）”或再次设置故障诊断码，" +
                    "则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0659")
            {
                explan = "故障码：P0659\r\n" + "故障说明：压力控制 (PC)/ 换档锁止电磁阀控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆\r\n"
                    + "2. 观察“High Side Driver CKT Status （高电平侧驱动器电路状态）”参数。该参数应显示“OK（正常）\r\n"
                    + "3. 如果参数显示“Open （开路）”、“Short toGround （对搭铁短路）”、“Short to Voltage（对电压短路）”或再次设置故障诊断码，" +
                    "则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0667")
            {
                explan = "故障码：P0667\r\n" + "故障说明：变速器控制模块 (TCM) 温度传感器性能\r\n"
                    + "故障对策：\r\n"
                    + "如果再次设置故障诊断码，则重新编程变速器控制模块并重新测试故障诊断码。更换控制电磁阀（带阀体和变速器控制模块）总成之前，" +
                    "执行“控制电磁阀的检查”和“变速器控制模块总成检查”\r\n";
            }
            if (code == "P0668")
            {
                explan = "故障码：P0668\r\n" + "故障说明：变速器控制模块 (TCM) 温度传感器电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "如果再次设置故障诊断码，则重新编程变速器控制模块并重新测试故障诊断码。更换控制电磁阀（带阀体和变速器控制模块）总成之前，" +
                    "执行“控制电磁阀的检查”和“变速器控制模块总成检查”\r\n";
            }
            if (code == "P0669")
            {
                explan = "故障码：P0669\r\n" + "故障说明：变速器控制模块 (TCM) 温度传感器电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "如果再次设置故障诊断码，则重新编程变速器控制模块并重新测试故障诊断码。更换控制电磁阀（带阀体和变速器控制模块）总成之前，" +
                    "执行“控制电磁阀的检查”和“变速器控制模块总成检查”\r\n";
            }
            if (code == "P06AC")
            {
                explan = "故障码：P06AC\r\n" + "故障说明：变速器控制模块 (TCM) 通电温度传感器性能\r\n"
                    + "故障对策：\r\n"
                    + "1. 清除故障诊断码，在运行和设置故障诊断码的条件下操作车辆\r\n"
                    + "2. 将点火开关置于 OFF 位置 30 秒钟，检查并确认不会再次设置故障诊断码\r\n"
                    + "3. 如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P06AD")
            {
                explan = "故障码：P06AD\r\n" + "故障说明：变速器控制模块 (TCM) 通电温度传感器电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 清除故障诊断码，在运行和设置故障诊断码的条件下操作车辆\r\n"
                    + "2. 将点火开关置于 OFF 位置 30 秒钟，检查并确认不会再次设置故障诊断码\r\n"
                    + "3. 如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P06AE")
            {
                explan = "故障码：P06AE\r\n" + "故障说明：变速器控制模块 (TCM) 通电温度传感器电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 清除故障诊断码，在运行和设置故障诊断码的条件下操作车辆\r\n"
                    + "2. 将点火开关置于 OFF 位置 30 秒钟，检查并确认不会再次设置故障诊断码\r\n"
                    + "3. 如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0711")
            {
                explan = "故障码：P0711\r\n" + "故障说明：变速器油温度 (TFT) 传感器性能\r\n"
                    + "故障对策：\r\n"
                    + "清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆。检查并确认故障诊断码未再次设置。" +
                    "如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0712")
            {
                explan = "故障码：P0712\r\n" + "故障说明：变速器油温度 (TFT) 传感器电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆。检查并确认故障诊断码未再次设置。" +
                    "如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0713")
            {
                explan = "故障码：P0713\r\n" + "故障说明：变速器油温度 (TFT) 传感器电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆。检查并确认故障诊断码未再次设置。" +
                    "如果再次设置故障诊断码，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0716")
            {
                explan = "故障码：P0716\r\n" + "故障说明：输入轴转速传感器性能\r\n"
                    + "故障对策：\r\n"
                    + "检测输出轴转速传感器、线束、连接器和控制电磁阀（带阀体和变速器控制模块）总成销是否有金属碎屑，" +
                    "以及输出轴机加工面是否损坏或错位。输出轴转速传感器安装螺栓的正确扭矩对输出轴转速传感器正常工作至关重要。" +
                    "使用 GM 许可的端子测试组件 J 35616，按测试要求，探测控制电磁阀（带阀体和变速器控制模块）总成线束连接器或部件线束连接器。\r\n";
            }
            if (code == "P0717")
            {
                explan = "故障码：P0717\r\n" + "故障说明：输入轴转速传感器电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "检测输出轴转速传感器、线束、连接器和控制电磁阀（带阀体和变速器控制模块）总成销是否有金属碎屑，" +
                    "以及输出轴机加工面是否损坏或错位。输出轴转速传感器安装螺栓的正确扭矩对输出轴转速传感器正常工作至关重要。" +
                    "使用 GM 许可的端子测试组件 J 35616，按测试要求，探测控制电磁阀（带阀体和变速器控制模块）总成线束连接器或部件线束连接器。\r\n";
            }
            if (code == "P0722")
            {
                explan = "故障码：P0722\r\n" + "故障说明：输出轴转速传感器 (OSS) 电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "检测输出轴转速传感器、线束、连接器和控制电磁阀（带阀体和变速器控制模块）总成销是否有金属碎屑，" +
                    "以及输出轴机加工面是否损坏或错位。输出轴转速传感器安装螺栓的正确扭矩对输出轴转速传感器正常工作至关重要。" +
                    "使用 GM 许可的端子测试组件 J 35616，按测试要求，探测控制电磁阀（带阀体和变速器控制模块）总成线束连接器或部件线束连接器。\r\n";
            }
            if (code == "P0723")
            {
                explan = "故障码：P0723\r\n" + "故障说明：输出轴转速传感器 (OSS) 间歇性故障\r\n"
                    + "故障对策：\r\n"
                    + "检测输出轴转速传感器、线束、连接器和控制电磁阀（带阀体和变速器控制模块）总成销是否有金属碎屑，" +
                    "以及输出轴机加工面是否损坏或错位。输出轴转速传感器安装螺栓的正确扭矩对输出轴转速传感器正常工作至关重要。" +
                    "使用 GM 许可的端子测试组件 J 35616，按测试要求，探测控制电磁阀（带阀体和变速器控制模块）总成线束连接器或部件线束连接器。\r\n";
            }
            if (code == "P0741")
            {
                explan = "故障码：P0741\r\n" + "故障说明：变矩器离合器 (TCC) - 卡在分离位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“控制电磁阀和变速器控制模块总成的清理”以清理或释放潜在的可能造成阀门卡滞的灰尘或碎屑\r\n"
                    + "2. 如果在执行清理程序后再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”以测试控制电磁阀（带阀体和变速器控制模块总成中的变矩器离合器压力控制电磁阀是否卡在接合位置或分离位置\r\n"
                    + "4. 如果发现阀门卡在接合或分离位置，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "5. 检测下控制阀体是否存在阀门卡滞、碎屑或损坏\r\n"
                    + "6. 如果发现阀体故障，进行修理或必要时更换下控制阀体\r\n"
                    + "7. 检查油泵中的变矩器离合器控制阀\r\n"
                    + "8. 如果发现变矩器离合器控制阀故障，进行修理或必要时更换变矩器离合器控制阀或油泵\r\n"
                    + "9. 检查变矩器总成是否损坏或褪色\r\n"
                    + "10. 如果发现变矩器总成故障，必要时更换变矩器总\r\n";
            }
            if (code == "P0742")
            {
                explan = "故障码：P0742\r\n" + "故障说明：变矩器离合器 (TCC) - 卡在接合位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“控制电磁阀和变速器控制模块总成的清理”以清理或释放潜在的可能造成阀门卡滞的灰尘或碎屑\r\n"
                    + "2. 如果在执行清理程序后再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”以测试控制电磁阀（带阀体和变速器控制模块总成中的变矩器离合器压力控制电磁阀是否卡在接合位置或分离位置\r\n"
                    + "4. 如果发现阀门卡在接合或分离位置，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "5. 检测下控制阀体是否存在阀门卡滞、碎屑或损坏\r\n"
                    + "6. 如果发现阀体故障，进行修理或必要时更换下控制阀体\r\n"
                    + "7. 检查油泵中的变矩器离合器控制阀\r\n"
                    + "8. 如果发现变矩器离合器控制阀故障，进行修理或必要时更换变矩器离合器控制阀或油泵\r\n"
                    + "9. 检查变矩器总成是否损坏或褪色\r\n"
                    + "10. 如果发现变矩器总成故障，必要时更换变矩器总\r\n";
            }
            if (code == "P0751")
            {
                explan = "故障码：P0751\r\n" + "故障说明：换档电磁阀 (SS) 1 性能 - 卡在断电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力不在规定范围内，则首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现换档电磁阀 1 泄漏或卡在断电位置，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0752")
            {
                explan = "故障码：P0752\r\n" + "故障说明：换档电磁阀 (SS) 1 性能 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力不在规定范围内，则首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现换档电磁阀 1 泄漏或卡在断电位置，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0776")
            {
                explan = "故障码：P0776\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 2 - 卡在断电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路 / 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 3-5 档倒档离合器总成是否损坏\r\n"
                    + "12. 如果发现 3-5 档倒档离合器总成有故障，进行修理或必要时进行更换\r\n";
            }
            if (code == "P0777")
            {
                explan = "故障码：P0777\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 2 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路 / 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 3-5 档倒档离合器总成是否损坏\r\n"
                    + "12. 如果发现 3-5 档倒档离合器总成有故障，进行修理或必要时进行更换\r\n";
            }
            if (code == "P0796")
            {
                explan = "故障码：P0796\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 3 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路 / 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查低速档和倒档/4-5-6 档离合器总成是否损坏。\r\n"
                    + "12. 如果发现低速档和倒档/4-5-6 档离合器总成有故障，进行修理或必要时进行更换\r\n";
            }
            if (code == "P0797")
            {
                explan = "故障码：P0797\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 3 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路 / 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查低速档和倒档/4-5-6 档离合器总成是否损坏。\r\n"
                    + "12. 如果发现低速档和倒档/4-5-6 档离合器总成有故障，进行修理或必要时进行更换\r\n";
            }
            if (code == "P0815")
            {
                explan = "故障码：P0815\r\n" + "故障说明：加档开关电路\r\n"
                    + "故障对策：\r\n"
                    + "1. 选择“Driver Shift Request （驾驶员换档请求）”参数\r\n"
                    + "2. 断开自动变速器 (AT) 自适应压力换档开关连接器\r\n"
                    + "3. 如果参数状态从“Upshift （加档）”变为“Invalid （无效）”，则更换自动变速器自适应压力换档开关\r\n"
                    + "4. 果参数状态未改变，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0816")
            {
                explan = "故障码：P0816\r\n" + "故障说明：减档开关电路\r\n"
                    + "故障对策：\r\n"
                    + "1. 选择“Driver Shift Request （驾驶员换档请求）”参数\r\n"
                    + "2. 断开自动变速器 (AT) 自适应压力换档开关连接器\r\n"
                    + "3. 如果参数状态从“Downshift （减档）”变为“Invalid （无效）”，则更换自动变速器自适应压力换档开关\r\n"
                    + "4. 如果参数状态未改变，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0826")
            {
                explan = "故障码：P0826\r\n" + "故障说明：加档和减档开关电路\r\n"
                    + "故障对策：\r\n"
                    + "1. 检查点火 1 电压电路中的保险丝是否熔断\r\n"
                    + "2. 如果保险丝熔断，测试点火 1 和变速器自适应压力加档和减档电路是否对搭铁短路\r\n"
                    + "3. 如果保险丝完好，测试点火 1 和变速器自适应压力加档和减档信号电路是否开路或对电压短路\r\n"
                    + "4. 测试自动变速器自适应压力换档开关的电阻值是否正确，应为 8.11-8.39 欧\r\n"
                    + "5. 如果电阻值不符合规定，则更换开关总成\r\n"
                    + "6. 如果电阻值符合规定，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P1876")
            {
                explan = "故障码：P1876\r\n" + "故障说明：加档和减档启用开关电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 点火开关置于ON 位置，发动机关闭，变速器换档杆挂PARK （驻车档），将触动式加档/ 减档启用开关连接器从开关上断开\r\n"
                    + "2. 使用故障诊断仪，观察车身故障诊断仪数据列表“Body and Accessories/Instrument Panel （车身和附件/ 仪表板）”上的“Tap Up/DownEnable （触动式加档/ 减档启用）”参数。参数应显示“Disabled （停用）”。\r\n"
                    + " -如果断开的时候参数不显示“Disabled （停用）”，测试触动式加档 / 减档启用信号电路是否对搭铁短路。如果没有对搭铁短路，则更换车身控制模块。\r\n"
                    + " -如果参数在断开的时候显示“Disabled （停用）”，连接的时候显示“Enabled （启用）”，确保机构启用开关传动机构没有弯曲、卡滞或损坏。"
                    + " -如果不存在上述情况，则更换触动式加档 / 减档启用开关。\r\n"
                    + " -如果参数始终显示“Disabled （停用）”，测试触动式加档 / 减档搭铁电路是否开路或对电压短路。\r\n";
            }
            if (code == "P0842")
            {
                explan = "故障码：P0842\r\n" + "故障说明：变速器油压力 (TFP) 开关 1 电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将换档杆从 Park （驻车档）挂到Reverse （空档）时，观察“TFP Switch 1（变速器油压力开关 1）”参数。变速器油压力开关 1 将从 Park （驻车档）时的高压转换到Reverse （空档）时的低压\r\n"
                    + "3. 如果变速器油压力 1 参数不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "4. 如未发现故障，则更换控制电磁阀和变速器控制模块总成\r\n";
            }
            if (code == "P0843")
            {
                explan = "故障码：P0843\r\n" + "故障说明：变速器油压力 (TFP) 开关 1 电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将换档杆从 Park （驻车档）挂到Reverse （空档）时，观察“TFP Switch 1（变速器油压力开关 1）”参数。变速器油压力开关 1 将从 Park （驻车档）时的高压转换到Reverse （空档）时的低压\r\n"
                    + "3. 如果变速器油压力 1 参数不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "4. 如未发现故障，则更换控制电磁阀和变速器控制模块总成\r\n";
            }
            if (code == "P0872")
            {
                explan = "故障码：P0872\r\n" + "故障说明：变速器油压力 (TFP) 开关 3 电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂一档时观察“TFP Switch 3 （变速器油压力开关 3）”参数，指令挂二档时观察故障诊断仪。变速器油压力开关 3 应从一档时的低压转换到二档时的高压\r\n"
                    + "4. 如果“TFP Switch 3 （变速器油压力 3）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0873")
            {
                explan = "故障码：P0873\r\n" + "故障说明：变速器油压力 (TFP) 开关 3 电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂一档时观察“TFP Switch 3 （变速器油压力开关 3）”参数，指令挂二档时观察故障诊断仪。变速器油压力开关 3 应从一档时的低压转换到二档时的高压\r\n"
                    + "4. 如果“TFP Switch 3 （变速器油压力 3）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0877")
            {
                explan = "故障码：P0877\r\n" + "故障说明：变速器油压力 (TFP) 开关 4 电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂四档时观察“TFP switch 4 （变速器油压力开关 4）”参数，指令挂五档时观察故障诊断仪。变速器油压力开关 4 应从四档的低压转换为五档的高压\r\n"
                    + "4. 如果“TFP Switch 4 （变速器油压力 4）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0878")
            {
                explan = "故障码：P0878\r\n" + "故障说明：变速器油压力 (TFP) 开关 4 电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂四档时观察“TFP switch 4 （变速器油压力开关 4）”参数，指令挂五档时观察故障诊断仪。变速器油压力开关 4 应从四档的低压转换为五档的高压\r\n"
                    + "4. 如果“TFP Switch 4 （变速器油压力 4）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0961")
            {
                explan = "故障码：P0961\r\n" + "故障说明：管路压力控制 (PC) 电磁阀系统性能\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后让车辆在 Park （驻车档）运行 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“Line PC Sol.CKTStatus （管路压力控制电磁阀电路状态）”，该参数应显示“OK （正常）”\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0962")
            {
                explan = "故障码：P0962\r\n" + "故障说明：管路压力控制 (PC) 电磁阀控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后让车辆在 Park （驻车档）运行 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“Line PC Sol.CKTStatus （管路压力控制电磁阀电路状态）”，该参数应显示“OK （正常）”\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0963")
            {
                explan = "故障码：P0963\r\n" + "故障说明：管路压力控制 (PC) 电磁阀控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后让车辆在 Park （驻车档）运行 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“Line PC Sol.CKTStatus （管路压力控制电磁阀电路状态）”，该参数应显示“OK （正常）”\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0965")
            {
                explan = "故障码：P0965\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 2 系统性能\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC 2 Sol.CKT Status（离合器压力控制电磁阀 2 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0966")
            {
                explan = "故障码：P0966\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 2 控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                   + "故障诊断仪数据参数“Clutch PC 2 Sol.CKT Status（离合器压力控制电磁阀 2 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0967")
            {
                explan = "故障码：P0967\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 2 控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC 2 Sol.CKT Status（离合器压力控制电磁阀 2 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0969")
            {
                explan = "故障码：P0969\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 3 系统性能\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC3 Sol.CKT Status（离合器压力控制电磁阀 3 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0970")
            {
                explan = "故障码：P0970\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 3 控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC3 Sol.CKT Status（离合器压力控制电磁阀 3 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0971")
            {
                explan = "故障码：P0971\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 3 控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC3 Sol.CKT Status（离合器压力控制电磁阀 3 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P0973")
            {
                explan = "故障码：P0973\r\n" + "故障说明：换档电磁阀 (SS) 1 控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)\r\n"
                    + "3. 将换档杆挂 Reverse （倒档）持续 5 秒钟，再使车辆怠速行驶 5 秒钟\r\n"
                    + "4. 观察故障诊断仪数据参数“Shift Sol.1 CKTStatus （换档电磁阀 1 电路状态）”。 该参数应显示“OK （正常）”\r\n"
                    + "5. 如果参数显示“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0974")
            {
                explan = "故障码：P0974\r\n" + "故障说明：换档电磁阀 (SS) 1 控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)\r\n"
                    + "3. 将换档杆挂 Reverse （倒档）持续 5 秒钟，再使车辆怠速行驶 5 秒钟\r\n"
                    + "4. 观察故障诊断仪数据参数“Shift Sol.1 CKTStatus （换档电磁阀 1 电路状态）”。 该参数应显示“OK （正常）”\r\n"
                    + "5. 如果参数显示“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0989")
            {
                explan = "故障码：P0989\r\n" + "故障说明：变速器油压力 (TFP) 开关 5 电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂一档时观察“TFP Switch 5 （变速器油压力开关 5）”参数，指令挂二档时观察故障诊断仪。变速器油压力开关 5 将从一档时的低压转到二档时的高压\r\n"
                    + "4. 如果“TFP Switch 5 （变速器油压力 5）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P0990")
            {
                explan = "故障码：P0990\r\n" + "故障说明：变速器油压力 (TFP) 开关 5 电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度近似于“故障记录”中显示的温度\r\n"
                    + "2. 起动发动机，将变速器换档杆挂 D6 档\r\n"
                    + "3. 指令挂一档时观察“TFP Switch 5 （变速器油压力开关 5）”参数，指令挂二档时观察故障诊断仪。变速器油压力开关 5 将从一档时的低压转到二档时的高压\r\n"
                    + "4. 如果“TFP Switch 5 （变速器油压力 5）”参数状态不改变，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P1751")
            {
                explan = "故障码：P1751\r\n" + "故障说明：换档阀 1 性能\r\n"
                    + "故障对策：\r\n"
                    + "检查下控制阀体是否存在离合器选择阀 2 卡滞、离合器选择阀 2弹簧折断、有碎屑或损坏。如果发现阀体故障，进行修理或必要时更换下控制阀体。\r\n";
            }
            if (code == "P1825")
            {
                explan = "故障码：P1825\r\n" + "故障说明：内部模式开关 - 无效档位\r\n"
                    + "故障对策：\r\n"
                    + "1. 断开控制电磁阀（带阀体和变速器控制模块）总成的内部模式开关连接器。点火开关置于 ON 位置。故障诊断仪“IMS A/B/C/P （内部模式开关A/B/C/P）”参数应在四个信号电路中显示“HI（高电平）”\r\n"
                    + "2. 如果信号电路显示“LOW （低电平）”，则控制电磁阀（带阀体和变速器控制模块）总成有故障\r\n"
                    + "3. 将一根带 3 安培保险丝的跨接线连接在控制阀（带阀体和变速器控制模块）总成上的每一个变速器档位信号电路和搭铁之间，当连接至搭铁时，确保搭铁信号电路显示“LOW （低电平）”\r\n"
                    + "4. 当连接至搭铁时，若信号电路仍显示“HI （高电平）”，则控制电磁阀（带阀体和变速器控制模块）总成有故障\r\n"
                    + "5. 如果上述两种测试都表明变速器控制模块功能正常，则内部模式开关有故障\r\n";
            }
            if (code == "P1915")
            {
                explan = "故障码：P1915\r\n" + "故障说明：起动过程中内部模式开关未指示 PARK（驻车档） / NETUTRAL （空档） (P / N)\r\n"
                    + "故障对策：\r\n"
                    + "1. 断开控制电磁阀（带阀体和变速器控制模块）总成的内部模式开关连接器。点火开关置于 ON 位置。故障诊断仪“IMS A/B/C/P （内部模式开关A/B/C/P）”参数应在四个信号电路中显示“HI（高电平）”\r\n"
                    + "2. 如果信号电路显示“LOW （低电平）”，则控制电磁阀（带阀体和变速器控制模块）总成有故障\r\n"
                    + "3. 将一根带 3 安培保险丝的跨接线连接在控制阀（带阀体和变速器控制模块）总成上的每一个变速器档位信号电路和搭铁之间，当连接至搭铁时，确保搭铁信号电路显示“LOW （低电平）”\r\n"
                    + "4. 当连接至搭铁时，若信号电路仍显示“HI （高电平）”，则控制电磁阀（带阀体和变速器控制模块）总成有故障\r\n"
                    + "5. 如果上述两种测试都表明变速器控制模块功能正常，则内部模式开关有故障\r\n";
            }
            if (code == "P2534")
            {
                explan = "故障码：P2534\r\n" + "故障说明：点火 1 开关电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "清除故障诊断码并在运行和设置故障诊断码的条件下操作车辆。如果再次设置故障诊断码，则测试在变速器控制模块 14 路连接器上的变速器控制模块点火 1 电压电路的蓄电池正极电压。" +
                    "如果电压低于蓄电池电压，则测试点火 1 电压电路是否开路或对搭铁短路，必要时，修理电路。\r\n";
            }
            if (code == "P2714")
            {
                explan = "故障码：P2714\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 4 - 卡在断电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 2-6 档离合器总成是否损坏\r\n"
                    + "12. 如果在 2-6 档离合器总成中发现故障，进行维修或必要时进行更换\r\n";
            }
            if (code == "P2715")
            {
                explan = "故障码：P2715\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 4 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 2-6 档离合器总成是否损坏\r\n"
                    + "12. 如果在 2-6 档离合器总成中发现故障，进行维修或必要时进行更换\r\n";
            }
            if (code == "P2719")
            {
                explan = "故障码：P2719\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 4 系统性能\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC4 Sol.CKT Status（离合器压力控制电磁阀 4 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2720")
            {
                explan = "故障码：P2720\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 4 控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC4 Sol.CKT Status（离合器压力控制电磁阀 4 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2721")
            {
                explan = "故障码：P2721\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 4 控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC4 Sol.CKT Status（离合器压力控制电磁阀 4 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2723")
            {
                explan = "故障码：P2723\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 5 - 卡在断电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 1-2-3-4 档离合器总成是否损坏\r\n"
                    + "12. 如果在 1-2-3-4 档离合器总成中发现故障，进行维修或必要时将其更换\r\n";
            }
            if (code == "P2724")
            {
                explan = "故障码：P2724\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 5 - 卡在通电位置\r\n"
                    + "故障对策：\r\n"
                    + "1. 执行“管路压力检查”\r\n"
                    + "2. 如果压力超出规定范围，首先排除此故障\r\n"
                    + "3. 执行“控制电磁阀和变速器控制模块总成的清洁”，并重新测试“电路/ 系统检验”步骤中列出的故障诊断码\r\n"
                    + "4. 如果再次设置故障诊断码，继续进行测试\r\n"
                    + "5. 执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "6. 如果发现故障，维修或更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "7. 执行“控制电磁阀和变速器控制模块总成电磁阀性能测试”\r\n"
                    + "8. 如果发现电磁阀泄漏或卡在断电位置，更换控制电磁阀（带阀体和变速器控制模块）总成\r\n"
                    + "9. 检查下控制阀体总成是否存在阀门卡滞、损坏、划伤孔或碎屑\r\n"
                    + "10. 如果在阀体中发现故障，则进行维修或必要时更换阀体\r\n"
                    + "11. 检查 1-2-3-4 档离合器总成是否损坏\r\n"
                    + "12. 如果在 1-2-3-4 档离合器总成中发现故障，进行维修或必要时将其更换\r\n";
            }
            if (code == "P2728")
            {
                explan = "故障码：P2728\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 5 系统性能\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC5 Sol.CKT Status（离合器压力控制电磁阀 5 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2729")
            {
                explan = "故障码：P2729\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 5 控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC5 Sol.CKT Status（离合器压力控制电磁阀 5 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2730")
            {
                explan = "故障码：P2730\r\n" + "故障说明：离合器压力控制 (PC) 电磁阀 5 控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "故障诊断仪数据参数“Clutch PC5 Sol.CKT Status（离合器压力控制电磁阀 5 电路状态）”通常显示“OK（正常）”。" +
                    "如果参数显示“Open / Short to Volts （开路 / 对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，" +
                    "则执行“控制电磁阀和变速器控制模块总成检查”，如果没有发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成。\r\n";
            }
            if (code == "P2762")
            {
                explan = "故障码：P2762\r\n" + "故障说明：变矩器离合器 (TCC) 压力控制电磁阀系统性能\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后运行车辆以保证变矩器离合器接合 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“TCCPC Sol.CKTStatus （变矩器离合器压力控制电磁阀电路状态）”，该参数应显示“OK （正常）\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P2763")
            {
                explan = "故障码：P2763\r\n" + "故障说明：变矩器离合器 (TCC) 压力控制 (PC) 电磁阀控制电路电压过高\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后运行车辆以保证变矩器离合器接合 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“TCCPC Sol.CKTStatus （变矩器离合器压力控制电磁阀电路状态）”，该参数应显示“OK （正常）\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            if (code == "P2764")
            {
                explan = "故障码：P2764\r\n" + "故障说明：变矩器离合器 (TCC) 压力控制 (PC) 电磁阀控制电路电压过低\r\n"
                    + "故障对策：\r\n"
                    + "1. 确保变速器油温度为 50-80°C (122-176°F)\r\n"
                    + "2. 在二档下运行车辆足够长的时间，以确保变速器控制模块的温度至少上升 3°C (5°F)，然后运行车辆以保证变矩器离合器接合 5 秒钟\r\n"
                    + "3. 观察故障诊断仪数据参数“TCCPC Sol.CKTStatus （变矩器离合器压力控制电磁阀电路状态）”，该参数应显示“OK （正常）\r\n"
                    + "4. 如果参数显示为“Open （开路）”、“Short toVolts （对电压短路）”、“Short to GND （对搭铁短路）”或再次设置故障诊断码，则执行“控制电磁阀和变速器控制模块总成检查”\r\n"
                    + "5. 如在检查过程中未发现故障，则更换控制电磁阀（带阀体和变速器控制模块）总成\r\n";
            }
            return explan;
        }

        private void TcuFlashed()
        {
            programmeDialog.progreTimer.Stop();
            OutputTcuFlashed();
            OutputFlashinghed();
            OutputChooseFlashFile();
            OutputTextFileName();

            Outputprogressbar("0");
            MessageBox.Show("刷写完毕");
        }

        private void OutputTcuFlashed()
        {
                    programmeDialog.btn_FlashTcu.Dispatcher.Invoke(new ChangeBtnStats(TcuFlashedAction));
        }

        private void TcuFlashedAction()
        {
            programmeDialog.btn_FlashTcu.Visibility = Visibility.Visible;
            programmeDialog.btn_FlashTcu.IsEnabled = false;
            programmeDialog.btn_DlgHide.IsEnabled = true;
        }

        private void OutputFlashinghed()
        {
            programmeDialog.btn_Flashing.Dispatcher.Invoke(new ChangeBtnStats(FlashingAction));
        }

        private void FlashingAction()
        {
            programmeDialog.btn_Flashing.Visibility = Visibility.Collapsed;
        }

        private void OutputChooseFlashFile()
        {
            programmeDialog.btn_ChooseFlashFile.Dispatcher.Invoke(new ChangeBtnStats(ChooseFlashFileAction));
        }

        private void ChooseFlashFileAction()
        {
            programmeDialog.btn_ChooseFlashFile.IsEnabled = true;
        }
        private void Outputprogressbar(string data)
        {
            programmeDialog.progressbar.Dispatcher.Invoke(new outputDelegate(ProgressbarAction), data);
        }

        private void ProgressbarAction(string data)
        {
            programmeDialog.progressbar.Value = int.Parse(data);
        }
        private void OutputTextFileName()
        {
           programmeDialog.TextFileName.Dispatcher.Invoke(new ChangeBtnStats(PTextFileNameAction));
        }

        private void PTextFileNameAction()
        {
            programmeDialog.TextFileName.Text = "";
        }


        private void tcuProgrammeDialog_Click(object sender, RoutedEventArgs e)
        {

            
            programmeDialog.Show();
            DlgOpen = true;

            btn_ReadFT.IsEnabled = false;
            btn_ReadFC.IsEnabled = false;
            btn_CleakFC.IsEnabled = false;
            btn_ReadTI.IsEnabled = false;
            
        }


        /// <summary>
        /// 读取TCU信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            outputText_PartNumber("");
            outputTextNoFlashingSoftware("");
            outputTextCommercialStatus("");
            outputTextManufacturingTrackingCode("");
            outputTextVIN("");
            if (listBoxFaultCode.Items.Count > 0)
            {
                listBoxFaultCode.Items.Clear();
            }
            textFaultCodeExplan.Text = "";
            ((App)Application.Current).dataPro.InquireTCUInfo();
            btn_CleakFC.Visibility = Visibility.Visible;
            btn_ReadFC.Visibility = Visibility.Collapsed;
            btn_CleakFC.IsEnabled = false;
            btn_ReadFC.IsEnabled = false;
            btn_ReadFT.IsEnabled = false;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {

        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {

        }
        //入口
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            ((App) Application.Current).dataPro.RecvTcuInfo += new RecvTcuInfoEventHandler(TCUInfoMSG);
            ((App) Application.Current).dataPro.RcvTcuFaultCode += new RecvTcuFaultCodeEventHandler(ShowFaultCode);
            ((App)Application.Current).dataPro.TcuFlashFinish += new TcuFlashFinishEventHandler(TcuFlashed);
            ((App)Application.Current).dataPro.TcuFlashInterrupt += new TcuFlashInterruptEventHandler(FlashInterrupt);
            programmeDialog.FTDlgHideEvent += new FlashTcuDlgHide(FlashTcuDlgHided);
            programmeDialog.GetPartNumber += new GetTcuPartNumber(getpartnumber);
                programmeDialog.Flashtcu += new FlashTCU(StratFlashTcu);
                programmeDialog.SendFlsahData0 += new SendFlashTcuData0(SendFlashData0);
                programmeDialog.SendFlsahData1 += new SendFlashTcuData1(SendFlashData1);
                programmeDialog.SendFlsahData2 += new SendFlashTcuData2(SendFlashData2);
                programmeDialog.SendFlsahData3 += new SendFlashTcuData3(SendFlashData3);
                programmeDialog.SendFlsahData4 += new SendFlashTcuData4(SendFlashData4);
            
            
        }
        //出口
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            ((App) Application.Current).dataPro.RecvTcuInfo -= new RecvTcuInfoEventHandler(TCUInfoMSG);
            ((App) Application.Current).dataPro.RcvTcuFaultCode -= new RecvTcuFaultCodeEventHandler(ShowFaultCode);
            ((App)Application.Current).dataPro.TcuFlashFinish -= new TcuFlashFinishEventHandler(TcuFlashed);
            ((App)Application.Current).dataPro.TcuFlashInterrupt -= new TcuFlashInterruptEventHandler(FlashInterrupt);
            programmeDialog.FTDlgHideEvent += new FlashTcuDlgHide(FlashTcuDlgHided);
            programmeDialog.GetPartNumber -= new GetTcuPartNumber(getpartnumber);
                programmeDialog.Flashtcu -= new FlashTCU(StratFlashTcu);
                programmeDialog.SendFlsahData0 -= new SendFlashTcuData0(SendFlashData0);
                programmeDialog.SendFlsahData1 -= new SendFlashTcuData1(SendFlashData1);
                programmeDialog.SendFlsahData2 -= new SendFlashTcuData2(SendFlashData2);
                programmeDialog.SendFlsahData3 -= new SendFlashTcuData3(SendFlashData3);
                programmeDialog.SendFlsahData4 -= new SendFlashTcuData4(SendFlashData4);
            
            
        }

        /// <summary>
        /// 刷写VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_FlashVIN_Click(object sender, RoutedEventArgs e)
        {
            ((App) Application.Current).dataPro.FlashVin(TextVINCode.Text);
        }

        /// <summary>
        /// 读取故障码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ReadFC_Click(object sender, RoutedEventArgs e)
        {
            ((App) Application.Current).dataPro.GetFaultCode();
            if (listBoxFaultCode.Items.Count > 0)
            {
                listBoxFaultCode.Items.Clear();
            }
            textFaultCodeExplan.Text = "";
            btn_CleakFC.Visibility  = Visibility.Visible;
            btn_ReadFC.Visibility = Visibility.Collapsed;
        }

        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            _TestInformation[0] = TextOperator.Text;
            _TestInformation[1] = Text_ReportNo.Text;
            _TestInformation[2] = Text_PartNumber.Text;
            _TestInformation[3] = TextNoFlashingSoftware.Text;
            _TestInformation[4] = TextCommercialStatus.Text;
            _TestInformation[5] = TextManufacturingTrackingCode.Text;
            _TestInformation[6] = TextVIN.Text;
            if (ShowFault != null)
            {
                if (ShowFault.Length > 0)
                {
                    foreach (var data in ShowFault)
                    {
                        _TestInformation[7] += data + ";";
                    }
                }
                else
                { _TestInformation[7] = "无故障码"; }
            }
            else
            {
                _TestInformation[7] = "未读取故障码";
            }
            _TestInformation[8] = TestTime;
            ((App) Application.Current)._TestInformation = _TestInformation;

            if (TestFile.Text != "压力开关测试")
            {
                if(TestFile.Text != "单体测试")
                {
                    ((App)Application.Current).FilePath = Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_TestStandard.xls";
                }
                else
                {
                    ((App)Application.Current).FilePath = Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_UnitTesting.xls";
                }
                
            }
            else
            {
                ((App)Application.Current).FilePath = Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_PressureSwitchTest.xls";
            }
        }

        /// <summary>
        /// 清除故障码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CleakFC_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxFaultCode.Items.Count > 0)
            {
                listBoxFaultCode.Items.Clear();
            }
            textFaultCodeExplan.Text = "";
            ((App) Application.Current).dataPro.ClearFaultCode();
            btn_CleakFC.Visibility = Visibility.Collapsed;
            btn_ReadFC.Visibility = Visibility.Visible;
        }


        private void TextManufacturingTrackingCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextManufacturingTrackingCode.Text != "")
            {
                StartTest.IsEnabled = true;
            }
            else
            { StartTest.IsEnabled = false; }

        }
    }
    
}
