﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Common;

namespace Prime.Ui.Wpf
{
    public class OhlcDataUpdatedEvent : EventArgs
    {
        public readonly OhlcData NewData;
        public readonly Asset Asset;
        public bool IsLive;

        public OhlcDataUpdatedEvent(OhlcData newData, Asset asset, bool isLive)
        {
            NewData = newData;
            Asset = asset;
            IsLive = isLive;
        }
    }
}
