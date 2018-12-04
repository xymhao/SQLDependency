using Models.HHModel;
using Utils;

namespace DAL
{
    public class BTypeDao
    {
        public static BType GetPTypeByID(string bTypeID, string dbName)
        {
            BType type = new BType();
            string TSQL = @" SELECT BTypeID
                                   ,ParID
                                   ,Leveal as Level
                                   ,Deleted
                                   ,IsStop
                                   ,BSonNum
                                   ,BUserCode
                                   ,BFullName
                                   ,ARTotal
                                   ,APTotal
                                   ,YRTotal
                                   ,YPTotal
                              FROM BType 
                             WHERE BTypeID = @BTypeID ";
            type = DataBaseUtility.QueryFirst<BType>(TSQL, dbName, new { BTypeID = bTypeID });
            return type;
        }
    }
}
