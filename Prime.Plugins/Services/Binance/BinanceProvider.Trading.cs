﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;
using RestEase;

namespace Prime.Plugins.Services.Binance
{
    public partial class BinanceProvider : IBalanceProvider, IOrderLimitProvider
    {
        private void CheckResponseErrors<T>(Response<T> r, [CallerMemberName] string method = "Unknown")
        {
            if (r.ResponseMessage.IsSuccessStatusCode) return;

            if (r.GetContent() is BinanceSchema.ErrorResponseBase rError && !string.IsNullOrWhiteSpace(rError.msg))
                throw new ApiResponseException($"{rError.msg} ({rError.code})", this, method);

            throw new ApiResponseException(r.ResponseMessage.ReasonPhrase, this, method);
        }

        public async Task<BalanceResults> GetBalancesAsync(NetworkProviderPrivateContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.GetAccountInformationAsync().ConfigureAwait(false);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var balances = new BalanceResults(this);

            foreach (var b in r.balances)
            {
                var asset = b.asset.ToAsset(this);
                balances.Add(asset, b.free, b.locked);
            }

            return balances;
        }

        public async Task<PlacedOrderLimitResponse> PlaceOrderLimitAsync(PlaceOrderLimitContext context)
        {
            var api = ApiProvider.GetApi(context);
            var rRaw = await api.NewOrderAsync(context.Pair.ToTicker(this), context.IsBuy ? "BUY" : "SELL", "LIMIT", "GTC",
                context.Quantity, context.Rate);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            return new PlacedOrderLimitResponse(r.orderId.ToString());
        }

        public async Task<TradeOrderStatus> GetOrderStatusAsync(RemoteIdContext context)
        {
            var api = ApiProvider.GetApi(context);
            
            if(!context.HasMarket)
                throw new ApiResponseException("Market should be specified when querying order status", this);

            if(!long.TryParse(context.RemoteGroupId, out var orderId))
                throw new ApiResponseException("Incorrect order ID specified", this);

            var rRaw = await api.QueryOrderAsync(context.Market.ToTicker(this), orderId);
            CheckResponseErrors(rRaw);

            var r = rRaw.GetContent();

            var isCancelRequested = r.status.Equals("pending_cancel", StringComparison.OrdinalIgnoreCase);
            var isOpen = r.status.Equals("new", StringComparison.OrdinalIgnoreCase);
            
            return new TradeOrderStatus(r.orderId.ToString(), isOpen, isCancelRequested)
            {
                Rate = r.price,
                AmountInitial = new Money(r.origQty, context.Market.Asset1),
                AmountRemaining = new Money(r.origQty - r.executedQty, context.Market.Asset1),
            };
        }

        public MinimumTradeVolume[] MinimumTradeVolume => throw new NotImplementedException();
    }
}