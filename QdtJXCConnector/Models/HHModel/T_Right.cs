using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class T_Right : Base
    {
        public string ETypeID
        {
            get;
            set;
        }

        public string RightID
        {
            get;
            set;
        }

        public int RState
        {
            get;
            set;
        }

        /// <summary>
        /// 1表示商品，2表示仓库，3表示商品价格。
        /// </summary>
        public int RightType
        {
            get;
            set;
        }
    }
}
