﻿using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Plugins.Services.Braziliex;
using Xunit;

namespace Prime.Tests.Providers
{
    public class BraziliexTests : ProviderDirectTestsBase
    {
        public BraziliexTests()
        {
            Provider = Networks.I.Providers.OfType<BraziliexProvider>().FirstProvider();
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
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw()
            };

            base.PretestGetPricing(pairs, true, true);
        }

        [Fact]
        public override void TestGetAssetPairs()
        {
            var requiredPairs = new AssetPairs()
            {
                "ltc_btc".ToAssetPairRaw(),
                "eth_btc".ToAssetPairRaw()
            };

            base.PretestGetAssetPairs(requiredPairs);
        }


        [Fact]
        public override void TestGetOrderBook()
        {
            base.PretestGetOrderBook("ltc_btc".ToAssetPairRaw(), true);
        }
    }
}