using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Mochineko.WasmerUnity.Wasm.Attributes;

namespace Mochineko.WasmerUnity.Wasm.Types
{
    [OwnPointed]
    internal sealed class ImportType : IDisposable
    {
        internal string Module
        {
            get
            {
                unsafe
                {
                    var ptr = WasmAPIs.wasm_importtype_module(Handle);
                    return ptr->ToText();
                }
            }
        }

        internal string Name
        {
            get
            {
                unsafe
                {
                    var ptr = WasmAPIs.wasm_importtype_name(Handle);
                    return ptr->ToText();
                }
            }
        }

        internal ExternalKind Kind
        {
            get
            {
                using var type = Type;
                return type.Kind;
            }
        }

        internal ExternalType Type
            => ExternalType.FromPointer(
                WasmAPIs.wasm_importtype_type(Handle),
                hasOwnership: false);

        [return: OwnReceive]
        internal static ImportType FromFunction(string module, string functionName, [OwnPass] FunctionType functionType)
        {
            using var externalType = ExternalType.FromFunction(functionType);
            var importType = New(module, functionName, externalType);

            // Passes ownership to native.
            functionType.Handle.SetHandleAsInvalid();

            return importType;
        }

        [return: OwnReceive]
        internal static ImportType FromGlobal(string module, string functionName, [OwnPass] GlobalType globalType)
        {
            using var externalType = ExternalType.FromGlobal(globalType);
            var importType = New(module, functionName, externalType);

            // Passes ownership to native.
            globalType.Handle.SetHandleAsInvalid();

            return importType;
        }
        
        [return: OwnReceive]
        internal static ImportType FromTable(string module, string functionName, [OwnPass] TableType tableType)
        {
            using var externalType = ExternalType.FromTable(tableType);
            var importType = New(module, functionName, externalType);

            // Passes ownership to native.
            tableType.Handle.SetHandleAsInvalid();

            return importType;
        }
        
        [return: OwnReceive]
        internal static ImportType FromMemory(string module, string functionName, [OwnPass] MemoryType memoryType)
        {
            using var externalType = ExternalType.FromMemory(memoryType);
            var importType = New(module, functionName, externalType);

            // Passes ownership to native.
            memoryType.Handle.SetHandleAsInvalid();

            return importType;
        }

        [return: OwnReceive]
        private static ImportType New(string module, string name, [OwnPass] ExternalType type)
        {
            // Passes name vectors ownerships to native, then vectors are released by owner:ImportType.
            ByteVector.FromText(module, out var moduleVector);
            ByteVector.FromText(name, out var nameVector);

            return New(in moduleVector, in nameVector, type);
        }

        [return: OwnReceive]
        private static ImportType New(
            [OwnPass] in ByteVector module,
            [OwnPass] in ByteVector name,
            [OwnPass] ExternalType type)
        {
            var importType = new ImportType(
                WasmAPIs.wasm_importtype_new(in module, in name, type.Handle),
                hasOwnership: true);

            // Passes ownership to native.
            type.Handle.SetHandleAsInvalid();

            return importType;
        }

        private ImportType(IntPtr handle, bool hasOwnership)
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
                    throw new ObjectDisposedException(typeof(ImportType).FullName);
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
                WasmAPIs.wasm_importtype_delete(handle);
                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            [return: OwnReceive]
            public static extern IntPtr wasm_importtype_new(
                [OwnPass] in ByteVector module,
                [OwnPass] in ByteVector name,
                [OwnPass] [In] ExternalType.NativeHandle type);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_importtype_delete(
                [OwnPass] [In] IntPtr handle);

            [DllImport(NativePlugin.LibraryName)]
            public static extern bool wasm_importtype_same(
                [Const] NativeHandle left,
                [Const] NativeHandle right);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_importtype_module(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern unsafe ByteVector* wasm_importtype_name(
                [Const] NativeHandle handle);

            [DllImport(NativePlugin.LibraryName)]
            [return: Const]
            public static extern IntPtr wasm_importtype_type(
                [Const] NativeHandle handle);
        }
    }
}