using FirstFloor.ModernUI.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Threading;
using SolenoidTester.DataClass;
using System.Data;
//using ModernUINavigationApp.Pages;



namespace SolenoidTester.group
{
    /// <summary>
    /// Interaction logic for SignalsDefinedPage.xaml
    /// </summary>
    public partial class SignalsDefinedPage : UserControl, IContent,Observer
    {


        MySQL_client _MySQL_client = new MySQL_client();
        private delegate void outputDelegate(string msg);
        private delegate void changeDelegate();

        /// <summary>
        /// 界面更新计数
        /// </summary>
        private int updateCount = 0;
        /// <summary>
        /// 输入格式错误标志位
        /// </summary>
        private bool formatError = false;

        private DataTable DeviceDemarcate_Dt = new DataTable();

        public SignalsDefinedPage()
        {
            InitializeComponent();
            #region 界面赋值
            DeviceDemarcate_Dt = MySQL_client.Getdataset("devicedemarcate", "applydevicedemarcate", "").Tables[0];
            outputP_Source_K(DeviceDemarcate_Dt.Rows[0]["demarcatevalue"].ToString());
            outputP_Source_B(DeviceDemarcate_Dt.Rows[1]["demarcatevalue"].ToString());
            outputP_TCC_K(DeviceDemarcate_Dt.Rows[2]["demarcatevalue"].ToString());
            outputP_TCC_B(DeviceDemarcate_Dt.Rows[3]["demarcatevalue"].ToString());
            outputP_Line_K(DeviceDemarcate_Dt.Rows[4]["demarcatevalue"].ToString());
            outputP_Line_B(DeviceDemarcate_Dt.Rows[5]["demarcatevalue"].ToString());
            outputP_Shift_K(DeviceDemarcate_Dt.Rows[6]["demarcatevalue"].ToString());
            outputP_Shift_B(DeviceDemarcate_Dt.Rows[7]["demarcatevalue"].ToString());
            outputP_C1234_K(DeviceDemarcate_Dt.Rows[8]["demarcatevalue"].ToString());
            outputP_C1234_B(DeviceDemarcate_Dt.Rows[9]["demarcatevalue"].ToString());
            outputP_CB26_K(DeviceDemarcate_Dt.Rows[10]["demarcatevalue"].ToString());
            outputP_CB26_B(DeviceDemarcate_Dt.Rows[11]["demarcatevalue"].ToString());
            outputP_C35R_K(DeviceDemarcate_Dt.Rows[12]["demarcatevalue"].ToString());
            outputP_C35R_B(DeviceDemarcate_Dt.Rows[13]["demarcatevalue"].ToString());
            outputP_C456_K(DeviceDemarcate_Dt.Rows[14]["demarcatevalue"].ToString());
            outputP_C456_B(DeviceDemarcate_Dt.Rows[15]["demarcatevalue"].ToString());
            outputTemperature_K(DeviceDemarcate_Dt.Rows[16]["demarcatevalue"].ToString());
            outputTemperature_B(DeviceDemarcate_Dt.Rows[17]["demarcatevalue"].ToString());
            outputInputSpeed_K(DeviceDemarcate_Dt.Rows[18]["demarcatevalue"].ToString());
            outputOutputSpeed_K(DeviceDemarcate_Dt.Rows[20]["demarcatevalue"].ToString());
            #endregion
        }

       



        

        
        /// <summary>
        /// 应用更改的设备标定值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] demarcateValue = new string[22];
            string[] LastDemarcateValue = new string[22];
            formatError = false;
            demarcateValue = new string[22];
            demarcateValue[0] = ValueFilter(P_Source_K.Text);
            demarcateValue[1] = ValueFilter(P_Source_B.Text);
            demarcateValue[2] = ValueFilter(P_TCC_K.Text);
            demarcateValue[3] = ValueFilter(P_TCC_B.Text);
            demarcateValue[4] = ValueFilter(P_Line_K.Text);
            demarcateValue[5] = ValueFilter(P_Line_B.Text);
            demarcateValue[6] = ValueFilter(P_Shift_K.Text);
            demarcateValue[7] = ValueFilter(P_Shift_B.Text);
            demarcateValue[8] = ValueFilter(P_C1234_K.Text);
            demarcateValue[9] = ValueFilter(P_C1234_B.Text);
            demarcateValue[10] = ValueFilter(P_CB26_K.Text);
            demarcateValue[11] = ValueFilter(P_CB26_B.Text);
            demarcateValue[12] = ValueFilter(P_C35R_K.Text);
            demarcateValue[13] = ValueFilter(P_C35R_B.Text);
            demarcateValue[14] = ValueFilter(P_C456_K.Text);
            demarcateValue[15] = ValueFilter(P_C456_B.Text);
            demarcateValue[16] = ValueFilter(Temperature_K.Text);
            demarcateValue[17] = ValueFilter(Temperature_B.Text);
            demarcateValue[18] = ValueFilter(InputSpeed_K.Text);
            demarcateValue[19] = ValueFilter("0");
            demarcateValue[20] = ValueFilter(OutputSpeed_K.Text);
            demarcateValue[21] = ValueFilter("0");

