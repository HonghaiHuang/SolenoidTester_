
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using System.ComponentModel;
using SolenoidTester;
namespace SolenoidTester.DataClass
{
    /// <summary>
    /// abstract抽象类，不用实例化
    /// </summary>
   public class MySQL_client: Observer
    {
        //数据库连接字符串
        public static string Conn = "Database='wp';Data Source='localhost';User Id='root';Password='fuyuan';charset='utf8';pooling=true";
        public static string MySQL_name = "";
        private static DataSet myDataSet = new DataSet();
        public static MySqlConnection mysql;

        public volatile static bool IsSaveDataToDatabase = false;



        public static void SetMySQL_name(string name)
        {
            MySQL_name = name;
        }
        public static string GetMySQL_name()
        { return MySQL_name; }

        // 用于缓存参数的HASH表
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        ///  给定连接的数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 用现有的数据库连接执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="connection">一个现有的数据库连接</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {

            MySqlCommand cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary>
        ///使用现有的SQL事务执行一个sql命令（不返回数据集）
        /// </summary>
        /// <remarks>
        ///举例:
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个现有的事务</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public static int ExecuteNonQuery(MySqlTransaction trans, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 用执行的数据库连接执行一个返回数据集的sql命令
        /// </summary>
        /// <remarks>
        /// 举例:
        ///  MySqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>包含结果的读取器</returns>
        public static MySqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            //创建一个MySqlConnection对象
            MySqlConnection conn = new MySqlConnection(connectionString);

            //在这里我们用一个try/catch结构执行sql文本命令/存储过程，因为如果这个方法产生一个异常我们要关闭连接，因为没有读取器存在，
            //因此commandBehaviour.CloseConnection 就不会执行
            try
            {
                //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //调用 MySqlCommand  的 ExecuteReader 方法
                MySqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                //清除参数
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                //关闭连接，抛出异常
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns></returns>
        public static DataSet GetDataSet(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            //创建一个MySqlCommand对象
            MySqlCommand cmd = new MySqlCommand();
            //创建一个MySqlConnection对象
            MySqlConnection conn = new MySqlConnection(connectionString);

            //在这里我们用一个try/catch结构执行sql文本命令/存储过程，因为如果这个方法产生一个异常我们要关闭连接，因为没有读取器存在，

            try
            {
                //调用 PrepareCommand 方法，对 MySqlCommand 对象设置参数
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                //调用 MySqlCommand  的 ExecuteReader 方法
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                DataSet ds = new DataSet();

                adapter.Fill(ds);
                //清除参数
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        ///例如:
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        ///<param name="connectionString">一个有效的连接字符串</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 用指定的数据库连接执行一个命令并返回一个数据集的第一列
        /// </summary>
        /// <remarks>
        /// 例如:
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">一个存在的数据库连接</param>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param>
        /// <param name="cmdText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns>
        public static object ExecuteScalar(MySqlConnection connection, CommandType cmdType, string cmdText, params MySqlParameter[] commandParameters)
        {

            MySqlCommand cmd = new MySqlCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 将参数集合添加到缓存
        /// </summary>
        /// <param name="cacheKey">添加到缓存的变量</param>
        /// <param name="commandParameters">一个将要添加到缓存的sql参数集合</param>
        public static void CacheParameters(string cacheKey, params MySqlParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        /// <summary>
        /// 找回缓存参数集合
        /// </summary>
        /// <param name="cacheKey">用于找回参数的关键字</param>
        /// <returns>缓存的参数集合</returns>
        public static MySqlParameter[] GetCachedParameters(string cacheKey)
        {
            MySqlParameter[] cachedParms = (MySqlParameter[]) parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            MySqlParameter[] clonedParms = new MySqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (MySqlParameter) ((ICloneable) cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">OleDb连接</param>
        /// <param name="trans">OleDb事务</param>
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
        /// <param name="cmdText">命令文本,例如:Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }


        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="MySQL_name">需要创建数据库的名称</param>
        public static bool CreateSQL(string MySQL_name)
        {
            //      string createStatement = "CREATE TABLE Test (Field1 VarChar(50), Field2 Integer)";
            try
            {
                MySqlConnection conn1;
                conn1 = new MySqlConnection("Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan");
                MySqlCommand cmd = new MySqlCommand("CREATE DATABASE " + MySQL_name, conn1);
                SetMySQL_name(MySQL_name);
                conn1.Open();
                cmd.ExecuteNonQuery();

                conn1.Close();
                return true;
            }
            catch(Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);                    
                return false;
            }
            

        }

        /// <summary>
        /// 创建数据库表单
        /// </summary>
        /// <param name="_database">数据库名称</param>
        /// <param name="_tableName">数据库表名称</param>
        /// <param name="_columnsName">字段名，字符串数组</param>
        public static bool AlterTableExample(string _database,string _tableName,string[] _columnsName)
        {
            try
            {
                string _columename = " ("+ _columnsName[0] + " int PRIMARY KEY not null auto_increment,";
                for (int i = 1; i < _columnsName.Length; i++)
                {
                    if (i != _columnsName.Length - 1)
                    {

                        _columename += _columnsName[i] + " VarChar(30),";
                    }
                    else
                    {
                       
                            _columename += _columnsName[i] + " VarChar(30))";
                           
                        
                    }
                }
                string connStr = "Database = " + _database + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
                //     string createStatement = "CREATE TABLE "+ _tableName + " (updatetime VarChar(50),sdfasdf VarChar(50))";
                string createStatement = "CREATE TABLE " + _tableName + _columename;
                //   string alterStatement = "ALTER TABLE Test ADD Field3 Boolean";

                using (MySqlConnection AlterTable_conn = new MySqlConnection(connStr))
                {
                    AlterTable_conn.Open();
                    MySqlCommand cmd;
                    cmd = new MySqlCommand(createStatement, AlterTable_conn);
                    cmd.ExecuteNonQuery();
                    AlterTable_conn.Close();
                }
                return true;
            }
            catch(Exception AlterTableExampleErrmsg)
            {
                Debug.WriteLine(AlterTableExampleErrmsg.Message);
                return false;
            }
        }

        /// <summary>
        /// 查找所有的数据库文件
        /// </summary>
        /// <returns>返回所有数据库名称</returns>
        public static string[] Showdatabase()
        {
            ArrayList list = new ArrayList();
          //  string[] SQL_filename=new string[600];
            int SQL_filenameCount = 0;
            string connStr = "Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
            using (MySqlConnection conn1 = new MySqlConnection(connStr))
            {
                conn1.Open();
                string sql = "show databases";
                MySqlCommand command = new MySqlCommand(sql,conn1);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SQL_filenameCount++;
                        list.Add(reader[0].ToString());
                    //    list[SQL_filenameCount] = reader[0].ToString();
                     //   SQL_filename[SQL_filenameCount] = reader[0].ToString();
                        Console.WriteLine(reader[0].ToString());
                    }
                    string[] SQL_filename =(string[]) list.ToArray(typeof(string));
                    return SQL_filename;
                }
            }
        }


        /// <summary>
        /// 删除指定数据库
        /// </summary>
        /// <param name="SQL_name">数据库名称</param>
        public static void DeleteMySQL(string SQL_name)
        {
            try
            { 
                string connStr = "Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
                using (MySqlConnection conn1 = new MySqlConnection(connStr))
                {
                    conn1.Open();
                    string sql = "drop database "+ SQL_name;
                    MySqlCommand command = new MySqlCommand(sql, conn1);
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception DeleteMySQL_erro)
            {
                MessageBox.Show(DeleteMySQL_erro.Message);
            }
        }

        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <returns></returns>
        public static DataSet Getdataset(string _database,string _tableName, string _where)
        {
            try
            { 
            DataSet Ds=new DataSet();
            string sql = "select * from "+ _tableName+ _where;
            MySqlConnection MysqlConn = new MySqlConnection("Database = " + _database + ";"+"Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan");

            string k = MysqlConn.DataSource;
            if (MysqlConn.State == ConnectionState.Closed) 
            {
                MysqlConn.Open();
            }
            //     MySqlCommand com = new MySqlCommand(sql, MysqlConn);
            MySqlDataAdapter Data =new MySqlDataAdapter(sql, MysqlConn);
            Data.Fill(Ds, _tableName);
            return Ds;
            }
            catch(Exception er)
            {
                Debug.WriteLine(er.Message);
                return null;
            }
        }




        /// <summary>
        /// 入口
        /// </summary>
        public void InitMain()
        {

            MySqlConnection mysql = getconn("");

            string sqlSearch = "select * from company";
            string sqlInsert = "insert into company(name,id,address) values('测试',122,'北京')";
            string sqlUpdate = "update company set name = '成功' where id = 122";
            string sqlDelete = "delete from company where id = 122";
            string sqlTruncate = "truncate company";

            try
            {

                MySqlCommand mysqlselect = getsqlCommand(sqlSearch, mysql);
                MySqlCommand mysqlinsert = getsqlCommand(sqlInsert, mysql);
                MySqlCommand mysqlupdate = getsqlCommand(sqlUpdate, mysql);
                MySqlCommand mysqldelete = getsqlCommand(sqlDelete, mysql);
                MySqlCommand mysqlTruncate = getsqlCommand(sqlTruncate, mysql);

                mysql.Open();
                Console.WriteLine(mysql.ServerVersion + "n" + mysql.ConnectionString + "n" + mysql.Database + "n" + mysql.DataSource + "n");


                getTruncate(mysqlTruncate); //清空表
             //   InsertTestData(mysql);   //插入测试数据

                getResult(mysqlselect);
                Console.WriteLine();

                getDelete(mysqldelete);
                getInsert(mysqlinsert);
                getUpdate(mysqlupdate);
                //     getDelete(mysqldelete);
                getResult(mysqlselect);

                mysql.Close();
            }
            catch (MySqlException ex)
            {
                Console.Write(ex.Message);
            }

            Console.ReadLine();
        }
        public static void getResult(MySqlCommand mysqlcommand)
        {
            MySqlDataReader reader = mysqlcommand.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine(" 姓名:  " + reader.GetString(1) + "  编号  " + reader.GetString(0) + "  地址  " + reader.GetString(2));
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("查询失败!" + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }


        public static MySqlCommand getsqlCommand(string sql, MySqlConnection mysql)
        {
            MySqlCommand mysqlcommand = new MySqlCommand(sql, mysql);
            return mysqlcommand;
        }


        public static MySqlConnection getconn(string _database)
        {
            string mysqlStr = "Database = "+ _database + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
            MySqlConnection mysql = new MySqlConnection(mysqlStr);            
            return mysql;
        }
        public static void getTruncate(MySqlCommand mysqlcommand)
        {
            try
            {
                mysqlcommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                string message = ex.Message;
                Console.WriteLine("清空表失败! " + message);
            }
        }


        /// <summary>
        /// 更新数据库数据
        /// </summary>
        /// <param name="_database">数据库名</param>
        /// <param name="_columeName">字段名</param>
        /// <param name="_columeValue">更新的数值</param>
        /// <returns>是否更新成功</returns>
        public static bool _updata(string _database,string _columeName, string _columeValue)
        {
            try
            {
                MySqlConnection mysql = getconn(_database);
                string sqlUpdate = "update applydevicedemarcate set DemarcateValue = '" + _columeValue + "' where DemarcateName = '"+ _columeName+"'";
                MySqlCommand mysqlupdate = getsqlCommand(sqlUpdate, mysql);
                mysql.Open();
                getUpdate(mysqlupdate);
                mysql.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static void getUpdate(MySqlCommand mysqlcommand)
        {
            try
            {
                mysqlcommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                string message = ex.Message;
                Console.WriteLine("修改数据失败! " + message);
            }
        }
        public static void getDelete(MySqlCommand mysqlcommand)
        {
            try
            {
                mysqlcommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                string message = ex.Message;
                Console.WriteLine("删除数据失败! " + message);
            }
        }

        public static void getInsert(MySqlCommand mysqlcommand)
        {
            try
            {
                mysqlcommand.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                string message = ex.Message;
                Console.WriteLine("插入数据失败! " + message);
            }
        }
        public async static Task<bool> Inser_data(string[,] data)
        {
            //   MySqlConnection mysql = getconn();

            try
            {
                string mysqlStr = "Database =" + GetMySQL_name() + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan;";
                //if (mysql == null)
                //{
                mysql = new MySqlConnection(mysqlStr);

                //  }

                if (mysql.State != ConnectionState.Open)
                    mysql.Open();
                string sqlInsert = "insert into 数据流(时间戳,P_Source,P_TCC,P_Line,P_Shift,P_C1234,P_CB26,P_C35R,P_C456,temperature,inputSpeed,outputSpeed,scramStatus,changeValve1Status,changeValve2Status,changeValve3Status,changeValve4Status,oilPumpStatus,heatingWireStatus,TCUPowerStatus,TCUInputSpeed,TCUOnputSpeed,gear,TCUtemperature,switchSolenoidStatus,CANConnectStatus,testItemNumber,solenoid1,solenoid1Pressure,solenoid2,solenoid2Pressure,adjustInpoutSpeed,adjustOutputSpeed) values";
                sqlInsert += "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[0, 0] + "','" + data[0, 1] + "','" + data[0, 2] + "','" + data[0, 3] + "','" + data[0, 4] + "','" + data[0, 5] + "','" + data[0, 6] + "','" + data[0, 7] + "','" + data[0, 8] + "','" + data[0, 9] + "','" + data[0, 10] + "','" + data[0, 11] + "','" + data[0, 12] + "','" + data[0, 13] + "','" + data[0, 14] + "','" + data[0, 15] + "','" + data[0, 16] + "','" + data[0, 17] + "','" + data[0, 18] + "','" + data[0, 19] + "','" + data[0, 20] + "','" + data[0, 21] + "','" + data[0, 22] + "','" + data[0, 23] + "','" + data[0, 24] + "','" + data[0, 25] + "','" + data[0, 26] + "','" + data[0, 27] + "','" + data[0, 28] + "','" + data[0, 29] + "','" + data[0, 30] + "','" + data[0, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[1, 0] + "','" + data[1, 1] + "','" + data[1, 2] + "','" + data[1, 3] + "','" + data[1, 4] + "','" + data[1, 5] + "','" + data[1, 6] + "','" + data[1, 7] + "','" + data[1, 8] + "','" + data[1, 9] + "','" + data[1, 10] + "','" + data[1, 11] + "','" + data[1, 12] + "','" + data[1, 13] + "','" + data[1, 14] + "','" + data[1, 15] + "','" + data[1, 16] + "','" + data[1, 17] + "','" + data[1, 18] + "','" + data[1, 19] + "','" + data[1, 20] + "','" + data[1, 21] + "','" + data[1, 22] + "','" + data[1, 23] + "','" + data[1, 24] + "','" + data[1, 25] + "','" + data[1, 26] + "','" + data[1, 27] + "','" + data[1, 28] + "','" + data[1, 29] + "','" + data[1, 30] + "','" + data[1, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[2, 0] + "','" + data[2, 1] + "','" + data[2, 2] + "','" + data[2, 3] + "','" + data[2, 4] + "','" + data[2, 5] + "','" + data[2, 6] + "','" + data[2, 7] + "','" + data[2, 8] + "','" + data[2, 9] + "','" + data[2, 10] + "','" + data[2, 11] + "','" + data[2, 12] + "','" + data[2, 13] + "','" + data[2, 14] + "','" + data[2, 15] + "','" + data[2, 16] + "','" + data[2, 17] + "','" + data[2, 18] + "','" + data[2, 19] + "','" + data[2, 20] + "','" + data[2, 21] + "','" + data[2, 22] + "','" + data[2, 23] + "','" + data[2, 24] + "','" + data[2, 25] + "','" + data[2, 26] + "','" + data[2, 27] + "','" + data[2, 28] + "','" + data[2, 29] + "','" + data[2, 30] + "','" + data[2, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[3, 0] + "','" + data[3, 1] + "','" + data[3, 2] + "','" + data[3, 3] + "','" + data[3, 4] + "','" + data[3, 5] + "','" + data[3, 6] + "','" + data[3, 7] + "','" + data[3, 8] + "','" + data[3, 9] + "','" + data[3, 10] + "','" + data[3, 11] + "','" + data[3, 12] + "','" + data[3, 13] + "','" + data[3, 14] + "','" + data[3, 15] + "','" + data[3, 16] + "','" + data[3, 17] + "','" + data[3, 18] + "','" + data[3, 19] + "','" + data[3, 20] + "','" + data[3, 21] + "','" + data[3, 22] + "','" + data[3, 23] + "','" + data[3, 24] + "','" + data[3, 25] + "','" + data[3, 26] + "','" + data[3, 27] + "','" + data[3, 28] + "','" + data[3, 29] + "','" + data[3, 30] + "','" + data[3, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[4, 0] + "','" + data[4, 1] + "','" + data[4, 2] + "','" + data[4, 3] + "','" + data[4, 4] + "','" + data[4, 5] + "','" + data[4, 6] + "','" + data[4, 7] + "','" + data[4, 8] + "','" + data[4, 9] + "','" + data[4, 10] + "','" + data[4, 11] + "','" + data[4, 12] + "','" + data[4, 13] + "','" + data[4, 14] + "','" + data[4, 15] + "','" + data[4, 16] + "','" + data[4, 17] + "','" + data[4, 18] + "','" + data[4, 19] + "','" + data[4, 20] + "','" + data[4, 21] + "','" + data[4, 22] + "','" + data[4, 23] + "','" + data[4, 24] + "','" + data[4, 25] + "','" + data[4, 26] + "','" + data[4, 27] + "','" + data[4, 28] + "','" + data[4, 29] + "','" + data[4, 30] + "','" + data[4, 31] + "')";

                MySqlCommand mysqlinsert = getsqlCommand(sqlInsert, mysql);
                if (await mysqlinsert.ExecuteNonQueryAsync() > 0)
                {
                    
                    //关闭连接、释放资源   
                    mysqlinsert.Dispose();
                    if (mysql.State != ConnectionState.Closed)
                    {
                        mysql.Close();
                        mysql.Dispose();
                    }
                    return true;
                    //Debug.WriteLine("数据插入成功！{0}");
                    //  Debug.WriteLine("数据插入成功！"+System.DateTime.Now.ToString("HH: mm:ss.fff"));
                }
                else
                {
                    
                    //关闭连接、释放资源   
                    mysqlinsert.Dispose();
                    if (mysql.State != ConnectionState.Closed)
                    {
                        mysql.Close();
                        mysql.Dispose();
                    }
                    return false;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show("操作超时！"+er.Message);
                return false;
            }


            //  getInsert(mysqlinsert);
            //  mysql.Close();
        }

        public async static Task<bool> Inser_dat_2(string[,] data)
        {
            //   MySqlConnection mysql = getconn();

            try
            {
                string mysqlStr = "Database =" + GetMySQL_name() + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
                //if (mysql == null)
                //{
                mysql = new MySqlConnection(mysqlStr);

                //  }

                if (mysql.State != ConnectionState.Open)
                    mysql.Open();
                string sqlInsert = "insert into 数据流(时间戳,P_Source,P_TCC,P_Line,P_Shift,P_C1234,P_CB26,P_C35R,P_C456,temperature,inputSpeed,outputSpeed,scramStatus,changeValve1Status,changeValve2Status,changeValve3Status,changeValve4Status,oilPumpStatus,heatingWireStatus,TCUPowerStatus,TCUInputSpeed,TCUOnputSpeed,gear,TCUtemperature,switchSolenoidStatus,CANConnectStatus,testItemNumber,solenoid1,solenoid1Pressure,solenoid2,solenoid2Pressure,adjustInpoutSpeed,adjustOutputSpeed) values";
                for(int j=0;j<5;j++)
                {
         //       string sqlInsertdata
                  string sqlInsertdata=  sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[j, 0] + "','" + data[j, 1] + "','" + data[j, 2] + "','" + data[j, 3] + "','" + data[j, 4] + "','" + data[j, 5] + "','" + data[j, 6] + "','" + data[j, 7] + "','" + data[j, 8] + "','" + data[j, 9] + "','" + data[j, 10] + "','" + data[j, 11] + "','" + data[j, 12] + "','" + data[j, 13] + "','" + data[j, 14] + "','" + data[j, 15] + "','" + data[j, 16] + "','" + data[j, 17] + "','" + data[j, 18] + "','" + data[j, 19] + "','" + data[j, 20] + "','" + data[j, 21] + "','" + data[j, 22] + "','" + data[j, 23] + "','" + data[j, 24] + "','" + data[j, 25] + "','" + data[j, 26] + "','" + data[j, 27] + "','" + data[j, 28] + "','" + data[j, 29] + "','" + data[j, 30] + "','" + data[j, 31] + "')";
                    MySqlCommand mysqlinsert = getsqlCommand(sqlInsert, mysql);
                    await mysqlinsert.ExecuteNonQueryAsync();
                }
                //sqlInsert += "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[0, 0] + "','" + data[0, 1] + "','" + data[0, 2] + "','" + data[0, 3] + "','" + data[0, 4] + "','" + data[0, 5] + "','" + data[0, 6] + "','" + data[0, 7] + "','" + data[0, 8] + "','" + data[0, 9] + "','" + data[0, 10] + "','" + data[0, 11] + "','" + data[0, 12] + "','" + data[0, 13] + "','" + data[0, 14] + "','" + data[0, 15] + "','" + data[0, 16] + "','" + data[0, 17] + "','" + data[0, 18] + "','" + data[0, 19] + "','" + data[0, 20] + "','" + data[0, 21] + "','" + data[0, 22] + "','" + data[0, 23] + "','" + data[0, 24] + "','" + data[0, 25] + "','" + data[0, 26] + "','" + data[0, 27] + "','" + data[0, 28] + "','" + data[0, 29] + "','" + data[0, 30] + "','" + data[0, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[1, 0] + "','" + data[1, 1] + "','" + data[1, 2] + "','" + data[1, 3] + "','" + data[1, 4] + "','" + data[1, 5] + "','" + data[1, 6] + "','" + data[1, 7] + "','" + data[1, 8] + "','" + data[1, 9] + "','" + data[1, 10] + "','" + data[1, 11] + "','" + data[1, 12] + "','" + data[1, 13] + "','" + data[1, 14] + "','" + data[1, 15] + "','" + data[1, 16] + "','" + data[1, 17] + "','" + data[1, 18] + "','" + data[1, 19] + "','" + data[1, 20] + "','" + data[1, 21] + "','" + data[1, 22] + "','" + data[1, 23] + "','" + data[1, 24] + "','" + data[1, 25] + "','" + data[1, 26] + "','" + data[1, 27] + "','" + data[1, 28] + "','" + data[1, 29] + "','" + data[1, 30] + "','" + data[1, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[2, 0] + "','" + data[2, 1] + "','" + data[2, 2] + "','" + data[2, 3] + "','" + data[2, 4] + "','" + data[2, 5] + "','" + data[2, 6] + "','" + data[2, 7] + "','" + data[2, 8] + "','" + data[2, 9] + "','" + data[2, 10] + "','" + data[2, 11] + "','" + data[2, 12] + "','" + data[2, 13] + "','" + data[2, 14] + "','" + data[2, 15] + "','" + data[2, 16] + "','" + data[2, 17] + "','" + data[2, 18] + "','" + data[2, 19] + "','" + data[2, 20] + "','" + data[2, 21] + "','" + data[2, 22] + "','" + data[2, 23] + "','" + data[2, 24] + "','" + data[2, 25] + "','" + data[2, 26] + "','" + data[2, 27] + "','" + data[2, 28] + "','" + data[2, 29] + "','" + data[2, 30] + "','" + data[2, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[3, 0] + "','" + data[3, 1] + "','" + data[3, 2] + "','" + data[3, 3] + "','" + data[3, 4] + "','" + data[3, 5] + "','" + data[3, 6] + "','" + data[3, 7] + "','" + data[3, 8] + "','" + data[3, 9] + "','" + data[3, 10] + "','" + data[3, 11] + "','" + data[3, 12] + "','" + data[3, 13] + "','" + data[3, 14] + "','" + data[3, 15] + "','" + data[3, 16] + "','" + data[3, 17] + "','" + data[3, 18] + "','" + data[3, 19] + "','" + data[3, 20] + "','" + data[3, 21] + "','" + data[3, 22] + "','" + data[3, 23] + "','" + data[3, 24] + "','" + data[3, 25] + "','" + data[3, 26] + "','" + data[3, 27] + "','" + data[3, 28] + "','" + data[3, 29] + "','" + data[3, 30] + "','" + data[3, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[4, 0] + "','" + data[4, 1] + "','" + data[4, 2] + "','" + data[4, 3] + "','" + data[4, 4] + "','" + data[4, 5] + "','" + data[4, 6] + "','" + data[4, 7] + "','" + data[4, 8] + "','" + data[4, 9] + "','" + data[4, 10] + "','" + data[4, 11] + "','" + data[4, 12] + "','" + data[4, 13] + "','" + data[4, 14] + "','" + data[4, 15] + "','" + data[4, 16] + "','" + data[4, 17] + "','" + data[4, 18] + "','" + data[4, 19] + "','" + data[4, 20] + "','" + data[4, 21] + "','" + data[4, 22] + "','" + data[4, 23] + "','" + data[4, 24] + "','" + data[4, 25] + "','" + data[4, 26] + "','" + data[4, 27] + "','" + data[4, 28] + "','" + data[4, 29] + "','" + data[4, 30] + "','" + data[4, 31] + "')";

                //if (await mysqlinsert.ExecuteNonQueryAsync() > 0)
                //{
                //    return true;
                //    //关闭连接、释放资源   
                //    mysqlinsert.Dispose();
                //    if (mysql.State != ConnectionState.Closed)
                //    {
                //        mysql.Close();
                //        mysql.Dispose();
                //    }

                //    //Debug.WriteLine("数据插入成功！{0}");
                //    //  Debug.WriteLine("数据插入成功！"+System.DateTime.Now.ToString("HH: mm:ss.fff"));
                //}
                //else
                //{
                    //关闭连接、释放资源   
                 //   mysqlinsert.Dispose();
                    if (mysql.State != ConnectionState.Closed)
                    {
                        mysql.Close();
                        mysql.Dispose();
                    

                }
                return true;
                //}
            }
            catch (Exception er)
            {
                MessageBox.Show("操作超时！");
                return false;
            }


            //  getInsert(mysqlinsert);
            //  mysql.Close();
        }


        public static MySqlDataAdapter myDa;

        /// <summary>
        /// 插入数据流数据
        /// </summary>
        /// <param name="data"></param>
        public static void InsertAlarmsToTable(string[,] data)
        {
            try
            {
                string mysqlStr = "Database =" + GetMySQL_name() + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
                if (mysql == null)
                {
                    mysql = new MySqlConnection(mysqlStr);
                }

                //    con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath);

                if (mysql.State != ConnectionState.Open)
                    mysql.Open();
                string sqlInsert = "insert into 数据流(时间戳,P_Source,P_TCC,P_Line,P_Shift,P_C1234,P_CB26,P_C35R,P_C456,temperature,inputSpeed,outputSpeed,scramStatus,changeValve1Status,changeValve2Status,changeValve3Status,changeValve4Status,oilPumpStatus,heatingWireStatus,TCUPowerStatus,TCUInputSpeed,TCUOnputSpeed,gear,TCUtemperature,switchSolenoidStatus,CANConnectStatus,testItemNumber,solenoid1,solenoid1Pressure,solenoid2,solenoid2Pressure,adjustInpoutSpeed,adjustOutputSpeed) values";
                sqlInsert += "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[0, 0] + "','" + data[0, 1] + "','" + data[0, 2] + "','" + data[0, 3] + "','" + data[0, 4] + "','" + data[0, 5] + "','" + data[0, 6] + "','" + data[0, 7] + "','" + data[0, 8] + "','" + data[0, 9] + "','" + data[0, 10] + "','" + data[0, 11] + "','" + data[0, 12] + "','" + data[0, 13] + "','" + data[0, 14] + "','" + data[0, 15] + "','" + data[0, 16] + "','" + data[0, 17] + "','" + data[0, 18] + "','" + data[0, 19] + "','" + data[0, 20] + "','" + data[0, 21] + "','" + data[0, 22] + "','" + data[0, 23] + "','" + data[0, 24] + "','" + data[0, 25] + "','" + data[0, 26] + "','" + data[0, 27] + "','" + data[0, 28] + "','" + data[0, 29] + "','" + data[0, 30] + "','" + data[0, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[1, 0] + "','" + data[1, 1] + "','" + data[1, 2] + "','" + data[1, 3] + "','" + data[1, 4] + "','" + data[1, 5] + "','" + data[1, 6] + "','" + data[1, 7] + "','" + data[1, 8] + "','" + data[1, 9] + "','" + data[1, 10] + "','" + data[1, 11] + "','" + data[1, 12] + "','" + data[1, 13] + "','" + data[1, 14] + "','" + data[1, 15] + "','" + data[1, 16] + "','" + data[1, 17] + "','" + data[1, 18] + "','" + data[1, 19] + "','" + data[1, 20] + "','" + data[1, 21] + "','" + data[1, 22] + "','" + data[1, 23] + "','" + data[1, 24] + "','" + data[1, 25] + "','" + data[1, 26] + "','" + data[1, 27] + "','" + data[1, 28] + "','" + data[1, 29] + "','" + data[1, 30] + "','" + data[1, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[2, 0] + "','" + data[2, 1] + "','" + data[2, 2] + "','" + data[2, 3] + "','" + data[2, 4] + "','" + data[2, 5] + "','" + data[2, 6] + "','" + data[2, 7] + "','" + data[2, 8] + "','" + data[2, 9] + "','" + data[2, 10] + "','" + data[2, 11] + "','" + data[2, 12] + "','" + data[2, 13] + "','" + data[2, 14] + "','" + data[2, 15] + "','" + data[2, 16] + "','" + data[2, 17] + "','" + data[2, 18] + "','" + data[2, 19] + "','" + data[2, 20] + "','" + data[2, 21] + "','" + data[2, 22] + "','" + data[2, 23] + "','" + data[2, 24] + "','" + data[2, 25] + "','" + data[2, 26] + "','" + data[2, 27] + "','" + data[2, 28] + "','" + data[2, 29] + "','" + data[2, 30] + "','" + data[2, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[3, 0] + "','" + data[3, 1] + "','" + data[3, 2] + "','" + data[3, 3] + "','" + data[3, 4] + "','" + data[3, 5] + "','" + data[3, 6] + "','" + data[3, 7] + "','" + data[3, 8] + "','" + data[3, 9] + "','" + data[3, 10] + "','" + data[3, 11] + "','" + data[3, 12] + "','" + data[3, 13] + "','" + data[3, 14] + "','" + data[3, 15] + "','" + data[3, 16] + "','" + data[3, 17] + "','" + data[3, 18] + "','" + data[3, 19] + "','" + data[3, 20] + "','" + data[3, 21] + "','" + data[3, 22] + "','" + data[3, 23] + "','" + data[3, 24] + "','" + data[3, 25] + "','" + data[3, 26] + "','" + data[3, 27] + "','" + data[3, 28] + "','" + data[3, 29] + "','" + data[3, 30] + "','" + data[3, 31] + "')";
                sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[4, 0] + "','" + data[4, 1] + "','" + data[4, 2] + "','" + data[4, 3] + "','" + data[4, 4] + "','" + data[4, 5] + "','" + data[4, 6] + "','" + data[4, 7] + "','" + data[4, 8] + "','" + data[4, 9] + "','" + data[4, 10] + "','" + data[4, 11] + "','" + data[4, 12] + "','" + data[4, 13] + "','" + data[4, 14] + "','" + data[4, 15] + "','" + data[4, 16] + "','" + data[4, 17] + "','" + data[4, 18] + "','" + data[4, 19] + "','" + data[4, 20] + "','" + data[4, 21] + "','" + data[4, 22] + "','" + data[4, 23] + "','" + data[4, 24] + "','" + data[4, 25] + "','" + data[4, 26] + "','" + data[4, 27] + "','" + data[4, 28] + "','" + data[4, 29] + "','" + data[4, 30] + "','" + data[4, 31] + "')";

                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[0, 0] + "','" + data[0, 1] + "','" + data[0, 2] + "','" + data[0, 3] + "','" + data[0, 4] + "','" + data[0, 5] + "','" + data[0, 6] + "','" + data[0, 7] + "','" + data[0, 8] + "','" + data[0, 9] + "','" + data[0, 10] + "','" + data[0, 11] + "','" + data[0, 12] + "','" + data[0, 13] + "','" + data[0, 14] + "','" + data[0, 15] + "','" + data[0, 16] + "','" + data[0, 17] + "','" + data[0, 18] + "','" + data[0, 19] + "','" + data[0, 20] + "','" + data[0, 21] + "','" + data[0, 22] + "','" + data[0, 23] + "','" + data[0, 24] + "','" + data[0, 25] + "','" + data[0, 26] + "','" + data[0, 27] + "','" + data[0, 28] + "','" + data[0, 29] + "','" + data[0, 30] + "','" + data[0, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[1, 0] + "','" + data[1, 1] + "','" + data[1, 2] + "','" + data[1, 3] + "','" + data[1, 4] + "','" + data[1, 5] + "','" + data[1, 6] + "','" + data[1, 7] + "','" + data[1, 8] + "','" + data[1, 9] + "','" + data[1, 10] + "','" + data[1, 11] + "','" + data[1, 12] + "','" + data[1, 13] + "','" + data[1, 14] + "','" + data[1, 15] + "','" + data[1, 16] + "','" + data[1, 17] + "','" + data[1, 18] + "','" + data[1, 19] + "','" + data[1, 20] + "','" + data[1, 21] + "','" + data[1, 22] + "','" + data[1, 23] + "','" + data[1, 24] + "','" + data[1, 25] + "','" + data[1, 26] + "','" + data[1, 27] + "','" + data[1, 28] + "','" + data[1, 29] + "','" + data[1, 30] + "','" + data[1, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[2, 0] + "','" + data[2, 1] + "','" + data[2, 2] + "','" + data[2, 3] + "','" + data[2, 4] + "','" + data[2, 5] + "','" + data[2, 6] + "','" + data[2, 7] + "','" + data[2, 8] + "','" + data[2, 9] + "','" + data[2, 10] + "','" + data[2, 11] + "','" + data[2, 12] + "','" + data[2, 13] + "','" + data[2, 14] + "','" + data[2, 15] + "','" + data[2, 16] + "','" + data[2, 17] + "','" + data[2, 18] + "','" + data[2, 19] + "','" + data[2, 20] + "','" + data[2, 21] + "','" + data[2, 22] + "','" + data[2, 23] + "','" + data[2, 24] + "','" + data[2, 25] + "','" + data[2, 26] + "','" + data[2, 27] + "','" + data[2, 28] + "','" + data[2, 29] + "','" + data[2, 30] + "','" + data[2, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[3, 0] + "','" + data[3, 1] + "','" + data[3, 2] + "','" + data[3, 3] + "','" + data[3, 4] + "','" + data[3, 5] + "','" + data[3, 6] + "','" + data[3, 7] + "','" + data[3, 8] + "','" + data[3, 9] + "','" + data[3, 10] + "','" + data[3, 11] + "','" + data[3, 12] + "','" + data[3, 13] + "','" + data[3, 14] + "','" + data[3, 15] + "','" + data[3, 16] + "','" + data[3, 17] + "','" + data[3, 18] + "','" + data[3, 19] + "','" + data[3, 20] + "','" + data[3, 21] + "','" + data[3, 22] + "','" + data[3, 23] + "','" + data[3, 24] + "','" + data[3, 25] + "','" + data[3, 26] + "','" + data[3, 27] + "','" + data[3, 28] + "','" + data[3, 29] + "','" + data[3, 30] + "','" + data[3, 31] + "')";
                //sqlInsert += ",('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[4, 0] + "','" + data[4, 1] + "','" + data[4, 2] + "','" + data[4, 3] + "','" + data[4, 4] + "','" + data[4, 5] + "','" + data[4, 6] + "','" + data[4, 7] + "','" + data[4, 8] + "','" + data[4, 9] + "','" + data[4, 10] + "','" + data[4, 11] + "','" + data[4, 12] + "','" + data[4, 13] + "','" + data[4, 14] + "','" + data[4, 15] + "','" + data[4, 16] + "','" + data[4, 17] + "','" + data[4, 18] + "','" + data[4, 19] + "','" + data[4, 20] + "','" + data[4, 21] + "','" + data[4, 22] + "','" + data[4, 23] + "','" + data[4, 24] + "','" + data[4, 25] + "','" + data[4, 26] + "','" + data[4, 27] + "','" + data[4, 28] + "','" + data[4, 29] + "','" + data[4, 30] + "','" + data[4, 31] + "')";

                myDa = new MySqlDataAdapter(sqlInsert, mysql);
                myDa.Fill(myDataSet, "数据流");
                myDataSet.Clear();
                //关闭连接、释放资源   
                myDa.Dispose();
                if (mysql.State != ConnectionState.Closed)
                {
                    mysql.Close();
                    mysql.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        /// <summary>
        /// 插入测试信息数据
        /// </summary>
        /// <param name="data"></param>
        public void InsertTestInformationToTable(string[] TestInformationData)
        {
            try
            {
                string mysqlStr = "Database =" + GetMySQL_name() + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan";
                if (mysql == null)
                {
                    mysql = new MySqlConnection(mysqlStr);
                    mysql.Open();
                }
                //    con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbpath);
                string sqlInsert = "insert into TestInformation(操作者,报告编号,零件号,刷写软件号,商用状态,制造追踪码,VIN,故障码) values('";
                sqlInsert += TestInformationData[0] + "','" + TestInformationData[1] + "','" + TestInformationData[2] + "','" + TestInformationData[3] + "','" + TestInformationData[4] + "','" + TestInformationData[5] + "','" + TestInformationData[6] + "','" + TestInformationData[7] + "')";
                MySqlDataAdapter myDa = new MySqlDataAdapter();
                myDa = new MySqlDataAdapter(sqlInsert, mysql);
                myDa.Fill(myDataSet, "TestInformation");
                myDataSet.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        //public static void InsertTestData(MySqlConnection mysql)
        //{
        //    MySqlCommand mycmd;
        //    int i = 0;
        //    while (i++ != 10000)
        //    {
        //        string cmd12 = "insert into company(P_Source,P_TCC,P_Line,P_Shift,P_C1234,P_CB26,P_C35R,P_C456,Temperature,InputSpeed,OutputSpeed,Gear,InputSpeed_TCU,OutputSpeed_TCU,Temperature_TCU,PressureSwitch) values('小王'," + i + ",'西安')";
        //        mycmd = new MySqlCommand(cmd12, mysql);
        //        if (mycmd.ExecuteNonQuery() > 0)
        //        {
        //            Debug.WriteLine("数据插入成功！{0}", i);
        //            Debug.WriteLine(System.DateTime.Now.ToString("HH: mm:ss.fff"));
        //        }
        //        Debug.WriteLine(System.DateTime.Now.ToString("HH: mm:ss.fff"));
        //    }

        //}



        #region 初始化数据库
        #endregion


        #region 初始化数据处理模块




        // 创建一个Hashtable实例
       private static Hashtable ht = new Hashtable();
        private static List<String> SQLStringList = new List<string>();
        private static int con = 0;
        public static void connetstring()
        { DbHelperMySQL.connectionString ="Database =" + GetMySQL_name() + "; Data Source=localhost;Persist Security Info=yes;UserId=root; PWD=fuyuan"; }
        public static void InitHashtabletomysql(string[,] data)
        {
            con++;
            string sqlInsert = "insert into 数据流(时间戳,P_Source,P_TCC,P_Line,P_Shift,P_C1234,P_CB26,P_C35R,P_C456,temperature,inputSpeed,outputSpeed,scramStatus,changeValve1Status,changeValve2Status,changeValve3Status,changeValve4Status,oilPumpStatus,heatingWireStatus,TCUPowerStatus,TCUInputSpeed,TCUOnputSpeed,gear,TCUtemperature,switchSolenoidStatus,CANConnectStatus,testItemNumber,solenoid1,solenoid1Pressure,solenoid2,solenoid2Pressure,adjustInpoutSpeed,adjustOutputSpeed) values";
            string sqlInsert0 = sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + con.ToString() + "','" + data[0, 1] + "','" + data[0, 2] + "','" + data[0, 3] + "','" + data[0, 4] + "','" + data[0, 5] + "','" + data[0, 6] + "','" + data[0, 7] + "','" + data[0, 8] + "','" + data[0, 9] + "','" + data[0, 10] + "','" + data[0, 11] + "','" + data[0, 12] + "','" + data[0, 13] + "','" + data[0, 14] + "','" + data[0, 15] + "','" + data[0, 16] + "','" + data[0, 17] + "','" + data[0, 18] + "','" + data[0, 19] + "','" + data[0, 20] + "','" + data[0, 21] + "','" + data[0, 22] + "','" + data[0, 23] + "','" + data[0, 24] + "','" + data[0, 25] + "','" + data[0, 26] + "','" + data[0, 27] + "','" + data[0, 28] + "','" + data[0, 29] + "','" + data[0, 30] + "','" + data[0, 31] + "')";
            string sqlInsert1 = sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[1, 0] + "','" + data[1, 1] + "','" + data[1, 2] + "','" + data[1, 3] + "','" + data[1, 4] + "','" + data[1, 5] + "','" + data[1, 6] + "','" + data[1, 7] + "','" + data[1, 8] + "','" + data[1, 9] + "','" + data[1, 10] + "','" + data[1, 11] + "','" + data[1, 12] + "','" + data[1, 13] + "','" + data[1, 14] + "','" + data[1, 15] + "','" + data[1, 16] + "','" + data[1, 17] + "','" + data[1, 18] + "','" + data[1, 19] + "','" + data[1, 20] + "','" + data[1, 21] + "','" + data[1, 22] + "','" + data[1, 23] + "','" + data[1, 24] + "','" + data[1, 25] + "','" + data[1, 26] + "','" + data[1, 27] + "','" + data[1, 28] + "','" + data[1, 29] + "','" + data[1, 30] + "','" + data[1, 31] + "')";
            string sqlInsert2 = sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[2, 0] + "','" + data[2, 1] + "','" + data[2, 2] + "','" + data[2, 3] + "','" + data[2, 4] + "','" + data[2, 5] + "','" + data[2, 6] + "','" + data[2, 7] + "','" + data[2, 8] + "','" + data[2, 9] + "','" + data[2, 10] + "','" + data[2, 11] + "','" + data[2, 12] + "','" + data[2, 13] + "','" + data[2, 14] + "','" + data[2, 15] + "','" + data[2, 16] + "','" + data[2, 17] + "','" + data[2, 18] + "','" + data[2, 19] + "','" + data[2, 20] + "','" + data[2, 21] + "','" + data[2, 22] + "','" + data[2, 23] + "','" + data[2, 24] + "','" + data[2, 25] + "','" + data[2, 26] + "','" + data[2, 27] + "','" + data[2, 28] + "','" + data[2, 29] + "','" + data[2, 30] + "','" + data[2, 31] + "')";
            string sqlInsert3 = sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[3, 0] + "','" + data[3, 1] + "','" + data[3, 2] + "','" + data[3, 3] + "','" + data[3, 4] + "','" + data[3, 5] + "','" + data[3, 6] + "','" + data[3, 7] + "','" + data[3, 8] + "','" + data[3, 9] + "','" + data[3, 10] + "','" + data[3, 11] + "','" + data[3, 12] + "','" + data[3, 13] + "','" + data[3, 14] + "','" + data[3, 15] + "','" + data[3, 16] + "','" + data[3, 17] + "','" + data[3, 18] + "','" + data[3, 19] + "','" + data[3, 20] + "','" + data[3, 21] + "','" + data[3, 22] + "','" + data[3, 23] + "','" + data[3, 24] + "','" + data[3, 25] + "','" + data[3, 26] + "','" + data[3, 27] + "','" + data[3, 28] + "','" + data[3, 29] + "','" + data[3, 30] + "','" + data[3, 31] + "')";
            string sqlInsert4 = sqlInsert + "('" + System.DateTime.Now.ToString("HH:ss:mm.fff") + "','" + data[4, 0] + "','" + data[4, 1] + "','" + data[4, 2] + "','" + data[4, 3] + "','" + data[4, 4] + "','" + data[4, 5] + "','" + data[4, 6] + "','" + data[4, 7] + "','" + data[4, 8] + "','" + data[4, 9] + "','" + data[4, 10] + "','" + data[4, 11] + "','" + data[4, 12] + "','" + data[4, 13] + "','" + data[4, 14] + "','" + data[4, 15] + "','" + data[4, 16] + "','" + data[4, 17] + "','" + data[4, 18] + "','" + data[4, 19] + "','" + data[4, 20] + "','" + data[4, 21] + "','" + data[4, 22] + "','" + data[4, 23] + "','" + data[4, 24] + "','" + data[4, 25] + "','" + data[4, 26] + "','" + data[4, 27] + "','" + data[4, 28] + "','" + data[4, 29] + "','" + data[4, 30] + "','" + data[4, 31] + "')";

            SQLStringList.Add(sqlInsert0);
            SQLStringList.Add(sqlInsert1);
            SQLStringList.Add(sqlInsert2);
            SQLStringList.Add(sqlInsert3);
            SQLStringList.Add(sqlInsert4);

            //ht.Add(sqlInsert1, null);
            //ht.Add(sqlInsert2, null);
            //ht.Add(sqlInsert3, null);
            //ht.Add(sqlInsert4, null);
            //   DbHelperMySQL.ExecuteSqlTranWithIndentity(ht);
            if(SQLStringList.Count==300)
            {
                //System.Threading.Thread MysqlThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_MysqlThread));
                //MysqlThread.Start(SQLStringList);
                //System.Threading.Thread.Sleep(10);
                DbHelperMySQL.ExecuteSqlTran(SQLStringList);
                // 移除所有元素
                SQLStringList.Clear();
            }
        }
        private static void _MysqlThread(object SQLStringList)
        {
            List<String> SQLStringList2 = (List<String>) SQLStringList;
               DbHelperMySQL.ExecuteSqlTran(SQLStringList2);
        }







        /// <summary>
        /// 数据处理模块接口，更新数据
        /// </summary>
        /// <param name="data"></param>
        public async void Update(string[,] data)
        {
            System.DateTime daeee = System.DateTime.Now;
            //if(await MySQL_client.Inser_data(data)==false)
            //{
            //    MessageBox.Show("数据库存储异常！");
            //}
            try {
                //  InitHashtabletomysql(data);
                if (await MySQL_client.Inser_data(data) == false)
                {
                    MessageBox.Show("数据库存储异常！");
                }

            }
            catch (Exception err)
            { MessageBox.Show("数据库存储异常！");
            }
            

              
                Debug.WriteLine("数据库更新时间：" + (daeee - System.DateTime.Now).TotalMilliseconds);
        }


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

        public void RemoveMySQLdataPro()
        {
            RemoveRegister((Application.Current as App).dataPro);
        }

        /// <summary>
        /// 继承数据处理模块接口
        /// </summary>
        private Subject dataPro;

        /// <summary>
        /// 初始化数据库处理模块
        /// </summary>
        public void InitdataPro()
        {
            //  (Application.Current as App).dataPro.ConnectCan();
            RegisterObserver((Application.Current as App).dataPro);

        //    (Application.Current as App).dataPro.CollectMcuData(true);


            //    backgroundWorker1.RunWorkerAsync(backgroundWorker1_DoWork);
        }
        #endregion




    }





    /// <summary>
    /// 备份数据库
    /// </summary>
    public class BackupMySQL
    {

        /// <summary>
        /// 保存文件框
        /// </summary>
        /// <param name="_DefaultExt"></param>
        /// <param name="_Filter"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string _saveFileDialog(string _DefaultExt, string _Filter, string fileName)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = _DefaultExt;
            saveFileDialog.Filter = _Filter;
            saveFileDialog.AddExtension = false;
            saveFileDialog.CheckFileExists = false;
            saveFileDialog.CheckPathExists = false;
            saveFileDialog.FileName = fileName;
            if (saveFileDialog.ShowDialog() == true)
            {
                String directory = saveFileDialog.FileName;
                return directory;
            }
            else
            { return null; }
       }
        /// <summary>
        /// 备份MySQL
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="directory"></param>
        public static void Backup(string fileName,string directory)
        {
            try
            {
                //String command = "mysqldump --quick --host=localhost --default-character-set=gb2312 --lock-tables --verbose  --force --port=端口号 --user=用户名 --password=密码 数据库名 -r 备份到的地址";

                //构建执行的命令
                StringBuilder sbcommand = new StringBuilder();

              //  String fileName = sbfileName.ToString();

                    sbcommand.AppendFormat("mysqldump --quick --host=localhost --default-character-set=gbk --lock-tables --verbose  --force --port=3306 --user=root --password=fuyuan "+ fileName + " -r \"{0}\"", directory);
                    String command = sbcommand.ToString();

                    //获取mysqldump.exe所在路径
                    //     string str5 = Application.;
                    //    = Application.ExecutablePath;//获取启动了应用程序的可执行文件的路径，包括可执行文件的名称。



                    //获取mysqldump.exe所在路径
                //    String appDirecroty = "D:\\Program Files (x86)\\mysql - 5.7.14 - winx64\\bin";
                    String appDirecroty = AppDomain.CurrentDomain.BaseDirectory + "\\";
                    Cmd.StartCmd(appDirecroty, command);
                //    MessageBox.Show(@"数据库已成功备份到 " + directory + " 文件中", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库备份失败！");

            }
        }

    }

    public class Cmd
    {
        /// <summary>
        /// 执行Cmd命令
        /// </summary>
        /// <param name="workingDirectory">要启动的进程的目录</param>
        /// <param name="command">要执行的命令</param>
        public static void StartCmd(String workingDirectory, String command)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");
        }


    }

    public class RestoreMySql
    {
        public static void InitRestoreMySql(string MysqlName,string[] _AllDatabase,string FilePath)
        {

            //string s = "mysql --port=端口号 --user=用户名 --password=密码 数据库名<还原文件所在路径";

            //        sbcommand.AppendFormat("mysql --host=localhost --default-character-set=gbk --port=3306 --user=root --password=fuyuan dv8587721082ved0_24245322_201609182 <C:/Users/HHH/Desktop/11112222.sql
            try
            {
                StringBuilder sbcommand = new StringBuilder();

                 //   String directory = openFileDialog.FileName;
                    bool IsExits = _AllDatabase.Contains<string>(MysqlName);
                    if (IsExits==false)
                    {                     //先创建数据库
                        MySQL_client.CreateSQL(MysqlName);
                    }



                    //在文件路径后面加上""避免空格出现异常");
                    //在文件路径后面加上""避免空格出现异常
                    sbcommand.AppendFormat("mysql --host=localhost --default-character-set=gbk --port=3306 --user=root --password=fuyuan "+MysqlName + "<\"{0}\"", FilePath);
                    String command = sbcommand.ToString();

                    //获取mysql.exe所在路径
                //    String appDirecroty = "D:\\Program Files (x86)\\mysql - 5.7.14 - winx64\bin";
                    String appDirecroty = AppDomain.CurrentDomain.BaseDirectory ;

                    if (IsExits == true)
                    {
                      //  MessageBoxResult result = MessageBox.Show("数据库："+ MysqlName +"已存在，"+ "您是否真的想覆盖以前的数据库吗？那么以前的数据库数据将丢失！！！", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                       // if (result == MessageBoxResult.Yes)
                       // {
                            Cmd.StartCmd(appDirecroty, command);
                            //  MessageBox.Show("数据库还原成功！");
                     //   }
                    }
                    else
                    { Cmd.StartCmd(appDirecroty, command); }
                

            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库还原失败！");
            }

        }


    }


}

    //   & 


