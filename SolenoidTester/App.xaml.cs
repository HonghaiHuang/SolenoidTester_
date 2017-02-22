using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SolenoidTester
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {


        public App()
        {

        }
        ~App()
        {

        }
        /// <summary>
        /// 实例化数据处理模块
        /// </summary>
        public DataProcess dataPro = new DataProcess();

        /// <summary>
        /// 存储测试信息
        /// </summary>
        public string[] _TestInformation = new string[9];

        public bool IsNormallyClosedWindow = false;

        public string FilePath = "";


    }
}
