using Models.HHModel;
using System.Collections.Generic;
using Utils;

namespace DAL
{
    public static class PTypeBarCodeDao
    {
        public static List<PTypeBarCode> GetPTypeBarCodeByID(string pTypeID, string dbName)
        {
            List<PTypeBarCode> ls = new List<PTypeBarCode>();
            string TSQL = @" SELECT BarCode
                                   ,PTypeID
                                   ,UnitID 
                               FROM xw_PTypeBarCode ;
                              WHERE PTypeID= @PTypeID ";
            ls = DataBaseUtility.Query<PTypeBarCode>(TSQL, dbName, new { PTypeID = pTypeID });
            return ls;
        }
    }
}
