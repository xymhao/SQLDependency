using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class Stock :Base
    {
        public string KTypeID
        {
            get;
            set;
        }

        public string ParID
        {
            get;
            set;
        }

        public int Leveal
        {
            get;
            set;
        }

        public int Level
        {
            get
            {
                return Leveal;
            }
        }

        public int KSonNum
        {
            get;
            set;
        }

        public int Deleted
        {
            get;
            set;
        }

        public int IsStop
        {
            get;
            set;
        }

        public string KFullName
        {
            get;
            set;
        }

        public string KUserCode
        {
            get;
            set;
        }
    }
}
