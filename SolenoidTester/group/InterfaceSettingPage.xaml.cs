using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
//using ModernUINavigationApp.DataClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace SolenoidTester.group
{
    /// <summary>
    /// Interaction logic for InterfaceSettingPage.xaml
    /// </summary>
    public partial class InterfaceSettingPage : UserControl, IContent
    {


        /// <summary>
        /// MCU连接异常提示
        /// </summary>
        private Thread MCUConnectNotice;

        /// <summary>
        /// MCU连接异常提示时间
        /// </summary>
        private DateTime MCUConnectExTime;

        /// <summary>
        /// MCU异常处理标志位
        /// </summary>
        private bool MCUConExFlag = false;

        private delegate void outputDelegate(string msg);

        System.Timers.Timer t = new System.Timers.Timer(500); //实例化Timer类，设置间隔时间为10000毫秒；

        public InterfaceSettingPage()
        {
            InitializeComponent();
                //string OpenPortDefinePath = Directory.GetCurrentDirectory() + "/txt文件/PortDefine.txt";
                //string[] MidDeviceDemarcate = new string[27];
                //StreamReader sr = new StreamReader(OpenPortDefinePath, Encoding.UTF8);
                //int[] InitPort = new int[15];
                //for (int i = 0; i < 15; i++)
                //{
                //    InitPort[i] = int.Parse(sr.ReadLine());
                //    if (InitPort[i] > 15 && InitPort[i] < 21)
                //    {
                //        InitPort[i] = InitPort[i] - 12;
                //    }
                //    else if (InitPort[i] > 3 && InitPort[i] < 7)
                //    {
                //        InitPort[i] = InitPort[i] - 4;
                //    }
                //    else if (InitPort[i] > 9 && InitPort[i] < 13)
                //    {
                //        InitPort[i] = InitPort[i] - 10;
                //    }
                //}

                //#region 接口定义初始化

                ////设置P_LineComboBox
                //Dictionary<int, string> P_Line = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_Line.ItemsSource = P_Line;
                //CBP_Line.SelectedValuePath = "Key";
                //CBP_Line.DisplayMemberPath = "Value";
                ////设置P_CoolantComboBox
                //Dictionary<int, string> P_Coolant = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_Coolant.ItemsSource = P_Coolant;
                //CBP_Coolant.SelectedValuePath = "Key";
                //CBP_Coolant.DisplayMemberPath = "Value";
                ////设置M_CoolantComboBox
                //Dictionary<int, string> M_Coolant = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBM_Coolant.ItemsSource = M_Coolant;
                //CBM_Coolant.SelectedValuePath = "Key";
                //CBM_Coolant.DisplayMemberPath = "Value";
                ////设置T_CoolantComboBox
                //Dictionary<int, string> T_Coolant = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBT_Coolant.ItemsSource = T_Coolant;
                //CBT_Coolant.SelectedValuePath = "Key";
                //CBT_Coolant.DisplayMemberPath = "Value";
                ////设置P_1ComboBox
                //Dictionary<int, string> P_1 = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_1.ItemsSource = P_1;
                //CBP_1.SelectedValuePath = "Key";
                //CBP_1.DisplayMemberPath = "Value";
                ////设置P_2ComboBox
                //Dictionary<int, string> P_2 = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_2.ItemsSource = P_2;
                //CBP_2.SelectedValuePath = "Key";
                //CBP_2.DisplayMemberPath = "Value";
                ////设置P_3ComboBox
                //Dictionary<int, string> P_3 = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_3.ItemsSource = P_3;
                //CBP_3.SelectedValuePath = "Key";
                //CBP_3.DisplayMemberPath = "Value";
                ////设置P_4ComboBox
                //Dictionary<int, string> P_4 = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_4.ItemsSource = P_4;
                //CBP_4.SelectedValuePath = "Key";
                //CBP_4.DisplayMemberPath = "Value";
                ////设置P_5ComboBox
                //Dictionary<int, string> P_5 = new Dictionary<int, string>()
                //{
                //    {0, "P7"},
                //    {1, "P8"},
                //    {2, "F1"},
                //    {3, "T"},
                //    {16, "P1"},
                //    {17, "P2"},
                //    {18, "P3"},
                //    {19, "P4"},
                //    {20, "P5"},
                //};
                //CBP_5.ItemsSource = P_5;
                //CBP_5.SelectedValuePath = "Key";
                //CBP_5.DisplayMemberPath = "Value";
                ////设置Torque_InComboBox
                //Dictionary<int, string> Torque_In = new Dictionary<int, string>()
                //{
                //    {4, "Torque_1"},
                //    {5, "Torque_2"},
                //    {6, "Torque_3"},
                //};
                //CBTorque_In.ItemsSource = Torque_In;
                //CBTorque_In.SelectedValuePath = "Key";
                //CBTorque_In.DisplayMemberPath = "Value";
                ////设置Torque_Out1ComboBox
                //Dictionary<int, string> Torque_Out1 = new Dictionary<int, string>()
                //{
                //    {4, "Torque_1"},
                //    {5, "Torque_2"},
                //    {6, "Torque_3"},
                //};
                //CBTorque_Out1.ItemsSource = Torque_Out1;
                //CBTorque_Out1.SelectedValuePath = "Key";
                //CBTorque_Out1.DisplayMemberPath = "Value";
                ////设置Torque_Out2ComboBox
                //Dictionary<int, string> Torque_Out2 = new Dictionary<int, string>()
                //{
                //    {4, "Torque_1"},
                //    {5, "Torque_2"},
                //    {6, "Torque_3"},
                //};
                //CBTorque_Out2.ItemsSource = Torque_Out2;
                //CBTorque_Out2.SelectedValuePath = "Key";
                //CBTorque_Out2.DisplayMemberPath = "Value";
                ////设置Speed_InComboBox
                //Dictionary<int, string> Speed_In = new Dictionary<int, string>()
                //{
                //    {10, "Speed_1"},
                //    {11, "Speed_2"},
                //    {12, "Speed_3"},
                //};
                //CBSpeed_In.ItemsSource = Speed_In;
                //CBSpeed_In.SelectedValuePath = "Key";
                //CBSpeed_In.DisplayMemberPath = "Value";
                ////设置Speed_Out1ComboBox
                //Dictionary<int, string> Speed_Out1 = new Dictionary<int, string>()
                //{
                //    {10, "Speed_1"},
                //    {11, "Speed_2"},
                //    {12, "Speed_3"},
                //};
                //CBSpeed_Out1.ItemsSource = Speed_Out1;
                //CBSpeed_Out1.SelectedValuePath = "Key";
                //CBSpeed_Out1.DisplayMemberPath = "Value";
                ////设置Torque_Out2ComboBox
                //Dictionary<int, string> Speed_Out2 = new Dictionary<int, string>()
                //{
                //    {10, "Speed_1"},
                //    {11, "Speed_2"},
                //    {12, "Speed_3"},
                //};
                //CBSpeed_Out2.ItemsSource = Speed_Out2;
                //CBSpeed_Out2.SelectedValuePath = "Key";
                //CBSpeed_Out2.DisplayMemberPath = "Value";
                ////设置定义接口的初始值
                //CBP_Line.SelectedIndex = InitPort[0];
                //CBP_Coolant.SelectedIndex = InitPort[1];
                //CBM_Coolant.SelectedIndex = InitPort[2];
                //CBT_Coolant.SelectedIndex = InitPort[3];
                //CBTorque_In.SelectedIndex = InitPort[4];
                //CBTorque_Out1.SelectedIndex = InitPort[5];
                //CBTorque_Out2.SelectedIndex = InitPort[6];
                //CBSpeed_In.SelectedIndex = InitPort[7];
                //CBSpeed_Out1.SelectedIndex = InitPort[8];
                //CBSpeed_Out2.SelectedIndex = InitPort[9];
                //CBP_1.SelectedIndex = InitPort[10];
                //CBP_2.SelectedIndex = InitPort[11];
                //CBP_3.SelectedIndex = InitPort[12];
                //CBP_4.SelectedIndex = InitPort[13];
                //CBP_5.SelectedIndex = InitPort[14];

        //    #endregion 接口定义初始化
        }

        private void ListenMainPageStats(object source, System.Timers.ElapsedEventArgs e)
        {
            //var app = Application.Current as App;
            //if (app != null && app.MainPageExit)
            //{
            //    ((App) Application.Current).TcpipSetting = false;
            //    ((App) Application.Current).MainPageExit = false;
            //    if (IniAdmin() == true)
            //    {
            //        t.Stop();
            //        //  t = null;
            //        //  throw new NotImplementedException();
            //        ((App) Application.Current).datapro.SendTCData("06", "00001"); //关闭前的时候禁用下位机旋钮
            //        if (MCUConnectNotice != null)
            //        {
            //            MCUConnectNotice.Abort();
            //            MCUConnectNotice = null;
            //        }


            //        // Thread.Sleep(500);
            //        DateTime nowtime = DateTime.Now;
            //        while (nowtime.AddMilliseconds(500) > DateTime.Now)
            //        {
            //        }
            //        //Thread.Sleep(500);
            //    }
            //}
        }

        /// <summary>
        /// MCU连接异常
        /// </summary>
        private void MCUConnectException()
        {
            //if (((App) Application.Current).datapro != null)
            //{
            //    while (!((App) Application.Current).datapro.McuConStats() && !MCUConExFlag)
            //    {
            //        if (DateTime.Now > MCUConnectExTime.AddSeconds(3))
            //        {
            //            if (MessageBox.Show("下位机连接异常！请中止！", "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
            //            {
            //                if (MCUConnectNotice != null)
            //                {
            //                    MCUConnectNotice.Abort();
            //                    MCUConnectNotice = null;
            //                }


            //                MCUConExFlag = true;
            //            }
            //        }
            //    }
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] PortDefineValue = new string[15];
            string SavePath = Directory.GetCurrentDirectory() + "/txt文件/PortDefine.txt";
            PortDefineValue[0] = CBP_Line.SelectedValue.ToString();
            PortDefineValue[1] = CBP_Coolant.SelectedValue.ToString();
            PortDefineValue[2] = CBM_Coolant.SelectedValue.ToString();
            PortDefineValue[3] = CBT_Coolant.SelectedValue.ToString();
            PortDefineValue[4] = CBTorque_In.SelectedValue.ToString();
            PortDefineValue[5] = CBTorque_Out1.SelectedValue.ToString();
            PortDefineValue[6] = CBTorque_Out2.SelectedValue.ToString();
            PortDefineValue[7] = CBSpeed_In.SelectedValue.ToString();
            PortDefineValue[8] = CBSpeed_Out1.SelectedValue.ToString();
            PortDefineValue[9] = CBSpeed_Out2.SelectedValue.ToString();
            PortDefineValue[10] = CBP_1.SelectedValue.ToString();
            PortDefineValue[11] = CBP_2.SelectedValue.ToString();
            PortDefineValue[12] = CBP_3.SelectedValue.ToString();
            PortDefineValue[13] = CBP_4.SelectedValue.ToString();
            PortDefineValue[14] = CBP_5.SelectedValue.ToString();

            StreamWriter swStream;
            if (File.Exists(SavePath))
            {
                swStream = new StreamWriter(SavePath);
            }
            else
            {
                swStream = File.CreateText(SavePath);
            }
            for (int i = 0; i < 15; i++)
            {
                swStream.Write(PortDefineValue[i]);
                swStream.WriteLine();
            }
            swStream.Flush();
            swStream.Close();
        }

        private string[] strInsert;

        private void TCUMSG(string[] data)
        {
            strInsert = data;

            //显示接口定义数值
            outputtextBoxP_Line(strInsert[0]);
            outputtextBoxP_Coolant(strInsert[1]);
            outputtextBoxM_Coolant(strInsert[2]);
            outputtextBoxT_Coolant(strInsert[3]);
            outputtextBoxTorque_In(strInsert[4]);
            outputtextBoxTorque_Out1(strInsert[5]);
            outputtextBoxTorque_Out2(strInsert[6]);
            outputtextBoxSpeed_In(strInsert[10]);
            outputtextBoxSpeed_Out1(strInsert[11]);
            outputtextBoxSpeed_Out2(strInsert[12]);
            outputtextBoxP_1(strInsert[16]);
            outputtextBoxP_2(strInsert[17]);
            outputtextBoxP_3(strInsert[18]);
            outputtextBoxP_4(strInsert[19]);
            outputtextBoxP_5(strInsert[20]);
        }

        #region 显示接口定义数值

        /// <summary>
        /// textBoxP_Line
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_Line(string msg)
        {
            this.textBoxP_Line.Dispatcher.Invoke(new outputDelegate(textBoxP_LineAction), msg);
        }

        private void textBoxP_LineAction(string msg)
        {
            textBoxP_Line.Text = msg;
        }

        /// <summary>
        /// textBoxP_Line
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_Coolant(string msg)
        {
            this.textBoxP_Coolant.Dispatcher.Invoke(new outputDelegate(textBoxP_CoolantAction), msg);
        }

        private void textBoxP_CoolantAction(string msg)
        {
            textBoxP_Coolant.Text = msg;
        }

        /// <summary>
        /// textBoxM_Coolant
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxM_Coolant(string msg)
        {
            this.textBoxM_Coolant.Dispatcher.Invoke(new outputDelegate(textBoxM_CoolantAction), msg);
        }

        private void textBoxM_CoolantAction(string msg)
        {
            textBoxM_Coolant.Text = msg;
        }

        /// <summary>
        /// textBoxT_Coolant
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxT_Coolant(string msg)
        {
            this.textBoxT_Coolant.Dispatcher.Invoke(new outputDelegate(textBoxT_CoolantAction), msg);
        }

        private void textBoxT_CoolantAction(string msg)
        {
            textBoxT_Coolant.Text = msg;
        }

        /// <summary>
        /// textBoxTorque_In
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxTorque_In(string msg)
        {
            this.textBoxTorque_In.Dispatcher.Invoke(new outputDelegate(textBoxTorque_InAction), msg);
        }

        private void textBoxTorque_InAction(string msg)
        {
            textBoxTorque_In.Text = msg;
        }

        /// <summary>
        /// textBoxTorque_Out1
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxTorque_Out1(string msg)
        {
            this.textBoxTorque_Out1.Dispatcher.Invoke(new outputDelegate(textBoxTorque_Out1Action), msg);
        }

        private void textBoxTorque_Out1Action(string msg)
        {
            textBoxTorque_Out1.Text = msg;
        }

        /// <summary>
        /// textBoxTorque_Out2
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxTorque_Out2(string msg)
        {
            this.textBoxTorque_Out2.Dispatcher.Invoke(new outputDelegate(textBoxTorque_Out2Action), msg);
        }

        private void textBoxTorque_Out2Action(string msg)
        {
            textBoxTorque_Out2.Text = msg;
        }

        /// <summary>
        /// textBoxSpeed_In
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxSpeed_In(string msg)
        {
            this.textBoxSpeed_In.Dispatcher.Invoke(new outputDelegate(textBoxSpeed_InAction), msg);
        }

        private void textBoxSpeed_InAction(string msg)
        {
            textBoxSpeed_In.Text = msg;
        }

        /// <summary>
        /// textBoxSpeed_Out1
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxSpeed_Out1(string msg)
        {
            this.textBoxSpeed_Out1.Dispatcher.Invoke(new outputDelegate(textBoxSpeed_Out1Action), msg);
        }

        private void textBoxSpeed_Out1Action(string msg)
        {
            textBoxSpeed_Out1.Text = msg;
        }

        /// <summary>
        /// textBoxSpeed_Out2
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxSpeed_Out2(string msg)
        {
            this.textBoxSpeed_Out2.Dispatcher.Invoke(new outputDelegate(textBoxSpeed_Out2Action), msg);
        }

        private void textBoxSpeed_Out2Action(string msg)
        {
            textBoxSpeed_Out2.Text = msg;
        }

        /// <summary>
        /// textBoxP_1
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_1(string msg)
        {
            this.textBoxP_1.Dispatcher.Invoke(new outputDelegate(textBoxP_1Action), msg);
        }

        private void textBoxP_1Action(string msg)
        {
            textBoxP_1.Text = msg;
        }

        /// <summary>
        /// textBoxP_2
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_2(string msg)
        {
            this.textBoxP_2.Dispatcher.Invoke(new outputDelegate(textBoxP_2Action), msg);
        }

        private void textBoxP_2Action(string msg)
        {
            textBoxP_2.Text = msg;
        }

        /// <summary>
        /// textBoxP_3
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_3(string msg)
        {
            this.textBoxP_3.Dispatcher.Invoke(new outputDelegate(textBoxP_3Action), msg);
        }

        private void textBoxP_3Action(string msg)
        {
            textBoxP_3.Text = msg;
        }

        /// <summary>
        /// textBoxP_4
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_4(string msg)
        {
            this.textBoxP_4.Dispatcher.Invoke(new outputDelegate(textBoxP_4Action), msg);
        }

        private void textBoxP_4Action(string msg)
        {
            textBoxP_4.Text = msg;
        }

        /// <summary>
        /// textBoxP_5
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_5(string msg)
        {
            this.textBoxP_5.Dispatcher.Invoke(new outputDelegate(textBoxP_5Action), msg);
        }

        private void textBoxP_5Action(string msg)
        {
            textBoxP_5.Text = msg;
        }

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {
            //    throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            //   throw new NotImplementedException();
        }

        //UserControl入口
        public void OnNavigatedTo(NavigationEventArgs e)
        {


                //throw new NotImplementedException();

                t.Elapsed += new System.Timers.ElapsedEventHandler(ListenMainPageStats); //到达时间的时候执行事件；
                t.AutoReset = true; //设置是执行一次（false）还是一直执行(true)；
                t.Enabled = true; //是否执行System.Timers.Timer.Elapsed事件；
                //DataPro.ConnectDataProcess(2, false, false);
                MCUConnectExTime = DateTime.Now;
                MCUConnectNotice = new Thread(MCUConnectException);
                MCUConnectNotice.IsBackground = true;
                MCUConnectNotice.Start();
                textBoxTorque_In.Text = "";
                textBoxTorque_Out1.Text = "";
                textBoxTorque_Out2.Text = "";
                textBoxP_Line.Text = "";
                textBoxP_Coolant.Text = "";
                textBoxP_1.Text = "";
                textBoxP_3.Text = "";
                textBoxP_5.Text = "";
                textBoxSpeed_In.Text = "";
                textBoxSpeed_Out1.Text = "";
                textBoxSpeed_Out2.Text = "";
                textBoxM_Coolant.Text = "";
                textBoxT_Coolant.Text = "";
                textBoxP_2.Text = "";
                textBoxP_4.Text = "";
        }

        //UserControl出口
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            t.Stop();
                if (MCUConnectNotice != null)
                {
                    MCUConnectNotice.Abort();
                    MCUConnectNotice = null;
                }
                // Thread.Sleep(500);

            //    throw new NotImplementedException();
        }

        #endregion 显示接口定义数值
    }
}