using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Maticsoft.DBUtility;
using System.Configuration;
using System.Windows;

namespace SolenoidTester.DataClass
{
    class DbHelperMySQL
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.       
        //    public static string connectionString = PubConstant.ConnectionString;
        public static string connectionString;
      //  public static string connectionStringHangUpQuery = PubConstant.ConnectionStringHangUpQuery;

        public DbHelperMySQL()
        {
        }
        /// <summary>  
        /// 通用分页存储过程  
        /// </summary>  
        /// <param name="connectionString">连接</param>  
        /// <param name="tblName">要显示的表或多个表的连接</param>  
        /// <param name="fldName">要显示的字段列表,可为Null,表示*</param>  
        /// <param name="pageSize">每页显示的记录个数</param>  
        /// <param name="pageIndex">要显示那一页的记录</param>  
        /// <param name="fldSort">排序字段列表或条件</param>  
        /// <param name="Sort">排序方法，False为升序，True为降序(如果是多字段排列Sort指代最后一个排序字段的排列顺序(最后一个排序字段不加排序标记)--程序传参如：' SortA Asc,SortB Desc,SortC ')</param>  
        /// <param name="strCondition">查询条件,不需where,以And开始,可为Null,表示""</param>  
        /// <param name="ID">主表的主键</param>  
        /// <param name="Disk">是否添加查询字段的 DISTINCT 默认False不添加/True添加</param>  
        /// <param name="pageCount">查询结果分页后的总页数</param>  
        /// <param name="Counts">查询到的记录数</param>  
        /// <param name="strSql">最后返回的SQL语句</param>  
        /// <returns>查询当前页的数据集</returns>  
        public static DataSet PageList(string connectionString, string tblName, string fldName, int pageSize, int pageIndex,
            string fldSort, bool Sort, string strCondition, string ID, bool Dist,
            out int pageCount, out int Counts, out string strSql)
        {
            MySqlParameter[] parameters ={ new MySqlParameter("@tblName",MySqlDbType.VarChar,200),
                new MySqlParameter("@fldName",MySqlDbType.VarChar,500),
                new MySqlParameter("@pageSize",MySqlDbType.Int32),
                new MySqlParameter("@page",MySqlDbType.Int32),
                new MySqlParameter("@fldSort",MySqlDbType.VarChar,200),
                new MySqlParameter("@Sort",MySqlDbType.Bit),
                new MySqlParameter("@strCondition",MySqlDbType.VarChar,4000),
                new MySqlParameter("@ID",MySqlDbType.VarChar,150),
                new MySqlParameter("@Dist",MySqlDbType.Bit),
                new MySqlParameter("@pageCount",MySqlDbType.Int32),
                new MySqlParameter("@Counts",MySqlDbType.Int32),
                new MySqlParameter("@strSql",MySqlDbType.VarChar,4000)};

            parameters[0].Value = tblName;
            parameters[1].Value = (fldName == null) ? "*" : fldName;
            parameters[2].Value = (pageSize == 0) ? int.Parse(ConfigurationManager.AppSettings["PageSize"]) : pageSize;
            parameters[3].Value = pageIndex;
            parameters[4].Value = fldSort;
            parameters[5].Value = Sort;
            parameters[6].Value = strCondition == null ? "" : strCondition;
            parameters[7].Value = ID;
            parameters[8].Value = Dist;
            parameters[9].Direction = ParameterDirection.Output;
            parameters[10].Direction = ParameterDirection.Output;
            parameters[11].Direction = ParameterDirection.Output;

            DataSet ds = RunProcedure(connectionString, "PageList", parameters, "ds");

            pageCount = (int) parameters[9].Value;
            Counts = (int) parameters[10].Value;
            strSql = parameters[11].Value.ToString();
            return ds;
        }

