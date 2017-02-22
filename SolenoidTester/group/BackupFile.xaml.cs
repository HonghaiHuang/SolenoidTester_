using Microsoft.Office.Interop.Excel;
using SolenoidTester.DataClass;
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
    public partial class BackupFile : System.Windows.Window
    {



        /// <summary>
        /// 数据源
        /// </summary>
        public System.Data.DataTable ExportExcelDt = new System.Data.DataTable("Char");

        public BackupFile()
        {
            InitializeComponent();
        }

        private void Export_star_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker1 = new BackgroundWorker();
            //这一句，就是让后台工作开始。
            this.backgroundWorker1.RunWorkerAsync();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            Export_star.IsEnabled = false;
        }


        public  string[] SeleteDataRow1;
        public string[] SeleteDataRow2;
        string SavefilePate = "E:\\MySql\\Data" + System.DateTime.Now.ToString("yyyyMMddHHmm") + ".rar";
        string _SQLfilePath_2 = "E:\\MySql\\Data";
        string _ExcelfilePath_2 = "D:\\产品档案\\自动测试\\";

        private void Initdata()
        {

            Probar.Dispatcher.Invoke(

            delegate
            {
                Probar.Maximum = SeleteDataRow1.Length;
            }
            );

            DataSum.Dispatcher.Invoke(

            delegate
            {
                DataSum.Content = SeleteDataRow1.Length;
            }
            );

            FileHelper.CreateDirectory(_SQLfilePath_2);
           
            for (int k = 0; k < SeleteDataRow1.Length; k++)
            {
                BackupMySQL.Backup(SeleteDataRow1[k], _SQLfilePath_2 + "\\" + SeleteDataRow1[k] + ".sql");
                FileHelper.Copy(_ExcelfilePath_2 + SeleteDataRow2[k] + "\\" + SeleteDataRow1[k] + ".xls", _SQLfilePath_2 + "\\" + SeleteDataRow1[k] + ".xls");
                backgroundWorker1.ReportProgress(k);
                System.Threading.Thread.Sleep(350);
            }




           

            //   ZipHelper.Zip(filePath_2, FileHelper.GetFileNameNoExtension(SavefilePate) + ".rar");

        }
        BackgroundWorker backgroundWorker1;
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

            CurrentPercentage.Content = ((e.ProgressPercentage / Convert.ToDouble(DataSum.Content)) * 100).ToString("f2") + "%";
            //this.progressBar1.Value = e.ProgressPercentage;
            this.CurrentValue.Content = e.ProgressPercentage;
            //this.label1.Update();
        }

        //这里是后台工作完成后的消息处理，可以在这里进行后续的处理工作。

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
              Export_star.IsEnabled = true;
            System.Threading.Thread.Sleep(500);
            if (ZipHelper.Zip(_SQLfilePath_2, SavefilePate) == true)
            {
                    CurrentPercentage.Content = "100%";
                CurrentValue.Content = DataSum.Content;
                Probar.Value = Convert.ToDouble(DataSum.Content);
                MessageBox.Show("文件备份成功：" + SavefilePate);
                   System.Threading.Thread.Sleep(300);
                FileHelper.ClearDirectory(_SQLfilePath_2);
            }
            else
            {
                MessageBox.Show("备份失败！");
            }

            backgroundWorker1 = null;
            //  MessageBox.Show("导出成功！");
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
            Initdata();
            // work(this.backgroundWorker1);
            //   DataTabletoExcel(ExportExcelDt, SaveFilePath);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FilePath.Text = SavefilePate;

        }
    }
}
