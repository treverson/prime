﻿using System.Composition;
using Prime.Core;

namespace Prime.IPFS.Win32
{
    [Export(typeof(IExtensionExecute))]
    public class IpfsWin32Extension : IpfsExtension
    {
        public override Platform Platform => Platform.Win386;

        public override IpfsPlatformBase GetPlatformBase() => new IpfsWin32(this);
    }
}