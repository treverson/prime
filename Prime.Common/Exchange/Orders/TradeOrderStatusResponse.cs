﻿namespace Prime.Common
{
    public class TradeOrderStatusResponse : ResponseModelBase
    {
        public TradeOrderStatus TradeOrderStatus { get; }

        public TradeOrderStatusResponse(TradeOrderStatus tradeOrderStatus)
        {
            TradeOrderStatus = tradeOrderStatus;
        }

        public TradeOrderStatusResponse(string remoteOrderId, bool isBuy, bool isOpen, bool isCancelRequested)
        {
            TradeOrderStatus = new TradeOrderStatus(remoteOrderId, isBuy, isOpen, isCancelRequested);
        }
    }
}