        /// <summary>  
        /// 执行存储过程  
        /// </summary>  
        /// <param name="storedProcName">存储过程名</param>  
        /// <param name="parameters">存储过程参数</param>  
        /// <param name="tableName">DataSet结果中的表名</param>  
        /// <returns>DataSet</returns>  
        public static DataSet RunProcedure(string connectionString, string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                MySqlDataAdapter sqlDA = new MySqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }
        /// <summary>  
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)  
        /// </summary>  
        /// <param name="connection">数据库连接</param>  
        /// <param name="storedProcName">存储过程名</param>  
        /// <param name="parameters">存储过程参数</param>  
        /// <returns>SqlCommand</returns>  
        private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                foreach (MySqlParameter parameter in parameters)
                {
                    if (parameter.MySqlDbType == MySqlDbType.DateTime)
                    {
                        if ((DateTime) parameter.Value == DateTime.MinValue)
                            parameter.Value = System.DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            return command;
        }

        public static bool RunprocedureT(string storedProcName, IDataParameter[] parameters)
        {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
                command.CommandTimeout = 120;
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch
                { return false; }

            }

        }


        #region 公用方法  
        /// <summary>  
        /// 得到最大值  
        /// </summary>  
        /// <param name="FieldName"></param>  
        /// <param name="TableName"></param>  
        /// <returns></returns>  
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        /// <summary>  
        /// 是否存在  
        /// </summary>  
        /// <param name="strSql"></param>  
        /// <returns></returns>  
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>  
        /// 是否存在（基于MySqlParameter）  
        /// </summary>  
        /// <param name="strSql"></param>  
        /// <param name="cmdParms"></param>  
        /// <returns></returns>  
        public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句  

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        public static int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>  
        /// 执行MySql和Oracle滴混合事务  
        /// </summary>  
        /// <param name="list">SQL命令行列表</param>  
        /// <param name="oracleCmdSqlList">Oracle命令行列表</param>  
        /// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>  
        public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (CommandInfo myDE in list)
                    {
                        string cmdText = myDE.CommandText;
                        MySqlParameter[] cmdParms = (MySqlParameter[]) myDE.Parameters;
                        PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
                        if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
                        {
                            if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                tx.Rollback();
                                throw new Exception("违背要求" + myDE.CommandText + "必须符合select count(..的格式");
                                //return 0;  
                            }

                            object obj = cmd.ExecuteScalar();
                            bool isHave = false;
                            if (obj == null && obj == DBNull.Value)
                            {
                                isHave = false;
                            }
                            isHave = Convert.ToInt32(obj) > 0;
                            if (isHave)
                            {
                                //引发事件  
                                myDE.OnSolicitationEvent();
                            }
                        }
                        if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                        {
                            if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
                                //return 0;  
                            }

                            object obj = cmd.ExecuteScalar();
                            bool isHave = false;
                            if (obj == null && obj == DBNull.Value)
                            {
                                isHave = false;
                            }
                            isHave = Convert.ToInt32(obj) > 0;

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
                                //return 0;  
                            }
                            if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
                                //return 0;  
                            }
                            continue;
                        }
                        int val = cmd.ExecuteNonQuery();
                        if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                        {
                            tx.Rollback();
                            throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
                            //return 0;  
                        }
                        cmd.Parameters.Clear();
                    }
                    //string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");  
                    //bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);  
                    //if (!res)  
                    //{  
                    //    tx.Rollback();  
                    //    throw new Exception("执行失败");  
                    //    // return -1;  
                    //}  
                    tx.Commit();
                    return 1;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    tx.Rollback();
                    throw e;
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    throw e;
                }
            }
        }
        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">多条SQL语句</param>       
        public static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch(Exception er)
                {
                    MessageBox.Show(er.Message);
                    tx.Rollback();
                    return 0;
                }
            }
        }
        /// <summary>  
        /// 执行带一个存储过程参数的的SQL语句。  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>  
        /// 执行带一个存储过程参数的的SQL语句。  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>  
        /// <returns>影响的记录数</returns>  
        public static object ExecuteSqlGet(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>  
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)  
        /// </summary>  
        /// <param name="strSQL">SQL语句</param>  
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(strSQL, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        public static object GetSingle(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }
        /// <summary>  
        /// 执行查询语句，返回MySqlDataReader ( 注意：调用该方法后，一定要对MySqlDataReader进行Close )  
        /// </summary>  
        /// <param name="strSQL">查询语句</param>  
        /// <returns>MySqlDataReader</returns>  
        public static MySqlDataReader ExecuteReader(string strSQL)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }

        }
        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public static DataSet Query(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        public static DataSet Query(string SQLString, string connectionString_)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString_))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }

        public static DataSet ExecuteSelectSql(string selectSql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        DataSet ds = new DataSet();
                        MySqlDataAdapter command = new MySqlDataAdapter();

                        cmd.CommandType = CommandType.Text;
                        command.SelectCommand = cmd;
                        command.Fill(ds);
                        return ds;

                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        public static DataSet Query(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
        }



        #endregion

        #region 执行带参数的SQL语句  

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        //高效执行多条sql  
        public static int ExecuteSql1(MySqlConnection connection, string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
            }
        }


        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>  
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        //循环  
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[]) myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>  
        public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int count = 0;
                        //循环  
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[]) myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>  
        public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环  
                        foreach (CommandInfo myDE in SQLStringList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[]) myDE.Parameters;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>  
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //循环  
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[]) myDE.Value;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>  
        /// 执行查询语句，返回MySqlDataReader ( 注意：调用该方法后，一定要对MySqlDataReader进行Close )  
        /// </summary>  
        /// <param name="strSQL">查询语句</param>  
        /// <returns>MySqlDataReader</returns>  
        public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                throw e;
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }

        }

        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }


        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;  
            if (cmdParms != null)
            {


                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        public class SQLParameter
        {
            public string sKey;
            public MySqlParameter[] MySqlParameter;
        }


        /// <summary>  
        /// 执行多条SQL语句，实现数据库事务。  
        /// </summary>  
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的MySqlParameter[]）</param>  
        public static bool ExecuteSqlTran(Hashtable SQLStringList, ref string errMsg)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlTransaction trans = conn.BeginTransaction())
                    {
                        MySqlCommand cmd = new MySqlCommand();
                        try
                        {
                            //循环  
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = ((SQLParameter) myDE.Value).sKey;
                                MySqlParameter[] cmdParms = ((SQLParameter) myDE.Value).MySqlParameter;
                                //string cmdText = myDE.Key.ToString();  
                                //MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;  
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                            return true;
                        }
                        catch (Exception Ex)
                        {
                            errMsg = Ex.Message;
                            trans.Rollback();
                            return false;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                errMsg = Ex.Message;
                return false;
            }
        }

        #endregion

        #region 由Object取值  
        /// <summary>  
        /// 取得Int值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static int GetInt(object obj)
        {
            if (obj.ToString() != "")
                return int.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>  
        /// 取得byte值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static byte Getbyte(object obj)
        {
            if (obj.ToString() != "")
                return byte.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>  
        /// 获得Long值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static long GetLong(object obj)
        {
            if (obj.ToString() != "")
                return long.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>  
        /// 取得Decimal值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static decimal GetDecimal(object obj)
        {
            if (obj.ToString() != "")
                return decimal.Parse(obj.ToString());
            else
                return 0;
        }

        /// <summary>  
        /// 取得Guid值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static Guid GetGuid(object obj)
        {
            if (obj.ToString() != "")
                return new Guid(obj.ToString());
            else
                return Guid.Empty;
        }

        /// <summary>  
        /// 取得DateTime值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static DateTime GetDateTime(object obj)
        {
            if (obj.ToString() != "")
                return DateTime.Parse(obj.ToString());
            else
                return DateTime.MinValue;
        }

        /// <summary>  
        /// 取得bool值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static bool GetBool(object obj)
        {
            if (obj.ToString() == "1" || obj.ToString().ToLower() == "true")
                return true;
            else
                return false;
        }

        /// <summary>  
        /// 取得byte[]  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static Byte[] GetByte(object obj)
        {
            if (obj.ToString() != "")
            {
                return (Byte[]) obj;
            }
            else
                return null;
        }

        /// <summary>  
        /// 取得string值  
        /// </summary>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static string GetString(object obj)
        {
            return obj.ToString();
        }
        #endregion

        #region 序列化与反序列化  
        /// <summary>  
        /// 序列化对象  
        /// </summary>  
        /// <param name="obj">要序列化的对象</param>  
        /// <returns>返回二进制</returns>  
        public static byte[] SerializeModel(Object obj)
        {
            if (obj != null)
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                byte[] b;
                binaryFormatter.Serialize(ms, obj);
                ms.Position = 0;
                b = new Byte[ms.Length];
                ms.Read(b, 0, b.Length);
                ms.Close();
                return b;
            }
            else
                return new byte[0];
        }

        /// <summary>  
        /// 反序列化对象  
        /// </summary>  
        /// <param name="b">要反序列化的二进制</param>  
        /// <returns>返回对象</returns>  
        public static object DeserializeModel(byte[] b, object SampleModel)
        {
            if (b == null || b.Length == 0)
                return SampleModel;
            else
            {
                object result = new object();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                try
                {
                    ms.Write(b, 0, b.Length);
                    ms.Position = 0;
                    result = binaryFormatter.Deserialize(ms);
                    ms.Close();
                }
                catch { }
                return result;
            }
        }
        #endregion

        #region Model与XML互相转换  
        /// <summary>  
        /// Model转化为XML的方法  
        /// </summary>  
        /// <param name="model">要转化的Model</param>  
        /// <returns></returns>  
        public static string ModelToXML(object model)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlElement ModelNode = xmldoc.CreateElement("Model");
            xmldoc.AppendChild(ModelNode);

            if (model != null)
            {
                foreach (PropertyInfo property in model.GetType().GetProperties())
                {
                    XmlElement attribute = xmldoc.CreateElement(property.Name);
                    if (property.GetValue(model, null) != null)
                        attribute.InnerText = property.GetValue(model, null).ToString();
                    else
                        attribute.InnerText = "[Null]";
                    ModelNode.AppendChild(attribute);
                }
            }

            return xmldoc.OuterXml;
        }

        /// <summary>  
        /// XML转化为Model的方法  
        /// </summary>  
        /// <param name="xml">要转化的XML</param>  
        /// <param name="SampleModel">Model的实体示例，New一个出来即可</param>  
        /// <returns></returns>  
        public static object XMLToModel(string xml, object SampleModel)
        {
            if (string.IsNullOrEmpty(xml))
                return SampleModel;
            else
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(xml);

                XmlNodeList attributes = xmldoc.SelectSingleNode("Model").ChildNodes;
                foreach (XmlNode node in attributes)
                {
                    foreach (PropertyInfo property in SampleModel.GetType().GetProperties())
                    {
                        if (node.Name == property.Name)
                        {
                            if (node.InnerText != "[Null]")
                            {
                                if (property.PropertyType == typeof(System.Guid))
                                    property.SetValue(SampleModel, new Guid(node.InnerText), null);
                                else
                                    property.SetValue(SampleModel, Convert.ChangeType(node.InnerText, property.PropertyType), null);
                            }
                            else
                                property.SetValue(SampleModel, null, null);
                        }
                    }
                }
                return SampleModel;
            }
        }
        #endregion

        #region 数据库长连接  
        public static MySqlConnection OpenSqlConn(MySqlConnection conn)
        {
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                return conn;
            }
            catch
            {
                return null;
            }
        }

        public static bool CloseSqlConn(MySqlConnection conn)
        {
            try
            {
                conn.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>  
        /// 是否存在（基于MySqlParameter）  
        /// </summary>  
        /// <param name="strSql"></param>  
        /// <param name="cmdParms"></param>  
        /// <returns></returns>  
        public static bool Exists(MySqlConnection connection, string strSql, params MySqlParameter[] cmdParms)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            object obj = GetSingle(connection, strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(MySqlConnection connection, string SQLString, params MySqlParameter[] cmdParms)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
            }
        }


        public static DataSet Query(MySqlConnection connection, string SQLString, params MySqlParameter[] cmdParms)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }

        }

        /// <summary>  
        /// 执行一条计算查询结果语句，返回查询结果（object）。  
        /// </summary>  
        /// <param name="SQLString">计算查询结果语句</param>  
        /// <returns>查询结果（object）</returns>  
        public static object GetSingle(MySqlConnection connection, string SQLString, params MySqlParameter[] cmdParms)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    throw e;
                }
            }

        }

        /// <summary>  
        /// 执行查询语句，返回DataSet  
        /// </summary>  
        /// <param name="SQLString">查询语句</param>  
        /// <returns>DataSet</returns>  
        public static DataSet Query(MySqlConnection connection, string SQLString)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            DataSet ds = new DataSet();
            try
            {
                MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                command.Fill(ds, "ds");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
            return ds;
        }

        /// <summary>  
        /// 执行SQL语句，返回影响的记录数  
        /// </summary>  
        /// <param name="SQLString">SQL语句</param>  
        /// <returns>影响的记录数</returns>  
        public static int ExecuteSql(MySqlConnection connection, string SQLString)
        {
            if (connection.State != ConnectionState.Open)
            {
                OpenSqlConn(connection);
            }
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    connection.Close();
                    throw e;
                }
            }

        }
        #endregion

    }
}
