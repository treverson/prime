﻿using System;
using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Whaleclub;
using Xunit;

namespace Prime.Tests.Providers
{
    /// <author email="yasko.alexander@gmail.com">Alexander Yasko</author>
    public class WhaleclubTests : ProviderDirectTestsBase
    {
        public WhaleclubTests()
        {
            Provider = Networks.I.Providers.OfType<WhaleclubProvider>().FirstProvider();
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "BTC_USD".ToAssetPairRaw(),
                "DASH_USD".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Obsolete("Public methods require key.")]
        [Fact]
        public override void TestGetAssetPairs()
        { 
            var requiredPairs = new AssetPairs()
            {
                "BTC_USD".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }
    }
}