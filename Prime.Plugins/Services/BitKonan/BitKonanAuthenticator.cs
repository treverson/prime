﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.BitKonan
{
    public class BitKonanAuthenticator : BaseAuthenticator
    {

        public BitKonanAuthenticator(ApiKey apiKey) : base(apiKey)
        {
        }

        public override void RequestModify(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var timeStamp = (long)(DateTime.UtcNow.ToUnixTimeStamp());

            string strForSign = $"{ApiKey.Key}{timeStamp}";
            var signature = HashHMACSHA256Hex(strForSign, ApiKey.Secret);

            var parameters = request.Content?.ReadAsStringAsync()?.Result;

            string jsonParams = "";

            if (parameters != null)
            {
                jsonParams = parameters.Replace("=", "\":\"").Replace("&", "\",\"");
            }

            var contentBuilder = new StringBuilder("{\"key\":\"" + ApiKey.Key + "\",\"sign\":\"" + signature + "\", \"timestamp\":\"" + timeStamp + "\"");

            if (string.IsNullOrWhiteSpace(jsonParams))
            {
                contentBuilder.Append("}");
            }
            else
            {
                contentBuilder.Append(", \"");
                contentBuilder.Append(jsonParams);
                contentBuilder.Append("\"}");
                contentBuilder.Replace("%2F","/");
            }

            request.Content = new StringContent(contentBuilder.ToString(), Encoding.UTF8, "application/json");
        }
    }
}