using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Pranam
{
    public partial class Rest : IRestme, IDisposable
    {
        internal Dictionary<string, string> _params;
        internal Dictionary<string, List<string>> _headers;
        internal object _objAsParam;
        public RestConfig Configuration { get; set; }

        public Uri BaseUri { get; set; }
        public string ConnectionString { get; set; }
        public string RequestUrlPath { get; set; }
        public bool Initialized { get; set; }


        private void Init(RestConfig config = null)
        {
            _params = new Dictionary<string, string>();
            _headers = new Dictionary<string, List<string>>();

            Configuration = config ?? new RestConfig();
            this.PrepareRestMode();
        }

        public Rest(Uri baseUri = null, string urlPath = null, RestConfig config = null, ILogger logger = null)
        {
            BaseUri = baseUri;
            RequestUrlPath = urlPath;
            Logger = logger;
            Init(config);
        }

        public Rest(string endPointOrConnectionString, RestConfig configuration = null, ILogger logger = null)
        {
            var lowerConn = endPointOrConnectionString;
            if (lowerConn != null && lowerConn.StartsWith("http"))
                BaseUri = new Uri(endPointOrConnectionString);
            else
                ConnectionString = endPointOrConnectionString;
            Logger = logger;
            Init(configuration);
        }


        public RestMode CurrentMode
        {
            get => Configuration.OperationMode;
            set
            {
                Configuration.OperationMode = value;
                switch (value)
                {
                    case RestMode.AzureStorageClient:
                        PrepareAzureStorageRestme();
                        break;
                    case RestMode.S3Client:
                        PrepareS3StorageRestme();
                        break;
                    case RestMode.RedisCacheClient:
                        PrepareRedisRestme();
                        break;
                    case RestMode.HTTPClient:
                    case RestMode.HTTPRestClient:
                    default:
                        PrepareHttpRestme();
                        break;
                }
            }
        }


        public void Add(object value)
        {
            if (_params?.Count > 0)
                throw new InvalidOperationException(
                    "Additional parameters have been added, try use Add(string key, object value) instead of Add(object value).");
            _objAsParam = value;
        }

        public void Add(string key, string value)
        {
            _params = _params ?? new Dictionary<string, string>();

            if (_params.ContainsKey(key))
                _params[key] = value;
            else
                _params.Add(key, value);
        }

        public void Add(string key, object value)
        {
            _params = _params ?? new Dictionary<string, string>();

            if (_params.ContainsKey(key))
                _params[key] = value.JsonSerialize(Configuration.UseRestConvertForCollectionSerialization,
                    Configuration.SerializerSettings);
            else
                _params.Add(key,
                    value.JsonSerialize(Configuration.UseRestConvertForCollectionSerialization,
                        Configuration.SerializerSettings));
        }

        public void AddHeader(string header, string value, bool allowMultipleValues = false)
        {
            _headers = _headers ?? new Dictionary<string, List<string>>();
            if (_headers.ContainsKey(header))
            {
                _headers[header] = _headers[header] ?? new List<string>();
                if (allowMultipleValues)
                    _headers[header].Add(value);
                else
                    _headers[header] = new List<string> {value};
            }
            else
                _headers.Add(header, new List<string> {value});
        }

        public void AddAuthorizationHeader(string token, string authTypePrefix = "Bearer ")
        {
            AddHeader("Authorization", $"{authTypePrefix}{token}");
        }

        public T HttpRequest<T>(HttpMethod method, string relativeUrlPath = null)
        {
            return HttpRequestAsync<T>(method, relativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<T> HttpRequestAsync<T>(HttpMethod method, string relativePath = null)
        {
            switch (CurrentMode)
            {
                case RestMode.HTTPClient:
                case RestMode.HTTPRestClient:
                    return Task.Run(() =>
                        RestmeHttpExtensions.HttpRequestAsync<T>(this, method, relativePath)
                            .WaitAndGetResult(Configuration.DefaultTimeout));
                case RestMode.AzureStorageClient:
                case RestMode.RedisCacheClient:
                default:
                    throw new NotSupportedException(
                        "Generic request async method only supports HTTP requests, please use other extension methods or switch operation RestMode to HTTPClient");
            }
        }

        #region GET

        public T Get<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return GetAsync<T>(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<T> GetAsync<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            if (dataObject != null)
            {
                _objAsParam = dataObject;
            }

            var task = Task.Run(() =>
            {
                if (keyOrRelativeUrlPath.IsNotNullOrEmpty())
                {
                    switch (CurrentMode)
                    {
                        case RestMode.HTTPClient:
                        case RestMode.HTTPRestClient:
                            return this.HttpGetAsync<T>(keyOrRelativeUrlPath)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        case RestMode.AzureStorageClient:
                            return this.AzureStorageGetAsync<T>(keyOrRelativeUrlPath)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        case RestMode.RedisCacheClient:
                            return this.RedisGetAsync<T>(keyOrRelativeUrlPath)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        case RestMode.S3Client:
                            return this.S3GetAsync<T>(keyOrRelativeUrlPath)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        default:
                            throw new NotSupportedException(
                                "Generic request async method only supports HTTP requests, please use other extension methods or switch operation RestMode to HTTPClient");
                    }
                }

                throw new SyntaxErrorException("No key or relative url path provided.");
            });
            return task;
        }

        public string Get(string keyOrRelativePath = null, object dataObject = null)
        {
            return GetAsync(keyOrRelativePath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<string> GetAsync(string keyOrRelativePath = null, object dataObject = null)
        {
            return GetAsync<string>(keyOrRelativePath, dataObject);
        }

        #endregion

        #region PUT

        public T Put<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return PostAsync<T>(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<T> PutAsync<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            if (dataObject != null)
                _objAsParam = dataObject;
            var task = Task.Run<T>(() =>
            {
                switch (CurrentMode)
                {
                    case RestMode.HTTPClient:
                    case RestMode.HTTPRestClient:
                        return HttpRequestAsync<T>(HttpMethod.Put, keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.AzureStorageClient:
                        if (dataObject != null)
                            return this.AzureStoragePostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam.GetType() is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.AzureStoragePostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.RedisCacheClient:
                        if (dataObject != null)
                            return this.RedisPostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.RedisPostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.S3Client:
                        if (dataObject != null)
                            return this.S3PostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.S3PostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    default:
                        throw new NotSupportedException("Unexpected RestMode, let me call it a break!");
                }
            });

            return task;
        }

        public Task<string> PutAsync(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return PutAsync<string>(keyOrRelativeUrlPath, dataObject);
        }

        public string Put(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return PutAsync(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        #endregion

        #region DELETE

        public T Delete<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return DeleteAsync<T>(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<T> DeleteAsync<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            if (dataObject != null)
                _objAsParam = dataObject;
            var task = Task.Run(() =>
            {
                switch (CurrentMode)
                {
                    case RestMode.HTTPClient:
                    case RestMode.HTTPRestClient:
                        return HttpRequestAsync<T>(HttpMethod.Delete, keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.AzureStorageClient:
                        return this.AzureStorageDeleteAsync<T>(keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.RedisCacheClient:
                        return this.RedisDeleteAsync<T>(keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.S3Client:
                        return this.S3DeleteAsync<T>(keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    default:
                        throw new NotSupportedException("Unexpected RestMode, let me call it a break!");
                }
            });
            return task;
        }

        public Task<string> DeleteAsync(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return DeleteAsync<string>(keyOrRelativeUrlPath, dataObject);
        }

        public string Delete(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return DeleteAsync(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        #endregion

        #region POST

        public T Post<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            return PostAsync<T>(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        public Task<T> PostAsync<T>(string keyOrRelativeUrlPath = null, object dataObject = null)
        {
            var task = Task.Run<T>(() =>
            {
                switch (CurrentMode)
                {
                    case RestMode.HTTPClient:
                    case RestMode.HTTPRestClient:
                        if (dataObject != null)
                            _objAsParam = dataObject;
                        return HttpRequestAsync<T>(HttpMethod.Post, keyOrRelativeUrlPath)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.AzureStorageClient:
                        if (dataObject != null)
                            return this.AzureStoragePostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam.GetType() is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.AzureStoragePostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.RedisCacheClient:
                        if (dataObject != null)
                            return this.RedisPostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.RedisPostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    case RestMode.S3Client:
                        if (dataObject != null)
                            return this.S3PostAsync<T>(keyOrRelativeUrlPath, dataObject)
                                .WaitAndGetResult(Configuration.DefaultTimeout);
                        if (_objAsParam == null)
                        {
                            DeleteAsync<T>(keyOrRelativeUrlPath).WaitAndGetResult(Configuration.DefaultTimeout);
                            return default(T);
                        }
                        else if (_objAsParam is T)
                        {
                            dataObject = (T) Convert.ChangeType(_objAsParam, typeof(T));
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "A object parameter is detected, however it is not same generic type as the return type for the current call.");
                        }

                        return this.S3PostAsync<T>(keyOrRelativeUrlPath, dataObject)
                            .WaitAndGetResult(Configuration.DefaultTimeout);
                    default:
                        throw new NotSupportedException("Unexpected RestMode, let me call it a break!");
                }
            });

            return task;
        }

        public Task<string> PostAsync(string keyOrRelativeUrlPath = null, string dataObject = null)
        {
            return PostAsync<string>(keyOrRelativeUrlPath, dataObject);
        }

        public string Post(string keyOrRelativeUrlPath = null, string dataObject = null)
        {
            return PostAsync(keyOrRelativeUrlPath, dataObject).WaitAndGetResult(Configuration.DefaultTimeout);
        }

        #endregion


        #region Private Methods

        public void PrepareHeaders(HttpRequestHeaders headers)
        {
            if (!(_headers?.Count > 0)) return;

            foreach (var item in _headers)
            {
                try
                {
                    headers.TryAddWithoutValidation(item.Key, item.Value);
                }
                catch (Exception ex)
                {
                    //ignore
                }
            }
        }

        public string PrepareInjectParamsIntoQuery(string urlPath, bool convertObjectAsParam = true)
        {
            urlPath = urlPath ?? string.Empty;
            var nvc = urlPath.IdentifyQueryParams();
            if (_params?.Count > 0)
            {
                foreach (var k in _params.Keys)
                {
                    nvc.Add(k, _params[k]);
                }
            }

            if (convertObjectAsParam && _objAsParam != null)
            {
                var values = _objAsParam.GetType().GetProperties()
                    .Where(item => item.GetValue(_objAsParam, null) != null)
                    .Select(item =>
                        new KeyValuePair<string, string>(item.Name, item.GetValue(_objAsParam, null)?.ToString()));
                var keyValuePairs = values as KeyValuePair<string, string>[] ?? values.ToArray();
                if (keyValuePairs?.Count() > 0)
                {
                    foreach (var item in keyValuePairs)
                    {
                        if (!nvc.ContainsKey(item.Key))
                        {
                            nvc.Add(item.Key, HttpUtility.UrlEncode(item.Value));
                        }

                        //respect existing parameters so ignore the value from object
                    }
                }
            }

            var indexOfQuestionMark = urlPath.IndexOf('?');
            if (indexOfQuestionMark > 0)
                return urlPath.Substring(0, indexOfQuestionMark) + nvc.ParseIntoQueryString();
            return urlPath + nvc.ParseIntoQueryString();
        }

        #endregion


        public void Dispose()
        {
            var disposeTasks = new List<Task> {AttemptDisposeRedis()};

            Task.WaitAll(disposeTasks.ToArray());
        }
    }
}