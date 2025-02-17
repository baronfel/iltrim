﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace Mono.Linker.Tests.TestCasesRunner
{
    public class TrimmerOptions
    {
        public List<string> AdditionalLinkAssemblies { get; set; } = new();
        public string? InputPath { get; set; }
        public string? OutputDirectory { get; set; }
        public List<string> ReferencePaths { get; set; } = new List<string> ();
        public bool IsLibraryMode { get; set; } = false;
        public Dictionary<string, bool> FeatureSwitches { get; set; } = new Dictionary<string, bool>();
    }
}
