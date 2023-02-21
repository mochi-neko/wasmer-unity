using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
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
            using var exportType = ExportType.New(functionName, functionType);
            
            var exportTypes = new[]
            {
                exportType
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