using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge.Tests
{
    [OwnPointed]
    internal sealed class ImportFunction
    {
        internal ImportFunction(ImportType.NativeHandle importType)
        {
            unsafe
            {
                var moduleName = WasmAPIs.wasm_importtype_module(importType);
                if (moduleName->size == 0)
                {
                    ModuleName = String.Empty;
                }
                else
                {
                    ModuleName = Marshal.PtrToStringUTF8((IntPtr)moduleName->data, checked((int)moduleName->size));
                }

                var name = WasmAPIs.wasm_importtype_name(importType);
                if (name is null || name->size == 0)
                {
                    Name = String.Empty;
                }
                else
                {
                    Name = Marshal.PtrToStringUTF8((IntPtr)name->data, checked((int)name->size));
                }

                // var type = FunctionExport.Native.wasm_externtype_as_functype_const(externType);
                // if (type == IntPtr.Zero)
                // {
                //     throw new InvalidOperationException();
                // }
                //
                // Parameters = (*Function.Native.wasm_functype_params(type)).ToArray();
                // Results = (*Function.Native.wasm_functype_results(type)).ToArray();
            }
        }

        public string ModuleName { get; }

        public string Name { get; }
        
        public IReadOnlyList<ValueKind> Parameters { get; }

        public IReadOnlyList<ValueKind> Results { get; }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern unsafe ByteVector* wasm_importtype_module(ImportType.NativeHandle type);

            [DllImport(NativePlugin.LibraryName)]
            public static extern unsafe ByteVector* wasm_importtype_name(ImportType.NativeHandle type);
        }
    }

    public sealed class Function
    {
        internal static class WasmApIs
        {
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern unsafe ValueTypeArray* wasm_functype_params(FunctionType.NativeHandle type);
            //
            // [DllImport(NativePlugin.LibraryName)]
            // public static extern unsafe ValueTypeArray* wasm_functype_results(FunctionType.NativeHandle type);

        }
    }
}