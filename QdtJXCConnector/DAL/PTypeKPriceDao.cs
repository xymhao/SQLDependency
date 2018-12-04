using Models.HHModel;
using System.Collections.Generic;
using Utils;
namespace DAL
{
    public static class PTypeKPriceDao
    {
        public static List<PTypeKPrice> GetPTypeKPriceByID(string pTypeID, string dbName)
        {
            List<PTypeKPrice> ls = new List<PTypeKPrice>();
            string TSQL = @" SELECT PTypeID
                                   ,UnitID
                                   ,KTypeID
                                   ,PRTypeID
                                   ,Price 
                               FROM XW_P_PtypeKPrice 
                              WHERE PTypeID= @PTypeID ";
            ls = DataBaseUtility.Query<PTypeKPrice>(TSQL, dbName, new { PTypeID = pTypeID });
            return ls;
        }
    }
}
