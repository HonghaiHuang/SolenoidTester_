using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Data;
using Microsoft.Win32;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls;
using System.Windows.Threading;
using System.Collections;
using System.Diagnostics;
using Telerik.Windows.Data;
using System.Threading;
namespace SolenoidTester.group
{
    // taken from MSDN (http://msdn.microsoft.com/en-us/library/system.windows.controls.datagrid.aspx)

    /// <summary>
    /// Interaction logic for reportsManagement.xaml
    /// </summary>
    public partial class ReportsCharView : Window
    {


        /// <summary>
        /// 变量定义表单
        /// </summary>
        public DataTable _VariableDefinitions = new DataTable("变量定义");


        public DataTable ViewTable;

        public double _LineNum = 0;



        public ReportsCharView()
        {
            InitializeComponent();

        }



        private void Initdata()
        {


            //   this._LineData_0.SetResourceReference(LineSeries.VerticalAxisProperty, "torqueAxis");


            if (_LineNum >= 1)
            {
                this._LineData_0.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[0]["纵坐标类型"]));
                _dataLine_Text0.Text = Convert.ToString(_VariableDefinitions.Rows[0]["说明"]);
            }
            else
            {
                _dataLine_Text0.Text = "未定义";
            }
            if (_LineNum >= 2)
            {
                this._LineData_1.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[1]["纵坐标类型"]));
                _dataLine_Text1.Text = Convert.ToString(_VariableDefinitions.Rows[1]["说明"]);
            }
            else
            {
                _dataLine_Text1.Text = "未定义";
            }

            if (_LineNum >= 3)
            {
                this._LineData_2.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[2]["纵坐标类型"]));
                _dataLine_Text2.Text = Convert.ToString(_VariableDefinitions.Rows[2]["说明"]);
            }
            else
            {
                _dataLine_Text2.Text = "未定义";
            }

            if (_LineNum >= 4)
            {
                this._LineData_3.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[3]["纵坐标类型"]));
                _dataLine_Text3.Text = Convert.ToString(_VariableDefinitions.Rows[3]["说明"]);
            }
            else
            {
                _dataLine_Text3.Text = "未定义";
            }

            if (_LineNum >= 5)
            {
                this._LineData_4.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[4]["纵坐标类型"]));

                _dataLine_Text4.Text = Convert.ToString(_VariableDefinitions.Rows[4]["说明"]);
            }
            else
            {
                _dataLine_Text4.Text = "未定义";
            }

            if (_LineNum >= 6)
            {
                this._LineData_5.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[5]["纵坐标类型"]));

                _dataLine_Text5.Text = Convert.ToString(_VariableDefinitions.Rows[5]["说明"]);
            }
            else
            {
                _dataLine_Text5.Text = "未定义";
            }

            if (_LineNum >= 7)
            {
                this._LineData_6.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[6]["纵坐标类型"]));

                _dataLine_Text6.Text = Convert.ToString(_VariableDefinitions.Rows[6]["说明"]);
            }
            else
            {
                _dataLine_Text6.Text = "未定义";
            }

            if (_LineNum >= 8)
            {
                this._LineData_7.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[7]["纵坐标类型"]));

                _dataLine_Text7.Text = Convert.ToString(_VariableDefinitions.Rows[7]["说明"]);
            }
            else
            {
                _dataLine_Text7.Text = "未定义";
            }

            if (_LineNum >= 9)
            {
                this._LineData_8.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[8]["纵坐标类型"]));

                _dataLine_Text8.Text = Convert.ToString(_VariableDefinitions.Rows[8]["说明"]);
            }
            else
            {
                _dataLine_Text8.Text = "未定义";
            }

            if (_LineNum >= 10)
            {
                this._LineData_9.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[9]["纵坐标类型"]));

                _dataLine_Text9.Text = Convert.ToString(_VariableDefinitions.Rows[9]["说明"]);
            }
            else
            {
                _dataLine_Text9.Text = "未定义";
            }

            if (_LineNum >= 11)
            {
                this._LineData_10.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[10]["纵坐标类型"]));

                _dataLine_Text10.Text = Convert.ToString(_VariableDefinitions.Rows[10]["说明"]);
            }
            else
            {
                _dataLine_Text10.Text = "未定义";
            }

            if (_LineNum >= 12)
            {
                this._LineData_11.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[11]["纵坐标类型"]));

                _dataLine_Text11.Text = Convert.ToString(_VariableDefinitions.Rows[11]["说明"]);
            }
            else
            {
                _dataLine_Text11.Text = "未定义";
            }

            if (_LineNum >= 13)
            {
                this._LineData_12.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[12]["纵坐标类型"]));

                _dataLine_Text12.Text = Convert.ToString(_VariableDefinitions.Rows[12]["说明"]);
            }
            else
            {
                _dataLine_Text12.Text = "未定义";
            }

            if (_LineNum >= 14)
            {
                this._LineData_13.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[13]["纵坐标类型"]));

                _dataLine_Text13.Text = Convert.ToString(_VariableDefinitions.Rows[13]["说明"]);
            }
            else
            {
                _dataLine_Text13.Text = "未定义";
            }

            if (_LineNum >= 15)
            {
                this._LineData_14.SetResourceReference(LineSeries.VerticalAxisProperty, Convert.ToString(_VariableDefinitions.Rows[14]["纵坐标类型"]));

                _dataLine_Text14.Text = Convert.ToString(_VariableDefinitions.Rows[14]["说明"]);
            }
            else
            {
                _dataLine_Text14.Text = "未定义";
            }



            //测试项目初始化
            for (int i = 0; i < _VariableDefinitions.Rows.Count; i++)
            {
                Convert.ToString(_VariableDefinitions.Rows[i]["测试项目"]);
                GearCase1.Items.Add(Convert.ToString(_VariableDefinitions.Rows[i]["测试项目"]));
                //   GearCase2.Text = "huang";

            }



        }


        #region 图表控件check

        /// <summary>
        /// 图表控件check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void _LineData_0_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_0_Check.IsChecked == true)
            {
                this._LineData_0.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_0_Check.IsChecked == false)
            {
                this._LineData_0.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_1_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_1_Check.IsChecked == true)
            {
                this._LineData_1.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_1_Check.IsChecked == false)
            {
                this._LineData_1.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_2_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_2_Check.IsChecked == true)
            {
                this._LineData_2.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_2_Check.IsChecked == false)
            {
                this._LineData_2.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void _LineData_3_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_3_Check.IsChecked == true)
            {
                this._LineData_3.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_3_Check.IsChecked == false)
            {
                this._LineData_3.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_4_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_4_Check.IsChecked == true)
            {
                this._LineData_4.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_4_Check.IsChecked == false)
            {
                this._LineData_4.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_5_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_5_Check.IsChecked == true)
            {
                this._LineData_5.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_5_Check.IsChecked == false)
            {
                this._LineData_5.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        private void _LineData_6_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_6_Check.IsChecked == true)
            {
                this._LineData_6.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_6_Check.IsChecked == false)
            {
                this._LineData_6.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void _LineData_7_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_7_Check.IsChecked == true)
            {
                this._LineData_7.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_7_Check.IsChecked == false)
            {
                this._LineData_7.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void _LineData_8_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_8_Check.IsChecked == true)
            {
                this._LineData_8.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_8_Check.IsChecked == false)
            {
                this._LineData_8.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_9_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_9_Check.IsChecked == true)
            {
                this._LineData_9.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_9_Check.IsChecked == false)
            {
                this._LineData_9.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_10_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_10_Check.IsChecked == true)
            {
                this._LineData_10.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_10_Check.IsChecked == false)
            {
                this._LineData_10.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void _LineData_11_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_11_Check.IsChecked == true)
            {
                this._LineData_11.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_11_Check.IsChecked == false)
            {
                this._LineData_11.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_12_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_12_Check.IsChecked == true)
            {
                this._LineData_12.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_12_Check.IsChecked == false)
            {
                this._LineData_12.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _LineData_13_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_13_Check.IsChecked == true)
            {
                this._LineData_13.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_13_Check.IsChecked == false)
            {
                this._LineData_13.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void _LineData_14_Check_Checked(object sender, RoutedEventArgs e)
        {
            if (_LineData_14_Check.IsChecked == true)
            {
                this._LineData_14.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_14_Check.IsChecked == false)
            {
                this._LineData_14.Visibility = System.Windows.Visibility.Collapsed;
            }

        }


        #endregion



        /// <summary>
        /// 初始化导入曲线
        /// </summary>
        DataTable CharViewDt;
        public void datatable(string path_14)
        {
            //AccessCom com = new AccessCom();
            //CharViewDt = new DataTable("Char");
            //string sql8 = "select * from " + "数据流";
            //CharViewDt = com.SelectToDataTable_CharView(sql8, path_14);
            //// this.FillData();

            FillData_1();
        }
        public DataTable table2;
        /// <summary>
        /// 绑定table到CharView
        /// </summary>

        public void FillData_1()
        {
            try
            {
                Initdata();

                //绑定资源
                this.DataContext = table2.Rows;
                //  InitGear();

                if (_LineNum >= 1)
                {
                    //时间轴
                    this._LineData_0.CategoryBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row["时间"]
                        };
                    //_LineData_0主油压
                    this._LineData_0.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[0]["变量"])]
                        };
                }

                if (_LineNum >= 2)
                {
                    //时间轴
                    this._LineData_1.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_1油压
                    this._LineData_1.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[1]["变量"])]
                        };
                }


                if (_LineNum >= 3)
                {
                    //时间轴
                    this._LineData_2.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_1油压
                    this._LineData_2.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[2]["变量"])]
                        };

                }

                if (_LineNum >= 4)
                {
                    //时间轴
                    this._LineData_3.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //
                    this._LineData_3.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[3]["变量"])]
                        };

                }



                if (_LineNum >= 5)
                {
                    //时间轴
                    this._LineData_4.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //油温
                    this._LineData_4.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[4]["变量"])]
                        };

                }

                if (_LineNum >= 6)
                {
                    //时间轴
                    this._LineData_5.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //传动比
                    this._LineData_5.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[5]["变量"])]
                        };

                }

                if (_LineNum >= 7)
                {
                    //时间轴
                    this._LineData_6.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_1油压
                    this._LineData_6.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[6]["变量"])]
                        };
                }


                if (_LineNum >= 8)
                {
                    //时间轴
                    this._LineData_7.CategoryBinding =
                new GenericDataPointBinding<DataRow, object>()
                {
                    ValueSelector = row => row["时间"]
                };
                    //_LineData_1油压
                    this._LineData_7.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[7]["变量"])]
                        };
                }


                if (_LineNum >= 9)
                {
                    //时间轴
                    this._LineData_8.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_1油压
                    this._LineData_8.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[8]["变量"]) + "Standard"]
                        };
                }


                if (_LineNum >= 10)
                {
                    //时间轴
                    this._LineData_9.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_9
                    this._LineData_9.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[9]["变量"]) + "Standard"]
                        };

                }


                if (_LineNum >= 11)
                {
                    //时间轴
                    this._LineData_10.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_9
                    this._LineData_10.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[10]["变量"]) + "Standard"]
                        };
                }



                if (_LineNum >= 12)
                {
                    //时间轴
                    this._LineData_11.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_9
                    this._LineData_11.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[11]["变量"]) + "Standard"]
                        };

                }


                if (_LineNum >= 13)
                {

                    //时间轴
                    this._LineData_12.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_9
                    this._LineData_12.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[12]["变量"]) + "Standard"]
                        };
                }


                if (_LineNum >= 14)
                {
                    //时间轴
                    this._LineData_13.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_9
                    this._LineData_13.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[13]["变量"]) + "Standard"]
                        };
                }


                if (_LineNum >= 15)
                {
                    //时间轴
                    this._LineData_14.CategoryBinding =
                    new GenericDataPointBinding<DataRow, object>()
                    {
                        ValueSelector = row => row["时间"]
                    };
                    //_LineData_14
                    this._LineData_14.ValueBinding =
                        new GenericDataPointBinding<DataRow, object>()
                        {
                            ValueSelector = row => row[Convert.ToString(_VariableDefinitions.Rows[14]["变量"]) + "Standard"]
                        };
                }

            }
            catch (Exception err)
            { MessageBox.Show(err.Message); }
        }

        //private void timemax_KeyDown(object sender, KeyEventArgs e)
        //{
        //    timemaxin(FileterDatatable(timemin.Text, timemax.Text));
        //}
        //private void timemin_KeyDown(object sender, KeyEventArgs e)
        //{
        //    timemaxin(FileterDatatable(timemin.Text, timemax.Text));
        //}


        public void UpdataDatatableToDatacontext(DataTable dt9)
        {
            this.DataContext = dt9.Rows;

            //this._LineData_0.ValueBinding =
            //    new GenericDataPointBinding<DataRow, object>()
            //    {
            //        ValueSelector = row => row["_LineData_0"]
            //    };

            ////时间轴
            //this._LineData_0.CategoryBinding =
            //    new GenericDataPointBinding<DataRow, object>()
            //    {
            //        ValueSelector = row => row["时间"]
            //    };

        }

        private DataTable FileterDatatable(string TIME1, string TIME2)
        {
            if (TIME1 == "")
            {
                TIME1 = "0";
            }
            if (TIME2 == "")
            {
                TIME2 = "0";
            }
            DataView view = new DataView();
            view.Table = table2;
            DataTable B = new DataTable();
            view.RowFilter = "时间戳 >= '" + Convert.ToDouble(TIME1) + "' and 时间戳 <= '" + Convert.ToDouble(TIME2) + "'";//itemType是A中的一个字段
            B = view.ToTable();
            return B;
        }

        private void LinearAxis_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private DataTable FileterDatatableGerr(string Gear1)
        {
            DataView view = new DataView();
            view.Table = table2;
            DataTable B = new DataTable();
            //      view.RowFilter = "Beat >= '" + Gear1 + "' and Beat <= '" + Gear2 + "'";//itemType是A中的一个字段
            view.RowFilter = "testItemNumber = '" + Gear1 + "'";//itemType是A中的一个字段
            B = view.ToTable();
            return B;
        }



        /// <summary>
        /// 测试项目确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            int CaseNumber = GearCase1.SelectedIndex;
            SwitchTestProject(CaseNumber);
        }

        private void SwitchTestProject(int caseNumber)
        {
            switch (caseNumber)
            {
                case -1:
                    break;
                case 0:
                    UpdataDatatableToDatacontext(table2);
                    break;
                default:
                    UpdataDatatableToDatacontext(FileterDatatableGerr(Convert.ToString(_VariableDefinitions.Rows[caseNumber - 1]["测试代号"])));
                    break;
            }




        }


        private void ChartTrackBallBehavior_TrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            DataPointInfo closestDataPoint = e.Context.ClosestDataPoint;
            if (closestDataPoint != null)
            {
                //   FinancialData data = closestDataPoint.DataPoint.DataItem as FinancialData;
                //this.volume.Text = data.Volume.ToString("##,#");
                //this.date.Text = data.Date.ToString("MMM dd, yyyy");
                //this.price.Text = data.Close.ToString("0,0.00");
                //  texttime.Text = closestDataPoint.DataPoint.DataItem
                DataRow tt = (DataRow) closestDataPoint.DataPoint.DataItem;
                Time_text.Text = Convert.ToDouble(tt[1]).ToString("0.000") + "S";
                if (_LineNum >= 1)
                    _LineData_0_text.Text = tt[2].ToString();
                if (_LineNum >= 2)
                    _LineData_1_text.Text = tt[3].ToString();
                if (_LineNum >= 3)
                    _LineData_2_text.Text = tt[4].ToString();
                if (_LineNum >= 4)
                    _LineData_3_text.Text = tt[5].ToString();
                if (_LineNum >= 5)
                    _LineData_4_text.Text = tt[6].ToString();
                if (_LineNum >= 6)
                    _LineData_5_text.Text = tt[7].ToString();
                if (_LineNum >= 7)
                    _LineData_7_text.Text = tt[8].ToString();
                if (_LineNum >= 8)
                    _LineData_6_text.Text = tt[9].ToString();
                if (_LineNum >= 9)
                    _LineData_8_text.Text = tt[10].ToString();
                if (_LineNum >= 10)
                    _LineData_9_text.Text = tt[11].ToString();
                if (_LineNum >= 11)
                    _LineData_10_text.Text = tt[12].ToString();
                if (_LineNum >= 12)
                    _LineData_11_text.Text = tt[13].ToString();
                if (_LineNum >= 13)
                    _LineData_12_text.Text = tt[14].ToString();
                if (_LineNum >= 14)
                    _LineData_13_text.Text = tt[15].ToString();
                if (_LineNum >= 15)
                    _LineData_14_text.Text = tt[16].ToString();

                //switch (tt[16].ToString())
                //{
                //    case "1":
                //        _LineData_14_text.Text = "P档";
                //        break;
                //    case "2":
                //        _LineData_14_text.Text = "R档";
                //        break;
                //    case "3":
                //        _LineData_14_text.Text = "N档";
                //        break;
                //    case "4":
                //        _LineData_14_text.Text = "D档";
                //        break;
                //    case "5":
                //        _LineData_14_text.Text = "D1档";
                //        break;
                //    case "6":
                //        _LineData_14_text.Text = "D2档";
                //        break;
                //    case "7":
                //        _LineData_14_text.Text = "D3档";
                //        break;
                //    case "8":
                //        _LineData_14_text.Text = "D4档";
                //        break;
                //    case "9":
                //        _LineData_14_text.Text = "D5档";
                //        break;
                //    case "10":
                //        _LineData_14_text.Text = "D6档";
                //        break;
                //    default:
                //        _LineData_14_text.Text = "";
                //        break;

                //}
                //  _LineData_14_text.Text = tt[16].ToString();


                // Retrieve the state value for the current row. 
                //      String state = rowView["state"].ToString();
            }
        }

        private void Time_Button_Click(object sender, RoutedEventArgs e)
        {

            UpdataDatatableToDatacontext(FileterDatatable(timemin.Text, timemax.Text));
        }

    }






}

