using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;

namespace SolenoidTester.group
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            (Application.Current as App).IsNormallyClosedWindow = false;
         //   Timer1_Initialization();
        }
        DispatcherTimer Timer1;
        /// <summary>
        /// 周期性扫描定时器
        /// </summary>
        private void Timer1_Initialization()
        {
            //初始化
            
            Timer1 = new DispatcherTimer();
            Timer1.Tick += Timer_Tick1;
            //     Timer1.Tick += (this.DataContext as LiveDataViewModel).OnTimer;
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
            Debug.WriteLine(System.DateTime.Now.ToString("mm:ss.fff"));
        }

    }
}
