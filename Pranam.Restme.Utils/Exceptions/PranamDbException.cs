using System;


namespace Pranam
{
    public class PranamDbException : PranamException
    {
        private const int ERROR_CODE = 1003;
        private const string ERROR_NAME = "GENERAL_DATA";
        public PranamDbException() : base(ERROR_CODE, ERROR_NAME) { }
        public PranamDbException(string msg) : base(msg, ERROR_CODE, ERROR_NAME, true) { }
        public PranamDbException(string msg, int errorCode, string errorName) : base(msg, errorCode, errorName, true) { }
        public PranamDbException(string msg, Exception innerException) : base(msg, innerException, ERROR_CODE, ERROR_NAME, true) { }
        public PranamDbException(string msg, Exception innerException, int errorCode, string errorName) : base(msg, innerException, errorCode, errorName, true) { }

    }
}
