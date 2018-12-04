using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class P_PriceType : Base
    {
        public string PrTypeid
        {
            get;
            set;
        }

        public string PrDisName
        {
            get;
            set;
        }

        public int IsVisible
        {
            get;
            set;
        }

        public int IsSystem
        {
            get;
            set;
        }
    }
}
