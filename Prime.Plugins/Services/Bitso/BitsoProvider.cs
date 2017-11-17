﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using Prime.Common;
using Prime.Utility;

namespace Prime.Plugins.Services.Bitso
{
    // https://bitso.com/api_info
    public class BitsoProvider : IPublicPricingProvider, IAssetPairsProvider
    {
        private const string BitsoApiVersion = "v3";
        private const string BitsoApiUrl = "https://api.bitso.com/" + BitsoApiVersion + "/";

        private static readonly ObjectId IdHash = "prime:bitso".GetObjectIdHashCode();

        // Rate limits are are based on one minute windows. 
        // If you do more than 300 requests in five minutes, you get locked out for one minute. 
        private static readonly IRateLimiter Limiter = new PerMinuteRateLimiter(300, 5);

        private RestApiClientProvider<IBitsoApi> ApiProvider { get; }

        public Network Network { get; } = Networks.I.Get("Bitso");

        public bool Disabled => false;
        public int Priority => 100;
        public string AggregatorName => null;
        public string Title => Network.Name;
        public ObjectId Id => IdHash;
        public IRateLimiter RateLimiter => Limiter;

        public bool IsDirect => true;

        public bool CanGenerateDepositAddress => true; // TODO: confirm
        public bool CanPeekDepositAddress => false; // TODO: confirm
        public ApiConfiguration GetApiConfiguration => ApiConfiguration.Standard2;

        public BitsoProvider()
        {
            ApiProvider = new RestApiClientProvider<IBitsoApi>(BitsoApiUrl, this, (k) => null);
        }

        public async Task<bool> TestPublicApiAsync(NetworkProviderContext context)
        {
            var r = await GetAssetPairsAsync(context).ConfigureAwait(false);

            return r.Count > 0;
        }

        private static readonly PricingFeatures StaticPricingFeatures = new PricingFeatures(true, false);
        public PricingFeatures PricingFeatures => StaticPricingFeatures;

        public async Task<MarketPricesResult> GetPricesAsync(PublicPricesContext context)
        {
            var api = ApiProvider.GetApi(context);
            var pairCode = context.Pair.ToTicker(this, "_").ToLower();
            var r = await api.GetTickerAsync(pairCode).ConfigureAwait(false);

            CheckResponseErrors(r);

            var price = new MarketPrice(Network, context.Pair.Asset1, new Money(r.payload.last, context.Pair.Asset2))
            {
                PriceStatistics = new PriceStatistics(Network, context.Pair.Asset2, r.payload.ask, r.payload.bid, r.payload.low, r.payload.high),
                Volume = new NetworkPairVolume(Network, context.Pair, null, r.payload.volume)
            };
            return new MarketPricesResult(price);
        }

        private void CheckResponseErrors<T>(BitsoSchema.BaseResponse<T> response)
        {
            if(!response.success)
                throw new ApiResponseException("Error occurred", this);
        }

        public async Task<AssetPairs> GetAssetPairsAsync(NetworkProviderContext context)
        {
            var api = ApiProvider.GetApi(context);

            var r = await api.GetAssetPairsAsync().ConfigureAwait(false);

            var pairs = new AssetPairs();

            foreach (var rCurrentPayloadResponse in r.payload)
            {
                pairs.Add(rCurrentPayloadResponse.book.ToAssetPairRaw());
            }

            return pairs;
        }

        public IAssetCodeConverter GetAssetCodeConverter()
        {
            return null;
        }

        public bool DoesMultiplePairs => false; // TODO: confirm

        public bool PricesAsAssetQuotes => false; // TODO: confirm
    }
}
