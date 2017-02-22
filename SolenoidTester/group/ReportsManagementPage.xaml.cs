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
using FirstFloor.ModernUI.Windows.Controls;
using Telerik.Windows.Controls;
using System.Windows.Threading;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls.Primitives;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Navigation;
using SolenoidTester.DataClass;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;

namespace SolenoidTester.group
{
    // taken from MSDN (http://msdn.microsoft.com/en-us/library/system.windows.controls.datagrid.aspx)
    public enum OrderStatus { None, New, Processing, Shipped, Received };
    public class Customer
    {
        public string TestDate { get; set; }
        public string WorkOrder { get; set; }
        public string CustomerName { get; set; }
        public bool IsSelected { get; set; }
        public OrderStatus Status { get; set; }
    }


    public class LogRecord
    {
     //   public string Name { get; set; }
    }


    /// <summary>
    /// Interaction logic for reportsManagement.xaml
    /// </summary>
    public partial class ReportsManagementPage : UserControl, IContent
    {

        /// <summary>
        /// 变量定义表单
        /// </summary>
        private System.Data.DataTable _VariableDefinitions = new System.Data.DataTable("变量定义");

        private double _LineNum = 0;

        string[] databaseName;
        /// <summary>
        /// 实例化详细曲线窗口
        /// </summary>
        ReportsCharView windowChar;


        /// <summary>
        /// 数据库文件文件夹路径
        /// </summary>
        public string Path_mdb = Directory.GetCurrentDirectory() + "/数据库文件";


        /// <summary>
        /// 实例化数据库处理模块
        /// </summary>
      //  private AccessCom com = new AccessCom();

        /// <summary>
        /// 当前绑定datagrid资源数据表
        /// </summary>
        private System.Data.DataTable tblDatas = new System.Data.DataTable("Datas");

        /// <summary>
        /// 用于筛选转换数据表
        /// </summary>
        private System.Data.DataTable tbl = new System.Data.DataTable("Datas");

        /// <summary>
        /// 右击预览数据表
        /// </summary>
        private System.Data.DataTable dt2 = new System.Data.DataTable();


