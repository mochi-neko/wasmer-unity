using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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
            using var importType = ImportType.New(moduleName, functionName, functionType);
            
            var importTypes = new[]
            {
                importType
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