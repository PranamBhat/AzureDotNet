using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pranam.Restme.Utils;

namespace Pranam
{
    public interface IRestme
    {
        Uri BaseUri { get; set; }
        string RequestUrlPath { get; set; }

        void Add(string key, string value);
        void Add(string key, object value);
        void Add(object value);

        void AddHeader(string header, string value, bool allowMultipleValues = false);
        void AddAuthorizationHeader(string token, string authTypePrefix = "Bearer ");


        T HttpRequest<T>(HttpMethod method, string keyOrRelativePath = null);
        Task<T> HttpRequestAsync<T>(HttpMethod method, string keyOrRelativePath = null);

        HttpResponseMessage<T> HttpRequestFull<T>(HttpMethod method, string keyOrRelativePath = null,
            object dataObject = null);

        Task<HttpResponseMessage<T>> HttpRequestFullAsync<T>(HttpMethod method, string keyOrRelativePath = null,
            object dataObject = null);

        T Get<T>(string keyOrRelativePath = null, object dataObject = null);
        Task<T> GetAsync<T>(string keyOrRelativePath = null, object dataObject = null);
        string Get(string keyOrRelativePath = null, object dataObject = null);
        Task<string> GetAsync(string keyOrRelativePath = null, object dataObject = null);

        T Put<T>(string keyOrRelativePath = null, object dataObject = null);
        Task<T> PutAsync<T>(string keyOrRelativePath = null, object dataObject = null);
        string Put(string keyOrRelativePath = null, object dataObject = null);
        Task<string> PutAsync(string keyOrRelativePath = null, object dataObject = null);


        T Post<T>(string keyOrRelativePath = null, object dataObject = null);
        Task<T> PostAsync<T>(string keyOrRelativePath = null, object dataObject = null);
        string Post(string keyOrRelativePath = null, string dataValue = null);
        Task<string> PostAsync(string keyOrRelativePath = null, string dataValue = null);

        T Delete<T>(string keyOrRelativePath = null, object dataObject = null);
        Task<T> DeleteAsync<T>(string keyOrRelativePath = null, object dataObject = null);
        string Delete(string keyOrRelativePath = null, object dataObject = null);
        Task<string> DeleteAsync(string keyOrRelativePath = null, object dataObject = null);
    }
}