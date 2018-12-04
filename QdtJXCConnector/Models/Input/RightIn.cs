using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Input
{
    public class RightIn
    {
        /// <summary>
        /// 数据库表名
        /// </summary>
        public string DbName
        {
            get;
            set;
        }
        /// <summary>
        /// 1商品权限，2仓库权限
        /// </summary>
        public int RightType
        {
            get;
            set;
        }
        /// <summary>
        /// 人员ID
        /// </summary>
        public List<string> ETypeIDs
        {
            get;
            set;
        }
    }
}
