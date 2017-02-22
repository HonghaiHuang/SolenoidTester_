using FirstFloor.ModernUI.Windows.Controls;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Gauge;
using System.Data;
using System.Windows.Threading;
using System.Collections;
using System.Diagnostics;
using Telerik.Windows.Data;
using System.Threading;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using SolenoidTester.DataClass;
using ThreadState = System.Threading.ThreadState;
using System.ComponentModel;
//public delegate void TestEventDelegate(object sender, SystemX.EventArgs e);

/// <summary>
/// 中止测试
/// </summary>
public delegate void StopTestEventHandler();

namespace SolenoidTester.group.Tests
{

    /// <summary>
    /// Interaction logic for TestStandard.xaml
    /// </summary>
    public partial class SingleTest : UserControl, IContent, Observer
    {

        /// <summary>
        /// TCU刷写完成委托事件
        /// </summary>
        public event StopTestEventHandler StopTest;
        /// <summary>
        /// 测试温度
        /// </summary>
        string[] TCUtemperature = new string []{ "0","10"};


        /// <summary>
        /// 实例化定时器1
        /// </summary>
        DispatcherTimer Timer1;

        /// <summary>
        /// 是否正在测试
        /// </summary>
        private bool IsTesting = false;

        /// <summary>
        /// 保存获取到excel的表单
        /// </summary>
        private DataTable _excelDt;

        /// <summary>
        /// 提示栏内容
        /// </summary>
        private string _Prompts;

        /// <summary>
        /// 测试流程ID
        /// </summary>
        private int _autoTestID = 0;

        /// <summary>
        /// 输入测试模块
        /// </summary>
        private string _testModule;

        /// <summary>
        /// 开启自动测试线程
        /// </summary>
        private Thread _autoTestThread;

        /// <summary>
        /// 更新队列
        /// </summary>
        Queue MyQData = new Queue();

        /// <summary>
        /// 测试开始时间
        /// </summary>
        System.DateTime _timeStar;


        private int[] SpeedDtData = new int[6];


        /// <summary>
        /// 数据库名称
        /// </summary>
        private string MySql_database = "";






        private volatile string[] _dataNew = new string[32];
        public string[] DataNew
        {
            get { return _dataNew; }
            set { _dataNew = value; }
        }






        /// <summary>
        /// 存储数据流数据，用于更新数据库
        /// </summary>
        private string[,] _database = new string[5, 32];


        /// <summary>
        /// 数据更新计数，偶数更新界面
        /// </summary>
        private int DataCount = 0;


        /// <summary>
        /// 更新数据流标志位,数据库更新
        /// </summary>
        private bool dataUpdate_database = false;



        /// <summary>
        /// 构造函数
        /// </summary>
        public SingleTest()
        {
            InitializeComponent();

            try
            {

                //   TestReportAlgorithm();

                //
                //是否正常关闭窗口
                (Application.Current as App).IsNormallyClosedWindow = false;
               
                _excelDt = ImportExcel.InsernExcelFile("AutoTest", ((App)Application.Current).FilePath);
                Timer1_Initialization();
                InitMySql();

                //  _excelDt = ImportExcel.InsernExcelFile("测试报告", "");
                InitdataPro();



            }

            //    for (int kk = 0; kk < 5; kk++)
            //    {
            //        for (int j = 0; j < 32; j++)
            //        {
            //            ddddd[kk, j] = "00"+kk.ToString();
            //        }

            //    }

            //    InitMySql();
            //        Timer1_Initialization();
            //    MySQL_client.connetstring();

            //}
            catch (Exception df)
            { MessageBox.Show(df.Message); }




        }




      //  string[,] ddddd = new string[5, 32];









        /// <summary>
        /// 变量初始化
        /// </summary>
        private Style gearChart_Current;
        private Style gearChart;


        private void Style_Initialization()
        {
            gearChart_Current = Application.Current.Resources["gearChart_Current"] as Style;
            gearChart = Application.Current.Resources["gearChart"] as Style;
            TatusMaxNum.Text= _excelDt.Rows.Count.ToString();
        }


        /// <summary>
        /// 周期性扫描定时器
        /// </summary>
        private void Timer1_Initialization()
        {
            //初始化
            Style_Initialization();
            Timer1 = new DispatcherTimer();
            Timer1.Tick += Timer_Tick1;
            Timer1.Tick += (this.DataContext as LiveDataViewModel).OnTimer;
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 50);//每一轮判断周期为100ms      
            Timer1.Start();
        }
        /// <summary>
        /// 周期性更新数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick1(object sender, object e)
        {
            //    inputSpeed.VauleTxt = DataNew[0];
            ////  _UpdataToUI(DataNew);
            //try
            //{
            //    System.DateTime daeee = System.DateTime.Now;
            //    MySQL_client.InitHashtabletomysql(ddddd);
            ////    MySQL_client.Inser_data(ddddd);
            //    Step.Text = (daeee - System.DateTime.Now).TotalMilliseconds.ToString();
            //    Debug.WriteLine("数据库更新时间：" + (daeee - System.DateTime.Now).TotalMilliseconds);
            //}
            //catch(Exception ex)
            //{ MessageBox.Show(ex.Message); }


            //if (MySQL_client.Inser_data_2(ddddd) == false)
            //{
            //    MessageBox.Show("数据库存储异常！");





            try
            {
                (this.DataContext as LiveDataViewModel)._data = DataNew;

                TimeConsuming();

                SysTime.Text = System.DateTime.Now.ToString("HH:mm.ss");
            }
            catch(Exception errormsg)
            { MessageBox.Show("Timer stop!"+ errormsg.Message); }





            //      Debug.WriteLine(System.DateTime.Now.ToString("HH:mm.ss.fff"));











        }



        /// <summary>
        /// 测试总耗时
        /// </summary>
        private void TimeConsuming()
        {
            if(_autoTestThread!=null)
            {
                if (_timeStar != null &&_autoTestThread.IsAlive == true)
                {
                    TimeSpan tt = DateTime.Now - _timeStar;
                    if (tt.Hours < 10)
                        TimeH.Text = "0" + tt.Hours.ToString();
                    else
                        TimeH.Text = tt.Hours.ToString();

                    if (tt.Minutes < 10)
                        TimeM.Text = "0" + tt.Minutes.ToString();
                    else
                        TimeM.Text = tt.Minutes.ToString();
                    if (tt.Seconds < 10)
                        TimeS.Text = "0" + tt.Seconds.ToString();
                    else
                        TimeS.Text = tt.Seconds.ToString();
                }
            }
        }


        /// <summary>
        /// 测试状态
        /// </summary>
        private void Status_Init(int num, string Test7)
        {


            switch (num)
            {
                case 0:
                    Status.Visibility = System.Windows.Visibility.Visible;
                    Status_processing.Visibility = System.Windows.Visibility.Collapsed;
                    Status_abnormal.Visibility = System.Windows.Visibility.Collapsed;
                    Status_complete.Visibility = System.Windows.Visibility.Collapsed;
                    Status.Text = "就绪";
                    Test_Led.Source = new BitmapImage(new Uri("/Assets/led_default.png", UriKind.RelativeOrAbsolute)); break;
                case 1:
                    Status.Visibility = System.Windows.Visibility.Collapsed;
                    Status_processing.Visibility = System.Windows.Visibility.Visible;
                    Status_abnormal.Visibility = System.Windows.Visibility.Collapsed;
                    Status_complete.Visibility = System.Windows.Visibility.Collapsed;
                    Status_processing.Text = "测试中";
                    Test_Led.Source = new BitmapImage(new Uri("/Assets/led_pass.png", UriKind.RelativeOrAbsolute));
                    break;
                case 2:
                    Status.Visibility = System.Windows.Visibility.Collapsed;
                    Status_processing.Visibility = System.Windows.Visibility.Collapsed;
                    Status_abnormal.Visibility = System.Windows.Visibility.Visible;
                    Status_complete.Visibility = System.Windows.Visibility.Collapsed;
                    Status_abnormal.Text = Test7;
                    Test_Led.Source = new BitmapImage(new Uri("/Assets/led_fail.png", UriKind.RelativeOrAbsolute));
                    break;
                case 3:
                    Status.Visibility = System.Windows.Visibility.Collapsed;
                    Status_processing.Visibility = System.Windows.Visibility.Collapsed;
                    Status_abnormal.Visibility = System.Windows.Visibility.Collapsed;
                    Status_complete.Visibility = System.Windows.Visibility.Visible;
                    Status_complete.Text = "测试完成";
                    Test_Led.Source = new BitmapImage(new Uri("/Assets/led_pass.png", UriKind.RelativeOrAbsolute));
                    break;
                case 4:
                    Status.Visibility = System.Windows.Visibility.Visible;
                    Status_processing.Visibility = System.Windows.Visibility.Collapsed;
                    Status_abnormal.Visibility = System.Windows.Visibility.Collapsed;
                    Status_complete.Visibility = System.Windows.Visibility.Collapsed;
                    Status.Text = "";
                    Test_Led.Source = new BitmapImage(new Uri("/Assets/led_pass.png", UriKind.RelativeOrAbsolute));
                    break;
                default:
                    break;
            }
        }



