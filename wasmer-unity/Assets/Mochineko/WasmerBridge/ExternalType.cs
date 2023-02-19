using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ExternalType : IDisposable
    {
        private ExternalType(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(ExternalType).FullName);
                }

                return handle;
            }
        }

        internal sealed class NativeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public NativeHandle(IntPtr handle) : base(true)
            {
                this.handle = handle;
            }

            protected override bool ReleaseHandle()
            {
                WasmAPIs.wasm_externtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_externtype_delete([OwnPass] IntPtr externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_externtype_copy([Const] NativeHandle externalType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern ExternalKind wasm_externtype_kind([Const] NativeHandle externalType);
            
            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_functype_as_externtype(FunctionType.NativeHandle functionType);
            
            //TODO:
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_globaltype_as_externtype(GlobalType.NativeHandle functionType);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_tabletype_as_externtype(TableType.NativeHandle functionType);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern IntPtr wasm_memorytype_as_externtype(MemoryType.NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_functype(NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_globaltype(NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_tabletype(NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern IntPtr wasm_externtype_as_memorytype(NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_functype_as_externtype_const([Const] FunctionType.NativeHandle functionType);

            // [DllImport(NativePlugin.LibraryName)]
            // [return: Const]
            // public static extern IntPtr wasm_globaltype_as_externtype_const([Const] GlobalType.NativeHandle functionType);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // [return: Const]
            // public static extern IntPtr wasm_tabletype_as_externtype_const([Const] TableType.NativeHandle functionType);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // [return: Const]
            // public static extern IntPtr wasm_memorytype_as_externtype_const([Const] MemoryType.NativeHandle functionType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_functype_const([Const] NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_globaltype_const([Const] NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_tabletype_const([Const] NativeHandle externalType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_externtype_as_memorytype_const([Const] NativeHandle externalType);

        }
    }
}