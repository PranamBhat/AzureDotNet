using System;


namespace Pranam
{
    public class PranamSecurityException : PranamException
    {
        private const int ERROR_CODE = 1002;
        private const string ERROR_NAME = "GENERAL_SECURITY";

        public PranamSecurityException() : base("permission denied.", ERROR_CODE, ERROR_NAME) { }
        public PranamSecurityException(int errorCode, string errorName) : base("permission denied.", errorCode, errorName) { }
        public PranamSecurityException(string msg) : base(msg, ERROR_CODE, ERROR_NAME) { }
        public PranamSecurityException(string msg, int errorCode, string errorName) : base(msg, errorCode, errorName) { }
        public PranamSecurityException(string msg, Exception innerException) : base(msg, innerException, ERROR_CODE, ERROR_NAME) { }
        public PranamSecurityException(string msg, Exception innerException, int errorCode, string errorName) : base(msg, innerException, errorCode, errorName) { }

    }
}
