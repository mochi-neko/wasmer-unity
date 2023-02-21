using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ImportType : IDisposable
    {
        internal string Module
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
                }

                unsafe
                {
                    var ptr = WasmAPIs.wasm_importtype_module(handle);
                    return ptr->ToText();
                }
            }
        }
        
        internal string Name
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
                }

                unsafe
                {
                    var ptr = WasmAPIs.wasm_importtype_name(handle);
                    return ptr->ToText();
                }
            }
        }

        internal ExternalType Type
        {
            get
            {
                if (handle.IsInvalid)
                {
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
                }

                var ptr = WasmAPIs.wasm_importtype_type(handle);
                return ExternalType.FromPointer(ptr);
            }
        }

        internal static ImportType New(string module, string functionName, FunctionType functionType)
        {
            return New(module, functionName, ExternalType.ToExternalType(functionType));
        }

        private static ImportType New(string module, string name, ExternalType type)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(module, out var moduleVector);
            ByteVector.FromText(name, out var nameVector);

            return New(in moduleVector, in nameVector, type);
        }

        private static ImportType New(in ByteVector module, in ByteVector name, ExternalType type)
        {
            var importType = new ImportType(WasmAPIs.wasm_importtype_new(in module, in name, type.Handle));

            // Passes ownership to native.
            // type.Handle.SetHandleAsInvalid();
            
            return importType;
        }

        private ImportType(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
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
                WasmAPIs.wasm_importtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_importtype_new(
                [OwnPass][In] in ByteVector module,
                [OwnPass][In] in ByteVector name,
                [OwnPass][In] ExternalType.NativeHandle type);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_delete(
                [OwnPass][In] IntPtr importType);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_importtype_copy(
                [Const] NativeHandle importType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_importtype_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_importtype_module(
                [Const] NativeHandle importType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_importtype_name(
                [Const] NativeHandle importType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_importtype_type(
                [Const] NativeHandle importType);
        }
    }
}