﻿using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Exx;
using Xunit;

namespace Prime.Tests.Providers
{
    public class ExxTests : ProviderDirectTestsBase
    {
        public ExxTests()
        {
            Provider = Networks.I.Providers.OfType<ExxProvider>().FirstProvider();
        }

        [Fact]
        public override void TestApiPublic()
        {
            base.TestApiPublic();
        }

        [Fact]
        public override void TestGetPricing()
        {
            var pairs = new List<AssetPair>()
            {
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, false);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "bts_eth".ToAssetPairRaw(),
                "BTM_ETH".ToAssetPairRaw(),
                "EOS_BTC".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }

        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("EOS_BTC".ToAssetPairRaw(), true);
        }
    }
}