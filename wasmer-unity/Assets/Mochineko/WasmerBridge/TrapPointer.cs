using System;
using System.Runtime.InteropServices;
using Mochineko.WasmerBridge.Attributes;

namespace Mochineko.WasmerBridge
{
    [OwnStruct]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly ref struct TrapPointer
    {
        private readonly IntPtr trapPointer;

        public Trap Trap(bool hasOwnership)
            => WasmerBridge.Trap.FromPointer(trapPointer, hasOwnership);

        public static void New(Store store, [OwnOut] out TrapPointer pointer)
        {
            var trap = WasmerBridge.Trap.NewWithEmptyMessage(store);
            pointer = new TrapPointer(trap.Handle.DangerousGetHandle());
        }

        private TrapPointer(IntPtr trapPointer)
        {
            this.trapPointer = trapPointer;
        }
    }
}