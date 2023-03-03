using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.Types.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class ImportTypeVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ImportTypeVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)0);
            }

            ImportTypeVector.New(ArraySegment<ImportType>.Empty, out var emptyVector);
            using (emptyVector)
            {
                emptyVector.size.Should().Be((nuint)0);   
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedArrayTest()
        {
            var moduleName = "ModuleName";
            
            var functionName = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());
            using var asFunction = ImportType.FromFunction(moduleName, functionName, functionType);
            
            var globalName = "GlobalName";
            using var valueType = ValueType.FromKind(ValueKind.Int32);
            using var globalType = GlobalType.New(valueType, Mutability.Constant);
            using var asGlobal = ImportType.FromGlobal(moduleName, globalName, globalType);
            
            var importTypes = new[]
            {
                asFunction,
                asGlobal,
            };

            ImportTypeVector.New(importTypes, out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)importTypes.Length);
            }
            
            GC.Collect();
        }
    }
}