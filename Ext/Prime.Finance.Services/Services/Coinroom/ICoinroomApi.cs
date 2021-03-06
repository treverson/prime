﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Prime.Finance.Services.Services.Coinroom
{
    internal interface ICoinroomApi
    {
        [Get("/ticker/{currencyPair}")]
        Task<CoinroomSchema.TickerResponse> GetTickerAsync([Path(UrlEncode = false)] string currencyPair);

        [Get("/availableCurrencies")]
        Task<CoinroomSchema.CurrenciesResponse> GetCurrenciesAsync();

        [Post("/orderbook")]
        Task<CoinroomSchema.OrderBookResponse> GetOrderBookAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> body);
    }
}
