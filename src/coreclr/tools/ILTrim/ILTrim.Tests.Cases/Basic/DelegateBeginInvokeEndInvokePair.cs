﻿using System;
using System.Collections.Generic;
using System.Text;
using Mono.Linker.Tests.Cases.Expectations.Assertions;

namespace Mono.Linker.Tests.Cases.Basic
{
    [IgnoreTestCase("Support for delegates is not implemented yet")]
    class DelegateBeginInvokeEndInvokePair
    {
        public static void Main()
        {
            SomeDelegate d1 = Method;
            d1.BeginInvoke(null, null);

            OtherDelegate d2 = Method;
            d2.EndInvoke(null);

            StrippedDelegate d3 = Method;
            d3.DynamicInvoke(null);
        }

        [Kept]
        static void Method()
        {
        }

        [Kept]
        [KeptBaseType(typeof(MulticastDelegate))]
        [KeptMember(".ctor(System.Object,System.IntPtr)")]
        [KeptMember("Invoke()")]
        [KeptMember("BeginInvoke(System.AsyncCallback,System.Object)")]
        [KeptMember("EndInvoke(System.IAsyncResult)")]
        public delegate void SomeDelegate();

        [Kept]
        [KeptBaseType(typeof(MulticastDelegate))]
        [KeptMember(".ctor(System.Object,System.IntPtr)")]
        [KeptMember("Invoke()")]
        [KeptMember("BeginInvoke(System.AsyncCallback,System.Object)")]
        [KeptMember("EndInvoke(System.IAsyncResult)")]
        public delegate void OtherDelegate();

        [Kept]
        [KeptBaseType(typeof(MulticastDelegate))]
        [KeptMember(".ctor(System.Object,System.IntPtr)")]
        [KeptMember("Invoke()")]
        public delegate void StrippedDelegate();
    }
}
