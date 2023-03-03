using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.Types.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class ExportTypeVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ExportTypeVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)0);
            }

            ExportTypeVector.New(ArraySegment<ExportType>.Empty, out var emptyVector);
            using (emptyVector)
            {
                emptyVector.size.Should().Be((nuint)0);   
            }
            
            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedArrayTest()
        {
            var functionName = "FunctionName";
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());
            using var asFunction = ExportType.FromFunction(functionName, functionType);
            
            var globalName = "GlobalName";
            using var valueType = ValueType.FromKind(ValueKind.Int32);
            using var globalType = GlobalType.New(valueType, Mutability.Constant);
            using var asGlobal = ExportType.FromGlobal(globalName, globalType);
            
            var exportTypes = new[]
            {
                asFunction,
                asGlobal,
            };

            ExportTypeVector.New(exportTypes, out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)exportTypes.Length);
            }
            
            GC.Collect();
        }
    }
}