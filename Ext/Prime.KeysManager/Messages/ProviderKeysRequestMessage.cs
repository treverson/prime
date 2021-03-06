﻿using Prime.Core;

namespace Prime.KeysManager.Messages
{
    public class ProviderKeysRequestMessage : BaseTransportRequestMessage
    {
        public string Id { get; set; }
        
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Extra { get; set; }
    }
}