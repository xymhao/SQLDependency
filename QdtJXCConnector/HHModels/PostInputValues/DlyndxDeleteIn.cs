using Models.HHModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PostInputValues
{
    public class DlyndxDeleteIn : Base
    {
        public string OrderDate
        {
            get;
            set;
        }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public string BTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 经手人ID
        /// </summary>
        public string ETypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public string KTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// 制单人
        /// </summary>
        public string InputNo
        {
            get;
            set;
        }
    }
}
