using System;
using System.Net;
using System.Net.Http.Headers;

namespace Pranam.Restme.Utils
{
    public class HttpResponseMessage<T>
    {
        public T Data { get; set; }

        public HttpHeaders ResponseHeaders { get; set; }
        public HttpHeaders RequestHeaders { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime ReceivedOnUtc { get; set; }

        public Exception ErrorMessage { get; set; }
        public string DataInString { get; set; }
    }
}