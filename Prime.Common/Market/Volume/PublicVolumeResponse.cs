﻿using System.Collections.Generic;
using System.Linq;
using Prime.Common;
using Prime.Utility;

namespace Prime.Common
{
    public class PublicVolumeResponse
    {
        public readonly IReadOnlyList<NetworkPairVolume> Volume;
        public readonly IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> Missing;

        public PublicVolumeResponse(NetworkPairVolume volume)
        {
            Volume = new UniqueList<NetworkPairVolume>() { volume };
            Missing = new Dictionary<Network, IReadOnlyList<AssetPair>>();
        }

        public PublicVolumeResponse(Network network, AssetPair pair, decimal volume24) : this(new NetworkPairVolume(network, pair, volume24)) { }

        public PublicVolumeResponse(Network network, AssetPair pair, decimal? vol24Base, decimal? vol24Quote = null) : this(new NetworkPairVolume(network, pair, vol24Base, vol24Quote)) { }

        public PublicVolumeResponse(IEnumerable<NetworkPairVolume> volume, Network network, IEnumerable<AssetPair> missing)
        {
            Volume = volume.ToUniqueList();
            Missing = new Dictionary<Network, IReadOnlyList<AssetPair>> {{network, missing.ToUniqueList()}};
        }

        public PublicVolumeResponse(UniqueList<NetworkPairVolume> volume, Dictionary<Network, UniqueList<AssetPair>> missing)
        {
            Volume = volume;
            Missing = missing.ToDictionary(x => x.Key, y => y.Value.AsReadOnlyList());
        }

        public NetworkPairVolume FirstOrDefault()
        {
            return Volume.FirstOrDefault();
        }
    }
}