        /// <summary>
        /// 入口函数
        /// </summary>
        public ReportsManagementPage()
        {
            InitializeComponent();


            try
            {
                
                testx();
                
                _VariableDefinitions = ImportExcel.InsernExcelFile("变量定义", Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_TestStandard_dataAnalysis.xls");//
                Initdata();
                //   this.DataContext = new NorthwindEntities().Customers.OrderBy(c => c.CustomerID);
            }
            catch(Exception erro)
            {
                MessageBox.Show(erro.Message);
            }
            //ObservableCollection<Customer> userdata1 = testx();
            //DataContext = userdata1;
            //   IniAdmin();

            //   intdable();
            //  ObservableCollection<User> userdata = PreviewGetData(dt2);
            //  dataGrid.DataContext = userdata;

            //    Inid();

        }

        DataRow newRow;
        private void CreatetblDatas()
        {
            DataColumn dc = null;
            dc = tblDatas.Columns.Add("ID", Type.GetType("System.Int32"));
            dc.AutoIncrement = true;//自动增加    
            dc = tblDatas.Columns.Add("流水号", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("操作者", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("报告编号", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("零件号", Type.GetType("System.String"));

            dc = tblDatas.Columns.Add("刷写软件号", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("商用状态", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("制造追踪码", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("VIN", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("故障码", Type.GetType("System.String"));
            dc = tblDatas.Columns.Add("测试日期", Type.GetType("System.DateTime"));

            


            newRow = tblDatas.NewRow();

        }


        private void testx()
        {

            string SavePath = "D:/产品档案/自动测试/";
            //  System.IO.DirectoryInfo dir = new DirectoryInfo(SavePath);

            //    FileInfo[] fiList = dir.GetFiles();
            //  string[] ll= GetDirectories(SavePath, "*", true);
            //  string[] l2 = GetFileNames(SavePath, "*", true);
            CreateDirectory(SavePath);//判断目录是否存在，不存在则创建
            string[] databaseName_Excel = GetFileNameNoExtension(GetFileNames(SavePath, "*.xls", true));
            int fiList_Count = 0;


            CreatetblDatas();
            //            DataTable Dt = new DataTable();



            //    var logs = new ObservableCollection<LogRecord>();
            databaseName = MySQL_client.Showdatabase();
            var _IntersectionResults = databaseName.Intersect<string>(databaseName_Excel);

            foreach (var _Value in _IntersectionResults)
            {
                System.Data.DataTable MySqlDt = MySQL_client.Getdataset(_Value, "TestInformation", "").Tables[0];
                if (MySqlDt != null)
                {
                    try
                    { 
                    newRow = tblDatas.NewRow();
                    //newRow["TypeName"] = Dt.Rows[j-1]["告警日志"];
                    newRow["流水号"] = _Value;
                    newRow["操作者"] = MySqlDt.Rows[0]["操作者"].ToString();
                    newRow["报告编号"] = MySqlDt.Rows[0]["报告编号"].ToString();
                    newRow["零件号"] = MySqlDt.Rows[0]["零件号"].ToString();
                    newRow["刷写软件号"] = MySqlDt.Rows[0]["刷写软件号"].ToString();
                    newRow["商用状态"] = MySqlDt.Rows[0]["商用状态"].ToString();
                    newRow["制造追踪码"] = MySqlDt.Rows[0]["制造追踪码"].ToString();
                    newRow["VIN"] = MySqlDt.Rows[0]["VIN"].ToString();
                    newRow["故障码"] = MySqlDt.Rows[0]["故障码"].ToString();
                    newRow["测试日期"] = Convert.ToDateTime(MySqlDt.Rows[0]["报告编号"].ToString().Substring(0, 4) + "/" + MySqlDt.Rows[0]["报告编号"].ToString().Substring(4, 2) + "/" + MySqlDt.Rows[0]["报告编号"].ToString().Substring(6, 2));
                    tblDatas.Rows.Add(newRow);
                    Debug.WriteLine("交集：" + _Value);
                    }
                    catch
                    {
                    }
                }
                fiList_Count++;
            }

            DG1.ItemsSource = tblDatas.DefaultView;
          //  return logs;
        }

        #region 创建一个目录  
        /// <summary>  
        /// 创建一个目录  
        /// </summary>  
        /// <param name="directoryPath">目录的绝对路径</param>  
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录  
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion

        #region 检测指定目录是否存在  
        /// <summary>  
        /// 检测指定目录是否存在  
        /// </summary>  
        /// <param name="directoryPath">目录的绝对路径</param>          
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion


        /// <summary>  
        /// 获取指定目录及子目录中所有子目录列表  
        /// </summary>  
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。  
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>  
        /// <param name="isSearchChild">是否搜索子目录</param>  
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                return Directory.GetDirectories(directoryPath, searchPattern, isSearchChild ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch
            {
                throw null;
            }
        }

        /// <summary>  
        /// 获取指定目录及子目录中所有文件列表  
        /// </summary>  
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。  
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>  
        /// <param name="isSearchChild">是否搜索子目录</param>  
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            ////如果目录不存在，则抛出异常  
            //if (!IsExistDirectory(directoryPath))
            //{
            //    throw new FileNotFoundException();
            //    direct
            //}

            try
            {
                return Directory.GetFiles(directoryPath, searchPattern, isSearchChild ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>  
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )  
        /// </summary>  
        /// <param name="filePath">文件的绝对路径</param>          
        public static string[] GetFileNameNoExtension(string[] filePath)
        {
            FileInfo fi;
            string[]  filePath2 = new string[filePath.Length];
            //获取文件的名称  
            for (int i=0;i< filePath.Length;i++)
            {
                 fi= new FileInfo(filePath[i]);
                filePath2[i] = fi.Name.Split('.')[0];
            }
            return filePath2;
        }

        /// <summary>
        /// 合并重复，删除下拉选项框中重复项
        /// </summary>
        /// <param name="combobox"></param>
        public void MergeRepeat(ComboBox combobox)
        {
            int count = combobox.Items.Count;
            for (int i = 0; i < count; i++)
            {
                //int num=cb.FindString(cb.Items[1].ToString());
                string str = combobox.Items[i].ToString();
                for (int j = i + 1; j < count; j++)
                {
                    string str1 = combobox.Items[j].ToString();
                    if (str1 == str)
                    {
                        combobox.Items.RemoveAt(j);
                        count--;
                        j--;
                    }
                }

            }
        }


        /// <summary>
        /// 单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DG1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView mySelectedElement = (DataRowView) DG1.SelectedItem;
                //要添加这条件
                if (mySelectedElement != null)
                {
                    WorkID.Text = mySelectedElement.Row[2].ToString();
                    ComboCustomer.Text = mySelectedElement.Row[3].ToString();
                    Identifier_Text.Text = mySelectedElement.Row[4].ToString();
            //        Transmission_Number_Text.Text = mySelectedElement.Row[5].ToString();
                  //  result1 = Path_mdb + "/" + mySelectedElement.Row[7].ToString() + ".mdb";
                }
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }

        }

        //预览
        System.Data.DataTable PreviewDt = new System.Data.DataTable("PreviewDts");







        /// <summary>
        /// 双击打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        int i;
        int j = 0;
        private void DG1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string result = "";
                i += 1;
                if (e.ChangedButton == MouseButton.Left)
                {
                    System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

                    timer.Interval = new TimeSpan(0, 0, 0, 0, 300);

                    timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };

                    timer.IsEnabled = true;

                    if (i % 2 == 0)

                    {
                        DataRowView mySelectedElement = (DataRowView) DG1.SelectedItem;
                        //   Customer mySelectedElement = (Customer) DG1.SelectedItem;
                        //要添加这条件
                        if (mySelectedElement != null)
                        {
                            //     result = mySelectedElement.WorkOrder;
                            result = mySelectedElement.Row[7].ToString();
                            System.Diagnostics.Process.Start(Path_mdb + "/" + result + ".mdb");
                        }




                        timer.IsEnabled = false;

                        i = 0;

                        //this.WindowState = this.WindowState == WindowState.Maximized ?

                        //              WindowState.Normal : WindowState.Maximized;

                    }
                }
                if (e.ChangedButton == MouseButton.Middle)
                {
                    DataRowView mySelectedElement = (DataRowView) DG1.SelectedItem;
                    //要添加这条件
                    if (mySelectedElement != null)
                    {
                        result = mySelectedElement.Row[7].ToString();
                        System.Diagnostics.Process.Start(Path_mdb + "/" + result + ".mdb");
                    }

                }

            }
            catch (Exception erromsg)
            {

                MessageBox.Show(erromsg.Message);
            }
        }
        /// <summary>
        /// 工单号模糊查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DataView view = new DataView();
                if (tblDatas.Rows.Count > 0)
                {
                    view.Table = tblDatas;
                    view.RowFilter = "零件号 like '%" + WorkID.Text + "%'";//itemType是A中的一个字段
                                                                              //               Debug.WriteLine(tblDatas.Rows[i]["工单号"].ToString());
                    tbl = view.ToTable();
                    DG1.ItemsSource = tbl.DefaultView;
                }
            }
        }

        #region 功能按钮

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string[] SeleteDataRow1 = DataRowViewToString("流水号");
                string[] SeleteDataRow2 = DataRowViewToString("零件号");
                string filePath= "D:/产品档案/自动测试/";//判断目录是否存在，不存在则创建
                for(int k=0;k< SeleteDataRow1.Length;k++)
                {

                    string _filePath = filePath + SeleteDataRow2[k] + "/" + SeleteDataRow1[k] + ".xls";
                    FileHelper.DeleteFile(_filePath);
                    MySQL_client.DeleteMySQL(SeleteDataRow1[k]);                 
                }
                tblDatas = new System.Data.DataTable("Datas");
                testx();
                //  FileHelper.DeleteFile();
                //foreach (DataRow dr in dt.Rows)
                //{
                //    string rrr = dr[0].ToString();
                //    if (rrr == "True")
                //    {
                //        if (File.Exists(Path_mdb + "/" + dr[7].ToString() + ".mdb"))
                //        {
                //            //如果存在则删除
                //            File.Delete(Path_mdb + "/" + dr[7].ToString() + ".mdb");

                //            tblDatas = new System.Data.DataTable("Datas");
                //        }
                //        else
                //        {
                //            MessageBox.Show("请选中需要删除的文件！");
                //        }
                //    }
                //}

                //MessageBox.Show("删除文件成功！");
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }

        }

