using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public class RightDao
    {
        public static List<T_Right> GetPRightsByETypeIDs(List<string> eTypeIDs, string dBName)
        {
            string TSQL = @" SELECT ETypeID
                                   ,RightID
                                   ,RState
                                   ,1 AS RightType
                               FROM T_PRight 
                              WHERE ETypeID IN @ETypeID ";
            var ls = DataBaseUtility.Query<T_Right>(TSQL, dBName, new { ETypeID = eTypeIDs });
            return ls;
        }

        public static List<T_Right> GetKRightsByETypeIDs(List<string> eTypeIDs, string dBName)
        {
            string TSQL = @" SELECT ETypeID
                                   ,RightID
                                   ,RState
                                   ,2 AS RightType
                               FROM T_KRight 
                              WHERE ETypeID IN @ETypeID ";
            var ls = DataBaseUtility.Query<T_Right>(TSQL, dBName, new { ETypeID = eTypeIDs });
            return ls;
        }
    }
}
