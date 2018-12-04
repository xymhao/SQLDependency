using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public class PTypeDao
    {
        public static PType GetPTypeByID(string pTypeID, string dbName)
        {
            PType type = new PType();
            string TSQL = @" SELECT PTypeID
                                   ,ParID
                                   ,Leveal
                                   ,PSonNum
                                   ,PUserCode
                                   ,PFullName
                                   ,Standard
                                   ,Type 
                               FROM PType
                              WHERE PTypeID= @PTypeID ";
            type = DataBaseUtility.QueryFirst<PType>(TSQL, dbName, new { PTypeID = pTypeID });
            return type;
        }

        public static List<PType> GetPTypeByIDs(List<string> pTypeIDs, string dbName)
        {
            string TSQL = @" SELECT PTypeID
                                   ,ParID
                                   ,Leveal
                                   ,PSonNum
                                   ,PUserCode
                                   ,PFullName
                                   ,Standard
                                   ,Type 
                               FROM PType
                              WHERE PTypeID IN @PTypeID 
                                AND IsStop = 0
                                AND Deleted = 0";
            var ls  = DataBaseUtility.Query<PType>(TSQL, dbName, new { PTypeID = pTypeIDs });
            return ls;
        }
    }
}
