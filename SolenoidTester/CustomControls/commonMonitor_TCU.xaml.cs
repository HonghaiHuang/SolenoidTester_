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

namespace SolenoidTester.CustomControls
{
    /// <summary>
    /// commonMonitor_TCU.xaml 的交互逻辑
    /// </summary>
    public partial class commonMonitor_TCU : UserControl
    {
        public static readonly DependencyProperty VauleTxtProperty = DependencyProperty.Register("VauleTxt", typeof(String), typeof(commonMonitor_TCU), new PropertyMetadata(""));
        public static readonly DependencyProperty UnitLabelProperty = DependencyProperty.Register("UnitLabel", typeof(String), typeof(commonMonitor_TCU), new PropertyMetadata(""));
        public static readonly DependencyProperty TitleTxtProperty = DependencyProperty.Register("TitleTxt", typeof(String), typeof(commonMonitor_TCU), new PropertyMetadata(""));
        public commonMonitor_TCU()
        {
            InitializeComponent();
        }
        public string VauleTxt
        {
            get { return (String) GetValue(VauleTxtProperty); }
            set { SetValue(VauleTxtProperty, value); }
        }

        public string UnitLabel
        {
            get { return (String) GetValue(UnitLabelProperty); }
            set { SetValue(UnitLabelProperty, value); }
        }
        public string TitleTxt
        {
            get { return (String) GetValue(TitleTxtProperty); }
            set { SetValue(TitleTxtProperty, value); }
        }
    }
}