            DeviceDemarcate_Dt = MySQL_client.Getdataset("devicedemarcate", "applydevicedemarcate", "").Tables[0];
            LastDemarcateValue[0]=(DeviceDemarcate_Dt.Rows[0]["demarcatevalue"].ToString());
            LastDemarcateValue[1] = (DeviceDemarcate_Dt.Rows[1]["demarcatevalue"].ToString());
            LastDemarcateValue[2] = (DeviceDemarcate_Dt.Rows[2]["demarcatevalue"].ToString());
            LastDemarcateValue[3] = (DeviceDemarcate_Dt.Rows[3]["demarcatevalue"].ToString());
            LastDemarcateValue[4] = (DeviceDemarcate_Dt.Rows[4]["demarcatevalue"].ToString());
            LastDemarcateValue[5] = (DeviceDemarcate_Dt.Rows[5]["demarcatevalue"].ToString());
            LastDemarcateValue[6] = (DeviceDemarcate_Dt.Rows[6]["demarcatevalue"].ToString());
            LastDemarcateValue[7] = (DeviceDemarcate_Dt.Rows[7]["demarcatevalue"].ToString());
            LastDemarcateValue[8] = (DeviceDemarcate_Dt.Rows[8]["demarcatevalue"].ToString());
            LastDemarcateValue[9] = (DeviceDemarcate_Dt.Rows[9]["demarcatevalue"].ToString());
            LastDemarcateValue[10] = (DeviceDemarcate_Dt.Rows[10]["demarcatevalue"].ToString());
            LastDemarcateValue[11] = (DeviceDemarcate_Dt.Rows[11]["demarcatevalue"].ToString());
            LastDemarcateValue[12] = (DeviceDemarcate_Dt.Rows[12]["demarcatevalue"].ToString());
            LastDemarcateValue[13] = (DeviceDemarcate_Dt.Rows[13]["demarcatevalue"].ToString());
            LastDemarcateValue[14] = (DeviceDemarcate_Dt.Rows[14]["demarcatevalue"].ToString());
            LastDemarcateValue[15] = (DeviceDemarcate_Dt.Rows[15]["demarcatevalue"].ToString());
            LastDemarcateValue[16] = (DeviceDemarcate_Dt.Rows[16]["demarcatevalue"].ToString());
            LastDemarcateValue[17] = (DeviceDemarcate_Dt.Rows[17]["demarcatevalue"].ToString());
            LastDemarcateValue[18] = (DeviceDemarcate_Dt.Rows[18]["demarcatevalue"].ToString());
            LastDemarcateValue[19] = (DeviceDemarcate_Dt.Rows[19]["demarcatevalue"].ToString());
            LastDemarcateValue[20] = (DeviceDemarcate_Dt.Rows[20]["demarcatevalue"].ToString());
            LastDemarcateValue[21] = (DeviceDemarcate_Dt.Rows[21]["demarcatevalue"].ToString());
            if (!formatError)
            {
                //从数据库读取上次标定值
                DeviceDemarcateValueCompare(LastDemarcateValue, demarcateValue);
                //向数据库存储目前标定值
            }
            else
            {
                MessageBox.Show("输入格式错误，请输入正确数值，再进行标定！");
            }
        }

