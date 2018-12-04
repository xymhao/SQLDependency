using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.OleDb;
using Utils;

namespace Utils
{
    public static class DataBaseUtility
    {       
        public static string defaultConnection;

        public static string GetConnectionStr(string dbName)
        {
            string DataSource = ConfigurationManager.AppSettings["DataSource"];
            string UserId = ConfigurationManager.AppSettings["AdminID"];
            string Password = EncryptUtility.MD5Decrypt(ConfigurationManager.AppSettings["Password"]);
            return string.Format("Data Source = {0};Initial Catalog = {1}; User Id = {2};Password = {3};", DataSource, dbName, UserId, Password);
        }

        public static void Validate()
        {
            var con = new SqlConnection(GetConnectionStr("master"));
            try
            {
                con.Open();
                var version = con.ServerVersion;
                var vArr = version.Split('.');
                if (vArr.Length > 0
                    && (int.Parse(vArr[0]) < 10
                    || (int.Parse(vArr[0]) == 10 && int.Parse(vArr[1]) != 50)))
                {
                    throw new Exception("请将数据库升级到SQL Server2008 R2");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                con.Close();
            }
        }

        #region 基于Dapper的封装方法
        public static T ExecuteScalar<T>(string sql, string dbName, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = new SqlConnection(GetConnectionStr(dbName)))
            {
                try
                {
                    connection.Open();
                    return connection.ExecuteScalar<T>(sql, param, transaction, commandTimeout, commandType);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        internal static object Query<T>(T v, Func<object, T> p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行SQL语句,返回受影响的行数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int Execute(string sql, string dbName, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var connection = new SqlConnection(GetConnectionStr(dbName)))
            {
                try
                {
                    connection.Open();
                    return connection.Execute(sql, param, commandType: commandType);
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 查询并返回第一个结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T QueryFirst<T>(string sql, string dbName, object param = null, CommandType? commandType = null)
        {
            using (var connection = new SqlConnection(GetConnectionStr(dbName)))
            {
                try
                {
                    connection.Open();
                    var ret = connection.QueryFirstOrDefault<T>(sql, param, commandType: commandType);
                    return ret;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static List<T> Query<T>(string sql, string dbName, object param = null)
        {
            using (var connection = new SqlConnection(GetConnectionStr(dbName)))
            {
                try
                {
                    connection.Open();
                    var list = connection.Query<T>(sql, param);
                    if (list != null)
                        return list.ToList<T>();
                    return null;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static List<TReturn> Query<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null)
        {
            using (var connection = new SqlConnection(defaultConnection))
            {
                try
                {
                    connection.Open();
                    var list = connection.Query<TFirst, TSecond, TReturn>(sql, map, param);
                    if (list != null)
                        return list.ToList<TReturn>();
                    return null;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static List<TReturn> Query<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null)
        {
            using (var connection = new SqlConnection(defaultConnection))
            {
                try
                {
                    connection.Open();
                    var list = connection.Query<TFirst, TSecond, TThird, TReturn>(sql, map, param);
                    if (list != null)
                        return list.ToList<TReturn>();
                    return null;
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion

        #region 原生DataReader方法
        public static List<TReturn> Query<TReturn>(string sql, string dbName, Func<SqlDataReader, TReturn> map)
        {
            var list = new List<TReturn>();
            using (var connection = new SqlConnection(GetConnectionStr(dbName)))
            {
                try
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();

                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            TReturn t = map(dr);
                            if (t != null)
                                list.Add(t);
                        }
                        while (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                TReturn t = map(dr);
                                if (t != null)
                                    list.Add(t);
                            }
                        }
                        dr.Close();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return list;
        }
        #endregion
    }
}