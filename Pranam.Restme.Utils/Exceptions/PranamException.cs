using System;

namespace Pranam
{

    public class PranamException : Exception
    {
        //protected static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public int PranamErrorCode { get; set; }
        public string PranamErrorName { get; set; }
        public PranamException(bool createlog = false) : base()
        {
            PranamErrorCode = 1001;
            PranamErrorName = "GENERAL";
            //if (createlog)
            //    logger.Error("[Pranam Exception]", this);
        }
        public PranamException(int errorCode, string errorName, bool createlog = false) : base()
        {
            PranamErrorCode = errorCode;
            PranamErrorName = errorName;
            //if (createlog)
            //    logger.Error("[Pranam Exception]", this);
        }
        public PranamException(string msg, bool createlog = false) : base(msg)
        {
            PranamErrorCode = 1001; PranamErrorName = "GENERAL";
            //if (createlog)
            //    logger.Error("[Pranam Exception: " + msg + " ]", this);
        }
        public PranamException(string msg, int errorCode, string errorName, bool createlog = false) : base(msg)
        {
            PranamErrorCode = errorCode; PranamErrorName = errorName;
            //if (createlog)
            //    logger.Error(msg, this);
        }
        public PranamException(string msg, Exception innerException, bool createlog = false) : base(msg, innerException)
        {
            PranamErrorCode = 1001; PranamErrorName = "GENERAL";
            //if (createlog)
            //    logger.Error(innerException, msg);
        }
        public PranamException(string msg, Exception innerException, int errorCode, string errorName, bool createlog = false) : base(msg, innerException)
        {
            PranamErrorCode = errorCode; PranamErrorName = errorName;
            //if (createlog)
            //    logger.Error(innerException, "[Pranam Exception: " + msg + " ]");
        }
    }
}