        /// <summary>
        /// 标定值比较
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        private void DeviceDemarcateValueCompare(string[] str1, string[] str2)
        {
            var diffStr = new string[22];
            var diffCount = 0;
            string[] data1 = str1;
            string[] data2 = str2;
            for (int j = 0; j < 22; j++)
            {
                if (j == 18 || j == 20)
                {
                    data1[j] = (Convert.ToDouble(data1[j])).ToString();
                    data2[j] = (Convert.ToDouble(data2[j])).ToString();
                }
                else
                {
                    data1[j] = (Convert.ToDouble(data1[j]) * 10).ToString();
                    data2[j] = (Convert.ToDouble(data2[j]) * 10).ToString();
                }
                
            }
            for (var i = 0; i < 22; i++)
            {
                if (data1[i] != data2[i])
                {
                    diffStr[diffCount] = "02 " + ((i + 2) / 2).ToString("X2") + " ";
                    if (i % 2 == 0)
                    {
                        if (Convert.ToInt32(data2[i]) >= 0)
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "00 ff " +
                                                 (Convert.ToInt32(data2[i]) % 256).ToString("X2") + " " +
                                                 (Convert.ToInt32(data2[i]) / 256).ToString("X2") + " ff ff";
                        }
                        else
                        {
                            data2[i] = data2[i].Substring(1, data2[i].Length - 1);
                            diffStr[diffCount] = diffStr[diffCount] + "01 ff " +
                                                 (Convert.ToInt32(data2[i]) % 256).ToString("X2") + " " +
                                                 (Convert.ToInt32(data2[i]) / 256).ToString("X2") + " ff ff";
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(data2[i]) >= 0)
                        {
                            diffStr[diffCount] = diffStr[diffCount] + "02 ff " +
                                                 (Convert.ToInt32(data2[i]) % 256).ToString("X2") + " " +
                                                 (Convert.ToInt32(data2[i]) / 256).ToString("X2") + " ff ff";
                        }
                        else
                        {
                            data2[i] = data2[i].Substring(1, data2[i].Length - 1);
                            diffStr[diffCount] = diffStr[diffCount] + "03 ff " +
                                                 (Convert.ToInt32(data2[i]) % 256).ToString("X2") + " " +
                                                 (Convert.ToInt32(data2[i]) / 256).ToString("X2") + " ff ff";
                        }
                    }
                    diffCount++;
                }
                
            }
            var sendDiffStr = new string[diffCount];
            for (int k = 0; k < diffCount; k++)
            {
                sendDiffStr[k] = diffStr[k];
                DemarcateKey(sendDiffStr[k]);
                DemarcateValue(sendDiffStr[k]);
                MySQL_client._updata("devicedemarcate", DemarcateKey(sendDiffStr[k]), DemarcateValue(sendDiffStr[k]));
                
            }
            if (diffCount > 0)
            {
                ((App)Application.Current).dataPro.DeviceDemarcate(sendDiffStr);
                
            }
            
        }

        /// <summary>
        /// 返回标定键值对key
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string DemarcateKey(string data)
        {
            var key = "";
            if (data.Length == 23)
            {
                var keyNum = data.Substring(3, 2);
                var keyCount = Convert.ToInt16(data.Substring(6, 2),16);
                if (keyNum == "01")
                {
                    key = "P_Source_"+KorB(keyCount);
                }
                if (keyNum == "02")
                {                   
                    key = "P_TCC_" + KorB(keyCount);
                }
                if (keyNum == "03")
                {
                    key = "P_Line_" + KorB(keyCount);
                }
                if (keyNum == "04")
                {
                    key = "P_Shift_" + KorB(keyCount);
                }
                if (keyNum == "05")
                {
                    key = "P_C1234_" + KorB(keyCount);
                }
                if (keyNum == "06")
                {
                    key = "P_CB26_" + KorB(keyCount);
                }
                if (keyNum == "07")
                {
                    key = "P_C35R_" + KorB(keyCount);
                }
                if (keyNum == "08")
                {
                    key = "P_C456_" + KorB(keyCount);
                }
                if (keyNum == "09")
                {
                    key = "Temperature_" + KorB(keyCount);
                }
                if (keyNum == "0A")
                {
                    key = "InputSpeed_" + KorB(keyCount);
                }
                if (keyNum == "0B")
                {
                    key = "OutputSpeed_" + KorB(keyCount);
                }
            }
            return key;

        }

        private string KorB(int data)
        {
            var kb = "";
            if (data < 2)
            {
                kb = "K";
            }
            else
            {
                kb = "B";
            }
            return kb;
        } 

        /// <summary>
        /// 返回标定键值对value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string DemarcateValue(string data)
        {
            var value = "";
            if(data.Length == 23)
            {
                if (Convert.ToInt32(data.Substring(6, 2)) % 2 != 0)
                {
                    value = "-";
                }
                if (data.Substring(0, 5) == "02 0A" || data.Substring(0, 5) == "02 0B")
                {
                    value = value + (Convert.ToDouble((Convert.ToInt32(data.Substring(12, 2), 16) + Convert.ToInt32(data.Substring(15, 2), 16) * 256))).ToString();
                }
                else
                {
                    value = value + (Convert.ToDouble((Convert.ToInt32(data.Substring(12, 2), 16) + Convert.ToInt32(data.Substring(15, 2), 16) * 256)) / 10).ToString();
                }
                
            }
            return value;
        }

        /// <summary>
        /// 去小数点后一位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string ValueFilter(string data)
        {
            var opintCount = data.Split('.');
            var spaceCount = data.Split(' ');

            if (spaceCount.Length == 2 || opintCount.Length >= 3 || Convert.ToDouble(data)>65535)
            {
                formatError = true;
            }
            int pointPostion = 0;
            pointPostion = data.IndexOf('.');
            if (pointPostion != -1)
            {
                if (data.Length == pointPostion + 1)
                {
                    data = data.Substring(0, data.Length - 1);
                }
                if (data.Length > pointPostion + 1)
                {
                    data = data.Substring(0, pointPostion + 2);
                }
            }
            return data;
        }

        #region 界面赋值
        /// <summary>
        /// P_Source_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Source_K(string msg)
        {
            this.P_Source_K.Dispatcher.Invoke(new outputDelegate(P_Source_KAction), msg);
        }

        private void P_Source_KAction(string msg)
        {
            P_Source_K.Text = msg;
        }

        /// <summary>
        /// P_Source_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Source_B(string msg)
        {
            this.P_Source_B.Dispatcher.Invoke(new outputDelegate(P_Source_BAction), msg);
        }

        private void P_Source_BAction(string msg)
        {
            P_Source_B.Text = msg;
        }

        /// <summary>
        /// P_TCC_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_TCC_K(string msg)
        {
            this.P_TCC_K.Dispatcher.Invoke(new outputDelegate(P_TCC_KAction), msg);
        }

        private void P_TCC_KAction(string msg)
        {
            P_TCC_K.Text = msg;
        }

        /// <summary>
        /// P_TCC_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_TCC_B(string msg)
        {
            this.P_TCC_B.Dispatcher.Invoke(new outputDelegate(P_TCC_BAction), msg);
        }

        private void P_TCC_BAction(string msg)
        {
            P_TCC_B.Text = msg;
        }

        /// <summary>
        /// P_Line_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Line_K(string msg)
        {
            this.P_Line_K.Dispatcher.Invoke(new outputDelegate(P_Line_KAction), msg);
        }

        private void P_Line_KAction(string msg)
        {
            P_Line_K.Text = msg;
        }

        /// <summary>
        /// P_Line_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Line_B(string msg)
        {
            this.P_Line_B.Dispatcher.Invoke(new outputDelegate(P_Line_BAction), msg);
        }

        private void P_Line_BAction(string msg)
        {
            P_Line_B.Text = msg;
        }

        /// <summary>
        /// P_Shift_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Shift_K(string msg)
        {
            this.P_Shift_K.Dispatcher.Invoke(new outputDelegate(P_Shift_KAction), msg);
        }

        private void P_Shift_KAction(string msg)
        {
            P_Shift_K.Text = msg;
        }

        /// <summary>
        /// P_Shift_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_Shift_B(string msg)
        {
            this.P_Shift_B.Dispatcher.Invoke(new outputDelegate(P_Shift_BAction), msg);
        }

        private void P_Shift_BAction(string msg)
        {
            P_Shift_B.Text = msg;
        }

        /// <summary>
        /// P_C1234_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C1234_K(string msg)
        {
            this.P_C1234_K.Dispatcher.Invoke(new outputDelegate(P_C1234_KAction), msg);
        }

        private void P_C1234_KAction(string msg)
        {
            P_C1234_K.Text = msg;
        }

        /// <summary>
        /// P_C1234_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C1234_B(string msg)
        {
            this.P_C1234_B.Dispatcher.Invoke(new outputDelegate(P_C1234_BAction), msg);
        }

        private void P_C1234_BAction(string msg)
        {
            P_C1234_B.Text = msg;
        }

        /// <summary>
        /// P_CB26_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_CB26_K(string msg)
        {
            this.P_CB26_K.Dispatcher.Invoke(new outputDelegate(P_CB26_KAction), msg);
        }

        private void P_CB26_KAction(string msg)
        {
            P_CB26_K.Text = msg;
        }

        /// <summary>
        /// P_CB26_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_CB26_B(string msg)
        {
            this.P_CB26_B.Dispatcher.Invoke(new outputDelegate(P_CB26_BAction), msg);
        }

        private void P_CB26_BAction(string msg)
        {
            P_CB26_B.Text = msg;
        }

        /// <summary>
        /// P_C35R_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C35R_K(string msg)
        {
            this.P_C35R_K.Dispatcher.Invoke(new outputDelegate(P_C35R_KAction), msg);
        }

        private void P_C35R_KAction(string msg)
        {
            P_C35R_K.Text = msg;
        }

        /// <summary>
        /// P_C35R_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C35R_B(string msg)
        {
            this.P_C35R_B.Dispatcher.Invoke(new outputDelegate(P_C35R_BAction), msg);
        }

        private void P_C35R_BAction(string msg)
        {
            P_C35R_B.Text = msg;
        }

        /// <summary>
        /// P_C456_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C456_K(string msg)
        {
            this.P_C456_K.Dispatcher.Invoke(new outputDelegate(P_C456_KAction), msg);
        }

        private void P_C456_KAction(string msg)
        {
            P_C456_K.Text = msg;
        }

        /// <summary>
        /// P_C456_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputP_C456_B(string msg)
        {
            this.P_C456_B.Dispatcher.Invoke(new outputDelegate(P_C456_BAction), msg);
        }

        private void P_C456_BAction(string msg)
        {
            P_C456_B.Text = msg;
        }

        /// <summary>
        /// Temperature_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputTemperature_K(string msg)
        {
            this.Temperature_K.Dispatcher.Invoke(new outputDelegate(Temperature_KAction), msg);
        }

        private void Temperature_KAction(string msg)
        {
            Temperature_K.Text = msg;
        }

        /// <summary>
        /// Temperature_B
        /// </summary>
        /// <param name="msg"></param>
        private void outputTemperature_B(string msg)
        {
            this.Temperature_B.Dispatcher.Invoke(new outputDelegate(Temperature_BAction), msg);
        }

        private void Temperature_BAction(string msg)
        {
            Temperature_B.Text = msg;
        }

        /// <summary>
        /// InputSpeed_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputInputSpeed_K(string msg)
        {
            this.InputSpeed_K.Dispatcher.Invoke(new outputDelegate(InputSpeed_KAction), msg);
        }

        private void InputSpeed_KAction(string msg)
        {
            InputSpeed_K.Text = msg;
        }

        /// <summary>
        /// OutputSpeed_K
        /// </summary>
        /// <param name="msg"></param>
        private void outputOutputSpeed_K(string msg)
        {
            this.OutputSpeed_K.Dispatcher.Invoke(new outputDelegate(OutputSpeed_KAction), msg);
        }

        private void OutputSpeed_KAction(string msg)
        {
            OutputSpeed_K.Text = msg;
        }

        /// <summary>
        /// textBoxTP_TCC
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_TCC(string msg)
        {
            this.textBoxTP_TCC.Dispatcher.Invoke(new outputDelegate(textBoxP_TCCAction), msg);
        }

        private void textBoxP_TCCAction(string msg)
        {
            textBoxTP_TCC.Text = msg;
        }

        /// <summary>
        /// textBoxP_Source
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_Source(string msg)
        {
            this.textBoxP_Source.Dispatcher.Invoke(new outputDelegate(textBoxP_SourceAction), msg);
        }

        private void textBoxP_SourceAction(string msg)
        {
            textBoxP_Source.Text = msg;
        }

        /// <summary>
        /// textBoxP_Shift
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_Shift(string msg)
        {
            this.textBoxP_Shift.Dispatcher.Invoke(new outputDelegate(textBoxP_ShiftAction), msg);
        }

        private void textBoxP_ShiftAction(string msg)
        {
            textBoxP_Shift.Text = msg;
        }

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
        /// textBoxP_C1234
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_C1234(string msg)
        {
            this.textBoxP_C1234.Dispatcher.Invoke(new outputDelegate(textBoxP_C1234Action), msg);
        }

        private void textBoxP_C1234Action(string msg)
        {
            textBoxP_C1234.Text = msg;
        }

        /// <summary>
        /// textBoxP_C1234
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_CB26(string msg)
        {
            this.textBoxP_CB26.Dispatcher.Invoke(new outputDelegate(textBoxP_CB26Action), msg);
        }

        private void textBoxP_CB26Action(string msg)
        {
            textBoxP_CB26.Text = msg;
        }

        /// <summary>
        /// textBoxP_C35R
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_C35R(string msg)
        {
            this.textBoxP_C35R.Dispatcher.Invoke(new outputDelegate(textBoxP_C35RAction), msg);
        }

        private void textBoxP_C35RAction(string msg)
        {
            textBoxP_C35R.Text = msg;
        }

        /// <summary>
        /// textBoxP_C35R
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxP_C456(string msg)
        {
            this.textBoxP_C456.Dispatcher.Invoke(new outputDelegate(textBoxP_C456Action), msg);
        }

        private void textBoxP_C456Action(string msg)
        {
            textBoxP_C456.Text = msg;
        }

        /// <summary>
        /// textBoxTemperature
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxTemperature(string msg)
        {
            this.textBoxTemperature.Dispatcher.Invoke(new outputDelegate(textBoxTemperatureAction), msg);
        }

        private void textBoxTemperatureAction(string msg)
        {
            textBoxTemperature.Text = msg;
        }

        /// <summary>
        /// textBoxInputSpeed
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxInputSpeed(string msg)
        {
            this.textBoxInputSpeed.Dispatcher.Invoke(new outputDelegate(textBoxInputSpeedAction), msg);
        }

        private void textBoxInputSpeedAction(string msg)
        {
            textBoxInputSpeed.Text = msg;
        }

        /// <summary>
        /// textBoxOutputSpeed
        /// </summary>
        /// <param name="msg"></param>
        private void outputtextBoxOutputSpeed(string msg)
        {
            this.textBoxOutputSpeed.Dispatcher.Invoke(new outputDelegate(textBoxOutputSpeedAction), msg);
        }

        private void textBoxOutputSpeedAction(string msg)
        {
            textBoxOutputSpeed.Text = msg;
        }
        
        #endregion



        /// <summary>
        /// 重置设备标定值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            string[] lastDemarcateValue = new string[22];
            string[] initDemarcateValue = new string[22];
            formatError = false;
            #region 获取界面标定值
            lastDemarcateValue[0] = ValueFilter(P_Source_K.Text);
            lastDemarcateValue[1] = ValueFilter(P_Source_B.Text);
            lastDemarcateValue[2] = ValueFilter(P_TCC_K.Text);
            lastDemarcateValue[3] = ValueFilter(P_TCC_B.Text);
            lastDemarcateValue[4] = ValueFilter(P_Line_K.Text);
            lastDemarcateValue[5] = ValueFilter(P_Line_B.Text);
            lastDemarcateValue[6] = ValueFilter(P_Shift_K.Text);
            lastDemarcateValue[7] = ValueFilter(P_Shift_B.Text);
            lastDemarcateValue[8] = ValueFilter(P_C1234_K.Text);
            lastDemarcateValue[9] = ValueFilter(P_C1234_B.Text);
            lastDemarcateValue[10] = ValueFilter(P_CB26_K.Text);
            lastDemarcateValue[11] = ValueFilter(P_CB26_B.Text);
            lastDemarcateValue[12] = ValueFilter(P_C35R_K.Text);
            lastDemarcateValue[13] = ValueFilter(P_C35R_B.Text);
            lastDemarcateValue[14] = ValueFilter(P_C456_K.Text);
            lastDemarcateValue[15] = ValueFilter(P_C456_B.Text);
            lastDemarcateValue[16] = ValueFilter(Temperature_K.Text);
            lastDemarcateValue[17] = ValueFilter(Temperature_B.Text);
            lastDemarcateValue[18] = ValueFilter(InputSpeed_K.Text);
            lastDemarcateValue[19] = ValueFilter("0");
            lastDemarcateValue[20] = ValueFilter(OutputSpeed_K.Text);
            lastDemarcateValue[21] = ValueFilter("0");
            #endregion

            #region 获取重置值
            DeviceDemarcate_Dt = MySQL_client.Getdataset("devicedemarcate", "initdevicedemarcate", "").Tables[0];
            initDemarcateValue[0] = (DeviceDemarcate_Dt.Rows[0]["demarcatevalue"].ToString());
            initDemarcateValue[1] = (DeviceDemarcate_Dt.Rows[1]["demarcatevalue"].ToString());
            initDemarcateValue[2] = (DeviceDemarcate_Dt.Rows[2]["demarcatevalue"].ToString());
            initDemarcateValue[3] = (DeviceDemarcate_Dt.Rows[3]["demarcatevalue"].ToString());
            initDemarcateValue[4] = (DeviceDemarcate_Dt.Rows[4]["demarcatevalue"].ToString());
            initDemarcateValue[5] = (DeviceDemarcate_Dt.Rows[5]["demarcatevalue"].ToString());
            initDemarcateValue[6] = (DeviceDemarcate_Dt.Rows[6]["demarcatevalue"].ToString());
            initDemarcateValue[7] = (DeviceDemarcate_Dt.Rows[7]["demarcatevalue"].ToString());
            initDemarcateValue[8] = (DeviceDemarcate_Dt.Rows[8]["demarcatevalue"].ToString());
            initDemarcateValue[9] = (DeviceDemarcate_Dt.Rows[9]["demarcatevalue"].ToString());
            initDemarcateValue[10] = (DeviceDemarcate_Dt.Rows[10]["demarcatevalue"].ToString());
            initDemarcateValue[11] = (DeviceDemarcate_Dt.Rows[11]["demarcatevalue"].ToString());
            initDemarcateValue[12] = (DeviceDemarcate_Dt.Rows[12]["demarcatevalue"].ToString());
            initDemarcateValue[13] = (DeviceDemarcate_Dt.Rows[13]["demarcatevalue"].ToString());
            initDemarcateValue[14] = (DeviceDemarcate_Dt.Rows[14]["demarcatevalue"].ToString());
            initDemarcateValue[15] = (DeviceDemarcate_Dt.Rows[15]["demarcatevalue"].ToString());
            initDemarcateValue[16] = (DeviceDemarcate_Dt.Rows[16]["demarcatevalue"].ToString());
            initDemarcateValue[17] = (DeviceDemarcate_Dt.Rows[17]["demarcatevalue"].ToString());
            initDemarcateValue[18] = (DeviceDemarcate_Dt.Rows[18]["demarcatevalue"].ToString());
            initDemarcateValue[19] = (DeviceDemarcate_Dt.Rows[19]["demarcatevalue"].ToString());
            initDemarcateValue[20] = (DeviceDemarcate_Dt.Rows[20]["demarcatevalue"].ToString());
            initDemarcateValue[21] = (DeviceDemarcate_Dt.Rows[21]["demarcatevalue"].ToString());
            #endregion

            #region 将重置值显示到界面上
            outputP_Source_K(initDemarcateValue[0]);
            outputP_Source_B(initDemarcateValue[1]);
            outputP_TCC_K(initDemarcateValue[2]);
            outputP_TCC_B(initDemarcateValue[3]);
            outputP_Line_K(initDemarcateValue[4]);
            outputP_Line_B(initDemarcateValue[5]);
            outputP_Shift_K(initDemarcateValue[6]);
            outputP_Shift_B(initDemarcateValue[7]);
            outputP_C1234_K(initDemarcateValue[8]);
            outputP_C1234_B(initDemarcateValue[9]);
            outputP_CB26_K(initDemarcateValue[10]);
            outputP_CB26_B(initDemarcateValue[11]);
            outputP_C35R_K(initDemarcateValue[12]);
            outputP_C35R_B(initDemarcateValue[13]);
            outputP_C456_K(initDemarcateValue[14]);
            outputP_C456_B(initDemarcateValue[15]);
            outputTemperature_K(initDemarcateValue[16]);
            outputTemperature_B(initDemarcateValue[17]);
            outputInputSpeed_K(initDemarcateValue[18]);
            outputOutputSpeed_K(initDemarcateValue[20]);
            #endregion
            if (!formatError)
            {
                //从数据库读取上次标定值
                DeviceDemarcateValueCompare(lastDemarcateValue, initDemarcateValue);
                //向数据库存储目前标定值
            }
            else
            {
                MessageBox.Show("输入格式错误，请输入正确数值，再进行标定！");
            }
            
        }
        /// <summary>
        /// ResetBt
        /// </summary>
        /// <param name="msg"></param>
        private void outputBtnReset()
        {
            this.ResetBt.Dispatcher.Invoke(new changeDelegate(ResetBtAction));
        }

        private void ResetBtAction()
        {
            ResetBt.IsEnabled = true;
        }

        /// <summary>
        /// ResetBt
        /// </summary>
        /// <param name="msg"></param>
        private void outputBtnset()
        {
            this.ApplicationBt.Dispatcher.Invoke(new changeDelegate(BtnsetAction));
        }

        private void BtnsetAction()
        {
            ApplicationBt.IsEnabled = true;
        }

        #region 输入格式判断
        private void TB_MC_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_MC_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }




        private void TB_P1_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_P1_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_P2_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_P2_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }


        private void TB_PC_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_PC_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_PL_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_PL_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_SI_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9")
            {
                MessageBox.Show("只能输入正整数！");
                e.Handled = true;
            }
        }

        private void TB_SO1_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9")
            {
                MessageBox.Show("只能输入正整数！");
                e.Handled = true;
            }
        }

        private void TB_SO2_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9")
            {
                MessageBox.Show("只能输入正整数！");
                e.Handled = true;
            }
        }

        private void TB_TC_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TC_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TI_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TI_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TO1_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TO1_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TO2_B_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }

        private void TB_TO2_K_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text != "-" && e.Text != "0" && e.Text != "1" && e.Text != "2" && e.Text != "3" && e.Text != "4" && e.Text != "5" && e.Text != "6" && e.Text != "7" && e.Text != "8" && e.Text != "9" && e.Text != ".")
            {
                MessageBox.Show("只能输入数字和小数点！");
                e.Handled = true;
            }
        }
        #endregion



        #region 显示接口定义数值

        

        

        public void OnFragmentNavigation(FragmentNavigationEventArgs e)
        {

        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {

        }

        //UserControl入口
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            RegisterObserver((Application.Current as App).dataPro);
            (Application.Current as App).dataPro.CollectMcuData(true);
        }
        //UserControl出口
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            RemoveRegister((Application.Current as App).dataPro);
            (Application.Current as App).dataPro.CollectMcuData(false);
        }

        public void Update(string[,] data)
        {
            if (updateCount == 4)
            {
                outputtextBoxP_Source(data[4, 0]);
                outputtextBoxP_TCC(data[4, 1]);
                outputtextBoxP_Line(data[4, 2]);
                outputtextBoxP_Shift(data[4, 3]);
                outputtextBoxP_C1234(data[4, 4]);
                outputtextBoxP_CB26(data[4, 5]);
                outputtextBoxP_C35R(data[4, 6]);
                outputtextBoxP_C456(data[4, 7]);
                outputtextBoxTemperature(data[4, 7]);
                outputtextBoxInputSpeed(data[4, 8]);
                outputtextBoxOutputSpeed(data[4, 9]);
                updateCount = 0;
            }
            updateCount++;
        }


        #endregion 显示接口定义数值

        /// <summary>
        /// 继承数据处理模块接口
        /// </summary>
        private Subject dataPro;

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
    }
}