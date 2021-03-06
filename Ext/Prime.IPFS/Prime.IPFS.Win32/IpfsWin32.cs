﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prime.Core;

namespace Prime.IPFS.Win32
{
    public class IpfsWin32 : IpfsWin
    {
        public IpfsWin32(IExtension instance) : base(instance) {}

        public override string PackageInstallName => "go-ipfs_v0.4.14_windows-386.zip";
    }
}
