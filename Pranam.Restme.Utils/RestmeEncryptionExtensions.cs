namespace Pranam.Utils
{
    public partial class Restme
    {
        public static string MD5(string value)
        {
            return value.MD5Encrypt();
        }
    }
}