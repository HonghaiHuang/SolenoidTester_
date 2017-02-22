using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Shapes;

namespace SolenoidTester.group
{
    /// <summary>
    /// ExportToExcel.xaml 的交互逻辑
    /// </summary>
    public partial class ExportToExcel : System.Windows.Window
    {



        /// <summary>
        /// 数据源
        /// </summary>
       public System.Data.DataTable ExportExcelDt=new System.Data.DataTable("Char");

        public ExportToExcel()
        {
            InitializeComponent();
        }

        private void Export_star_Click(object sender, RoutedEventArgs e)
        {
            //这一句，就是让后台工作开始。
            this.backgroundWorker1.RunWorkerAsync();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            Export_star.IsEnabled = false;
        }


        BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        public void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        {
            if (tmpDataTable == null)
                return;
            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;

            Microsoft.Office.Interop.Excel.Application xlApp = new ApplicationClass();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Workbook xlBook = xlApp.Workbooks.Add(true);





            Probar.Dispatcher.Invoke(

            delegate
            {
                Probar.Maximum = rowNum;
            }
            );

            DataSum.Dispatcher.Invoke(

            delegate
            {
                DataSum.Content = rowNum;
            }
            );

            


            //将DataTable的列名导入Excel表第一行
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }

            //将DataTable中的数据导入Excel中
            for (int i = 0; i < rowNum; i++)
            {
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;
                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
                }
                    backgroundWorker1.ReportProgress(i);
            }
            //xlBook.SaveCopyAs(HttpUtility.UrlDecode(strFileName, System.Text.Encoding.UTF8));
            xlBook.SaveCopyAs(strFileName);
        }
        //这里就是通过响应消息，来处理界面的显示工作

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Probar.Value = e.ProgressPercentage;
            
            CurrentPercentage.Content=((e.ProgressPercentage/ Convert.ToDouble( DataSum.Content))*100).ToString("f2")+"%";
            //this.progressBar1.Value = e.ProgressPercentage;
            this.CurrentValue.Content = e.ProgressPercentage;
            //this.label1.Update();
        }

        //这里是后台工作完成后的消息处理，可以在这里进行后续的处理工作。

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Export_star.IsEnabled = true;
            MessageBox.Show("导出成功！");
        }

        //这里，就是后台进程开始工作时，调用工作函数的地方。你可以把你现有的处理函数写在这儿。

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string SaveFilePath = "";

            FilePath.Dispatcher.Invoke(

            delegate
            {
                SaveFilePath = FilePath.Text;
            }
            );

            // work(this.backgroundWorker1);
            System.Data.DataTable Dt = new System.Data.DataTable();
            DataTabletoExcel(ExportExcelDt, SaveFilePath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FilePath.Text = "D:/" + Window.Title + ".xlsx";

        }
    }
}
