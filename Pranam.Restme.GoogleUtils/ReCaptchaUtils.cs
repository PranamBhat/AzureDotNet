using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pranam.Restme.GoogleUtils.Models;

namespace Pranam.Restme.GoogleUtils
{
    public class ReCaptchaUtils
    {
        public const string GoogleSiteVerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        public const string GoogleResponseFieldName = "g-recaptcha-response";

        public static async Task<bool> VerifyRecaptchaResponse(string secretKey, string response,
            string remoteIp = null)
        {
            if (secretKey.IsNotNullOrEmpty() && response.IsNotNullOrEmpty())
            {
                using (var rest = new Rest(GoogleSiteVerifyUrl))
                {
                    rest.Add("secret", secretKey);
                    rest.Add("response", response);
                    if (remoteIp.IsNotNullOrEmpty())
                    {
                        rest.Add("remoteip", remoteIp);
                    }

                    var result = await rest.PostAsync<ReCaptchaVerificationResult>();
                    if (result?.ErrorCodes?.Length > 0)
                    {
                        foreach (var error in result.ErrorCodes)
                        {
                            switch (error)
                            {
                                case "missing-input-secret":
                                    rest.LogError("The secret parameter is missing.");
                                    break;
                                case "invalid-input-secret":
                                    rest.LogError(" The secret parameter is invalid or malformed.");
                                    break;
                                case "missing-input-response":
                                    rest.LogError("The response parameter is missing.");
                                    break;
                                case "invalid-input-response":
                                    rest.LogError("The response parameter is invalid or malformed.");
                                    break;
                                case "bad-request":
                                    rest.LogError("The request is invalid or malformed.");
                                    break;
                            }
                        }
                    }

                    return result?.Success == true;
                }
            }

            return false;
        }

        public static Task<bool> VerifyRecaptchaResponse(string secretKey, HttpRequest request)
        {
            var gResponse = request.HasFormContentType && request.Form.ContainsKey(GoogleResponseFieldName)
                ? request.Form[GoogleResponseFieldName].ToString()
                : (request.Query.ContainsKey(GoogleResponseFieldName)
                    ? request.Query[GoogleResponseFieldName].ToString()
                    : string.Empty);
            var remoteIp = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            return VerifyRecaptchaResponse(secretKey, gResponse, remoteIp);
        }
    }
}