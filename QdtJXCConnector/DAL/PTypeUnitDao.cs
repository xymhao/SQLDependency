using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public static class PTypeUnitDao
    {
        public static List<PTypeUnit> GetPTypeUnitByID(string pTypeID, string dbName)
        {
            List<PTypeUnit> ls = new List<PTypeUnit>();
            string TSQL = @" SELECT PTypeID
                                   ,Unit1
                                   ,URate
                                   ,IsBase
                                   ,OrdID 
                               FROM xw_PTypeUnit
                              WHERE PTypeID= @PTypeID; ";
            ls = DataBaseUtility.Query<PTypeUnit>(TSQL, dbName, new { PTypeID = pTypeID });
            return ls;
        }

    }
}
