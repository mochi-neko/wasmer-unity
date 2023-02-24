using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnPointed]
    internal sealed class ExportType : IDisposable
    {
        internal string Name
        {
            get
            {
                unsafe
                {
                    var ptr = WasmAPIs.wasm_exporttype_name(Handle);
                    return ptr->ToText();
                }
            }
        }

        internal ExternalKind Kind
        {
            get
            {
                using var type = Type;
                var kind = type.Kind;
                // Does not receive ownership of ExternalType from ImportType.
                type.Handle.SetHandleAsInvalid();

                return kind;
            }
        }

        private ExternalType Type
        {
            get
            {
                var ptr = WasmAPIs.wasm_exporttype_type(Handle);
                return ExternalType.FromPointer(ptr);
            }
        }

        [return: OwnReceive]
        internal static ExportType New(string functionName, [OwnPass] FunctionType functionType)
        {
            var exportType = New(functionName, ExternalType.FromFunction(functionType));

            // Passes ownership to native.
            functionType.Handle.SetHandleAsInvalid();

            return exportType;
        }

        [return: OwnReceive]
        private static ExportType New(string name, [OwnPass] ExternalType type)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(name, out var nameVector);

            return New(in nameVector, type);
        }

        [return: OwnReceive]
        private static ExportType New([OwnPass] in ByteVector name, [OwnPass] ExternalType type)
        {
            var exportType = new ExportType(WasmAPIs.wasm_exporttype_new(in name, type.Handle));

            // Passes ownership to native.
            type.Handle.SetHandleAsInvalid();

            return exportType;
        }

        private ExportType(IntPtr handle)
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
                    throw new ObjectDisposedException(typeof(ExportType).FullName);
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
                WasmAPIs.wasm_exporttype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_exporttype_new(
                [OwnPass] in ByteVector name,
                [OwnPass] [In] ExternalType.NativeHandle type);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_exporttype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_exporttype_copy(
                [Const] NativeHandle exportType);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_exporttype_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_exporttype_name(
                [Const] NativeHandle exportType);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_exporttype_type(
                [Const] NativeHandle exportType);
        }
    }
}