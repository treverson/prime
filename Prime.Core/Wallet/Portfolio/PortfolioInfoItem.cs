using System;
using System.Collections.Generic;
using Prime.Utility;

namespace Prime.Core.Wallet
{
    public class PortfolioInfoItem : IEquatable<PortfolioInfoItem>
    {
        public PortfolioInfoItem(Network network)
        {
            Network = network;
        }

        public Network Network { get; }

        public UniqueList<Asset> Assets { get; set; } = new UniqueList<Asset>();

        public decimal Percentage { get; set; }

        public bool IsQuerying { get; set; }

        public bool IsFailed { get; set; }

        public bool IsConnected { get; set; }

        public DateTime UtcLastConnect { get; set; }

        public Money TotalConvertedAssetValue { get; set; }

        public string ConnectReport => IsConnected ? "Connected" : "Disconnected";

        public string StateReport => IsFailed ? "Failed" : (IsQuerying ? "Querying" : "Idle");

        public bool Equals(PortfolioInfoItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Network, other.Network);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PortfolioInfoItem) obj);
        }

        public override int GetHashCode()
        {
            return (Network != null ? Network.GetHashCode() : 0);
        }
    }
}