/********************************************************/
//类名：ImportExcel
//作者：黄洪海
//创建日期：2016/08/22
//模块功能：导入Excel数据，返回table类型
/********************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SolenoidTester.DataClass
{
    public abstract class ImportExcel
    {


        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static DataTable InsernExcelFile(string Sheet, string path)
        {
            string path1;
            try
            {
                OleDbConnectionStringBuilder connectStringBuilder = new OleDbConnectionStringBuilder();
                if(path=="")
                {
                    path1 = Directory.GetCurrentDirectory() + "/Excel_File/TEHCM_TestStandard.xls";
                }
                else
                {
                    path1 = path;
                }
                connectStringBuilder.DataSource = @path1;
                connectStringBuilder.Provider = "Microsoft.Jet.OLEDB.4.0";
                connectStringBuilder.Add("Extended Properties", "Excel 8.0");
                using (OleDbConnection cn = new OleDbConnection(connectStringBuilder.ConnectionString))
                {
                    DataSet ds = new DataSet();
                    string sql = "Select * from [" + Sheet + "$]";
                    OleDbCommand cmdLiming = new OleDbCommand(sql, cn);
                    cn.Open();
                    using (OleDbDataReader drLiming = cmdLiming.ExecuteReader())
                    {
                        ds.Load(drLiming, LoadOption.OverwriteChanges, new string[] { Sheet });
                        System.Data.DataTable dt = ds.Tables[Sheet];
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
    }
}
