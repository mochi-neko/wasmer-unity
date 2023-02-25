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

                return kind;
            }
        }

        private ExternalType Type
        {
            get
            {
                var ptr = WasmAPIs.wasm_exporttype_type(Handle);
                return ExternalType.FromPointer(ptr, hasOwnership: false);
            }
        }

        [return: OwnReceive]
        internal static ExportType New(string functionName, [OwnPass] FunctionType functionType)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(functionName, out var nameVector);
            
            using var type = ExternalType.FromFunction(functionType);
            
            var exportType = new ExportType(
                WasmAPIs.wasm_exporttype_new(in nameVector, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            functionType.Handle.SetHandleAsInvalid();

            return exportType;
        }

        [return: OwnReceive]
        private static ExportType New([OwnPass] in ByteVector name, [OwnPass] ExternalType type)
        {
            var exportType = new ExportType(
                WasmAPIs.wasm_exporttype_new(in name, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            type.Handle.SetHandleAsInvalid();

            return exportType;
        }

        private ExportType(IntPtr handle, bool hasOwnership)
        {
            this.handle = new NativeHandle(handle, hasOwnership);
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
            public NativeHandle(IntPtr handle, bool ownsHandle)
                : base(ownsHandle)
            {
                SetHandle(handle);
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
                [OwnPass] ExternalType.NativeHandle type);

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