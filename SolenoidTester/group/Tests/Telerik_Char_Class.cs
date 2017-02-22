using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace SolenoidTester.group.Tests
{
    public class ChartBusinessObject
    {
        private double _value;
        private DateTime _category;

        public double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public DateTime Category
        {
            get
            {
                return this._category;
            }
            set
            {
                this._category = value;
            }
        }
    }



    public class LiveDataViewModel : ViewModelBase
    {
        public RadObservableCollection<ChartBusinessObject> _P_Source_data;
        public RadObservableCollection<ChartBusinessObject> _P_TCC_data;
        public RadObservableCollection<ChartBusinessObject> _P_Line_data;
        public RadObservableCollection<ChartBusinessObject> _P_Shift_data;
        public RadObservableCollection<ChartBusinessObject> _P_C1234_data;
        public RadObservableCollection<ChartBusinessObject> _P_CB26_data;

        public RadObservableCollection<ChartBusinessObject> _P_C35R_data;
        public RadObservableCollection<ChartBusinessObject> _P_C456_data;

        public RadObservableCollection<ChartBusinessObject> _temperature_data;
        public RadObservableCollection<ChartBusinessObject> _inputSpeed_data;

        public RadObservableCollection<ChartBusinessObject> _outputSpeed_data;
        public RadObservableCollection<ChartBusinessObject> _TCUInputSpeed_data;

        public RadObservableCollection<ChartBusinessObject> _TCUOnputSpeed_data;
        public RadObservableCollection<ChartBusinessObject> _TCUtemperature_data;

        public RadObservableCollection<ChartBusinessObject> _greanum_data;






        public delegate void CallFunction();//定义委托[和定义方法一个样,简单理解为static 换成了delegate]
        public static event CallFunction CallEvenHandle;//定义事件[简单理解,有事件必有委托]





        private string messagesPerSecond;
        private string messagesPerMinute;
        private int tickCountSecond;
        private int tickCountMinute;
        private DispatcherTimer timer;
        private DateTime lastDate;
        private Random random = new Random();
        private DateTime? alignmentDate;
        private bool useAlignmentDate;
        private bool IsStart = false;

        //   private const int TimerInterval = 10;
        private const double TimerInterval = 50;//
        public ArrayList list2 = new ArrayList() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] list3 = new int[48];

        public string[] _data = new string[32];
        public bool IsupdataChar = false;

        public LiveDataViewModel()
        {
          //    this.timer = new DispatcherTimer();
       //     this.timer.Tick += OnTimer;
          //  StartTimer();
            this.FillData();
            //  this.useAlignmentDate = false;
            UpdateAlignmentDate();
          //  CallEvenHandle += new CallFunction(FlashData);
        }

        public void FlashData(string[] info)
        {
           // OnTimer(null, null);
            _data = info;
            // IsupdataChar = true;
        }


        public string MessagesPerSecond
        {
            get
            {
                return this.messagesPerSecond;
            }
            set
            {
                if (this.messagesPerSecond != value)
                {
                    this.messagesPerSecond = value;
                    this.OnPropertyChanged("MessagesPerSecond");
                }
            }
        }

        public string MessagesPerMinute
        {
            get
            {
                return this.messagesPerMinute;
            }
            set
            {
                if (this.messagesPerMinute != value)
                {
                    this.messagesPerMinute = value;
                    this.OnPropertyChanged("MessagesPerMinute");
                }
            }
        }

        public RadObservableCollection<ChartBusinessObject> P_Source_Data
        {
            get
            {
                return this._P_Source_data;
            }
            set
            {
                if (this._P_Source_data != value)
                {
                    this._P_Source_data = value;



                    Action showMethod = delegate () { this.OnPropertyChanged("P_Source_Data"); };
                    //  this.OnPropertyChanged("P_Source_Data");
                    ViewModelBase.InvokeOnUIThread(showMethod);
                  //  this.OnPropertyChanged<string>(Expression < Func < string >> "P_Source_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_TCC_Data
        {
            get
            {
                return this._P_TCC_data;
            }
            set
            {
                if (this._P_TCC_data != value)
                {
                    this._P_TCC_data = value;
                    this.OnPropertyChanged("P_TCC_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_Line_Data
        {
            get
            {
                return this._P_Line_data;
            }
            set
            {
                if (this._P_Line_data != value)
                {
                    this._P_Line_data = value;
                    this.OnPropertyChanged("P_Line_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_Shift_Data
        {
            get
            {
                return this._P_Shift_data;
            }
            set
            {
                if (this._P_Shift_data != value)
                {
                    this._P_Shift_data = value;
                    this.OnPropertyChanged("P_Shift_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_C1234_Data
        {
            get
            {
                return this._P_C1234_data;
            }
            set
            {
                if (this._P_C1234_data != value)
                {
                    this._P_C1234_data = value;
                    this.OnPropertyChanged("P_C1234_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_CB26_Data
        {
            get
            {
                return this._P_CB26_data;
            }
            set
            {
                if (this._P_CB26_data != value)
                {
                    this._P_CB26_data = value;
                    this.OnPropertyChanged("P_CB26_Data");
                }
            }
        }


        public RadObservableCollection<ChartBusinessObject> P_C35R_Data
        {
            get
            {
                return this._P_C35R_data;
            }
            set
            {
                if (this._P_C35R_data != value)
                {
                    this._P_C35R_data = value;
                    this.OnPropertyChanged("P_C35R_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> P_C456_Data
        {
            get
            {
                return this._P_C456_data;
            }
            set
            {
                if (this._P_C456_data != value)
                {
                    this._P_C456_data = value;
                    this.OnPropertyChanged("P_C456_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> temperature_Data
        {
            get
            {
                return this._temperature_data;
            }
            set
            {
                if (this._temperature_data != value)
                {
                    this._temperature_data = value;
                    this.OnPropertyChanged("temperature_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> inputSpeed_Data
        {
            get
            {
                return this._inputSpeed_data;
            }
            set
            {
                if (this._inputSpeed_data != value)
                {
                    this._inputSpeed_data = value;
                    this.OnPropertyChanged("inputSpeed_Data");
                }
            }
        }

        public RadObservableCollection<ChartBusinessObject> outputSpeed_Data
        {
            get
            {
                return this._outputSpeed_data;
            }
            set
            {
                if (this._outputSpeed_data != value)
                {
                    this._outputSpeed_data = value;
                    this.OnPropertyChanged("outputSpeed_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> TCUInputSpeed_Data
        {
            get
            {
                return this._TCUInputSpeed_data;
            }
            set
            {
                if (this._TCUInputSpeed_data != value)
                {
                    this._TCUInputSpeed_data = value;
                    this.OnPropertyChanged("TCUInputSpeed_Data");
                }
            }
        }

        public RadObservableCollection<ChartBusinessObject> TCUOnputSpeed_Data
        {
            get
            {
                return this._TCUOnputSpeed_data;
            }
            set
            {
                if (this._TCUOnputSpeed_data != value)
                {
                    this._TCUOnputSpeed_data = value;
                    this.OnPropertyChanged("TCUOnputSpeed_Data");
                }
            }
        }
        public RadObservableCollection<ChartBusinessObject> TCUtemperature_Data
        {
            get
            {
                return this._TCUtemperature_data;
            }
            set
            {
                if (this._TCUtemperature_data != value)
                {
                    this._TCUtemperature_data = value;
                    this.OnPropertyChanged("TCUtemperature_Data");
                }
            }
        }

        public RadObservableCollection<ChartBusinessObject> GreaNum_Data
        {
            get
            {
                return this._greanum_data;
            }
            set
            {
                if (this._greanum_data != value)
                {
                    this._greanum_data = value;
                    this.OnPropertyChanged("GreaNum_Data");
                }
            }
        }





        public DateTime? AlignmentDate
        {
            get
            {
                return this.alignmentDate;
            }
            set
            {
                if (this.alignmentDate != value)
                {
                    this.alignmentDate = value;
                    this.OnPropertyChanged("AlignmentDate");
                }
            }
        }

        public bool UseAlignmentDate
        {
            get
            {
                return this.useAlignmentDate;
            }
            set
            {
                if (this.useAlignmentDate != value)
                {
                    this.useAlignmentDate = value;
                    this.OnPropertyChanged("UseAlignmentDate");
                    this.UpdateAlignmentDate();
                }
            }
        }

        public void StartTimer()
        {
            this.timer.Start();
        }

        public void StopTimer()
        {
            this.timer.Stop();
        }

        public void UpdateTimer(double interval)
        {
            this.timer.Interval = TimeSpan.FromMilliseconds(interval);
        }


        public void FillData()
        {
            RadObservableCollection<ChartBusinessObject> collection1 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection2 = new RadObservableCollection<ChartBusinessObject>();

            RadObservableCollection<ChartBusinessObject> collection3 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection4 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection5 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection6 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection7 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection8 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection9 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection10 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection11 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection12 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection13 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection14 = new RadObservableCollection<ChartBusinessObject>();
            RadObservableCollection<ChartBusinessObject> collection15 = new RadObservableCollection<ChartBusinessObject>();

            IsStart = false;
            this.lastDate = DateTime.Now;
            this.alignmentDate = this.lastDate;




            for (int i = 0; i < 101; i++)
            {
                this.lastDate = this.lastDate.AddMilliseconds(TimerInterval);
                collection1.Add(this.CreateBusinessObject1());
                collection2.Add(this.CreateBusinessObject2());
                collection3.Add(this.CreateBusinessObject3());
                collection4.Add(this.CreateBusinessObject4());
                collection5.Add(this.CreateBusinessObject5());
                collection6.Add(this.CreateBusinessObject6());
                collection7.Add(this.CreateBusinessObject7());
                collection8.Add(this.CreateBusinessObject8());
                collection9.Add(this.CreateBusinessObject9());
                collection10.Add(this.CreateBusinessObject10());
                collection11.Add(this.CreateBusinessObject11());
                collection12.Add(this.CreateBusinessObject12());
                collection13.Add(this.CreateBusinessObject13());
                collection14.Add(this.CreateBusinessObject14());
                collection15.Add(this.CreateBusinessObject15());

            }
            this.P_Source_Data = collection1;
            this.P_TCC_Data = collection2;
            this.P_Line_Data = collection3;
            this.P_Shift_Data = collection4;
            this.P_C1234_Data = collection5;
            this.P_CB26_Data = collection6;

            this.P_C35R_Data = collection7;
            this.P_C456_Data = collection8;
            this.temperature_Data = collection9;
            this.inputSpeed_Data = collection10;
            this.outputSpeed_Data = collection11;
            this.TCUInputSpeed_Data = collection12;
            this.TCUOnputSpeed_Data = collection13;
            this.TCUtemperature_Data = collection14;
            this.GreaNum_Data = collection15;

        }












        public void OnTimer(object sender, EventArgs e)
        {

                System.DateTime kkk = System.DateTime.Now;

                IsStart = true;
                this.lastDate = this.lastDate.AddMilliseconds(TimerInterval);



                
                this.P_Source_Data.SuspendNotifications();
                this.P_Source_Data.RemoveAt(0);
                this.P_Source_Data.Add(this.CreateBusinessObject1());
                this.P_Source_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_TCC_Data.SuspendNotifications();
                this.P_TCC_Data.RemoveAt(0);
                this.P_TCC_Data.Add(this.CreateBusinessObject2());
                this.P_TCC_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_Line_Data.SuspendNotifications();
                this.P_Line_Data.RemoveAt(0);
                this.P_Line_Data.Add(this.CreateBusinessObject3());
                this.P_Line_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_Shift_Data.SuspendNotifications();
                this.P_Shift_Data.RemoveAt(0);
                this.P_Shift_Data.Add(this.CreateBusinessObject4());
                this.P_Shift_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_C1234_Data.SuspendNotifications();
                this.P_C1234_Data.RemoveAt(0);
                this.P_C1234_Data.Add(this.CreateBusinessObject5());
                this.P_C1234_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_CB26_Data.SuspendNotifications();
                this.P_CB26_Data.RemoveAt(0);
                this.P_CB26_Data.Add(this.CreateBusinessObject6());
                this.P_CB26_Data.ResumeNotifications();
                this.UpdateMetrics();

                this.P_C35R_Data.SuspendNotifications();
                this.P_C35R_Data.RemoveAt(0);
                this.P_C35R_Data.Add(this.CreateBusinessObject7());
                this.P_C35R_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.P_C456_Data.SuspendNotifications();
                this.P_C456_Data.RemoveAt(0);
                this.P_C456_Data.Add(this.CreateBusinessObject8());
                this.P_C456_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.temperature_Data.SuspendNotifications();
                this.temperature_Data.RemoveAt(0);
                this.temperature_Data.Add(this.CreateBusinessObject9());
                this.temperature_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.inputSpeed_Data.SuspendNotifications();
                this.inputSpeed_Data.RemoveAt(0);
                this.inputSpeed_Data.Add(this.CreateBusinessObject10());
                this.inputSpeed_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.outputSpeed_Data.SuspendNotifications();
                this.outputSpeed_Data.RemoveAt(0);
                this.outputSpeed_Data.Add(this.CreateBusinessObject11());
                this.outputSpeed_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.TCUInputSpeed_Data.SuspendNotifications();
                this.TCUInputSpeed_Data.RemoveAt(0);
                this.TCUInputSpeed_Data.Add(this.CreateBusinessObject12());
                this.TCUInputSpeed_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.TCUOnputSpeed_Data.SuspendNotifications();
                this.TCUOnputSpeed_Data.RemoveAt(0);
                this.TCUOnputSpeed_Data.Add(this.CreateBusinessObject13());
                this.TCUOnputSpeed_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.TCUtemperature_Data.SuspendNotifications();
                this.TCUtemperature_Data.RemoveAt(0);
                this.TCUtemperature_Data.Add(this.CreateBusinessObject14());
                this.TCUtemperature_Data.ResumeNotifications();
                this.UpdateMetrics();
                this.GreaNum_Data.SuspendNotifications();
                this.GreaNum_Data.RemoveAt(0);
                this.GreaNum_Data.Add(this.CreateBusinessObject15());
                this.GreaNum_Data.ResumeNotifications();
                this.UpdateMetrics();

                Debug.WriteLine("时间间隔：" + (System.DateTime.Now - kkk).TotalMilliseconds);

        }

        private void UpdateMetrics()
        {
            this.tickCountSecond++;
            this.tickCountMinute++;

            if (this.tickCountSecond == 5)
            {
                this.tickCountSecond = 0;
                this.MessagesPerSecond = this.random.Next(900, 1100).ToString("#,#");
            }

            if (this.tickCountMinute == 300)
            {
                this.tickCountMinute = 0;
                this.MessagesPerMinute = this.random.Next(50000, 55000).ToString("#,#");
            }
        }


        /// <summary>
        /// 主油压P_Source
        /// </summary>
        /// <returns></returns>
        private ChartBusinessObject CreateBusinessObject1()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[0]);
            }
            //    obj.Value = Convert.ToDouble(list2[0]);
            obj.Category = this.lastDate;
            return obj;
        }



        /// <summary>
        /// P_TCC
        /// </summary>
        private ChartBusinessObject CreateBusinessObject2()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[1]);
            }
            // obj.Value = Convert.ToDouble(list2[16]);

            obj.Category = this.lastDate;

            return obj;
        }

        /// <summary>
        /// 油压2
        /// </summary>
        /// <returns></returns>
        private ChartBusinessObject CreateBusinessObject3()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {

                obj.Value = Convert.ToDouble(_data[2]);
            }
            obj.Category = this.lastDate;

            return obj;
        }



        /// <summary>
        /// 油压3
        /// </summary>
        private ChartBusinessObject CreateBusinessObject4()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //           obj.Value = this.random.Next(1200, 1500);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[3]);
            }
            obj.Category = this.lastDate;
            return obj;
        }

        /// <summary>
        /// 油压4
        /// </summary>
        /// <returns></returns>
        private ChartBusinessObject CreateBusinessObject5()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //       obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[4]);
            }
            obj.Category = this.lastDate;

            return obj;
        }



        /// <summary>
        /// 油压5
        /// </summary>
        private ChartBusinessObject CreateBusinessObject6()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[5]);
            }
            obj.Category = this.lastDate;

            return obj;
        }

        /// <summary>
        /// 输入扭矩
        /// </summary>
        /// <returns></returns>
        private ChartBusinessObject CreateBusinessObject7()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //       obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[6]);
            }
            obj.Category = this.lastDate;

            return obj;
        }



        /// <summary>
        /// 左输出扭矩
        /// </summary>
        private ChartBusinessObject CreateBusinessObject8()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //     obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[7]);
            }
            obj.Category = this.lastDate;

            return obj;
        }

        /// <summary>
        /// 右输出扭矩
        /// </summary>
        /// <returns></returns>
        private ChartBusinessObject CreateBusinessObject9()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //     obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[8]);
            }
            obj.Category = this.lastDate;

            return obj;
        }



        /// <summary>
        /// 输入转速
        /// </summary>
        private ChartBusinessObject CreateBusinessObject10()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //    obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[9]);
            }
            obj.Category = this.lastDate;

            return obj;
        }

        /// <summary>
        /// 左输出转速
        /// </summary>
        private ChartBusinessObject CreateBusinessObject11()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //   obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[10]);
            }
            // obj.Value = 0;
            obj.Category = this.lastDate;

            return obj;
        }
        /// <summary>
        /// 右输出转速
        /// </summary>
        private ChartBusinessObject CreateBusinessObject12()
        {
            ChartBusinessObject obj = new ChartBusinessObject();

            //     obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[19]);
            }
            obj.Category = this.lastDate;

            return obj;
        }
        /// <summary>
        /// 输出转速(CAN)
        /// </summary>
        private ChartBusinessObject CreateBusinessObject13()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            //     obj.Value = this.random.Next(800, 1800);
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[20]);
            }
            obj.Category = this.lastDate;
            return obj;
        }
        /// <summary>
        /// 输入转速(CAN)
        /// </summary>
        private ChartBusinessObject CreateBusinessObject14()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[22]);
            }
            obj.Category = this.lastDate;
            return obj;
        }

        /// <summary>
        ///数字档位
        /// </summary>
        private ChartBusinessObject CreateBusinessObject15()
        {
            ChartBusinessObject obj = new ChartBusinessObject();
            if (IsStart == true)
            {
                obj.Value = Convert.ToDouble(_data[22]);
            }
            obj.Category = this.lastDate;
            return obj;
        }


        private void UpdateAlignmentDate()
        {
            this.AlignmentDate = (false) ? (DateTime?) this.lastDate : null;
        }
    }

}
