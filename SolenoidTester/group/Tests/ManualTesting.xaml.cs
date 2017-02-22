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

namespace SolenoidTester.group.Tests
{

    /// <summary>
    /// Interaction logic for TestStandard.xaml
    /// </summary>
    public partial class ManualTesting : UserControl, IContent, Observer
    {

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
        public ManualTesting()
        {
            InitializeComponent();
            //设置P_4ComboBox
            Dictionary<int, string> method = new Dictionary<int, string>()
                {
                    {0, "低到高"},
                    {1, "高到低"},
                };
            CB_TestMethod.ItemsSource = method;
            CB_TestMethod.SelectedValuePath = "Key";
            CB_TestMethod.DisplayMemberPath = "Value";
            CB_TestMethod.Text = "低到高";

            Dictionary<int, string> solenoid = new Dictionary<int, string>()
                {
                    {0, "TCC"},
                    {1, "EPC"},
                    {2, "C1234"},
                    {3, "C456"},
                    {4, "C35R"},
                    {5, "CB26"},
                };
            CB_Solenoid.ItemsSource = solenoid;
            CB_Solenoid.SelectedValuePath = "Key";
            CB_Solenoid.DisplayMemberPath = "Value";
            CB_Solenoid.Text = "TCC";
            
            //   TestReportAlgorithm();


            Timer1_Initialization();
          //  InitMySql();
          //  _excelDt = ImportExcel.InsernExcelFile("AutoTest", "");
            //   _excelDt = ImportExcel.InsernExcelFile("测试报告", "");
            InitdataPro();
        }


        /// <summary>
        /// 变量初始化
        /// </summary>
        private Style gearChart_Current;
        private Style gearChart;


        private void Style_Initialization()
        {
            gearChart_Current = Application.Current.Resources["gearChart_Current"] as Style;
            gearChart = Application.Current.Resources["gearChart"] as Style;

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
            Timer1.Interval = new TimeSpan(0, 0, 0, 0, 100);//每一轮判断周期为100ms      
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
            //  _UpdataToUI(DataNew);
            (this.DataContext as LiveDataViewModel)._data = DataNew;

            //TimeConsuming();

            SysTime.Text = System.DateTime.Now.ToString("HH:mm.ss");
        }



        /// <summary>
        /// 测试总耗时
        /// </summary>
        /*private void TimeConsuming()
        {
            if (_autoTestThread != null)
            {
                if (_timeStar != null && _autoTestThread.IsAlive == true)
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
        }*/


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
                    Status_complete.Text = "完成";
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
                        this.Pressure_switch_image4.Dispatcher.Invoke
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


                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 2)
                {
                    if (Pressure_switch_value.Substring(1, 1) == "1")
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










                if (Convert.ToString(Convert.ToInt32(Pressure_switch_value), 2).Length >= 1)
                {
                    if (Pressure_switch_value.Substring(0, 1) == "1")
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

            #endregion


        }




        /// <summary>
        /// 开始按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Star_Button_Click(object sender, RoutedEventArgs e)
        {
                
            if (CB_Solenoid.SelectedIndex >= 0)
            {
                if (CB_TestMethod.SelectedIndex == 0)
                {
                    tb_TextPress.Text = "25";
                    testMethod = true;
                }
                else
                {
                    tb_TextPress.Text = "775";
                    testMethod = false;
                }
                
                EnterKeyCount = 0;
                StartPerformanceTest = true;
                Star_Button.Visibility = Visibility.Collapsed;
                Stop_Button.Visibility = Visibility.Visible;
                CB_Solenoid.IsEnabled = false;
                CB_TestMethod.IsEnabled = false;
                outputStep("");
                IsTesting = true;
            }
            else
            {
                MessageBox.Show("请选择电磁阀");
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
                tb_TextPress.Text = "";
                EnterKeyCount = 0;
                StartPerformanceTest = false;
                Star_Button.Visibility = Visibility.Visible;
                Stop_Button.Visibility = Visibility.Collapsed;
                CB_Solenoid.IsEnabled = true;
                CB_TestMethod.IsEnabled = true;
                outputStep("");
                IsTesting = false;
                
            }
            catch (Exception exceptionMsg)
            {
                MessageBox.Show(exceptionMsg.Message);
            }

        }














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
                    (Application.Current as App).dataPro.SolenoidPressureControl(false);
                    Thread.Sleep(100);
                    (Application.Current as App).dataPro.SwitchSolenoidControl(false, false, false, false);
                    RemoveRegister((Application.Current as App).dataPro);
                    (Application.Current as App).IsNormallyClosedWindow = true;
                    if(Timer1 != null)
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
            }
            catch (Exception errormsg)
            { MessageBox.Show(errormsg.Message); }





