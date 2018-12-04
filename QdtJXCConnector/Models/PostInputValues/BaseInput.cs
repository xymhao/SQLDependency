using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.PostInputValues
{
    public class BaseInput
    {
        public object input
        {
            get;
            set;
        }

        public BaseInput(object obj)
        {
            input = obj;
        }
    }
}
