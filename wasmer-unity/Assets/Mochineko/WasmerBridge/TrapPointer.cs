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

        public Trap Trap
            => Trap.FromPointer(trapPointer);

        public static void New(Store store, out TrapPointer pointer)
        {
            var trap = Trap.NewWithEmptyMessage(store);
            pointer = new TrapPointer(trap.Handle.DangerousGetHandle());
        }

        private TrapPointer(IntPtr trapPointer)
        {
            this.trapPointer = trapPointer;
        }
    }
}