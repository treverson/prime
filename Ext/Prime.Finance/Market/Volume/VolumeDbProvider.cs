﻿using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Nito.AsyncEx;
using Prime.Base;
using Prime.Core;

namespace Prime.Finance.Market
{
    public class VolumeDbProvider
    {
        public static VolumeDbProvider I => Lazy.Value;
        private static readonly Lazy<VolumeDbProvider> Lazy = new Lazy<VolumeDbProvider>(() => new VolumeDbProvider("VOLUMEDATA_I".GetObjectIdHashCode()));
        private readonly object _lock = new object();
        public readonly bool CanSave;

        public readonly IAggVolumeDataProvider ProviderAggVolumeData;
        public readonly IReadOnlyList<IPublicVolumeProvider> ProvidersPublicVolume;
        public readonly IReadOnlyList<IPublicPricingProvider> ProvidersPublicPricing;

        public readonly VolumeData Data;

        public VolumeDbProvider()
        {
            ProviderAggVolumeData = Networks.I.Providers.OfType<IAggVolumeDataProvider>().FirstOrDefault();
            ProvidersPublicVolume = Networks.I.Providers.OfType<IPublicVolumeProvider>().Where(x => x.IsDirect).ToList();
            ProvidersPublicPricing = Networks.I.Providers.OfType<IPublicPricingProvider>().Where(x=>x.IsDirect && x.PricingFeatures.HasVolume).ToList();

            Data = new VolumeData();
        }

        public VolumeDbProvider(ObjectId dataId) : this()
        {
            //Data = PublicFast.GetCreate(dataId, () => new VolumeData());
            CanSave = true;
        }

        public IReadOnlyList<NetworkPairVolume> GetVolume(Network network, bool dontSave = false)
        {
            lock (_lock)
            {
                var v = Data.GetNormalised(network);
                if (v != null)
                    return v;

                var save = Data.PopulateFromApi(network);

                v = Data.GetNormalised(network);

                if (save && CanSave)
                    Data.SavePublic();
                return v;
            }
        }

        public NetworkPairVolume GetVolume(Network network, AssetPair pair, bool dontSave = false)
        {
            lock (_lock)
            {
                var v = Data.GetNormalised(network, pair);
                if (v != null)
                    return v;

                var canagg = ProviderAggVolumeData.NetworksSupported.Contains(network);

                var save = canagg && Data.PopulateFromAgg(ProviderAggVolumeData, pair); //the agg does bulk, so it goes up the list.
                v = canagg ? Data.GetNormalised(network, pair) : null;
                
                if (v != null)
                    return PostProcess(v, save && !dontSave);

                save = Data.PopulateFromApi(network) || save;

                v = Data.GetNormalised(network, pair);

                return v != null ? PostProcess(v, save && !dontSave) : v;
            }
        }

        private NetworkPairVolume PostProcess(NetworkPairVolume vol, bool mustSave)
        {
            if (mustSave && CanSave)
                Data.SavePublic();
            return vol;
        }
        
        public PublicVolumeResponse GetAllVolume(IReadOnlyDictionary<Network, IReadOnlyList<AssetPair>> pairsByNetwork, Action<Network, AssetPair> onPull = null, Action<Network, AssetPair, NetworkPairVolume> afterPull = null)
        {
            lock (_lock)
            {
                var volume = new UniqueList<NetworkPairVolume>();
                var missing = new Dictionary<Network, UniqueList<AssetPair>>();
                foreach (var network in pairsByNetwork.Keys)
                {
                    var pairs = pairsByNetwork[network];

                    foreach (var pair in pairs)
                    {
                        onPull?.Invoke(network, pair);
                        var r = GetVolume(network, pair, true);

                        if (r != null && Equals(r.Pair, pair.Reversed))
                            r = r.Reversed;

                        if (r == null)
                            missing.GetOrAdd(network, (k) => new UniqueList<AssetPair>()).Add(pair);
                        else
                            volume.Add(r);
                        afterPull?.Invoke(network, pair, r);
                    }
                }

                if (CanSave)
                    Data.SavePublic();

                return new PublicVolumeResponse(volume, missing);
            }
        }

        public static void Clear(ObjectId id)
        {
            //PublicFast.Delete<VolumeData>(id);
        }
    }
}