        /// <summary>
        /// DataRowView转数组
        /// </summary>
        /// <param name="_ColumnName"></param>
        /// <returns></returns>
        private string[] DataRowViewToString(string _ColumnName)
        {
            try
            {
                DataRowView[] Dtv = SelectedItemsToDataRowView();
                string[] datast = new string[Dtv.Length];
                int i = 0;
                foreach (var data in Dtv)
                {
                    datast[i] = data.Row[_ColumnName].ToString();
                    i++;
                }
                return datast;
            }
            catch
            { return null; }
        }


        /// <summary>
        /// 返回选中的数据
        /// </summary>
        /// <returns></returns>
        private DataRowView[] SelectedItemsToDataRowView()
        {
            //int count = DG1.SelectedItems.Count;
            //DataRowView[] drv = new DataRowView[count];
            //for (int i = 0; i < count; i++)
            //{
            //    drv[i] = DG1.SelectedItems[i] as DataRowView;
            //}

            //return drv;                //dt = DataGridToDatable(DG1);



            //当选中有多个单元格时，获取选中单元格所在行的数组  
            //排除数组中相同的行  
            if (DG1 != null && DG1.SelectedCells.Count > 0)
            {
                DataRowView[] dv = new DataRowView[DG1.SelectedCells.Count];
                for (int i = 0; i < DG1.SelectedCells.Count; i++)
                {
                    dv[i] = DG1.SelectedCells[i].Item as DataRowView;

                }
                //因为选中的单元格可能在同一行的，需要排除重复的行  
                return dv.Distinct().ToArray();
            }
            return null;
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Query_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataView view = new DataView();
                if (tblDatas.Rows.Count > 0)
                {
                    string TIME1 = Convert.ToDateTime(StartTime.Text).ToString("yyyy-MM-dd");
                    string TIME2 = Convert.ToDateTime(StopTime.Text).ToString("yyyy-MM-dd");


                    //string combostate;
                    //if (ComboState.Text != "全部")
                    //{ combostate = ComboState.Text; }
                    //else
                    //{ combostate = ""; }

                    string combocustomer;
                    if (ComboCustomer.Text != "全部")
                    { combocustomer = ComboCustomer.Text; }
                    else
                    { combocustomer = ""; }


                    view.Table = tblDatas;
                    //if (combostate != "")
                    //{
                    //    view.RowFilter = "WorkOrder like '" + WorkID.Text + "%'" + "and Transmission_Number like '" + Transmission_Number_Text.Text + "%'" + "and Identifier like '" + Identifier_Text.Text + "%'" + "and CustomerName like '" + combocustomer + "%'" + "and Status = '" + combostate + "'" + "and TestDate >= '" + TIME1 + "' and TestDate <= '" + TIME2 + "'";
                    //}
                    //else
                    //{
                        view.RowFilter = "零件号 like '" + WorkID.Text + "%'"  + "and 操作者 like '" + Identifier_Text.Text + "%'" + "and 制造追踪码 like '" + combocustomer + "%'" + "and 测试日期 >= '" + TIME1 + "' and 测试日期 <= '" + TIME2 + "'";
               //     }
                    tbl = view.ToTable();
                    DG1.ItemsSource = tbl.DefaultView;
                }


            }
            catch (Exception Erromsg)
            {

                MessageBox.Show(Erromsg.Message);
            }
        }

