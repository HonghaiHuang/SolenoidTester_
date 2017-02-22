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

namespace SolenoidTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow,Observer
    {
        public MainWindow()
        {
            InitializeComponent();
            if((Application.Current as App).IsNormallyClosedWindow ==false)
            {
                (Application.Current as App).dataPro.ConnectCan();
                //是否正常关闭窗口
                (Application.Current as App).IsNormallyClosedWindow = false;

                //RegisterObserver((Application.Current as App).dataPro);
                (Application.Current as App).dataPro.CollectMcuData(false);
            }

        }
        public void RegisterObserver(Subject dataPro)
        {
            this.dataPro = dataPro;
            dataPro.RegisterObserver(this);
        }
        public void RemoveRegister(Subject dataPro)
        {
            this.dataPro = dataPro;
            dataPro.RemoveObserver(this);
        }

        private Subject dataPro;
        public void Update(string[,] data)
        {
           
        }

        private void ModernWindow_Closed(object sender, EventArgs e)
        {
            if ((Application.Current as App).IsNormallyClosedWindow == false)
            {
                (Application.Current as App).dataPro.CollectMcuData(false);
                //关闭CAN
                (Application.Current as App).dataPro.DisConnectCan();
                //Window mainWindow = Application.Current.MainWindow;
                //if (mainWindow != null)
                //    mainWindow.Close();
                Application.Current.Shutdown();
            }


        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
