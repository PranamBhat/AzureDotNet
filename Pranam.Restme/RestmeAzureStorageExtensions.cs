using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Pranam
{
    public static class RestmeAzureStorageExtensions
    {
        public static async Task<T> AzureStorageGetAsync<T>(this Rest restme, string storageRelativePath)
        {
            MustBeStorageMode(restme);
            var container = await restme.GetAzureBlobContainerAsync(storageRelativePath);
            var blobItemPath = restme.IdentifyBlobItemPath(storageRelativePath);
            if (blobItemPath.IsNullOrEmpty())
                throw new PranamWebException("Invalid blob item name.");
            var blockBlob = container.GetBlockBlobReference(blobItemPath);
            using (var stream = new MemoryStream())
            {
                try
                {
                    if (!await blockBlob.ExistsAsync()) return default(T);

                    if (typeof(Stream).IsAssignableFrom(typeof(T)))
                    {
                        await blockBlob.DownloadToStreamAsync(stream);
                        var bytes = FileUtils.ReadStreamToEnd(stream);
                        T result;
                        if (typeof(T).GetTypeInfo().IsAbstract)
                        {
                            result = (T) Activator.CreateInstance(typeof(MemoryStream), bytes);
                        }
                        else
                            result = (T) Activator.CreateInstance(typeof(T), bytes);

                        return result;
                    }


                    var jsonStringValue = await blockBlob.DownloadTextAsync();
                    if (!jsonStringValue.IsNotNullOrEmpty()) return default(T);

                    if (typeof(T) == typeof(string))
                        return (T) Convert.ChangeType(jsonStringValue, typeof(T));

                    return jsonStringValue.JsonDeserialize<T>();
                }
                catch (Exception ex)
                {
                    restme.LogDebug(
                        $"Unable to fetch requested blob: {storageRelativePath}\n {ex.Message} \n {ex.StackTrace}", ex);
                    return default(T);
                }
            }
        }

        public static async Task<T> AzureStoragePostAsync<T>(this Rest restme, string storageRelativePath, object dataObject)
        {
            MustBeStorageMode(restme);
            if (dataObject == null)
                throw new PranamWebException(
                    "Uploading null blob is not supported, use delete method if you intended to delete.");

            var container = await restme.GetAzureBlobContainerAsync(storageRelativePath);
            var blobItemPath = restme.IdentifyBlobItemPath(storageRelativePath);
            if (blobItemPath.IsNullOrEmpty())
                throw new PranamWebException("Invalid blob item name.");
            var blockBlob = container.GetBlockBlobReference(blobItemPath);
            Stream stream = null;
            using (stream = new MemoryStream())
            {
                try
                {
                    var extension = FileUtils.GetFileExtensionName(storageRelativePath);
                    if (extension.IsNotNullOrEmpty())
                        blockBlob.Properties.ContentType = FileUtils.GetMimeType(extension);
                    if (typeof(Stream).IsAssignableFrom(typeof(T)))
                    {
                        stream = dataObject as Stream;
                        stream.Position = 0;
                        await blockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        var jsonValue =
                            dataObject.JsonSerialize(restme.Configuration.UseRestConvertForCollectionSerialization,
                                restme.Configuration.SerializerSettings);
                        await
                            blockBlob.UploadTextAsync(jsonValue, restme.Configuration.DefaultEncoding,
                                restme.DefaultAzureBlobAccessCondition, restme.DefaultAzureBlobRequestOptions,
                                restme.DefaultAzureBlobOperationContext);
                    }

                    return (T) dataObject;
                }
                catch (Exception ex)
                {
                    restme.LogDebug("Unable to upload requested data:\n" + ex.Message, ex);
                    return default(T);
                }
            }
        }

        public static async Task<T> AzureStorageDeleteAsync<T>(this Rest restme, string storageRelativePath)
        {
            MustBeStorageMode(restme);
            var container = await restme.GetAzureBlobContainerAsync(storageRelativePath);
            var blobItemPath = restme.IdentifyBlobItemPath(storageRelativePath);
            if (blobItemPath.IsNullOrEmpty())
                throw new PranamWebException("Invalid blob item name.");
            var blockBlob = container.GetBlockBlobReference(blobItemPath);
            try
            {
                await blockBlob.DeleteIfExistsAsync();
                if (typeof(T) == typeof(bool))
                    return (T) Convert.ChangeType(true, typeof(T));
            }
            catch (Exception ex)
            {
                restme.LogDebug("Unable to delete requested data:\n" + ex.Message, ex);
            }

            return default(T);
        }

        #region Private Methods

        private static void MustBeStorageMode(Rest restme)
        {
            if (restme?.CurrentMode != RestMode.AzureStorageClient)
                throw new InvalidOperationException(
                    $"current request is not valid operation, you are under RestMode: {restme.CurrentMode.ToString()}");
        }

        #endregion
    }
}