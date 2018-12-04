using Models.HHModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PostInputValues
{
    public class SysConIn : SysCon
    {
        public string DBName
        {
            get;
            set;
        }

        public SysConIn(SysCon sys)
        {
            Order = sys.Order;
            Stats = sys.Stats;
        }
    }
}
