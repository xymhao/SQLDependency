using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.HHModel
{
    public class BaseID<T> :Base
    {
        public T ID
        {
            get;
            set;
        }
    }
}
