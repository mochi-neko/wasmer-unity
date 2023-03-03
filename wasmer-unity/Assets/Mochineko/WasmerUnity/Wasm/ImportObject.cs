using System;
using System.Collections.Generic;
using Mochineko.WasmerUnity.Wasm.Attributes;
using Mochineko.WasmerUnity.Wasm.Instances;

namespace Mochineko.WasmerUnity.Wasm
{
    public sealed class ImportObject : IDisposable
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, ExternalInstance>> imports;

        internal void GetImports([OwnOut] out ExternalInstanceVector imports)
        {
            if (this.imports.Count == 0)
            {
                ExternalInstanceVector.NewEmpty(out imports);
                return;
            }
            
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
        
        public static ImportObject New()
        {
            return new ImportObject(new Dictionary<string, IReadOnlyDictionary<string, ExternalInstance>>());
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