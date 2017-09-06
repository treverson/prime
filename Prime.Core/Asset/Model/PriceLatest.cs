﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prime.Core
{
    public class PriceLatest
    {
        [Bson]
        public DateTime UtcCreated { get; set; }

        [Bson]
        public Asset Asset { get; set; }

        [Bson]
        public List<Money> Prices { get; set; } = new List<Money>();
    }
}
