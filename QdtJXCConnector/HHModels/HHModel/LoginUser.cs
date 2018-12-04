namespace Models.HHModel
{
    public class LoginUser : Base
    {
        public string ETypeID
        {
            get;
            set;
        }

        public string EFullName
        {
            get;
            set;
        }

        public string EUserCode
        {
            get;
            set;
        }

        public int IsStop
        {
            get;
            set;
        }
    }
}
