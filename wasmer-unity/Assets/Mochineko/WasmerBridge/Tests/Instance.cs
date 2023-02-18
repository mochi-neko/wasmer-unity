using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class InstanceTest
    {
        [Test, RequiresPlayMode(false)]
        [Ignore("Not implemented")]
        public void InstantiateTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            ByteVector.New(MockResource.EmptyWasmBinary, out var wasm);
            using (wasm)
            {
                using var module = Module.New(store, "empty", in wasm);
                //using var instance = Instance.New(store, module,);
            }
        }
    }
    
    [OwnPointed]
    public sealed class Instance : IDisposable
    {
        internal static unsafe Instance New(Store store, Module module, in ExternalVector imports, in Trap trap)
        {
            //return new Instance(WasmAPIs.wasm_instance_new(store.Handle, module.Handle, in imports, &trap);
            throw new NotImplementedException();
        }

        private Instance(IntPtr handle)
        {
            this.handle = new NativeHandle(handle);
        }
        
        public void Dispose()
        {
            handle.Dispose();
        }
        
        private readonly NativeHandle handle;

        internal NativeHandle Handle
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(Module).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle)
                : base(true)
            {
                SetHandle(handle);
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_instance_delete(handle);
                return true;
            }
        }
        
        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern unsafe void wasm_instance_new(
                Store.NativeHandle store,
                [Const] Module.NativeHandle module,
                [ConstVector] in ExternalVector imports,
                [OwnPass] Trap** trap);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_instance_delete([OwnPass] IntPtr instance);

            [DllImport(NativePlugin.LibraryName)]
            public static extern unsafe void wasm_instance_exports(
                [Const] NativeHandle instance,
                [OwnPass] out ExternalVector externalVector);
        }
    }

    internal struct Trap
    {
    }
}