using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Mochineko.WasmerBridge
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly unsafe struct ValueTypeVector
    {
        private readonly nuint size;
        private readonly IntPtr data;

        internal static NativeHandle New()
        {
            WasmAPIs.wasm_valtype_vec_new_empty(out var vector);

            return new NativeHandle(vector);
        }

        internal static NativeHandle New(IReadOnlyList<ValueKind> valueKinds)
        {
            if (valueKinds.Count == 0)
            {
                return New();
            }

            // NOTE: Allocate block memory of ValueType array to pass native.
            var valueTypes = stackalloc IntPtr[valueKinds.Count];
            var valueTypeHandles = new ValueType.NativeHandle[valueKinds.Count];
            for (var i = 0; i < valueKinds.Count; i++)
            {
                valueTypes[i] = ValueType.NewAsPointer(valueKinds[i]);
                valueTypeHandles[i] = new ValueType.NativeHandle(valueTypes[i]);
            }

            WasmAPIs.wasm_valtype_vec_new(out var vector, (nuint)valueKinds.Count, *valueTypes);

            return new NativeHandle(vector, (nuint)valueKinds.Count, *valueTypes, valueTypeHandles);
        }

        public static IReadOnlyList<ValueKind> ToValueKinds(NativeHandle valueTypes)
        {
            var size = (int)valueTypes.size;
            var array = new ValueKind[size];

            for (int i = 0; i < size; ++i)
            {
                array[i] = ValueType.ToKind(valueTypes.elementHandles[i].DangerousGetHandle());
            }

            return array;
        }

        internal sealed class NativeHandle : SafeHandle
        {
            internal readonly nuint size;
            internal readonly IntPtr data;
            internal readonly ValueType.NativeHandle[] elementHandles;

            public NativeHandle(IntPtr handle)
                : base(IntPtr.Zero, true)
            {
                SetHandle(handle);
                this.size = 0;
                this.data = IntPtr.Zero;
                this.elementHandles = Array.Empty<ValueType.NativeHandle>();
            }
            
            public NativeHandle(IntPtr handle, nuint size, IntPtr data, ValueType.NativeHandle[] elementHandles)
                : base(IntPtr.Zero, true)
            {
                SetHandle(handle);
                this.size = size;
                this.data = data;
                this.elementHandles = elementHandles;
            }

            public override bool IsInvalid
                => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                // NOTE: Use not "wasm_valtype_vec_delete" but explicit disposing of elements.
                // WasmAPIs.wasm_valtype_vec_delete(handle);
                foreach (var elementHandle in elementHandles)
                {
                    elementHandle.Dispose();
                }

                return true;
            }
        }

        private static class WasmAPIs
        {
            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_empty(out IntPtr vector);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new_uninitialized(out IntPtr vector, nuint size);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_new(out IntPtr vector, nuint size, IntPtr data);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_copy(out IntPtr destination, in IntPtr source);

            [DllImport(NativePlugin.LibraryName)]
            public static extern void wasm_valtype_vec_delete(IntPtr vector);
        }
    }
}