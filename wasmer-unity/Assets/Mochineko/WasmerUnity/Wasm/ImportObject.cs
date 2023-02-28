using System;
using System.Collections.Generic;

namespace Mochineko.WasmerUnity.Wasm
{
    public sealed class ImportObject : IDisposable
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, ExternalInstance>> imports;

        internal void GetImports(out ExternalInstanceVector imports)
        {
            var instances = new List<ExternalInstance>();

            foreach (var module in this.imports.Values)
            {
                foreach (var instance in module.Values)
                {
                    instances.Add(instance);
                }
            }
            
            ExternalInstanceVector.New(instances.ToArray(), out imports);
        }
        
        public static ImportObject New(IReadOnlyDictionary<string, IReadOnlyDictionary<string, ExternalInstance>> imports)
        {
            return new ImportObject(imports);
        }

        private ImportObject(IReadOnlyDictionary<string, IReadOnlyDictionary<string, ExternalInstance>> imports)
        {
            this.imports = imports;
        }

        public void Dispose()
        {
            foreach (var module in this.imports.Values)
            {
                foreach (var instance in module.Values)
                {
                    instance.Dispose();
                }
            }
        }
    }
}