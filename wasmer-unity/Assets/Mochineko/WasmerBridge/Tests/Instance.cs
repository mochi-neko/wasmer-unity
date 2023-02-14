using System;

namespace Mochineko.WasmerBridge.Tests
{
    public sealed class Instance
    {
        public Instance(Store store, Module module, ImportObject importObject)
        {
            throw new NotImplementedException();
        }

        public Func<T> ExportFunction<T>(Store store, string run)
        {
            throw new NotImplementedException();
        }
    }
}