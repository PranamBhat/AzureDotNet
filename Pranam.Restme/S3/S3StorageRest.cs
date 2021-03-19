namespace Pranam
{
    public partial class Rest
    {
        private void PrepareS3StorageRestme()
        {
            if (ConnectionString.IsNullOrEmpty())
                throw new PranamWebException("Unable to fetch s3 endpoint connection string.");
            if (Configuration?.RestKey?.IsNotNullOrEmpty() == true &&
                Configuration.RestSecret.IsNotNullOrEmpty())
            {
                // S3Client = new AmazonS3Client( Configuration.RestKey,
                //     Configuration.RestSecret,
                //     new AmazonS3Config
                //     {
                //         
                //     }
                //     ConnectionString.SplitCamelCase());
                if (Configuration.RestSSL)
                {
                    // S3Client = S3Client.WithSSL();
                }

                if (Configuration.DefaultTimeout > 0)
                {
                    // S3Client = S3Client.WithTimeout(Configuration.DefaultTimeout);
                }

                Initialized = true;
            }
            else
            {
                throw new PranamWebException("Invalid S3 Access Key & Secret");
            }
        }
    }
}