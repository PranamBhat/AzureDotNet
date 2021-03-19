using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pranam.Restme.Utils;

namespace Pranam
{
    public static class RestmeHttpExtensions
    {
        public static T HttpRequest<T>(this Rest restme, HttpMethod method, string relativeUrlPath = null)
            where T : class
        {
            return HttpRequestAsync<T>(restme, method, relativeUrlPath)
                .WaitAndGetResult(restme.Configuration.DefaultTimeout);
        }

        public static HttpResponseMessage<T> HttpRequestFull<T>(this Rest restme, HttpMethod method,
            string relativePath = null)
        {
            return HttpRequestFullAsync<T>(restme, method, relativePath)
                .WaitAndGetResult(restme.Configuration.DefaultTimeout);
        }

        public static async Task<HttpResponseMessage<T>> HttpRequestFullAsync<T>(this Rest restme, HttpMethod method,
            string relativePath = null)
        {
            using (var httpClient = new HttpClient {BaseAddress = restme.BaseUri})
            {
                restme.PrepareHeaders(httpClient.DefaultRequestHeaders);
                HttpResponseMessage response = null;
                ByteArrayContent submitContent = null;
                if (restme.CurrentMode == RestMode.HTTPClient)
                {
                    if (restme._params?.Count > 0)
                        submitContent = new PranamFormUrlEncodedContent(restme._params);
                    else if (restme?._objAsParam != null)
                    {
                        submitContent = new StringContent(restme._objAsParam.JsonSerialize());
                        submitContent.Headers.ContentType =
                            new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    }
                }
                else
                {
                    if (restme._params?.Count > 0)
                        submitContent = new PranamRestfulHttpContent(restme._params);
                    else if (restme._objAsParam != null)
                    {
                        submitContent = new StringContent(restme._objAsParam.JsonSerialize());
                    }
                    else
                        submitContent = new StringContent(string.Empty);

                    submitContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                }

                if (method == HttpMethod.Post)
                {
                    response =
                        await
                            httpClient.PostAsync(new Uri(restme.BaseUri, relativePath),
                                submitContent);
                }
                else if (method == HttpMethod.Put)
                {
                    response =
                        await
                            httpClient.PutAsync(new Uri(restme.BaseUri, relativePath),
                                submitContent);
                }
                else if (method == HttpMethod.Get)
                {
                    response =
                        await
                            httpClient.GetAsync(new Uri(restme.BaseUri,
                                restme.PrepareInjectParamsIntoQuery(relativePath)));
                }
                else if (method == HttpMethod.Delete)
                {
                    response =
                        await
                            httpClient.DeleteAsync(new Uri(restme.BaseUri,
                                restme.PrepareInjectParamsIntoQuery(relativePath)));
                }

                if (response == null) return default;

                var result = new HttpResponseMessage<T>();
                result.ResponseHeaders = response.Headers;
                result.RequestHeaders = httpClient.DefaultRequestHeaders;
                result.StatusCode = response.StatusCode;
                result.ReceivedOnUtc = DateTime.UtcNow;


                try
                {
                    result.DataInString = await response.Content.ReadAsStringAsync();
                }
                catch
                {
                    // ignore
                }

                if (typeof(T).IsSubclassOf(typeof(Stream)))
                {
                    var content = await response.Content.ReadAsStreamAsync();
                    try
                    {
                        result.Data = (T) Convert.ChangeType(content, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        restme.LogError(ex.Message, ex);
                        result.ErrorMessage = ex;
                        result.Data = default;
                    }
                }
                else
                {
                    var content = result.DataInString;

                    try
                    {
                        if (typeof(T).IsPrimitiveType())
                        {
                            result.Data = (T) Convert.ChangeType(content, typeof(T));
                        }
                        else
                        {
                            result.Data = content.JsonDeserialize<T>(
                                restme.Configuration.UseRestConvertForCollectionSerialization,
                                restme.Configuration.SerializerSettings);
                        }
                    }
                    catch (Exception ex)
                    {
                        restme?.LogError(ex.Message, ex);
                        result.ErrorMessage = ex;
                        result.Data = default;
                    }
                }

                return result;
            }
        }


        public static async Task<T> HttpRequestAsync<T>(this Rest restme, HttpMethod method, string relativePath = null)
        {
            var result = await HttpRequestFullAsync<T>(restme, method, relativePath);
            return result != null ? result.Data : default;
        }

        public static T HttpGet<T>(this Rest restme, string relativeUrlPath = null)
        {
            return restme.HttpGetAsync<T>(relativeUrlPath).WaitAndGetResult(restme.Configuration.DefaultTimeout);
        }

        public static Task<T> HttpGetAsync<T>(this Rest restme, string relativeUrlPath = null)
        {
            return restme.HttpRequestAsync<T>(HttpMethod.Get, relativeUrlPath);
        }

        public static T HttpPost<T>(this Rest restme, string relativeUrlPath = null)
        {
            return restme.HttpPostAsync<T>(relativeUrlPath).WaitAndGetResult(restme.Configuration.DefaultTimeout);
        }

        public static Task<T> HttpPostAsync<T>(this Rest restme, string relativeUrlPath = null)
        {
            return restme.HttpRequestAsync<T>(HttpMethod.Post, relativeUrlPath);
        }
    }
}