        /// <summary>
        /// 客户名称筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    DataView view = new DataView();
                    if (tblDatas.Rows.Count > 0)
                    {
                        view.Table = tblDatas;
                        view.RowFilter = "制造追踪码 like '%" + ComboCustomer.Text + "%'";//itemType是A中的一个字段
                                                                                            //               Debug.WriteLine(tblDatas.Rows[i]["工单号"].ToString());
                        tbl = view.ToTable();
                        DG1.ItemsSource = tbl.DefaultView;
                    }
                }

            }
            catch (Exception Erromsg)
            {

                MessageBox.Show(Erromsg.Message);
            }

        }

        /// <summary>
        /// 操作者识别码筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Identifier_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {

                    DataView view = new DataView();
                    if (tblDatas.Rows.Count > 0)
                    {
                        view.Table = tblDatas;
                        view.RowFilter = "操作者 like '%" + Identifier_Text.Text + "%'"; //itemType是A中的一个字段
                                                                                             //               Debug.WriteLine(tblDatas.Rows[i]["工单号"].ToString());
                        tbl = view.ToTable();
                        DG1.ItemsSource = tbl.DefaultView;
                    }
                }

            }
            catch (Exception Erromsg)
            {

                MessageBox.Show(Erromsg.Message);
            }

        }
        /// <summary>
        /// 变速箱号码筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Transmission_Number_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    DataView view = new DataView();
                    if (tblDatas.Rows.Count > 0)
                    {
                        view.Table = tblDatas;
                    //    view.RowFilter = "Transmission_Number like '%" + Transmission_Number_Text.Text + "%'";
                        //itemType是A中的一个字段
                        //               Debug.WriteLine(tblDatas.Rows[i]["工单号"].ToString());
                        tbl = view.ToTable();
                        DG1.ItemsSource = tbl.DefaultView;
                    }
                }

            }
            catch (Exception Erromsg)
            {

                MessageBox.Show(Erromsg.Message);
            }

        }




        /// <summary>
        /// 转存按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {

            BackupMySQL _BackupMySQL = new BackupMySQL();

            string[] SeleteDataRow1 = DataRowViewToString("流水号");
            string[] SeleteDataRow2 = DataRowViewToString("零件号");
            string filePath = "D:/产品档案/自动测试/";//判断目录是否存在，不存在则创建

            BackupFile _BackupFile = new BackupFile();
            _BackupFile.SeleteDataRow1 = SeleteDataRow1;
            _BackupFile.SeleteDataRow2 = SeleteDataRow2;
            _BackupFile.Show();

           
            //  string SavefilePate= BackupMySQL._saveFileDialog(".rar", "压缩文件|*.rar","Data"+System.DateTime.Now.ToString("yyyyMMddHHmm"));
            //  string filePath_2 = SavefilePate.Replace(SavefilePate.Substring(SavefilePate.LastIndexOf('\\')),"");

        }

        private void InitSaveFile(object dt9)
        {
            System.Data.DataTable dt8 = (System.Data.DataTable) dt9;
            SaveFileDialog dlg = new SaveFileDialog();
            //   dlg.DefaultExt = ".mdb";
            dlg.Filter = "Access 文件|*.mdb|*.";
            string resultpath1 = "";
            foreach (DataRow dr in dt8.Rows)
            {
                string rrr = dr[0].ToString();
                if (rrr == "True")
                {
                    resultpath1 += "," + dr[7].ToString() + ".mdb";
                    dlg.FileName = resultpath1;
                }
            }


            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                //dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\"));
                // System.IO.File.Copy(resultpath, dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\")) + "/" + WorkID.Text + ".mdb", true);
                //     System.IO.File.Copy(resultpath, dlg.FileName, true);



                foreach (DataRow dr in dt8.Rows)
                {


                    string rrr = dr[0].ToString();
                    if (rrr == "True")
                    {
                        string resultpath = Path_mdb + "/" + dr[7].ToString() + ".mdb";
                        System.IO.File.Copy(resultpath, dlg.FileName.Substring(0, dlg.FileName.LastIndexOf("\\")) + "/" + dr[7].ToString() + ".mdb", true);


                    }

                }
                MessageBox.Show("转存成功！");
                //        textBox1.Text = dlg.FileName;
            }

        }



        /// <summary>
        /// 导入按钮（导入数据库）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // string SavefilePate = BackupMySQL._saveFileDialog(".rar", "压缩文件|*.rar", "Data" + System.DateTime.Now.ToString("yyyyMMddHHmm"));
            OpenFileDialog dlg = new OpenFileDialog();
            //   dlg.DefaultExt = ".mdb";
            dlg.Filter = "压缩文件|*.rar";
            Nullable<bool> result = dlg.ShowDialog();
            string FileNameSart = dlg.FileName;
            string FileNameEnd = "E:\\MySql\\UnZIP";
            bool IsUnZIP = ZipHelper.UnZip(FileNameSart, FileNameEnd, null);

           // Thread.Sleep(5000);
            if(IsUnZIP==true)
            {
                string[] AllInsertFile = GetFileNameNoExtension(GetFileNames(FileNameEnd+ "/Data/", "*.sql", true));
                for(int j=0;j< AllInsertFile.Length;j++)
                {
                    if(FileHelper.IsExistFile("D:/产品档案/自动测试/" + DataIndexOf(AllInsertFile[j]) + "/Data/" + AllInsertFile[j] + ".xls") ==false)
                    {
                        RestoreMySql.InitRestoreMySql(AllInsertFile[j], databaseName, FileNameEnd + "\\Data\\" + AllInsertFile[j] + ".sql");
                  //      Thread.Sleep(300);
                        CreateDirectory("D:/产品档案/自动测试/" + DataIndexOf(AllInsertFile[j]));//判断目录是否存在，不存在则创建
                        FileHelper.Copy(FileNameEnd + "/Data/" + AllInsertFile[j] + ".xls", "D:/产品档案/自动测试/" + DataIndexOf(AllInsertFile[j]) + "/" + AllInsertFile[j] + ".xls");
                    }
                }
            }
            //tblDatas = new System.Data.DataTable("Datas");
            //testx();

            //  bool pppp = ZipArchive.UnZip2(FileName, @"E:\MySql\Data\");
            // RestoreMySql.InitRestoreMySql("", databaseName);






            //   SavefilePate.Replace(SavefilePate.Substring(SavefilePate.LastIndexOf('\\')), "");


            //string ksfsadkkk = "20162-haha-3po-555";
            //ksfsadkkk = ksfsadkkk.Substring(ksfsadkkk.IndexOf('-'));
            //ksfsadkkk = ksfsadkkk.Replace(ksfsadkkk.Substring(ksfsadkkk.LastIndexOf('-')), "");
            //ksfsadkkk = ksfsadkkk.Replace("-", "");

            // sksfsadkkk.LastIndexOf('\\'));
            //try
            //{
            //    Thread InitInsertAccessthread = new Thread(InitInsertAccessThread);
            //    InitInsertAccessthread.Start();
            //    InitInsertAccessthread.Join();
            //    if (IsInsertAccessOK == true)
            //    {
            //        tblDatas = new System.Data.DataTable("Datas");
            //        testx();
            //    }

            //}
            //catch (Exception Erromsg)
            //{
            //    MessageBox.Show(Erromsg.Message);
            //}
        }

        /// <summary>
        /// 从字符串查询相关字符
        /// </summary>
        /// <returns></returns>
        private string DataIndexOf(string data)
        {         
            data = data.Substring(data.IndexOf('_'));
            data = data.Replace(data.Substring(data.LastIndexOf('_')), "");
            data = data.Replace("_", "");
            return data;
        }

        private bool IsInsertAccessOK;
        private void InitInsertAccessThread()
        {
            IsInsertAccessOK = InitInsertAccess();
        }

        DataView InitInsert_view;
        private bool InitInsertAccess()
        {

            // DataTable dt3 = (DataTable) dt_3;
            System.Data.DataTable Dt2;

            DataRow newRow;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();


            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".mdb";
            dlg.Filter = "Access 数据库文件|*.mdb";
            dlg.Multiselect = true;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                Dt2 = null;
                // Open document 
                string filename2;
                string filename3 = "";//不包含路径文件名

                for (int i = 0; i < dlg.FileNames.Length; i++)//根据数组长度定义循环次数
                {
                    try
                    {


                        filename2 = dlg.FileNames.GetValue(i).ToString();//获取文件文件名
                        filename3 = System.IO.Path.GetFileName(filename2);
                        //   string sql = "select * from " + "测试报告";
                        //   Dt2 = com.SelectToDataTable_1(sql, filename2);


                        //    if (Dt2.Rows.Count != 0)
                        //    {
                        // newRow = dt3.NewRow();


                        // newRow["IsSelected"] = false;
                        // //    newRow["WorkOrder"] = Dt.Rows[1]["字段4"].ToString();
                        // newRow["WorkOrder"] = Dt2.Rows[1]["字段4"].ToString();
                        // newRow["CustomerName"] = Dt2.Rows[1]["字段2"].ToString();

                        // newRow["Identifier"] = Dt2.Rows[2]["字段7"].ToString();
                        // newRow["Transmission_Number"] = Dt2.Rows[2]["字段4"].ToString();

                        // //          newRow["TestDate"] = Dt.Rows[3]["字段4"].ToString();


                        // newRow["Serial_Number"] = filename3.Replace(".mdb", "");
                        // if (Dt2.Rows.Count > 3)
                        // {
                        //     newRow["Status"] = Dt2.Rows[3]["字段7"].ToString();
                        //     newRow["TestDate"] = Dt2.Rows[3]["字段4"].ToString();
                        //  //   this.ComboState.Items.Add(Dt2.Rows[3]["字段7"].ToString());
                        // }
                        //// this.ComboCustomer.Items.Add(Dt2.Rows[1]["字段2"].ToString());

                        // dt3.Rows.Add(newRow);


                        System.IO.File.Copy(filename2, Path_mdb + "/" + filename3, true);
                        //  }
                    }
                    catch (Exception erro2)
                    {

                        MessageBox.Show("数据库:" + filename3 + erro2.Message);
                    }

                }
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 导入曲线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //try
            //{
                DataRowView mySelectedElement3 = (DataRowView) DG1.SelectedItem;
                //要添加这条件
                if (mySelectedElement3 != null)
                {
                    CharView.IsEnabled = true;
                    string result3 =  mySelectedElement3.Row[1].ToString();
                    WindowTitle = "数据库-" + result3;
                    // (this.DataContext as LiveDataViewModel).datatable(result3);
                    datatable(result3);
                }
                else
                {
                    ModernDialog.ShowMessage("请选中一项数据导出曲线", "提示", MessageBoxButton.OK);
                }

            //}
            //catch (Exception p)
            //{
            //    MessageBox.Show(p.Message);
            //}



        }

        /// <summary>
        /// 全选按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            //DataTable dt4;
            //dt4 = DataGridToDatable(DG1);
            //foreach (DataRow dr in dt4.Rows)
            //{
            //    if (CheckAll.IsChecked == true)
            //        dr[0] = true;
            //    if (CheckAll.IsChecked == false)
            //        dr[0] = false;

            //}
            //DG1.ItemsSource = dt4.DefaultView;
        }
        #endregion

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
        private void _LineData_14_Check_Click(object sender, RoutedEventArgs e)
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

        private void _LineData_15_Check_Click(object sender, RoutedEventArgs e)
        {
            if (_LineData_15_Check.IsChecked == true)
            {
              //  this._LineData_14.Visibility = System.Windows.Visibility.Visible;
            }
            if (_LineData_15_Check.IsChecked == false)
            {
             //   this._LineData_14.Visibility = System.Windows.Visibility.Collapsed;
            }

        }


        #endregion



        #region 详细曲线

        /// <summary>
        /// 放大曲线表，从数据库提取存放
        /// </summary>
        System.Data.DataTable CharViewDt;

        /// <summary>
        /// 标准曲线放大曲线表，从数据库提取存放
        /// </summary>
        System.Data.DataTable CharViewDt_Standard;


        /// <summary>
        /// 绑定放大曲线资源
        /// </summary>
        private System.Data.DataTable table2;


        /// <summary>
        /// 初始化导入曲线
        /// </summary>
        public void datatable(string path_14)
        {
            DataSet CharViewDt_StandardDs = new DataSet();
            CharViewDt = new System.Data.DataTable("Char");
            CharViewDt_Standard = new System.Data.DataTable("Char_Standard");
            CharViewDt_Standard = null;
            CharViewDt = MySQL_client.Getdataset(path_14, "数据流", "").Tables[0];
            CharViewDt_StandardDs = MySQL_client.Getdataset(_VariableDefinitions.Rows[0]["标准曲线数据库名称"].ToString(), "数据流", "");
            if(CharViewDt_StandardDs!=null)
            { CharViewDt_Standard = CharViewDt_StandardDs.Tables[0]; }
          //  CharViewDt_Standard = MySQL_client.Getdataset(_VariableDefinitions.Rows[0]["标准曲线数据库名称"].ToString(), "数据流","").Tables[0];
           else
            {
               // CharViewDt_Standard = new System.Data.DataTable("Char_Standard");
                CharViewDt_Standard = CharViewDt; }

                //  string sql8 = "select * from " + "数据流";
                //    CharViewDt = com.SelectToDataTable_CharView(sql8, path_14);
                // this.FillData();
                filldata();
        }
        private void Initdata()
        {


            //   this._LineData_0.SetResourceReference(LineSeries.VerticalAxisProperty, "torqueAxis");

            _LineNum= Convert.ToDouble(_VariableDefinitions.Rows[0]["显示曲线数"]);
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



        }

        /// <summary>
        /// 绑定table到CharView
        /// </summary>
        private void filldata()
        {
            _LineNum = Convert.ToDouble(_VariableDefinitions.Rows[0]["显示曲线数"]);
            table2 = new System.Data.DataTable("Char");
            table2.Columns.Add("时间", typeof(DateTime));//[0]
            table2.Columns.Add("时间戳", typeof(double));//[1]

            if (_LineNum >= 1)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[0]["变量"]), typeof(double));//[2]
            if (_LineNum >= 2)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[1]["变量"]), typeof(double));//[3]
            if (_LineNum >= 3)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[2]["变量"]), typeof(double));//[4]
            if (_LineNum >= 4)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[3]["变量"]), typeof(double));//[5]
            if (_LineNum >= 5)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[4]["变量"]), typeof(double));//[2]
            if (_LineNum >= 6)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[5]["变量"]), typeof(double));//[3]
            if (_LineNum >= 7)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[6]["变量"]), typeof(double));//[4]
            if (_LineNum >= 8)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[7]["变量"]), typeof(double));//[5]
            if (_LineNum >= 9)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[8]["变量"] + "Standard"), typeof(double));//[2]
            if (_LineNum >= 10)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[9]["变量"] + "Standard"), typeof(double));//[3]
            if (_LineNum >= 11)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[10]["变量"] + "Standard"), typeof(double));//[4]
            if (_LineNum >= 12)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[11]["变量"] + "Standard"), typeof(double));//[5]
            if (_LineNum >= 13)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[12]["变量"] + "Standard"), typeof(double));//[2]
            if (_LineNum >= 14)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[13]["变量"] + "Standard"), typeof(double));//[3]
            if (_LineNum >= 15)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[14]["变量"] + "Standard"), typeof(double));//[4]
            if (_LineNum >= 16)
                table2.Columns.Add(Convert.ToString(_VariableDefinitions.Rows[15]["变量"] + "Standard"), typeof(double));//[5]
            table2.Columns.Add("testItemNumber", typeof(string));//




            System.DateTime ti;

            ti = Convert.ToDateTime("00:00:00.000");
            double time_count = 0;

            double _LineData_0 = 0;
            double _LineData_1 = 0;

            double _LineData_2 = 0;
            double _LineData_3 = 0;
            double _LineData_4 = 0;
            double _LineData_5 = 0;
            double _LineData_6 = 0;
            double _LineData_7 = 0;
            double _LineData_8 = 0;
            double _LineData_9 = 0;
            double _LineData_10 = 0;
            double _LineData_11 = 0;
            double _LineData_12 = 0;
            double _LineData_13 = 0;
            double _LineData_14 = 0;

            double _LineData_15 = 0;
            string testItemNumber = "";

            for (int i = 0; i < CharViewDt.Rows.Count; i++)
            {

                //   double timees = Convert.ToDouble(CharViewDt.Rows[i]["时间戳"]);
                if (_LineNum >= 1)
                {
                    _LineData_0 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[0]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 2)
                {
                    _LineData_1 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[1]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 3)
                {
                    _LineData_2 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[2]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 4)
                {
                    _LineData_3 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[3]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 5)
                {
                    _LineData_4 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[4]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 6)
                {
                    _LineData_5 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[5]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 7)
                {
                    _LineData_6 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[6]["变量"])].ToString().Replace("null", "0"));
                }
                if (_LineNum >= 8)
                {
                    _LineData_7 = Convert.ToDouble(CharViewDt.Rows[i][Convert.ToString(_VariableDefinitions.Rows[7]["变量"])].ToString().Replace("null", "0"));
                }
                if (i < CharViewDt_Standard.Rows.Count)
                {
                    if (_LineNum >= 9)
                    {
                        _LineData_8 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[8]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 10)
                    {
                        _LineData_9 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[9]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 11)
                    {
                        _LineData_10 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[10]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 12)
                    {
                        _LineData_11 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[11]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 13)
                    {
                        _LineData_12 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[12]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 14)
                    {
                        _LineData_13 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[13]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 15)
                    {
                        _LineData_14 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[14]["变量"])].ToString().Replace("null", "0"));
                    }
                    if (_LineNum >= 16)
                    {
                        _LineData_15 = Convert.ToDouble(CharViewDt_Standard.Rows[i][Convert.ToString(_VariableDefinitions.Rows[15]["变量"])].ToString().Replace("null", "0"));
                    }
                }
                testItemNumber = CharViewDt.Rows[i]["testItemNumber"].ToString().Replace("null", "0");
                //double Ratio = Convert.ToDouble(CharViewDt.Rows[i]["Ratio"]);

                //double Beat = Convert.ToDouble(CharViewDt.Rows[i]["节拍"]);


                //if(_LineNum == 1)
                //{
                //    table2.Rows.Add(ti.AddSeconds(time_count), _LineData_0,_LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, _LineData_13, _LineData_14, _LineData_15);
                //}

                switch (_LineNum.ToString())
                {
                    case "1":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, testItemNumber);
                        break;
                    case "2":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, testItemNumber);
                        break;
                    case "3":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, testItemNumber);
                        break;
                    case "4":
                        if (i < 1000)
                            table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, testItemNumber);
                        break;
                    case "5":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, testItemNumber);
                        break;
                    case "6":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, testItemNumber);
                        break;
                    case "7":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, testItemNumber);
                        break;
                    case "8":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, testItemNumber);
                        break;
                    case "9":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, testItemNumber);
                        break;
                    case "10":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, testItemNumber);
                        break;
                    case "11":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, testItemNumber);
                        break;
                    case "12":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, testItemNumber);
                        break;
                    case "13":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, testItemNumber);
                        break;
                    case "14":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, _LineData_13, testItemNumber);
                        break;
                    case "15":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, _LineData_13, _LineData_14, testItemNumber);
                        break;
                    case "16":
                        table2.Rows.Add(ti.AddSeconds(time_count), time_count, _LineData_0, _LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, _LineData_13, _LineData_14, _LineData_15, testItemNumber);
                        break;

                }

                //  table2.Rows.Add(ti.AddSeconds(time_count), timees, _LineData_0,_LineData_1, _LineData_2, _LineData_3, _LineData_4, _LineData_5, _LineData_6, _LineData_7, _LineData_8, _LineData_9, _LineData_10, _LineData_11, _LineData_12, _LineData_13, _LineData_14, _LineData_15, Ratio, Beat);
                time_count = time_count + 0.01;
            }
            //绑定资源
            this.DataContext = table2.Rows;
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


        /// <summary>
        /// 实例化详细曲线窗口
        /// </summary>
  //      ReportsCharView windowChar;

        /// <summary>
        /// 详细曲线标题
        /// </summary>
        private string WindowTitle = "";

        private void CharView_Click(object sender, RoutedEventArgs e)
        {

          //  datatable((MySql_DG.SelectedItem as DataRowView).Row["数据库名称"].ToString());
            windowChar = new ReportsCharView();
            windowChar._LineNum = _LineNum;
            windowChar.Title = WindowTitle;
            windowChar.table2 = table2;
            windowChar._VariableDefinitions = _VariableDefinitions;
            windowChar.FillData_1();
            windowChar.Show();

            //windowChar = new ReportsCharView();
            //windowChar.Title = WindowTitle;
            //windowChar.table2 = table2;
            //windowChar.filldata();
            //windowChar.Show();
            //CharView.IsEnabled = false;

        }
        #endregion
        /// <summary>
        /// DataGrid转Datable
        /// </summary>
        /// <param name="dg"></param>
        /// <returns></returns>
        public System.Data.DataTable DataGridToDatable(RadGridView dg)
        {
            try
            {

                System.Data.DataTable dt = null;

                System.Data.DataTable tmpdt = ((DataView) dg.ItemsSource).Table;
                return tmpdt;
            }
            catch (System.Exception ex)
            {
                throw ex;
                //  return null;
            }
        }

        /// <summary>
        /// 判断合格与不合格，行画刷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{

        //    int index = e.Row.GetIndex();
        //    if (index >= 1)
        //    {

        //        DataRowView drv = dataGrid.Items[index] as DataRowView;


        //        //   DataRowView drv = DG1.Items[i] as DataRowView;
        //        //    DataGridRow row = (DataGridRow) this.DG1.ItemContainerGenerator.ContainerFromIndex(i);

        //        if (drv != null && Convert.ToString(drv["判断"]) == "不合格")
        //        {
        //            e.Row.Background = Brushes.Red;  // color
        //        }
        //        else
        //        {
        //            e.Row.Background = null;
        //        }

        //    }


        //    //  int k = 0;

        //    //  //        if(IsColors==true)
        //    //  //         { 
        //    //  ////获取当前加载的行标(从0开始)
        //    //  int i = e.Row.GetIndex();
        //    //  //获取DataGrid绑定的数据集合
        //    //  ObservableCollection<User> list = dataGrid.ItemsSource as ObservableCollection<User>;

        //    //  //遍历集合
        //    ////  User model in list;
        //    //  foreach (User model in list)
        //    //  {

        //    //      if ((i+1).ToString() == model.ID)
        //    //      {

        //    //          //改变颜色的行条件
        //    //          if (model.判断 == "不合格")
        //    //          { 
        //    //              if(e.Row.Background != null)
        //    //              e.Row.Background = new SolidColorBrush(Colors.Red);
        //    //          }

        //    //      }
        //    //      k++;
        //    //  }
        //}

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
        }

        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
       //     (Application.Current as App).IsCloseTest = true;

        }

        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {

        }
        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportToExcelBt_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                DataRowView mySelectedElement3 = (DataRowView) DG1.SelectedItem;
                //要添加这条件
                if (mySelectedElement3 != null)
                {
                    CharView.IsEnabled = true;
                    string result3 = mySelectedElement3.Row[1].ToString();

                    CharViewDt = new System.Data.DataTable("Char");
                    CharViewDt = MySQL_client.Getdataset(result3, "数据流", "").Tables[0];




                    ExportToExcel windowExportToExcel = new ExportToExcel();
                    windowExportToExcel.Title = result3;
                    windowExportToExcel.ExportExcelDt = CharViewDt;
                    windowExportToExcel.Show();


                }
                else
                {
                    ModernDialog.ShowMessage("请选中一项数据导出曲线", "提示", MessageBoxButton.OK);
                }

            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }







        }

    }
}