            //  throw new NotImplementedException();
            //if (_autoTestThread != null)
            //{
            //    if (_autoTestThread.IsAlive == true)
            //    { e.Cancel = true; }
            //    else
            //    {
            //        MessageBox.Show("测试中，请中止测试！");
            //    }
            //}

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





        /// <summary>
        /// 数据处理模块接口，更新数据
        /// </summary>
        /// <param name="data"></param>
        public void Update(string[,] data)
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











            //Task.Run(() =>
            //{
            //    // backgroundWorker1_DoWork(_data);

            //});

            //    dataUpdate = true;
            //   dataUpdate_database = true;





            //            this.InputSpeed.Dispatcher.Invoke
            //    (
            //    new Action(
            //                    delegate
            //                    {
            //                        InputSpeed.VauleTxt = data[4, 0];
            //                    })
            //);


            //  backgroundWorker1.RunWorkerAsync(2000/*参数是可选的*/); 


            Debug.WriteLine("UI更新时间：" + System.DateTime.Now.ToString("ss.fff"));
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

          //  ((App) Application.Current).dataPro.SolenoidTest += new SolenoidTestEventHandler(SolenoidTestIsComplete);
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
            MySql_database = ((App) Application.Current)._TestInformation[5] + "_" + ((App) Application.Current)._TestInformation[2] + "_" + System.DateTime.Now.ToString("yyyyMMddHHmm");//数据库名称
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
                //  Star_Insert_data_to_MySql();
            }

        }


        #endregion

        private void Report_Button_Click(object sender, RoutedEventArgs e)
        {




            RemoveRegister((Application.Current as App).dataPro);
            Thread _TestReportTestThread = new Thread(new ThreadStart(ThreadTestReport));
            _TestReportTestThread.IsBackground = true;

            //     _TestReportTestThread.Priority = ThreadPriority.BelowNormal;
            _TestReportTestThread.Start();
            _TestReportTestThread.Join();
            Star_Button.Visibility = Visibility.Visible;
            Stop_Button.Visibility = Visibility.Collapsed;
            Report_Button.Visibility = Visibility.Collapsed;


        }



        private void ThreadTestReport()
        {
            try
            {
                ExportExcel _ExportExcel = new ExportExcel();
           //     _ExportExcel.exporttoexcel(TestReportAlgorithm(), UnitTesting_data, MySql_database);
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
                    //   MySql_database = "dv6524756216f1gv_24245322_201609182042";
                    _MySqlFilterData = MySQL_client.GetDataSet("Database=" + MySql_database + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan", CommandType.Text, "select " + _ParameterName + " from 数据流 where testItemNumber = '" + _TestNum + "'", null).Tables[0];

                    if (_TestNum == "A01" || _TestNum == "A02" || _TestNum == "A03" || _TestNum == "A04")
                    {
                        _dataValue[i] = GearTestAlgorithm(_TestReportDt, _MySqlFilterData, i);//档位测试
                    }
                    if (_TestNum == "B01" || _TestNum == "B02" || _TestNum == "B03" || _TestNum == "B04")
                    {
                        _dataValue[i] = PressureSwitchTestAlgorithm(_TestReportDt, _MySqlFilterData, i);//压力开关测试
                    }
                    if (_TestNum.Substring(0, 1) == "C")
                    {
                        UnitTesting(_TestReportDt, _MySqlFilterData, i);//单体测试
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

            for (int i = _MySqlFilterData.Rows.Count - 10; i < _MySqlFilterData.Rows.Count; i++)
            {
                string ddd = _TestReportDt.Rows[_iCount]["标定值"].ToString();
                if (_MySqlFilterData.Rows[i][0].ToString() == _TestReportDt.Rows[_iCount]["标定值"].ToString())
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
            return PressureSwitchTestValue;
        }


        private string[,] UnitTesting_data = new string[12, 800 / 25];


        /// <summary>
        /// 单体测试算法
        /// </summary>
        /// <param name="_TestReportDt"></param>
        /// <param name="_MySqlFilterData"></param>
        /// <param name="_iCount"></param>
        /// <returns></returns>
        private string UnitTesting(DataTable _TestReportDt, DataTable _MySqlFilterData, int _iCount)
        {
            if (_TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C01" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C02" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C03" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C04" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C05" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C06" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C07" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C08" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C09" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C10" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C11" || _TestReportDt.Rows[_iCount]["测试代号"].ToString() == "C12")
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

            return null;
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
        /// TCC全压标志位
        /// </summary>
        private bool TCC_TotalPress = true;

        private void btn_TCC_Press_Click(object sender, RoutedEventArgs e)
        {
            if (TCC_TotalPress)
            {
                btn_TCC_Press.Content = "无       压";
                TCC_TotalPress = false;
                (Application.Current as App).dataPro.AdjustTCCEPCPressure(2000, !EPC_TotalPress ? 2000 : 0);
            }
            else if (!TCC_TotalPress)
            {
                btn_TCC_Press.Content = "全       压";
                TCC_TotalPress = true;

                (Application.Current as App).dataPro.AdjustTCCEPCPressure(0, !EPC_TotalPress ? 2000 : 0);
            }
        }

        /// <summary>
        /// EPC全压标志位
        /// </summary>
        private bool EPC_TotalPress = true;

        private void btn_EPC_Press_Click(object sender, RoutedEventArgs e)
        {
            if (EPC_TotalPress)
            {
                btn_EPC_Press.Content = "无       压";
                EPC_TotalPress = false;
                (Application.Current as App).dataPro.AdjustTCCEPCPressure(!TCC_TotalPress ? 2000 : 0, 2000);
            }
            else if (!EPC_TotalPress)
            {
                btn_EPC_Press.Content = "全       压";
                EPC_TotalPress = true;
                (Application.Current as App).dataPro.AdjustTCCEPCPressure(!TCC_TotalPress ? 2000 : 0, 0);
            }
        }

        /// <summary>
        /// 换档电磁阀全压标志位
        /// </summary>
        private bool Shift_TotalPress = true;

        private void btn_Shift_Press_Click(object sender, RoutedEventArgs e)
        {
            if (Shift_TotalPress)
            {
                btn_Shift_Press.Content = "无       压";
                Shift_TotalPress = false;
                (Application.Current as App).dataPro.AdjustShiftSolenoid(true);
            }
            else if (!Shift_TotalPress)
            {
                btn_Shift_Press.Content = "全       压";
                Shift_TotalPress = true;

                (Application.Current as App).dataPro.AdjustShiftSolenoid(false);
            }
        }

        /// <summary>
        /// C1234全压标志位
        /// </summary>
        private bool C1234_TotalPress = true;

        private void btn_C1234_Press_Click(object sender, RoutedEventArgs e)
        {
            if (C1234_TotalPress)
            {
                btn_C1234_Press.Content = "无       压";
                C1234_TotalPress = false;
                (Application.Current as App).dataPro.AdjustC1234C456Pressure(2000, !C456_TotalPress ? 2000 : 0);

            }
            else if (!C1234_TotalPress)
            {
                btn_C1234_Press.Content = "全       压";
                C1234_TotalPress = true;
                (Application.Current as App).dataPro.AdjustC1234C456Pressure(0, !C456_TotalPress ? 2000 : 0);
            }
        }

        /// <summary>
        /// C456全压标志位
        /// </summary>
        private bool C456_TotalPress = true;

        private void btn_C456_Press_Click(object sender, RoutedEventArgs e)
        {
            if (C456_TotalPress)
            {
                btn_C456_Press.Content = "无       压";
                C456_TotalPress = false;
                (Application.Current as App).dataPro.AdjustC1234C456Pressure(!C1234_TotalPress ? 2000 : 0, 2000);
            }
            else if (!C456_TotalPress)
            {
                btn_C456_Press.Content = "全       压";
                C456_TotalPress = true;
                (Application.Current as App).dataPro.AdjustC1234C456Pressure(!C1234_TotalPress ? 2000 : 0, 0);
            }
        }


        /// <summary>
        /// C35R全压标志位
        /// </summary>
        private bool C35R_TotalPress = true;

        private void btn_C35R_Press_Click(object sender, RoutedEventArgs e)
        {
            if (C35R_TotalPress)
            {
                btn_C35R_Press.Content = "无       压";
                C35R_TotalPress = false;
                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(2000, !CB26_TotalPress ? 2000 : 0);
            }
            else if (!C35R_TotalPress)
            {
                btn_C35R_Press.Content = "全       压";
                C35R_TotalPress = true;
                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(0, !CB26_TotalPress ? 2000 : 0);
            }
        }

        /// <summary>
        /// CB26全压标志位
        /// </summary>
        private bool CB26_TotalPress = true;

        private void btn_CB26_Press_Click(object sender, RoutedEventArgs e)
        {
            if (CB26_TotalPress)
            {
                btn_CB26_Press.Content = "无       压";
                CB26_TotalPress = false;
                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(!C35R_TotalPress ? 2000 : 0, 2000);
            }
            else if (!CB26_TotalPress)
            {
                btn_CB26_Press.Content = "全       压";
                CB26_TotalPress = true;
                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(!C35R_TotalPress ? 2000 : 0, 0);
            }
        }

        private void btn_TotalPress_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).dataPro.SolenoidPressureControl(true);
            TCC_TotalPress = false;
            EPC_TotalPress = false;
            Shift_TotalPress = false;
            C1234_TotalPress = false;
            C456_TotalPress = false;
            C35R_TotalPress = false;
            CB26_TotalPress = false;
            btn_TCC_Press.Content = "无       压";
            btn_EPC_Press.Content = "无       压";
            btn_Shift_Press.Content = "无       压";
            btn_C1234_Press.Content = "无       压";
            btn_C456_Press.Content = "无       压";
            btn_C35R_Press.Content = "无       压";
            btn_CB26_Press.Content = "无       压";
        }

        private void btn_NonPress_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current as App).dataPro.SolenoidPressureControl(false);
            TCC_TotalPress = true;
            EPC_TotalPress = true;
            Shift_TotalPress = true;
            C1234_TotalPress = true;
            C456_TotalPress = true;
            C35R_TotalPress = true;
            CB26_TotalPress = true;
            btn_TCC_Press.Content = "全       压";
            btn_EPC_Press.Content = "全       压";
            btn_Shift_Press.Content = "全       压";
            btn_C1234_Press.Content = "全       压";
            btn_C456_Press.Content = "全       压";
            btn_C35R_Press.Content = "全       压";
            btn_CB26_Press.Content = "全       压";
        }

        /// <summary>
        /// 开始性能测试标志位
        /// </summary>
        private bool StartPerformanceTest = false;

        /// <summary>
        /// 按下回车键次数
        /// </summary>
        private int EnterKeyCount = 0;

        private bool testMethod = false;


        private int[] PerformanceTestValueL_H = new int[] { 25 , 50 , 75 , 100 , 125 ,  150 ,  175 ,  200 ,  225 ,250 ,
                                                          275 ,  300 ,  325 ,  350 ,  375 ,  400 ,  425 ,  450 ,  475 ,
                                                         500 ,  525 ,  550 ,  575 ,  600 ,  625 ,  650 ,  675 ,  700 , 725 ,
                                                         750  , 775,800 };

        private int[] PerformanceTestValueH_L = new int[] {775,750,725,700,675,650,625,600,
                                                           575,550,525,500,475,450,425,400,
                                                           375,350,325,300,275,250,225,200,
                                                           175,150,125,100,75,50,25,0};

        private void tb_TextPress_KeyDown(object sender, KeyEventArgs e)
        {
            if (StartPerformanceTest)
            {
                if (e.Key == System.Windows.Input.Key.Enter) //如果输入的是回车键
                {
                    switch (CB_Solenoid.SelectedIndex)
                    {
                        case 0:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustTCCEPCPressure(
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount], 2000);
                                if (testMethod)
                                {
                                    outputStep("TCC油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("TCC油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                    
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_TCC_Press.Content = "无       压";
                                        TCC_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_TCC_Press.Content = "全       压";
                                        TCC_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                        case 1:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustTCCEPCPressure(2000,
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount]);
                                if (testMethod)
                                {
                                    outputStep("EPC油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("EPC油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_EPC_Press.Content = "无       压";
                                        EPC_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_EPC_Press.Content = "全       压";
                                        EPC_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                        case 2:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustC1234C456Pressure(
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount], 2000);
                                if (testMethod)
                                {
                                    outputStep("C1234油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("C1234油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_C1234_Press.Content = "无       压";
                                        C1234_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_C1234_Press.Content = "全       压";
                                        C1234_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                        case 3:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustC1234C456Pressure(2000,
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount]);
                                if (testMethod)
                                {
                                    outputStep("C456油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("C456油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_C456_Press.Content = "无       压";
                                        C456_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_C456_Press.Content = "全       压";
                                        C456_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                        case 4:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount], 2000);
                                if (testMethod)
                                {
                                    outputStep("C35R油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("C35R油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_C35R_Press.Content = "无       压";
                                        C35R_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_C35R_Press.Content = "全       压";
                                        C35R_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                        case 5:
                            if (EnterKeyCount < 32)
                            {
                                (Application.Current as App).dataPro.AdjustC35RCB26Pressure(2000,
                                    testMethod
                                        ? PerformanceTestValueL_H[EnterKeyCount]
                                        : PerformanceTestValueH_L[EnterKeyCount]);
                                if (testMethod)
                                {
                                    outputStep("CB26油压调节（L_H）：" + PerformanceTestValueL_H[EnterKeyCount].ToString());
                                }
                                else
                                {
                                    outputStep("CB26油压调节（H_L）：" + PerformanceTestValueH_L[EnterKeyCount].ToString());
                                }
                                EnterKeyCount++;
                                if (EnterKeyCount < 32)
                                {
                                    tb_TextPress.Text = testMethod ? PerformanceTestValueL_H[EnterKeyCount].ToString() : PerformanceTestValueH_L[EnterKeyCount].ToString();
                                }
                                if (EnterKeyCount == 32)
                                {
                                    if (testMethod)
                                    {
                                        btn_CB26_Press.Content = "无       压";
                                        CB26_TotalPress = false;
                                    }
                                    else
                                    {
                                        btn_CB26_Press.Content = "全       压";
                                        CB26_TotalPress = true;
                                    }
                                    Star_Button.Visibility = Visibility.Visible;
                                    Stop_Button.Visibility = Visibility.Collapsed;
                                    CB_Solenoid.IsEnabled = true;
                                    CB_TestMethod.IsEnabled = true;
                                    tb_TextPress.Text = "";
                                }
                            }
                            break;
                    }

                }
            }
            
        }

        private string[] switchStatus = new string[4] {"false", "false", "false", "false" };

        private bool SwitchStatus(string status)
        {
            var data = true;
            if (status == "true")
            {
                data = true;
            }
            if (status == "false")
            {
                data = false;
            }
            return data;
        }

        private void switch1_Click(object sender, RoutedEventArgs e)
        {
            if (switchStatus[0] == "true")
            {
                switchStatus[0] = "false";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                switch1.Content = "开";
                switch1.Background = new SolidColorBrush(Colors.YellowGreen);
            }
            else if(switchStatus[0] == "false")
            {
                switchStatus[0] = "true";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                
                switch1.Content = "关";
                switch1.Background = new SolidColorBrush(Colors.Red);
            }
            
            
        }

        private void switch2_Click(object sender, RoutedEventArgs e)
        {
            if (switchStatus[1] == "true")
            {
                switchStatus[1] = "false";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                switch2.Content = "开";
                switch2.Background = new SolidColorBrush(Colors.YellowGreen);
            }
            else if (switchStatus[1] == "false")
            {
                switchStatus[1] = "true";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                
                switch2.Content = "关";
                switch2.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void switch3_Click(object sender, RoutedEventArgs e)
        {
            if (switchStatus[2] == "true")
            {
                switchStatus[2] = "false";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                switch3.Content = "开";
                switch3.Background = new SolidColorBrush(Colors.YellowGreen);
            }
            else if (switchStatus[2] == "false")
            {
                switchStatus[2] = "true";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                
                switch3.Content = "关";
                switch3.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void switch4_Click(object sender, RoutedEventArgs e)
        {
            if (switchStatus[3] == "true")
            {
                switchStatus[3] = "false";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                switch4.Content = "开";
                switch4.Background = new SolidColorBrush(Colors.YellowGreen);
            }
            else if (switchStatus[3] == "false")
            {
                switchStatus[3] = "true";
                (Application.Current as App).dataPro.SwitchSolenoidControl(SwitchStatus(switchStatus[0]), SwitchStatus(switchStatus[1]), SwitchStatus(switchStatus[2]), SwitchStatus(switchStatus[3]));
                
                switch4.Content = "关";
                switch4.Background = new SolidColorBrush(Colors.Red);
            }
        }

        private void btn_HalfPress_Click(object sender, RoutedEventArgs e)
        {
            if (txb_halfPress.Text != "" && txb_halfPress.Text != null)
            {
                //(Application.Current as App).dataPro.SolenoidHalfPressureControl(Convert.ToInt32(txb_halfPress.Text));
                (Application.Current as App).dataPro.AdjustTCCEPCPressure(0, (Convert.ToInt32(txb_halfPress.Text)));

            }
        }
    }

}





