using System;
using Newtonsoft.Json;

namespace Pranam.Restme.GoogleUtils.Models
{
    public class ReCaptchaVerificationResult
    {
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        public string Hostname { get; set; }

        [JsonProperty(PropertyName = "error-codes")]
        public string[] ErrorCodes { get; set; }

        [JsonProperty(PropertyName = "apk_package_name")]
        public string ApkPackageName { get; set; }
    }
}