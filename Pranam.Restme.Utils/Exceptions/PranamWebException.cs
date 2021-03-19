using System;

namespace Pranam
{
    public class PranamWebException : PranamException
    {
        private const int ERROR_CODE = 1004;
        private const string ERROR_NAME = "GENERAL_WEB";
        public PranamWebException(bool createLog = false) : base(ERROR_CODE, ERROR_NAME, createLog) { }
        public PranamWebException(int errorCode, string errorName, bool createLog = false) : base(errorCode, errorName, createLog) { }
        public PranamWebException(string msg, bool createLog = false) : base(msg, ERROR_CODE, ERROR_NAME, createLog) { }
        public PranamWebException(string msg, int errorCode, string errorName, bool createLog = false) : base(msg, errorCode, errorName, createLog) { }
        public PranamWebException(string msg, Exception innerException, bool createLog = false) : base(msg, innerException, ERROR_CODE, ERROR_NAME, createLog) { }
        public PranamWebException(string msg, Exception innerException, int errorCode, string errorName, bool createLog = false) : base(msg, innerException, errorCode, errorName, createLog) { }

    }
}
