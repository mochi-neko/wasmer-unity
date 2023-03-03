using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Microsoft.Win32.SafeHandles;

namespace Mochineko.WasmerUnity.Wasm
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

        internal ExternalType Type
            => ExternalType.FromPointer(
                WasmAPIs.wasm_exporttype_type(Handle),
                hasOwnership: false);

        [return: OwnReceive]
        internal static ExportType FromFunction(string functionName, [OwnPass] FunctionType functionType)
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
        internal static ExportType FromGlobal(string globalName, [OwnPass] GlobalType globalType)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(globalName, out var nameVector);

            using var type = ExternalType.FromGlobal(globalType);

            var exportType = new ExportType(
                WasmAPIs.wasm_exporttype_new(in nameVector, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            globalType.Handle.SetHandleAsInvalid();

            return exportType;
        }
        
        [return: OwnReceive]
        internal static ExportType FromTable(string globalName, [OwnPass] TableType tableType)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(globalName, out var nameVector);

            using var type = ExternalType.FromTable(tableType);

            var exportType = new ExportType(
                WasmAPIs.wasm_exporttype_new(in nameVector, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            tableType.Handle.SetHandleAsInvalid();

            return exportType;
        }
        
        [return: OwnReceive]
        internal static ExportType FromMemory(string globalName, [OwnPass] MemoryType memoryType)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(globalName, out var nameVector);

            using var type = ExternalType.FromMemory(memoryType);

            var exportType = new ExportType(
                WasmAPIs.wasm_exporttype_new(in nameVector, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            memoryType.Handle.SetHandleAsInvalid();

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

        internal static ExportType FromPointer(IntPtr handle, bool hasOwnership)
            => new ExportType(handle, hasOwnership);

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
            public static extern bool wasm_exporttype_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_exporttype_name(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_exporttype_type(
                [Const] NativeHandle handle);
        }
    }
}