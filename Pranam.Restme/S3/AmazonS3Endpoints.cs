using System.Collections.Generic;

namespace Pranam.Data
{
    public static class AmazonS3Endpoints
    {
        private static string[] amazonS3Endpoints =
        {
            "s3.amazonaws.com",
            "s3.us-east-2.amazonaws.com",
            "s3-fips.us-east-2.amazonaws.com",
            "s3.dualstack.us-east-2.amazonaws.com",
            "s3-fips.dualstack.us-east-2.amazonaws.com",
            "s3.us-east-1.amazonaws.com",
            "s3-fips.us-east-1.amazonaws.com",
            "s3.amazonaws.com",
            "s3.dualstack.us-east-1.amazonaws.com",
            "s3-fips.dualstack.us-east-1.amazonaws.com",
            "s3.us-west-1.amazonaws.com",
            "s3-fips.us-west-1.amazonaws.com",
            "s3.dualstack.us-west-1.amazonaws.com",
            "s3-fips.dualstack.us-west-1.amazonaws.com",
            "s3.us-west-2.amazonaws.com",
            "s3-fips.us-west-2.amazonaws.com",
            "s3.dualstack.us-west-2.amazonaws.com",
            "s3-fips.dualstack.us-west-2.amazonaws.com",
            "s3.af-south-1.amazonaws.com",
            "s3.dualstack.af-south-1.amazonaws.com",
            "s3.ap-east-1.amazonaws.com",
            "s3.dualstack.ap-east-1.amazonaws.com",
            "s3.ap-south-1.amazonaws.com",
            "s3.dualstack.ap-south-1.amazonaws.com",
            "s3.ap-northeast-3.amazonaws.com",
            "s3.dualstack.ap-northeast-3.amazonaws.com",
            "s3.ap-northeast-2.amazonaws.com",
            "s3.dualstack.ap-northeast-2.amazonaws.com",
            "s3.ap-southeast-1.amazonaws.com",
            "s3.dualstack.ap-southeast-1.amazonaws.com",
            "s3.ap-southeast-2.amazonaws.com",
            "s3.dualstack.ap-southeast-2.amazonaws.com",
            "s3.ap-northeast-1.amazonaws.com",
            "s3.dualstack.ap-northeast-1.amazonaws.com",
            "s3.ca-central-1.amazonaws.com",
            "s3-fips.ca-central-1.amazonaws.com",
            "s3.dualstack.ca-central-1.amazonaws.com",
            "s3-fips.dualstack.ca-central-1.amazonaws.com",
            "s3.cn-north-1.amazonaws.com.cn",
            "s3.dualstack.cn-north-1.amazonaws.com.cn",
            "s3.cn-northwest-1.amazonaws.com.cn",
            "s3.dualstack.cn-northwest-1.amazonaws.com.cn",
            "s3.eu-central-1.amazonaws.com",
            "s3.dualstack.eu-central-1.amazonaws.com",
            "s3.eu-west-1.amazonaws.com",
            "s3.dualstack.eu-west-1.amazonaws.com",
            "s3.eu-west-2.amazonaws.com",
            "s3.dualstack.eu-west-2.amazonaws.com",
            "s3.eu-south-1.amazonaws.com",
            "s3.dualstack.eu-south-1.amazonaws.com",
            "s3.eu-west-3.amazonaws.com",
            "s3.dualstack.eu-west-3.amazonaws.com",
            "s3.eu-north-1.amazonaws.com",
            "s3.dualstack.eu-north-1.amazonaws.com",
            "s3.sa-east-1.amazonaws.com",
            "s3.dualstack.sa-east-1.amazonaws.com",
            "s3.me-south-1.amazonaws.com",
            "s3.dualstack.me-south-1.amazonaws.com",
            "s3.us-gov-east-1.amazonaws.com",
            "s3-fips.us-gov-east-1.amazonaws.com",
            "s3.dualstack.us-gov-east-1.amazonaws.com",
            "s3-fips.dualstack.us-gov-east-1.amazonaws.com",
            "s3.us-gov-west-1.amazonaws.com",
            "s3-fips.us-gov-west-1.amazonaws.com",
            "s3.dualstack.us-gov-west-1.amazonaws.com",
            "s3-fips.dualstack.us-gov-west-1.amazonaws.com"
        };

        public static List<string> AddAmazonEndpoints(this List<string> list)
        {
            if (!(list?.Count >= 0))
            {
                list = new List<string>();
            }

            list.AddRange(amazonS3Endpoints);
            return list;
        }
    }
}