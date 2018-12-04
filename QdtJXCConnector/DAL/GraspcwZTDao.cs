using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public class GraspcwZTDao
    {
        public static List<GraspcwZT> GetByDogNumber(string dognumber)
        {
            string TSQL = @" SELECT [dbname] AS DbName
                                   ,[fullname] AS FullName
                                   ,[GraspHHType] AS GraspHHType
                                   ,[DogNo] AS DogNumber
                               FROM [master].[dbo].[GraspcwZt]
                              WHERE DogNo = @DogNo ";
            var list = DataBaseUtility.Query<GraspcwZT>(TSQL, "master", new { DogNo = dognumber });
            foreach (var l in list)
            {
                string sql = string.Format(@" SELECT SubValue FROM sysdata WHERE subname = 'versionno'; ");
                l.Version = DataBaseUtility.ExecuteScalar<string>(sql, l.DbName);
            }
            return list;
        }

        public static List<GraspcwZT> Get()
        {
            string TSQL = @" SELECT [dbname] AS DbName
                                   ,[fullname] AS FullName
                                   ,[GraspHHType] AS GraspHHType
                                   ,[DogNo] AS DogNumber
                               FROM [master].[dbo].[GraspcwZt]; ";
            var list = DataBaseUtility.Query<GraspcwZT>(TSQL, "master");
            return list;
        }

        public static string GetDogNumberByTableName(string tableName)
        {
            string TSQL = @" SELECT [DogNo] 
                               FROM [master].[dbo].[GraspcwZt]
                              WHERE dbname = @dbname ";
            var dognumber = DataBaseUtility.QueryFirst<string>(TSQL, "master", new { dbname = tableName });
            return dognumber;
        }
    }
}
