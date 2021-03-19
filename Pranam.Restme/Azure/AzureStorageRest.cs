using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Pranam
{
    public partial class Rest
    {
        private CloudStorageAccount azureStorageAccount;
        private CloudBlobClient azureBlobClient;

        public AccessCondition DefaultAzureBlobAccessCondition { get; set; }
        public BlobRequestOptions DefaultAzureBlobRequestOptions { get; set; }
        public OperationContext DefaultAzureBlobOperationContext { get; set; }


        public bool CreateAzureBlobContainerIfNotExists { get; set; }
        public BlobContainerPublicAccessType BlobContainerPublicAccessType { get; set; }

        private void PrepareAzureStorageRestme()
        {
            if (ConnectionString.IsNullOrEmpty())
                throw new PranamWebException("Unable to fetch azure storage connection string.");

            azureStorageAccount = CloudStorageAccount.Parse(ConnectionString);
            azureBlobClient = azureStorageAccount.CreateCloudBlobClient();
            Initialized = true;
        }

        internal async Task<CloudBlobContainer> GetAzureBlobContainerAsync(string relativePath)
        {
            var containerName = IdentifyBlobContainerName(relativePath);
            var container = azureBlobClient.GetContainerReference(containerName);
            if (CreateAzureBlobContainerIfNotExists)
            {
                var result = await container.CreateIfNotExistsAsync();
                if (result)
                    await container.SetPermissionsAsync(
                        new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType});
            }
            return container;
        }

        internal string IdentifyBlobContainerName(string relativePath)
        {
            relativePath = relativePath?.Replace('\\', '/')?.Replace("//", "/").Trim('/');
            var indexOfFirstSegmentEnd = relativePath?.IndexOf('/');
            if (indexOfFirstSegmentEnd > 0)
                return relativePath.Substring(0, indexOfFirstSegmentEnd.Value).Trim('/');
            else
                throw new PranamWebException("Unable to identify azure blob container name.");
        }

        internal string IdentifyBlobItemPath(string relativePath)
        {
            return relativePath?.Replace('\\', '/')
                ?
                .Replace("//", "/")
                ?
                .Trim('/')
                ?
                .Replace(IdentifyBlobContainerName(relativePath), string.Empty)
                ?
                .Trim('/');
        }
    }
}