        /// <summary>
        /// 委托更新数据到界面
        /// </summary>
        private void _UpdataToUI(string[,] _updata_data)
        {
            //
            this.P_Source.Dispatcher.Invoke
                                        (
                                        (
                                        delegate
                                        {
                                            P_Source.VauleTxt = _updata_data[4, 0];
                                        }
                                        ));

            //
            this.InputSpeed.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            InputSpeed.VauleTxt = _updata_data[4, 9];
                                        }
                                        );

            //
            this.OutputSpeed.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            OutputSpeed.VauleTxt = _updata_data[4, 10];
                                        }
                                        );

            //
            this.temperature.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            temperature.VauleTxt = _updata_data[4, 8];
                                        }
                                        );

            //InputSpeed_TCU
            this.InputSpeed_TCU.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            InputSpeed_TCU.VauleTxt = _updata_data[4, 19];
                                        }
                                        );

            //OutputSpeed_TCU
            this.OutputSpeed_TCU.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            OutputSpeed_TCU.VauleTxt = _updata_data[4, 20];
                                        }
                                        );

            //
            this.Temperature_TCU.Dispatcher.Invoke
                                        (

                                        delegate
                                        {
                                            Temperature_TCU.VauleTxt = _updata_data[4, 22];
                                        }
                                        );
            #region 档位
            //档位
















            #endregion
            #region 档位
            /*************档位*************/


            string _PRND = _updata_data[4, 21];
            if (_PRND == "P档" || _PRND == "R档" || _PRND == "N档" || _PRND == "D档")//PRND档位
            {


                //文本
                this.Gear.Dispatcher.Invoke
                                            (

                                            delegate
                                            {
                                                Gear.Text = _PRND;
                                            }
                                            );




                if (_PRND == "P档")
                {
                    //   PRND_P.Style = gearChart_Current;
                    //  PRND_R.Style = gearChart;
                    this.PRND_P.Dispatcher.Invoke
                   (

                   delegate
                   {
                       PRND_P.Style = gearChart_Current;
                   }
                   );


                    this.PRND_R.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_R.Style = gearChart;
                }
                );



                    this.PRND_N.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_N.Style = gearChart;
                }
                );



                    this.PRND_D.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_D.Style = gearChart;
                }
                );



                    //  PRND_N.Style = gearChart;
                    //  PRND_D.Style = gearChart;
                }
                if (_PRND == "R档")
                {
                    this.PRND_P.Dispatcher.Invoke
                   (

                   delegate
                   {
                       PRND_P.Style = gearChart;
                   }
                   );


                    this.PRND_R.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_R.Style = gearChart_Current;
                }
                );



                    this.PRND_N.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_N.Style = gearChart;
                }
                );



                    this.PRND_D.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_D.Style = gearChart;
                }
                );

                }




                if (_PRND == "N档")
                {
                    this.PRND_P.Dispatcher.Invoke
                   (

                   delegate
                   {
                       PRND_P.Style = gearChart;
                   }
                   );


                    this.PRND_R.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_R.Style = gearChart;
                }
                );



                    this.PRND_N.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_N.Style = gearChart_Current;
                }
                );



                    this.PRND_D.Dispatcher.Invoke
                (

                delegate
                {
                    PRND_D.Style = gearChart;
                }
                );
                }
                if (_PRND == "D档")
                {
                    {
                        this.PRND_P.Dispatcher.Invoke
                       (

                       delegate
                       {
                           PRND_P.Style = gearChart;
                       }
                       );


                        this.PRND_R.Dispatcher.Invoke
                    (

                    delegate
                    {
                        PRND_R.Style = gearChart;
                    }
                    );



                        this.PRND_N.Dispatcher.Invoke
                    (

                    delegate
                    {
                        PRND_N.Style = gearChart;
                    }
                    );



                        this.PRND_D.Dispatcher.Invoke
                    (

                    delegate
                    {
                        PRND_D.Style = gearChart_Current;
                    }
                    );
                    }

                }
            }
            else
            {
                //文本
                this.Gear.Dispatcher.Invoke
                                            (

                                            delegate
                                            {
                                                Gear.Text = _PRND;
                                            }
                                            );


                this.PRND_P.Dispatcher.Invoke
                   (

                   delegate
                   {
                       PRND_P.Style = gearChart;
                   }
                   );


                this.PRND_R.Dispatcher.Invoke
            (

            delegate
            {
                PRND_R.Style = gearChart;
            }
            );



                this.PRND_N.Dispatcher.Invoke
            (

            delegate
            {
                PRND_N.Style = gearChart;
            }
            );



                this.PRND_D.Dispatcher.Invoke
            (

            delegate
            {
                PRND_D.Style = gearChart;
            }
            );
            }



            #endregion

            #region 压力开关
            string Pressure_switch_value = _updata_data[4, 23];
            if (Pressure_switch_value != "" && Pressure_switch_value != null)
            {
                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 4)
                {
                    if (Pressure_switch_value.Substring(3, 1) == "1")
                    {
                        this.Pressure_switch_image1.Dispatcher.Invoke
                            (

                            delegate
                            {
                                Pressure_switch_image4.Source = new BitmapImage(new Uri("/Assets/light_on.png", UriKind.RelativeOrAbsolute));
                            }
                            );
                    }
                    else
                    {
                        this.Pressure_switch_image4.Dispatcher.Invoke
                      (

                      delegate
                      {
                          Pressure_switch_image4.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                      }
                      );
                    }

                }
                else
                {
                    this.Pressure_switch_image4.Dispatcher.Invoke
                    (

                    delegate
                    {
                        Pressure_switch_image4.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                    }
                    );


                }


                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 3)
                {
                    if (Pressure_switch_value.Substring(2, 1) == "1")
                    {
                        this.Pressure_switch_image1.Dispatcher.Invoke
                            (

                            delegate
                            {
                                Pressure_switch_image1.Source = new BitmapImage(new Uri("/Assets/light_on.png", UriKind.RelativeOrAbsolute));
                            }
                            );
                    }
                    else
                    {
                        this.Pressure_switch_image1.Dispatcher.Invoke
                      (

                      delegate
                      {
                          Pressure_switch_image1.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                      }
                      );
                    }

                }
                else
                {
                    this.Pressure_switch_image1.Dispatcher.Invoke
                    (

                    delegate
                    {
                        Pressure_switch_image1.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                    }
                    );


                }


                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 2)
                {
                    if (Pressure_switch_value.Substring(1, 1) == "1")
                    {
                        this.Pressure_switch_image3.Dispatcher.Invoke
                            (

                            delegate
                            {
                                Pressure_switch_image3.Source = new BitmapImage(new Uri("/Assets/light_on.png", UriKind.RelativeOrAbsolute));
                            }
                            );
                    }
                    else
                    {
                        this.Pressure_switch_image3.Dispatcher.Invoke
                      (

                      delegate
                      {
                          Pressure_switch_image3.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                      }
                      );
                    }

                }
                else
                {
                    this.Pressure_switch_image3.Dispatcher.Invoke
                    (

                    delegate
                    {
                        Pressure_switch_image3.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                    }
                    );


                }










                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 1)
                {
                    if (Pressure_switch_value.Substring(0, 1) == "1")
                    {
                        this.Pressure_switch_image2.Dispatcher.Invoke
                            (

                            delegate
                            {
                                Pressure_switch_image2.Source = new BitmapImage(new Uri("/Assets/light_on.png", UriKind.RelativeOrAbsolute));
                            }
                            );
                    }
                    else
                    {
                        this.Pressure_switch_image2.Dispatcher.Invoke
                      (

                      delegate
                      {
                          Pressure_switch_image2.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                      }
                      );
                    }

                }
                else
                {
                    this.Pressure_switch_image2.Dispatcher.Invoke
                    (

                    delegate
                    {
                        Pressure_switch_image2.Source = new BitmapImage(new Uri("/Assets/light_off.png", UriKind.RelativeOrAbsolute));
                    }
                    );


                }


            }



            #endregion

            #region 柱状图
            this.Tcccon1_1.Dispatcher.Invoke
                            (

                            delegate
                            {
                                Tcccon1_1.Value = Convert.ToDouble(_updata_data[4, 1]);
                            }
                            );
            this.Tcccon1_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon1_2.Text = _updata_data[4, 1];
                }
                );


            this.Tcccon2_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon2_1.Value = Convert.ToDouble(_updata_data[4, 2]);
                }
                );
            this.Tcccon2_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon2_2.Text = _updata_data[4, 2];
                }
                );


            this.Tcccon3_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon3_1.Value = Convert.ToDouble(_updata_data[4, 3]);
                }
                );
            this.Tcccon3_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon3_2.Text = _updata_data[4, 3];
                }
                );


            this.Tcccon4_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon4_1.Value = Convert.ToDouble(_updata_data[4, 4]);
                }
                );
            this.Tcccon4_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon4_2.Text = _updata_data[4, 4];
                }
                );


            this.Tcccon5_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon5_1.Value = Convert.ToDouble(_updata_data[4, 5]);
                }
                );
            this.Tcccon5_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon5_2.Text = _updata_data[4, 5];
                }
                );


            this.Tcccon6_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon6_1.Value = Convert.ToDouble(_updata_data[4, 6]);
                }
                );
            this.Tcccon6_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon6_2.Text = _updata_data[4, 6];
                }
                );



            this.Tcccon7_1.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon7_1.Value = Convert.ToDouble(_updata_data[4, 7]);
                }
                );
            this.Tcccon7_2.Dispatcher.Invoke
                (

                delegate
                {
                    Tcccon7_2.Text = _updata_data[4, 7];
                }
                );


            #endregion

            #region 换挡电磁阀
            //if((Convert.ToDouble(_updata_data[4, 3])>30&& Convert.ToDouble(_updata_data[4, 3]) < 900))
            //{
            //         this.solenoid_stateOn.Dispatcher.Invoke
            //        (

            //        delegate
            //        {
            //            solenoid_stateOn.Visibility = Visibility.Collapsed;
            //        }
            //        );

            //       this.solenoid_stateOff.Dispatcher.Invoke
            //        (

            //        delegate
            //        {
            //            solenoid_stateOff.Visibility = Visibility.Collapsed;
            //        }
            //        );


            //}
         //   else
         //   {
                if (Convert.ToDouble(_updata_data[4, 3]) < 400)
                {
                    this.solenoid_stateOn.Dispatcher.Invoke
                   (

                   delegate
                   {
                       solenoid_stateOn.Visibility = Visibility.Collapsed;
                   }
                   );

                    this.solenoid_stateOff.Dispatcher.Invoke
                     (

                     delegate
                     {
                         solenoid_stateOff.Visibility = Visibility.Visible;
                     }
                     );

                }
                if (Convert.ToDouble(_updata_data[4, 3]) >= 400)
                {
                    this.solenoid_stateOn.Dispatcher.Invoke
                   (

                   delegate
                   {
                       solenoid_stateOn.Visibility = Visibility.Visible;
                   }
                   );

                    this.solenoid_stateOff.Dispatcher.Invoke
                     (

                     delegate
                     {
                         solenoid_stateOff.Visibility = Visibility.Collapsed;
                     }
                     );


             //   }
            }
            #endregion


        }




        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Star_Button_Click(object sender, RoutedEventArgs e)
        {
            try {
                TCUtemperature[0]= Temperature_TCU.VauleTxt;
            MySQL_client.IsSaveDataToDatabase = true;
                IsTesting = true;
            _MySQL_client.InitdataPro();
            AutoTestThread();
            Status_Init(1, "");
            Star_Button.Visibility = Visibility.Collapsed;
            Stop_Button.Visibility = Visibility.Visible;
            Report_Button.Visibility = Visibility.Collapsed;
            }
            catch(Exception exceptionMsg)
            {
                MessageBox.Show(exceptionMsg.Message);
            }
        }

        /// <summary>
        /// 停止按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (Application.Current as App).dataPro.StopTest();
                /*********************************/
                /*********油泵上电执行接口********/
                /*********************************/
            (Application.Current as App).dataPro.OilPumpControl(false);
            MySQL_client.IsSaveDataToDatabase = false;

            Thread.Sleep(100);
            /**************************************/
            /*********7个电磁阀同时无压接口********/
            /**************************************/
            (Application.Current as App).dataPro.SolenoidPressureControl(false);
                Thread.Sleep(100);
                (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, false);
                IsTesting = false;
            _MySQL_client.RemoveMySQLdataPro();
            Status_Init(0, "");            
            Star_Button.Visibility = Visibility.Visible;
            Stop_Button.Visibility = Visibility.Collapsed;
            Report_Button.Visibility = Visibility.Collapsed;
            _autoTestThread.Abort();
            }
            catch (Exception exceptionMsg)
            {
                if (exceptionMsg.Message != "正在中止线程。" && exceptionMsg.Message != "线程被挂起；正在尝试中止。")
                {
                    MessageBox.Show(exceptionMsg.Message);
                }
            }

        }









        // 声明  
        public class BeepUp
        {
            /// <param name="iFrequency">声音频率（从37Hz到32767Hz）。在windows95中忽略</param>  
            /// <param name="iDuration">声音的持续时间，以毫秒为单位。</param>  
            [System.Runtime.InteropServices.DllImport("Kernel32.dll")] //引入命名空间 using System.Runtime.InteropServices;  
            public static extern bool Beep(int frequency, int duration);
        }


        /// <summary>
        /// 测试完成
        /// </summary>
        private void TestComplete()
        {
            try
            {

                /*********************************/
                /*********油泵上电执行接口********/
                /*********************************/
                (Application.Current as App).dataPro.OilPumpControl(false);

                IsTesting = false;


                this.Temperature_TCU.Dispatcher.BeginInvoke
    (
    new Action(
                    delegate
                    {
                        TCUtemperature[1] = Temperature_TCU.VauleTxt;
                    })
);



                (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, false);

                for (int i = 3; i > 0; i--)
            {             // 调用  
                BeepUp.Beep(500, 700); // 这个声音还不错  
                Thread.Sleep(300);
            }

            /**************************************/
            /*********7个电磁阀同时无压接口********/
            /**************************************/
            (Application.Current as App).dataPro.SolenoidPressureControl(false);

            MySQL_client.IsSaveDataToDatabase = false;
            _MySQL_client.RemoveMySQLdataPro();

            this.Star_Button.Dispatcher.Invoke
                (
                delegate
                {
                    Star_Button.Visibility = Visibility.Collapsed;
                }
                );
                        this.Stop_Button.Dispatcher.Invoke
                (

                delegate
                {
                    Stop_Button.Visibility = Visibility.Collapsed;
                }
                );
            this.Report_Button.Dispatcher.Invoke
                (

                delegate
                {
                    Report_Button.Visibility = Visibility.Visible;
                }
                );

            _autoTestThread.Abort();
            }
            catch (Exception exceptionMsg)
            {
                if (exceptionMsg.Message != "正在中止线程。" && exceptionMsg.Message != "线程被挂起；正在尝试中止。")
                {
                    MessageBox.Show(exceptionMsg.Message);
                }
            }

        }

        #region 流程控制

        /// <summary>
        /// 自动测试线程
        /// </summary>
        private void AutoTestThread()
        {
            _autoTestID = 0;
            _autoTestThread = new Thread(new ThreadStart(StartAutoTestThread));
            _autoTestThread.IsBackground = true;
            _autoTestThread.Priority = ThreadPriority.BelowNormal;
            _autoTestThread.Start();
        }


        private static ManualResetEvent _eventWorkList = new ManualResetEvent(false);

        /// <summary>
        /// 自动测试线程执行函数
        /// </summary>
        private void StartAutoTestThread()
        {


            _timeStar = System.DateTime.Now;      //开始时间
            /*********************************/
            /*********油泵上电执行接口********/
            /*********************************/
            (Application.Current as App).dataPro.OilPumpControl(true);


            while (true)
            {
             //   Thread.Sleep(100);
                //(this.DataContext as LiveDataViewModel).IsupdataChar = true;
                // LiveDataViewModel.CallEvenHandle.Invoke();
                try
                {
                _Prompts = Convert.ToString(_excelDt.Rows[_autoTestID]["提示栏内容"]);
                
                
                Thread.Sleep(200);
                WriteStepText("TestStep", Convert.ToString(_excelDt.Rows[_autoTestID]["测试项目"]));
                WriteStepText("Step", _Prompts);
                WriteStepText("TatusNum", _autoTestID.ToString());
                _testModule = Convert.ToString(_excelDt.Rows[_autoTestID]["测试代号"]);
                (Application.Current as App).dataPro.TextItemNumber(_testModule);
                SwitchTestModule(_testModule);
                _autoTestID++;
                    

                    Debug.WriteLine("跑完一个测试项" + System.DateTime.Now.ToString("mm:ss.fff"));
                if (_autoTestID >= 64)
                {
                    _autoTestID = 0;
                    TestComplete();
                    break;
                }
                }
                catch(Exception ExceptionMsg)
                {
                    if (ExceptionMsg.Message != "正在中止线程。")
                    {
                        MessageBox.Show(ExceptionMsg.Message);
                    }
                }

            }
        }

        /// <summary>
        /// 界面UI与线程交互
        /// </summary>
        /// <param name="para"></param>
        private void WriteStepText(string WhereValue, string para)
        {
            switch (WhereValue)
            {
                case "Step":
                    this.Step.Dispatcher.BeginInvoke
                        (
                        new Action(
                                        delegate
                                        {
                                            Step.Text = para;
                                        })
                  );
                    break;
                case "TestStep":
                    this.TestStep.Dispatcher.BeginInvoke
                    (
                    new Action(
                    delegate
                    {
                        TestStep.Text = para;
                    })
               );
                    break;

                case "TatusNum":
                    this.Step.Dispatcher.BeginInvoke
    (
    new Action(
                    delegate
                    {
                        TatusNum.Text = para;
                    })
);
                    break;

            }
        }

        /// <summary>
        /// 测试模块分支
        /// </summary>
        /// <param name="TestModule"></param>
        private void SwitchTestModule(string TestModule)
        {
            if (TestModule != "" && TestModule != null)
            {
                switch (TestModule.Substring(0, 1))
                {
                    case "A":
                        PreheatingProcess(TestModule);
                        break;
                    case "B":
                        PressureSwitchTestModule(TestModule);
                        break;
                    case "C":
                        ElectromagneticValveTestModule(TestModule);
                        break;
                    case "D":                        
                        ShiftPerformanceTest(TestModule);
                        break;


                    case "G":
                        PressureSwitchFunctionTest(TestModule);
                        break;
                    case "H":
                        PressureSwitchPerformanceTesting(TestModule);
                        break;
                    case "I":
                        PressureSwitchFunctionTest(TestModule);
                        break;
                }
            }
        }

        /// <summary>
        /// 预热流程模块和档位测试模块
        /// </summary>
        private void PreheatingProcess(string TestModule)
        {
            if (TestModule == "A00")
            {
                double k = 100;

                /***************************************/
                /*********打开电热丝加热执行接口********/
                /***************************************/
                (Application.Current as App).dataPro.HeatingWireControl(true);
                Debug.WriteLine("打开电热丝加热" + System.DateTime.Now.ToString("mm:ss.fff"));


                while (k >= Convert.ToDouble(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("预热油温=", "")))
                {
                    k = k - 1;
                    Thread.Sleep(100);
                }

                /***************************************/
                /*********关闭电热丝加热执行接口********/
                /***************************************/
                (Application.Current as App).dataPro.HeatingWireControl(false);
                Debug.WriteLine("关闭电热丝加热" + System.DateTime.Now.ToString("mm:ss.fff"));



                WriteStepText("Step", "温度到达,准备进入下一测试项");
                //等待预热时间
                Thread.Sleep(Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("预热时间=", "")));
            }
            else
            {
                /*******档位测试*******/
                if (TestModule != "A05" && TestModule != "A06")
                {
                   
                    System.DateTime time = System.DateTime.Now;
                    while (IsTimeOfArrival(time, Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("等待时间=", ""))))
                    {
                        Thread.Sleep(50);//不能让占用资源
                        if (_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("当前档位=", "") == "D")
                        {
                            WriteStepText("Step", "");
                        }
                        else
                        {
                            WriteStepText("Step", Convert.ToString(_excelDt.Rows[_autoTestID]["提示栏内容"]));
                        }
                    }
                }

                /*******转速端口设置*******/
                else
                {
                        //当前转速
                        int _currentSpeed = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("起始转速=", ""));
                        while (_currentSpeed <= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数2"].ToString().Replace("结束转速=", "")))
                        {
                        /*****************************/
                        /*********转速执行接口********/
                        /*****************************/
                        if (TestModule == "A05")
                        {

                            (Application.Current as App).dataPro.AdjustInputSpeed(_currentSpeed);
                            Debug.WriteLine("执行转速：" + _currentSpeed + "时间：" + System.DateTime.Now.ToString("mm:ss.fff"));
                            WriteStepText("Step", "输入转速：" + _currentSpeed);
                        }
                        if (TestModule == "A06")
                        {

                            (Application.Current as App).dataPro.AdjustOutputSpeed(_currentSpeed);
                            Debug.WriteLine("执行转速：" + _currentSpeed + "时间：" + System.DateTime.Now.ToString("mm:ss.fff"));


                            WriteStepText("Step", "输出转速：" + _currentSpeed);
                        }
                        //保持时间
                        Thread.Sleep(Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("保持时间=", "")));
                         _currentSpeed = _currentSpeed + Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("变化步长=", ""));
                        }


                     ((App) Application.Current).dataPro.AdjustInputSpeed(0);
                    ((App) Application.Current).dataPro.AdjustOutputSpeed(0);


                }
            }
        }

        /// <summary>
        /// 压力开关功能测试模块
        /// </summary>
        private void PressureSwitchTestModule(string TestModule)
        {
            /*********************************/
            /*********压力开关执行接口********/
            /*********************************/
            switch (TestModule)
            {
                case "B00":
                    //   (Application.Current as App).dataPro.SolenoidHalfPressureControl(40);
                    (Application.Current as App).dataPro.SolenoidPressureControl(true);
                    Thread.Sleep(100);
                    (Application.Current as App).dataPro.AdjustSolenoid("EPC", Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("EPC=", "")));
                    Thread.Sleep(100);
                    (Application.Current as App).dataPro.AdjustTCCEPCPressure(2000, Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("EPC=", "")));
                    Thread.Sleep(100);
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, false);
                    break;
                case "B01":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(true, false, false, false);

                    break;
                case "B02":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(true, true, false, false);

                    break;
                case "B03":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(true, true, true, false);
                    break;
                case "B04":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(true, true, true, true);
                    break;
                case "B05":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, true, true, true);
                    break;
                case "B06":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, true, true);
                    break;
                case "B07":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, true);
                    break;
                case "B08":
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, false);
                    break;

            }

            Thread.Sleep(1000);
        }

        /// <summary>
        /// 电磁阀测试模块
        /// </summary>
        private void ElectromagneticValveTestModule(string TestModule)
        {
            //电磁阀测试预备阶段
            if (TestModule == "C00")
            {
                
             //   (Application.Current as App).dataPro.SwitchSolenoidControl(true, false, false, false);
            //    Thread.Sleep(1000);
                for (int i = 0; i < Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("变化次数=", "")); i++)
                {
                    if (i % 2 == 0)
                    {
                        /**************************************/
                        /*********7个电磁阀同时全压接口********/
                        /**************************************/
                        (Application.Current as App).dataPro.SolenoidPressureControl(true);
                      //  Debug.WriteLine("7个电磁阀同时全压" + System.DateTime.Now.ToString("mm:ss.fff"));
                        WriteStepText("Step", "7个电磁阀同时全压");

                    }
                    else
                    {
                        /**************************************/
                        /*********7个电磁阀同时无压接口********/
                        /**************************************/
                        (Application.Current as App).dataPro.SolenoidPressureControl(false);
                      //  Debug.WriteLine("7个电磁阀同时无压" + System.DateTime.Now.ToString("mm:ss.fff"));
                        WriteStepText("Step", "7个电磁阀同时无压");

                    }
                    Thread.Sleep(Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("变化间隔时间=", "")));
                }

                //测试准备完成后给全压
                (Application.Current as App).dataPro.SolenoidPressureControl(true);
                //充油时间
                Thread.Sleep(Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("冲油时间=", "")));
            }
            //电磁阀单体测试/阶跃测试
            else
            {
                int param_Step= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("压力变化步长=", ""));
                int param_BeginPress= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("压力变化起点=", ""));
                int param_EndPress= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数2"].ToString().Replace("压力变化终点=", ""));
                int param_KeepTime= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("保持时间=", ""));
                string param_TestEndStatus= _excelDt.Rows[_autoTestID]["可修改参数4"].ToString().Replace("测试完成后状态=", "");

                string param_SolenoidName= _excelDt.Rows[_autoTestID]["可修改参数5"].ToString().Replace("被测电磁阀名称=", "");
                string param_TextItemNumber= TestModule;

                (Application.Current as App).dataPro.SolenoidStepTest(param_Step, param_BeginPress, param_EndPress, param_KeepTime, param_TestEndStatus, param_SolenoidName, param_TextItemNumber);
                _autoTestThread.Suspend();


            }
        }



        private void Adjust2Press(string  Name,int value)
        {
            switch(Name)
                {
                case "EPC":
                    (Application.Current as App).dataPro.AdjustTCCEPCPressure(2000, value);
                    WriteStepText("Step", "电磁阀:"+ Name +"；压力值："+ value);
                    break;
                case "TCC":
                    (Application.Current as App).dataPro.AdjustTCCEPCPressure(value, 2000);
                    WriteStepText("Step", "电磁阀:" + Name + "；压力值：" + value);

                    break;

                case "C1234":
                    (Application.Current as App).dataPro.AdjustC1234C456Pressure(value, 2000);
                    WriteStepText("Step", "电磁阀:" + Name + "；压力值：" + value);

                    break;
                case "CB26":
                    (Application.Current as App).dataPro.AdjustC35RCB26Pressure(2000, value);
                    WriteStepText("Step", "电磁阀:" + Name + "；压力值：" + value);

                    break;


                case "C35R":
                    (Application.Current as App).dataPro.AdjustC35RCB26Pressure(value, 2000);
                    WriteStepText("Step", "电磁阀:" + Name + "；压力值：" + value);

                    break;
                case "C456":
                    (Application.Current as App).dataPro.AdjustC1234C456Pressure(2000, value);
                    WriteStepText("Step", "电磁阀:" + Name + "；压力值：" + value);

                    break;


            }
        }
        /// <summary>
        /// 换挡性能测试
        /// </summary>
        private void ShiftPerformanceTest(string TestModule)
        {

            int param_ShiftInverval= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("换挡间隔时间=", ""));
            int param_Solenoid1_Step= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("第一电磁阀动作步数=", ""));
            int param_Solenoid2_Step= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数2"].ToString().Replace("第二电磁阀动作步数=", ""));
            string param_Solenoid1_Name= _excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("第一操作电磁阀名=", "");
            string param_Solenoid2_Name= _excelDt.Rows[_autoTestID]["可修改参数4"].ToString().Replace("第二操作电磁阀名=", "");
            int param_SendOrderTime= Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数6"].ToString().Replace("时间常量=", ""));


            (Application.Current as App).dataPro.ShiftPerformanceTest(param_ShiftInverval, param_Solenoid1_Step, param_Solenoid2_Step, param_Solenoid1_Name, param_Solenoid2_Name, param_SendOrderTime);
            _autoTestThread.Suspend();
        }

        private void Adjust4Press(string NameH_L, int valueH_L, string NameL_H, int valueL_H)
        {


            if (((NameH_L == "EPC" && NameL_H == "TCC") || (NameH_L == "TCC" && NameL_H == "EPC")) || ((NameH_L == "C1234" && NameL_H == "C456") || (NameH_L == "C456" && NameL_H == "C1234")) || ((NameH_L == "C35R" && NameL_H == "CB26") || (NameH_L == "CB26" && NameL_H == "C35R")))
            {
                switch (NameH_L)
                {
                    case "EPC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(valueL_H, valueH_L);
                        break;
                    case "TCC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(valueH_L, valueL_H);
                        break;

                    case "C1234":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(valueH_L, valueL_H);
                        break;
                    case "CB26":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(valueL_H, valueH_L);
                        break;


                    case "C35R":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(valueH_L, valueL_H);
                        break;
                    case "C456":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(valueL_H, valueH_L);
                        break;
                }
            }

            else
            {
                switch (NameH_L)
                {
                    case "EPC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(800, valueH_L);
                        break;
                    case "TCC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(valueH_L, 800);
                        break;

                    case "C1234":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(valueH_L, 800);
                        break;
                    case "CB26":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(800, valueH_L);
                        break;


                    case "C35R":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(valueH_L, 800);
                        break;
                    case "C456":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(800, valueH_L);
                        break;
                }
                switch (NameL_H)
                {
                    case "EPC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(800, valueL_H);
                        break;
                    case "TCC":
                        (Application.Current as App).dataPro.AdjustTCCEPCPressure(valueL_H, 800);
                        break;

                    case "C1234":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(valueL_H, 800);
                        break;
                    case "CB26":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(800, valueL_H);
                        break;


                    case "C35R":
                        (Application.Current as App).dataPro.AdjustC35RCB26Pressure(valueL_H, 800);
                        break;
                    case "C456":
                        (Application.Current as App).dataPro.AdjustC1234C456Pressure(800, valueL_H);
                        break;
                }

            }
        }

        /// <summary>
        /// 压力开关功能测试
        /// </summary>
        /// <param name="TestModule"></param>
        private void PressureSwitchFunctionTest(string TestModule)
        {
            (Application.Current as App).dataPro.SolenoidPressureControl(true);
            //  Debug.WriteLine("7个电磁阀同时全压" + System.DateTime.Now.ToString("mm:ss.fff"));
            WriteStepText("Step", "7个电磁阀同时全压");

            Thread.Sleep(100);
            (Application.Current as App).dataPro.SwitchSolenoidControl(true, true, true, true);//压力开关全开状态
            Thread.Sleep(100);


            int param_Step = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数0"].ToString().Replace("压力变化步长=", ""));
            int param_BeginPress = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("压力变化起点=", ""));
            int param_EndPress = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数2"].ToString().Replace("压力变化终点=", ""));
            int param_KeepTime = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("保持时间=", ""));
            string param_TestEndStatus = _excelDt.Rows[_autoTestID]["可修改参数4"].ToString().Replace("测试完成后状态=", "");

            string param_SolenoidName = _excelDt.Rows[_autoTestID]["可修改参数5"].ToString().Replace("被测电磁阀名称=", "");
            string param_TextItemNumber = TestModule;

            (Application.Current as App).dataPro.SolenoidStepTest(param_Step, param_BeginPress, param_EndPress, param_KeepTime, param_TestEndStatus, param_SolenoidName, param_TextItemNumber);
            _autoTestThread.Suspend();



        }


        /// <summary>
        /// 压力开关性能测试
        /// </summary>
        /// <param name="TestModule"></param>
        private void PressureSwitchPerformanceTesting(string TestModule)
        {
            string PressureSwitchStar = _excelDt.Rows[_autoTestID]["可修改参数1"].ToString().Replace("初始状态=", "");
            string PressureSwitchStop = _excelDt.Rows[_autoTestID]["可修改参数2"].ToString().Replace("末始状态=", "");
            int param_KeepTime = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数3"].ToString().Replace("保持时间=", ""));
            int TestTime = Convert.ToInt32(_excelDt.Rows[_autoTestID]["可修改参数5"].ToString().Replace("测试时间=", ""));
            System.DateTime Startime = System.DateTime.Now;
            while (true)
            {
                (Application.Current as App).dataPro.SwitchSolenoidControl(Convert.ToBoolean(PressureSwitchStar.Substring(0, 1)) , Convert.ToBoolean(PressureSwitchStar.Substring(1, 1)), Convert.ToBoolean(PressureSwitchStar.Substring(2, 1)), Convert.ToBoolean(PressureSwitchStar.Substring(3, 1)));
                Thread.Sleep(param_KeepTime);
                (Application.Current as App).dataPro.SwitchSolenoidControl(Convert.ToBoolean(Convert.ToInt32(PressureSwitchStop.Substring(0, 1))), Convert.ToBoolean(Convert.ToInt32(PressureSwitchStop.Substring(1, 1))), Convert.ToBoolean(Convert.ToInt32(PressureSwitchStop.Substring(2, 1))), Convert.ToBoolean(Convert.ToInt32(PressureSwitchStop.Substring(3, 1))));
                Thread.Sleep(param_KeepTime);
                if((System.DateTime.Now- Startime).TotalMilliseconds> TestTime)
                { break;}
            }
        }

        

        /// <summary>
        /// 时间定时
        /// </summary>
        /// <param name="time"></param>
        /// <param name="_timeNum"></param>
        /// <returns></returns>
        private bool IsTimeOfArrival(System.DateTime time, int _timeNum)
        {
            if ((System.DateTime.Now - time).TotalMilliseconds >= _timeNum)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion





        #region 图表控件check

        /// <summary>
        /// 图表控件check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void P_Source_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_Source_Check.IsChecked == true)
            {
                this.P_Source_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_Source_Check.IsChecked == false)
            {
                this.P_Source_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void P_TCC_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_TCC_Check.IsChecked == true)
            {
                this.P_TCC_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_TCC_Check.IsChecked == false)
            {
                this.P_TCC_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void P_Line_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_Line_Check.IsChecked == true)
            {
                this.P_Line_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_Line_Check.IsChecked == false)
            {
                this.P_Line_line.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void P_Shift_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_Shift_Check.IsChecked == true)
            {
                this.P_Shift_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_Shift_Check.IsChecked == false)
            {
                this.P_Shift_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void P_C1234_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_C1234_Check.IsChecked == true)
            {
                this.P_C1234_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_C1234_Check.IsChecked == false)
            {
                this.P_C1234_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void P_CB26_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_CB26_Check.IsChecked == true)
            {
                this.P_CB26_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_CB26_Check.IsChecked == false)
            {
                this.P_CB26_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        private void P_C456_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_C456_Check.IsChecked == true)
            {
                this.P_C456_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_C456_Check.IsChecked == false)
            {
                this.P_C456_line.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void P_C35R_Check_Click(object sender, RoutedEventArgs e)
        {
            if (P_C35R_Check.IsChecked == true)
            {
                this.P_C35R_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (P_C35R_Check.IsChecked == false)
            {
                this.P_C35R_line.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void temperature_Check_Click(object sender, RoutedEventArgs e)
        {
            if (temperature_Check.IsChecked == true)
            {
                this.temperature_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (temperature_Check.IsChecked == false)
            {
                this.temperature_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void inputSpeed_Check_Click(object sender, RoutedEventArgs e)
        {
            if (inputSpeed_Check.IsChecked == true)
            {
                this.inputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (inputSpeed_Check.IsChecked == false)
            {
                this.inputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void outputSpeed_Check_Click(object sender, RoutedEventArgs e)
        {
            if (outputSpeed_Check.IsChecked == true)
            {
                this.outputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (outputSpeed_Check.IsChecked == false)
            {
                this.outputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void TCUInputSpeed_Check_Click(object sender, RoutedEventArgs e)
        {
            if (TCUInputSpeed_Check.IsChecked == true)
            {
                this.TCUInputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (TCUInputSpeed_Check.IsChecked == false)
            {
                this.TCUInputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void TCUOnputSpeed_Check_Click(object sender, RoutedEventArgs e)
        {
            if (TCUOnputSpeed_Check.IsChecked == true)
            {
                this.TCUOnputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (TCUOnputSpeed_Check.IsChecked == false)
            {
                this.TCUOnputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void TCUtemperature_Check_Click(object sender, RoutedEventArgs e)
        {
            if (TCUtemperature_Check.IsChecked == true)
            {
                this.TCUtemperature_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (TCUtemperature_Check.IsChecked == false)
            {
                this.TCUtemperature_line.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void GreaNum_Check_Checked(object sender, RoutedEventArgs e)
        {
            if (GreaNum_Check.IsChecked == true)
            {
                this.GreaNum_line.Visibility = System.Windows.Visibility.Visible;
            }
            if (GreaNum_Check.IsChecked == false)
            {
                this.GreaNum_line.Visibility = System.Windows.Visibility.Collapsed;
            }

        }


        private void InitRadCartesianChart()
        {
            this.P_Source_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_TCC_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_Line_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_Shift_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C1234_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_CB26_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C456_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C35R_line.Visibility = System.Windows.Visibility.Collapsed;
            this.temperature_line.Visibility = System.Windows.Visibility.Collapsed;
            this.inputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.outputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.TCUInputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.TCUOnputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.TCUtemperature_line.Visibility = System.Windows.Visibility.Collapsed;
            this.GreaNum_line.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void InitRadCartesianChart2()
        {
            this.P_Source_line.Visibility = System.Windows.Visibility.Visible;
            this.P_TCC_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_Line_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_Shift_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C1234_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_CB26_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C456_line.Visibility = System.Windows.Visibility.Collapsed;
            this.P_C35R_line.Visibility = System.Windows.Visibility.Visible;
            this.temperature_line.Visibility = System.Windows.Visibility.Collapsed;
            this.inputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            this.outputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.TCUInputSpeed_line.Visibility = System.Windows.Visibility.Collapsed;
            this.TCUOnputSpeed_line.Visibility = System.Windows.Visibility.Visible;
            this.TCUtemperature_line.Visibility = System.Windows.Visibility.Visible;
            this.GreaNum_line.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion




        #region 界面导航事件处理
        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            //    throw new NotImplementedException();

        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            //   throw new NotImplementedException();

        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {


        }
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            try
            {
                //  throw new NotImplementedException();
                if (IsTesting == false)
                {
                    RemoveRegister((Application.Current as App).dataPro);
                    _MySQL_client.RemoveMySQLdataPro();
                    ((App)Application.Current).dataPro.SolenoidTest -= new SolenoidTestEventHandler(SolenoidTestIsComplete);
                    ((App)Application.Current).dataPro.ShowTestStep -= new ShowTestStepEventHandler(outputStep);
                    
                    (Application.Current as App).IsNormallyClosedWindow = true;
                    if (Timer1 != null)
                    {
                        Timer1.Stop();
                        Timer1 = null;
                    }

                    Window mainWindow = new Window();
                    mainWindow = Application.Current.MainWindow;
                    mainWindow.Close();
                    var newForm = new MainWindow();
                    newForm.Show();
                    
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("测试中，请中止测试！");

                }
            }catch(Exception errormsg)
            { MessageBox.Show(errormsg.Message); }

        }

        /// <summary>
        /// 注释：释放资源
        /// 作者：黄洪海
        /// 内容：终止线程和释放资源
        /// 时间：2016.06.15
        /// </summary>
        public void TerminateThread()
        {

            //   System.Threading.Thread.Sleep(300);

        }
        #endregion



        #region 处理数据处理模块


        private void SolenoidTestIsComplete(string msg)
        {
                _autoTestThread.Resume();
                // backgroundWorker1_DoWork(_data);


            
        }

        private delegate void outputDelegate(string msg);

        /// <summary>
        /// Step
        /// </summary>
        /// <param name="msg"></param>
        private void outputStep(string msg)
        {
            this.Step.Dispatcher.Invoke(new outputDelegate(StepAction), msg);
        }

        private void StepAction(string msg)
        {
            Step.Text = msg;
        }


        /// <summary>
        /// 数据处理模块接口，更新数据
        /// </summary>
        /// <param name="data"></param>
        public void Update(string[,] data)
        {
            try
            {
                System.DateTime daeee = System.DateTime.Now;
                for (int i = 0; i < 32; i++)
                {
                    DataNew[i] = data[4, i];
                }

                if (DataCount % 2 == 0)
                {
                    //    DataNew = data;
                    _UpdataToUI(data);
                    DataCount = 0;
                }
                DataCount++;


                Debug.WriteLine("UI更新时间：" + System.DateTime.Now.ToString("ss.fff"));
            }
            catch (Exception errormsg)
            { MessageBox.Show("UI updata stop!" + errormsg.Message); }


        }


        /// <summary>
        /// 数据处理模块接口
        /// </summary>
        /// <param name="dataPro"></param>
        public void RegisterObserver(Subject dataPro)
        {
            this.dataPro = dataPro;
            dataPro.RegisterObserver(this);
        }

        /// <summary>
        /// 停止数据更新
        /// </summary>
        /// <param name="dataPro"></param>
        public void RemoveRegister(Subject dataPro)
        {
            this.dataPro = dataPro;
            dataPro.RemoveObserver(this);
        }

        /// <summary>
        /// 继承数据处理模块接口
        /// </summary>
        private Subject dataPro;

        /// <summary>
        /// 初始化数据库处理模块
        /// </summary>
        private void InitdataPro()
        {
            //  (Application.Current as App).dataPro.ConnectCan();
            RegisterObserver((Application.Current as App).dataPro);
            (Application.Current as App).dataPro.CollectMcuData(true);

            ((App) Application.Current).dataPro.SolenoidTest += new SolenoidTestEventHandler(SolenoidTestIsComplete);
            ((App) Application.Current).dataPro.ShowTestStep += new ShowTestStepEventHandler(outputStep);
            ((App) Application.Current).dataPro.AdjustInputSpeed(0);
            ((App) Application.Current).dataPro.AdjustOutputSpeed(0);
            ((App) Application.Current).dataPro.SwitchSolenoidControl(false,false, false, false);
            //    backgroundWorker1.RunWorkerAsync(backgroundWorker1_DoWork);
        }
        #endregion


        #region 处理数据库处理模块

        /// <summary>
        /// 实例化数据库处理模块
        /// </summary>
        MySQL_client _MySQL_client = new MySQL_client();
        /// <summary>
        /// 初始化数据库模块，包括创建数据库，数据表，开启数据存储线程
        /// </summary>
        private void InitMySql()
        {
          //  MySql_database = "fuyuan" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
            MySql_database = ((App) Application.Current)._TestInformation[5]+"_"+ ((App) Application.Current)._TestInformation[2] + "_" + System.DateTime.Now.ToString("yyyyMMddHHmm");//数据库名称
            MySql_database = MySql_database.ToLower();
            string[] _DataColume = new string[] { "ID", "时间戳", "P_Source", "P_TCC", "P_Line", "P_Shift", "P_C1234", "P_CB26", "P_C35R", "P_C456", "temperature", "inputSpeed", "outputSpeed", "scramStatus", "changeValve1Status", "changeValve2Status", "changeValve3Status", "changeValve4Status", "oilPumpStatus", "heatingWireStatus", "TCUPowerStatus", "TCUInputSpeed", "TCUOnputSpeed", "gear", "TCUtemperature", "switchSolenoidStatus", "CANConnectStatus", "testItemNumber", "solenoid1", "solenoid1Pressure", "solenoid2", "solenoid2Pressure", "adjustInpoutSpeed", "adjustOutputSpeed" };
            string[] _TestInformationColume = new string[] { "ID", "操作者", "报告编号", "零件号", "刷写软件号", "商用状态", "制造追踪码", "VIN", "故障码" };
            if (MySQL_client.Showdatabase().Contains<string>(MySql_database) == true)
            {
                MySQL_client.DeleteMySQL(MySql_database.ToLower());
            }
            if (MySQL_client.CreateSQL(MySql_database) == true && MySQL_client.AlterTableExample(MySql_database, "数据流", _DataColume) == true && MySQL_client.AlterTableExample(MySql_database, "TestInformation", _TestInformationColume) == true)
            {
                
                _MySQL_client.InsertTestInformationToTable(((App) Application.Current)._TestInformation);
                MySQL_client.connetstring();
                //  Star_Insert_data_to_MySql();
            }
           
        }

        ///// <summary>
        ///// 初始化MySql更新数据线程
        ///// </summary>
        //private void Star_Insert_data_to_MySql()
        //{
        //    Thread MySql_Massage_Theard = new Thread(MySqlTheard);
        //    MySql_Massage_Theard.IsBackground = true;
        //    MySql_Massage_Theard.Start();
        //}

        ///// <summary>
        ///// 开启线程
        ///// </summary>
        //private void MySqlTheard()
        //{
        //    string[] data_database = new string[32];
        //    System.DateTime dkkd;
        //    while (true)
        //    {
        //        if(dataUpdate_database== false)
        //        {
        //            dkkd= System.DateTime.Now;
        //            for(int i=0;i<5;i++)
        //            {
        //                for(int j=0;j<32;j++)
        //                {
        //                    data_database[j]= _database[i,j];
        //                }
        //                _MySQL_client.Inser_data(data_database);
        //            }
        //            dataUpdate_database = false;
        //            Debug.WriteLine("5组数据时间"+(System.DateTime.Now- dkkd).TotalMilliseconds);
        //        }
        //    }
        //}

        #endregion

        private void Report_Button_Click(object sender, RoutedEventArgs e)
        {



            RemoveRegister((Application.Current as App).dataPro);
            Thread _TestReportTestThread = new Thread(new ThreadStart(ThreadTestReport));
            _TestReportTestThread.IsBackground = true;
            //_TestReportTestThread.Priority = ThreadPriority.BelowNormal;
            _TestReportTestThread.Start();
            _TestReportTestThread.Join();
            if (_ExportExcel.Isexporttoexcel == true)
            {
                MessageBox.Show("导出测试报告成功。", "提示", MessageBoxButton.OK);
            }
            else
            { MessageBox.Show("导出测试报告失败。", "提示", MessageBoxButton.OK); }


            Star_Button.Visibility = Visibility.Visible;
            Stop_Button.Visibility = Visibility.Collapsed;
            Report_Button.Visibility = Visibility.Collapsed;
            Status_Init(3,"");
        }
        ExportExcel _ExportExcel;
        private void ThreadTestReport()
        {
            try
            {
               _ExportExcel = new ExportExcel();
                if(((App)Application.Current).FilePath != System.IO.Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_PressureSwitchTest.xls")
                {
                    if(((App)Application.Current).FilePath== System.IO.Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_TestStandard.xls")
                    {
                        _ExportExcel.exporttoexcel(TestReportAlgorithm(), SpeedTestData, UnitTesting_data, MySql_database, TCUtemperature, StepTest_data, StepTest_Instruction, solenoid1Data, solenoid2Data);
                    }
                    else
                    {
                        _ExportExcel.UnitTesting(TestReportAlgorithm(), SpeedTestData, UnitTesting_data, MySql_database, TCUtemperature, StepTest_data, StepTest_Instruction, solenoid1Data, solenoid2Data);
                      
                    }
                }
                else
                {
                    _ExportExcel.exporttoexcelSwipress(TestReportAlgorithm(), MySql_database);
                }
            }
            catch (Exception excelerr0)
            {
                MessageBox.Show(excelerr0.Message);
            }

        }
        /// <summary>
        /// 测试报告算法
        /// </summary>
        private string[] TestReportAlgorithm()
        {
            //存储excel模块table数据
            DataTable _TestReportDt = new DataTable("_TestReportDt");

            //存储数据库数据流表筛选出来的数据
            DataTable _MySqlFilterData = new DataTable("_MySqlFilterData");
            _TestReportDt = ImportExcel.InsernExcelFile("测试报告", "");

            string[] _dataValue = new string[_TestReportDt.Rows.Count];

            for (int i = 0; i < _TestReportDt.Rows.Count; i++)
            {

                string _TestNum = _TestReportDt.Rows[i]["测试代号"].ToString();
                string _ParameterName = _TestReportDt.Rows[i]["参数名称"].ToString();
                string _CalibrationValue = _TestReportDt.Rows[i]["标定值"].ToString();
                //Getdataset(MySql_database, SQL.GetDataSet("Database=ceshi; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan", CommandType.Text, "select * from 数据流 where ID>'100' and ID<'1000'", null).Tables[0]);
                if (_TestNum != "" && _TestNum != null)
                {
                     //  MySql_database = "dv4420731316l786_4420_201609271216";
                    _MySqlFilterData = MySQL_client.GetDataSet("Database=" + MySql_database + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan", CommandType.Text, "select " + _ParameterName + " from 数据流 where testItemNumber = '" + _TestNum + "'", null).Tables[0];

                    if (_TestNum == "A01" || _TestNum == "A02" || _TestNum == "A03" || _TestNum == "A04")
                    {
                        _dataValue[i]=GearTestAlgorithm(_TestReportDt, _MySqlFilterData, i);//档位测试
                    }

                    if(_TestNum == "A05"|| _TestNum == "A06")
                    {
                        _dataValue[i] = SpeedAlgorithm(_TestReportDt, _MySqlFilterData, i);
                    }

                    if (_TestNum == "B01" || _TestNum == "B02" || _TestNum == "B03" || _TestNum == "B04"|| _TestNum == "B05" || _TestNum == "B06" || _TestNum == "B07" || _TestNum == "B08")
                    {
                        _dataValue[i] = PressureSwitchTestAlgorithm(_TestReportDt, _MySqlFilterData, i);//压力开关测试
                    }
                    if (_TestNum.Substring(0, 1) == "C")
                    {
                        UnitTesting(_TestReportDt, _MySqlFilterData, i);//单体测试
                    }

                    if (_TestNum.Substring(0, 1) == "D")
                    {
                        ShiftPerformanceTest(_TestReportDt, _MySqlFilterData, i);//换挡性能测试
                    }

                    if (_TestNum.Substring(0, 1) == "G" || _TestNum.Substring(0, 1) == "I")
                    {
                        _dataValue[i]=PressureSwitchTestAlgorithm_data(_TestReportDt, _MySqlFilterData, i);
                    }
                }
            }
            return _dataValue;
        }

        /// <summary>
        /// 档位测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        private string GearTestAlgorithm(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
            string GearTestValue = "";
            int _GearCount = 0;
            for (int i = _MySqlFilterData.Rows.Count - 10; i < _MySqlFilterData.Rows.Count; i++)
            {
                if (_MySqlFilterData.Rows[i][0].ToString() == _TestReportDt.Rows[_iCount]["标定值"].ToString())
                {
                    _GearCount++;
                }
                else
                {
                    _GearCount--;
                }
            }
            if (_GearCount == 10)
            { GearTestValue = "正常"; }
            else
            { GearTestValue = "不正常"; }
            return GearTestValue;

        }




        /// <summary>
        /// 压力开关测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        private string PressureSwitchTestAlgorithm(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
        string PressureSwitchTestValue = "";
        int _PressureSwitchCount = 0;
            if (_MySqlFilterData.Rows.Count > 10)
            {
                for (int i = _MySqlFilterData.Rows.Count - 10; i < _MySqlFilterData.Rows.Count; i++)
                {
                    //        string ddd = _MySqlFilterData.Rows[i][0].ToString().Substring(0, 4);
                    //      string ddd2 = _TestReportDt.Rows[_iCount]["标定值"].ToString();




                    if (_MySqlFilterData.Rows[i][0].ToString().Substring(0, 4) == _TestReportDt.Rows[_iCount]["标定值"].ToString())
                    {
                        _PressureSwitchCount++;
                    }
                    else
                    {
                        _PressureSwitchCount--;
                    }
                }

                if (_PressureSwitchCount == 10)
                { PressureSwitchTestValue = "正常"; }
                else
                { PressureSwitchTestValue = "不正常"; }
            }
            else
            { PressureSwitchTestValue = "不正常"; }
            return PressureSwitchTestValue;
        }


        /// <summary>
        /// 转速调节报告算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        /// <returns></returns>
        private string SpeedAlgorithm(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
                if (_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "A05")
                {

                    for (int k = 0; k < _excelDt.Rows.Count; k++)
                    {
                        if (Convert.ToString(_excelDt.Rows[k]["测试代号"]) == "A05")
                        {
                        
                            SpeedDtData[0] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数1"].ToString().Replace("起始转速=", ""));
                            SpeedDtData[1] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数2"].ToString().Replace("结束转速=", ""));
                            SpeedDtData[2] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数0"].ToString().Replace("变化步长=", ""));
                        SpeedTestData = new string[4, SpeedDtData[1]/ SpeedDtData[2]];

                        SpeedTestValue[0] = "合格";
                        SpeedTestValue[1] = "合格";

                    }
                        if (Convert.ToString(_excelDt.Rows[k]["测试代号"]) == "A06")
                        {
                            SpeedDtData[3] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数1"].ToString().Replace("起始转速=", ""));
                            SpeedDtData[4] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数2"].ToString().Replace("结束转速=", ""));
                            SpeedDtData[5] = Convert.ToInt32(_excelDt.Rows[k]["可修改参数0"].ToString().Replace("变化步长=", ""));
                        }

                    }





                for (int i = SpeedDtData[0]; i <= SpeedDtData[1];)
                    {
                      
                        DataTable FileterDt = FileterDatatableGerr(_MySqlFilterData, "adjustInpoutSpeed", i);
                        if (FileterDt.Rows.Count > 1)
                        {
                        SpeedTestData[0,  (i / SpeedDtData[2])-1] = FileterDt.Rows[FileterDt.Rows.Count - 1][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("adjustInpoutSpeed,", "")].ToString();
                          if (Math.Abs(Convert.ToDouble(FileterDt.Rows[FileterDt.Rows.Count - 1][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("adjustInpoutSpeed,", "")]) - i) <= Convert.ToDouble(_TestReportDt.Rows[_iCount]["标定值"].ToString()))
                            {
                                SpeedTestData[1, (i / SpeedDtData[5]) - 1] = "合格";
                            }
                          else
                            {
                            SpeedTestData[1, (i / SpeedDtData[5]) - 1] = "不合格";
                            SpeedTestValue[0] = "不合格";
                            // break;
                        }
                        }
                        else
                        {
                        SpeedTestData[1, (i / SpeedDtData[5]) - 1] = "不合格，指令发送不全。" ;
                          //  break;
                        };
                    i = i + SpeedDtData[2];
                }
                return SpeedTestValue[0];

            }
            if (_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "A06")
            {
                for (int i = SpeedDtData[3];i <= SpeedDtData[4];)
                {
                   
                    DataTable FileterDt = FileterDatatableGerr(_MySqlFilterData, "adjustOutputSpeed", i);
                    if (FileterDt.Rows.Count > 1)
                    {
                        SpeedTestData[2, (i / SpeedDtData[5])-1] = FileterDt.Rows[FileterDt.Rows.Count - 1][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("adjustOutputSpeed,", "")].ToString();
                        if (Math.Abs(Convert.ToDouble(FileterDt.Rows[FileterDt.Rows.Count - 1][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("adjustOutputSpeed,", "")]) - i) <= Convert.ToDouble(_TestReportDt.Rows[_iCount]["标定值"].ToString()))
                        {
                            SpeedTestData[3, (i / SpeedDtData[5]) - 1] = "合格";
                        }
                        else
                        {
                            SpeedTestData[3, (i / SpeedDtData[5]) - 1] = "不合格";
                            SpeedTestValue[1] = "不合格";
                            // break;
                        }
                    }
                    else
                    {
                        SpeedTestData[3, (i / SpeedDtData[5]) - 1] = "不合格，指令发送不全。";
                      //  break;
                    };
                    i = i + SpeedDtData[5];
                }

                return SpeedTestValue[1];

            }

            return null;
        }


        /// <summary>
        /// 转速测试结果
        /// </summary>
        private string[] SpeedTestValue = new string[2];

        /// <summary>
        /// 转速测试值
        /// </summary>
        private string[,] SpeedTestData ;


        /// <summary>
        /// 单体测试结果
        /// </summary>
        private string[,] UnitTesting_data = new string[14, 800 / 25];

        /// <summary>
        /// 换挡电磁阀测试结果
        /// </summary>
        private int[] ShiftingSolenoidDtData = new int[6];

        /// <summary>
        /// 阶跃测试结果
        /// </summary>
        private string[,] StepTest_data=null;

        /// <summary>
        /// 阶跃测试指令
        /// </summary>
        private string[,] StepTest_Instruction = null;

        /// <summary>
        /// 换挡性能测试电磁阀1数据
        /// </summary>
        private string[,] solenoid1Data = new string[10,300];

        /// <summary>
        /// 换挡性能测试电磁阀2数据
        /// </summary>
        private string[,] solenoid2Data = new string[10, 300];


        /// <summary>
        /// 单体测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        /// <returns></returns>
        private string UnitTesting(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
            if(_TestReportDt.Rows[_iCount]["测试代号"].ToString()=="C01"|| _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C02" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C03" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C04" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C05" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C06" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C07" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C08" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C09" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C10" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C11" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C12")
            {
                for (int i = 0; i < 800;)
                {
                    i = i + 25;
                    DataTable FileterDt = FileterDatatableGerr(_MySqlFilterData, "solenoid1Pressure", i);
                    if (FileterDt.Rows.Count > 1)
                    {
                        UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, i / 25 - 1] = FileterDt.Rows[FileterDt.Rows.Count - 1][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")].ToString();

                    }
                    else
                    {
                        UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, i / 25 - 1] = "9999";
                        MessageBox.Show("生成测试报告出现问题，位置在：" + _TestReportDt.Rows[_iCount]["测试代号"].ToString() + ";指令值为：" + i);
                    };
                }
            }


            //换挡电磁阀
            if(_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C13" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C14")
            {


                if (_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C13")
                {
                            ShiftingSolenoidDtData[0] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数1"].ToString().Replace("压力变化起点=", ""));
                            ShiftingSolenoidDtData[1] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数2"].ToString().Replace("压力变化终点=", ""));
                            ShiftingSolenoidDtData[2] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数0"].ToString().Replace("压力变化步长=", ""));




                    for (int j = ShiftingSolenoidDtData[0]; j <= ShiftingSolenoidDtData[1];)
                    {
                        DataTable FileterDt = FileterDatatableGerr(_MySqlFilterData, "solenoid1Pressure", j);

                        int FileterDtNum =Convert.ToInt32 ((FileterDt.Rows.Count - 1) *0.8);
                        if (j == ShiftingSolenoidDtData[0])
                        {

                            if (FileterDt.Rows.Count > 1)
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0] = FileterDt.Rows[FileterDtNum][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")].ToString();
                                if ((Convert.ToDouble(UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0]) - Convert.ToDouble(_TestReportDt.Rows[_iCount]["可修改参数4"].ToString().Replace("无压标准值=", ""))) <= 0)
                                {
                                    UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "合格";
                                }
                                else
                                { UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "不合格"; }

                            }
                            else
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0] = "9999";
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "不合格";
                                MessageBox.Show("生成测试报告出现问题，位置在：" + _TestReportDt.Rows[_iCount]["测试代号"].ToString() + ";指令值为：" + j);
                            };
                        }
                        else
                        {
                            if (FileterDt.Rows.Count > 1)
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2] = FileterDt.Rows[FileterDtNum][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")].ToString();
                                if ((Convert.ToDouble(UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2]) - Convert.ToDouble(_TestReportDt.Rows[_iCount]["可修改参数5"].ToString().Replace("全压标准值=", ""))) >= 0)
                                {
                                    UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "合格";
                                }
                                else
                                { UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "不合格"; }

                            }
                            else
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2] = "9999";
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "不合格";
                                MessageBox.Show("生成测试报告出现问题，位置在：" + _TestReportDt.Rows[_iCount]["测试代号"].ToString() + ";指令值为：" + j);
                            };

                        }
                        j = j + ShiftingSolenoidDtData[2];
                    }
                }
                else
                {
                        ShiftingSolenoidDtData[3] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数1"].ToString().Replace("压力变化起点=", ""));
                        ShiftingSolenoidDtData[4] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数2"].ToString().Replace("压力变化终点=", ""));
                        ShiftingSolenoidDtData[5] = Convert.ToInt32(_TestReportDt.Rows[_iCount]["可修改参数0"].ToString().Replace("压力变化步长=", ""));

                    for (int j = ShiftingSolenoidDtData[3]; j >= ShiftingSolenoidDtData[4];)
                    {
                        DataTable FileterDt = FileterDatatableGerr(_MySqlFilterData, "solenoid1Pressure",j);
                        int FileterDtNum2 = Convert.ToInt32((FileterDt.Rows.Count - 1) * 0.8);
                        if (j == ShiftingSolenoidDtData[4])
                        {

                                if (FileterDt.Rows.Count > 1)
                            {
                                    UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0] = FileterDt.Rows[FileterDtNum2][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")].ToString();
                                    if ((Convert.ToDouble(UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0]) - Convert.ToDouble(_TestReportDt.Rows[_iCount]["可修改参数4"].ToString().Replace("无压标准值=", ""))) <= 0)
                                    {
                                        UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "合格";
                                    }
                                    else
                                    { UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "不合格"; }
                            
                             }
                            else
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 0] = "9999";
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 1] = "不合格";
                                MessageBox.Show("生成测试报告出现问题，位置在：" + _TestReportDt.Rows[_iCount]["测试代号"].ToString() + ";指令值为：" + j);
                            };
                        }
                        else
                        {
                            if (FileterDt.Rows.Count > 1)
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2] = FileterDt.Rows[FileterDtNum2][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")].ToString();
                                if ((Convert.ToDouble(UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2]) - Convert.ToDouble(_TestReportDt.Rows[_iCount]["可修改参数5"].ToString().Replace("全压标准值=", ""))) >= 0)
                                {
                                    UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "合格";
                                }
                                else
                                { UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "不合格"; }

                            }
                            else
                            {
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 2] = "9999";
                                UnitTesting_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 1, 3] = "不合格";
                                MessageBox.Show("生成测试报告出现问题，位置在：" + _TestReportDt.Rows[_iCount]["测试代号"].ToString() + ";指令值为：" + j);
                            };

                        }
                        j = j - ShiftingSolenoidDtData[5];
                    }

                }

            }



            //阶跃测试
            if ( _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C15" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C16" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C17" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C18" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C19" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C20" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C21" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C22" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C23" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C24" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C25"||_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C26" )
            {
               
                int dataCount = Convert.ToInt32(_TestReportDt.Rows[_iCount]["标定值"].ToString());
                if (_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C15")
                {
                    StepTest_data = new string[12, dataCount];
                    StepTest_Instruction = new string[12, dataCount];
                }
                double interval = _MySqlFilterData.Rows.Count / dataCount;
                for (int i=0;i< dataCount; i++)
                {                 
                    StepTest_data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 15, i]= Convert.ToString(_MySqlFilterData.Rows[(int) (i * interval)][_TestReportDt.Rows[_iCount]["参数名称"].ToString().Replace("solenoid1Pressure,", "")]);
                    StepTest_Instruction[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("C", "")) - 15, i] = Convert.ToString(_MySqlFilterData.Rows[(int) (i * interval)]["solenoid1Pressure"]);

                }
            }





                return null;
        }



        /// <summary>
        /// 换挡性能测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        private void ShiftPerformanceTest(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
            
          //  DataTable solenoid1Dt = FileterDatatableGerr_string(_MySqlFilterData, "solenoid1", _TestReportDt.Rows[_iCount]["可修改参数0"].ToString());
         //DataTable solenoid2Dt = FileterDatatableGerr_string(_MySqlFilterData, "solenoid1", _TestReportDt.Rows[_iCount]["可修改参数1"].ToString());
            //if (solenoid1Dt.Rows.Count > 0)
            //{
            //    for (int Dt1 = 0;  Dt1 < solenoid1Dt.Rows.Count; Dt1++)
            //    {
            //        solenoid1Data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("D", "")), Dt1] = solenoid1Dt.Rows[Dt1]["P_" + _TestReportDt.Rows[_iCount]["可修改参数0"].ToString()].ToString();
            //    }
            //}
            //if (solenoid2Dt.Rows.Count > 0)
            //{
            //    for (int Dt2 = 0; Dt2 < solenoid2Dt.Rows.Count; Dt2++)
            //    {
            //        solenoid2Data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("D", "")), Dt2] = solenoid2Dt.Rows[Dt2]["P_" + _TestReportDt.Rows[_iCount]["可修改参数1"].ToString()].ToString();
            //    }

            //}



            if (_MySqlFilterData.Rows.Count > 0)
            {
                for (int Dt1 = 0; Dt1 < _MySqlFilterData.Rows.Count; Dt1++)
                {
                    if(Dt1<300)
                    {
                        solenoid1Data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("D", "")), Dt1] = _MySqlFilterData.Rows[Dt1]["P_" + _TestReportDt.Rows[_iCount]["可修改参数0"].ToString()].ToString();
                        solenoid2Data[Convert.ToInt32(_TestReportDt.Rows[_iCount]["测试代号"].ToString().Replace("D", "")), Dt1] = _MySqlFilterData.Rows[Dt1]["P_" + _TestReportDt.Rows[_iCount]["可修改参数1"].ToString()].ToString();
                    }
                }
            }


        }

        /// <summary>
        /// 压力开关测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        private string PressureSwitchTestAlgorithm_data(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
            bool StarCounting = false;
            string data = "无结果";
            for (int Dt1 = 0; Dt1 < _MySqlFilterData.Rows.Count; Dt1++)
            {
                if (StarCounting == false && _MySqlFilterData.Rows[Dt1][0].ToString().Substring(0, 4) == "1111")
                {
                    StarCounting = true;
                }
                if (StarCounting == true)
                {
                    if (
                        _MySqlFilterData.Rows[Dt1][0].ToString()
                            .Substring(Convert.ToInt32(_TestReportDt.Rows[_iCount]["标定值"].ToString()), 1) == "0")
                    {
                        data = _MySqlFilterData.Rows[Dt1][0].ToString();
                        break;
                    }
                }

            }
            return data;
        }

        /// <summary>
        /// Datatable筛选
        /// </summary>
        /// <param name="Gear1"></param>
        /// <param name="Gear2"></param>
        /// <returns></returns>
        private DataTable FileterDatatableGerr(DataTable _ScreeningDt, string _columnName, double _dataValue)
        {
            DataView view = new DataView();
            view.Table = _ScreeningDt;
          //  DataTable B = new DataTable();
            //      view.RowFilter = "Beat >= '" + Gear1 + "' and Beat <= '" + Gear2 + "'";//itemType是A中的一个字段
            view.RowFilter = _columnName + " = '" + _dataValue + "'";//itemType是A中的一个字段
         //   B = view.ToTable();
            return view.ToTable(); ;
        }


        /// <summary>
        /// Datatable筛选
        /// </summary>
        /// <param name="Gear1"></param>
        /// <param name="Gear2"></param>
        /// <returns></returns>
        private DataTable FileterDatatableGerr_string(DataTable _ScreeningDt, string _columnName, string _dataValue)
        {
            DataView view = new DataView();
            view.Table = _ScreeningDt;
            //  DataTable B = new DataTable();
            //      view.RowFilter = "Beat >= '" + Gear1 + "' and Beat <= '" + Gear2 + "'";//itemType是A中的一个字段
            view.RowFilter = _columnName + " = '" + _dataValue + "'";//itemType是A中的一个字段
                                                                     //   B = view.ToTable();
            return view.ToTable(); ;
        }


    }
}





