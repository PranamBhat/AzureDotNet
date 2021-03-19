using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pranam.Data;
using System.Linq;

namespace Pranam
{
    public static class RestmeS3Extensions
    {
        private static IEnumerable<string> IdentifiedS3ProviderEndpoints
        {
            get
            {
                var identifiedList = new List<string> {"stackpathstorage.com"}
                    .AddAmazonEndpoints();
                return identifiedList;
            }
        }

        public static bool IsS3Provider(this string connectionString)
        {
            if (connectionString.IsNotNullOrEmpty())
            {
                return IdentifiedS3ProviderEndpoints.Count(item =>
                    connectionString.ToLower().Contains(item)) > 0;
            }

            return false;
        }

        internal static string S3BucketName(this string storageRelativePath)
        {
            if (storageRelativePath.IsNotNullOrEmpty())
            {
                var segments = storageRelativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments?.Length > 0)
                {
                    return segments[0];
                }
            }

            return null;
        }

        internal static string S3FileName(this string storageRelativePath)
        {
            if (storageRelativePath.IsNotNullOrEmpty())
            {
                var segments = storageRelativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments?.Length > 1)
                {
                    return segments[^1];
                }
            }

            return null;
        }

        internal static string S3ObjectPath(this string storageRelativePath, bool fromFilePath = true)
        {
            if (storageRelativePath.IsNotNullOrEmpty())
            {
                var result = storageRelativePath.Trim('/')
                    .Replace(storageRelativePath.S3BucketName(), string.Empty)
                    .Trim('/');
                if (fromFilePath)
                {
                    result = result.Replace(storageRelativePath.S3FileName(), string.Empty)
                        .Trim('/');
                }

                return result;
            }

            return null;
        }


        public static async Task<T> S3GetAsync<T>(this Rest restme, string storageRelativePath)
        {
            // restme.S3Client.GetObjectAsync(storageRelativePath.S3BucketName(),storageRelativePath.S3ObjectPath())

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

        public static async Task<T> S3PostAsync<T>(this Rest restme, string storageRelativePath, object dataObject)
        {
            MustBeS3Mode(restme);
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

        public static async Task<T> S3DeleteAsync<T>(this Rest restme, string storageRelativePath)
        {
            MustBeS3Mode(restme);
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

        private static void MustBeS3Mode(Rest restme)
        {
            if (restme?.CurrentMode != RestMode.S3Client)
                throw new InvalidOperationException(
                    $"current request is not valid operation, you are under RestMode: {restme.CurrentMode.ToString()}");
        }

        #endregion
    }
}