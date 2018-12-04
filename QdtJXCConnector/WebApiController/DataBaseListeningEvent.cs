using Models.HHModel;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QdtJXCConnector.WebApiController
{
    public class DataBaseListeningEvent
    {
        public static string DataSource = ConfigurationManager.AppSettings["DataSource"];
        public static string UserId = ConfigurationManager.AppSettings["AdminID"];
        public static string Password = ConfigurationManager.AppSettings["Password"];
        public static string UserID = ConfigurationManager.AppSettings["UserID"];

        public void Star()
        {
            //1.获取所有账套
            List<GraspcwZT> list = GraspcwZTDao.Get();
            try
            {
                //2.编写监听
                foreach (var zt in list)
                {

                    string connStr = string.Format("Data Source = {0};Initial Catalog = {1}; User Id = {2};Password = {3};", DataSource, zt.DbName, UserId, Password);
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();
                        //将数据库设置为ENABLE_BROKER
                        using (SqlCommand cmd = new SqlCommand(string.Format(@"ALTER DATABASE {0} SET NEW_BROKER WITH ROLLBACK IMMEDIATE;
                                                                           ALTER DATABASE {0} SET ENABLE_BROKER;", zt.DbName), con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        SqlDependency.Start(connStr);
                        if (zt.DbName == "TestServer")
                        {
                            using (SqlCommand command = new SqlCommand(@" select ID,Name,Value From [dbo].[TestName]", con))
                            {
                                SqlDependency dependency = new SqlDependency(command);
                                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                                SqlDataReader sdr = command.ExecuteReader();
                                Console.WriteLine();
                                //while (sdr.Read())
                                //{
                                //    Console.WriteLine("Id:{0}\tUserId:{1}\tMessage:{2}", sdr["ID"].ToString(), sdr["Name"].ToString(), sdr["Value"].ToString());
                                //}
                            }
                        }
                        else
                        {
                            //using (SqlCommand command = new SqlCommand(@" SELECT ID 
                            //                                FROM [dbo].[xymtest]", con))
                            //{
                            //    SqlDependency dependency = new SqlDependency(command);
                            //    dependency.OnChange += new OnChangeEventHandler(LoginUser_OnChange);
                            //    var sdr = command.ExecuteReader();
                            //    while (sdr.Read())
                            //    {
                            //        var ID = sdr["ID"].ToString();
                            //    }
                            //    Console.WriteLine();

                            //}
                            //break;
                            using (SqlCommand command = new SqlCommand(@" SELECT ID 
                                                            FROM [dbo].[xymtest]", con))
                            {
                                SqlDependency dependency = new SqlDependency(command);
                                dependency.OnChange += new OnChangeEventHandler(LoginUser_OnChange);
                                var sdr = command.ExecuteReader();
                                while (sdr.Read())
                                {
                                    var ID = sdr["ID"].ToString();
                                }
                            }
                        }

                        con.Close();
                    }
                }
            }
            catch
            {

            }
        }

        public void LoginUserListen(SqlConnection con)
        {
            using (SqlCommand command = new SqlCommand(@" SELECT TB1.[etypeid] 
                                                            FROM [dbo].[LoginUser] TB1", con))
            {
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += new OnChangeEventHandler(LoginUser_OnChange);
                command.ExecuteNonQuery();
                Console.WriteLine();

            }
        }

        private void LoginUser_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //string msg = "Dependency Change \nINFO: {0} : SOURCE {1} :TYPE: {2}";
            Star();

        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //string msg = "Dependency Change \nINFO: {0} : SOURCE {1} :TYPE: {2}";
            Star();
        }
    }
}
