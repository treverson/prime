﻿using Prime.Utility;

namespace Prime.Core
{
    public class OhlcContext : NetworkProviderContext
    {
        public readonly AssetPair Pair;
        public readonly TimeResolution Market;
        public readonly TimeRange Range;

        public OhlcContext(AssetPair pair, TimeResolution market, TimeRange range, Logger logger) : base(logger)
        {
            Pair = pair;
            Market = market;
            Range = range;
        }
    }
}