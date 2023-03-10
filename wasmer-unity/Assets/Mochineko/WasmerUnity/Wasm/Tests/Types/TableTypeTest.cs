using System;
using FluentAssertions;
using Mochineko.WasmerUnity.Wasm.Types;
using NUnit.Framework;
using UnityEngine.TestTools;
using ValueType = Mochineko.WasmerUnity.Wasm.Types.ValueType;

namespace Mochineko.WasmerUnity.Wasm.Tests.Types
{
    [TestFixture]
    internal sealed class TableTypeTest
    {
        [TestCase(ValueKind.Int32, uint.MinValue, uint.MinValue)]
        [TestCase(ValueKind.Float32, uint.MaxValue, uint.MinValue)]
        [TestCase(ValueKind.Int64, uint.MaxValue, uint.MaxValue)]
        [RequiresPlayMode(false)]
        public void CreateTest(ValueKind kind, uint max, uint min)
        {
            var limits = new Limits(max, min);
            using var element = ValueType.FromKind(kind);
            using var tableType = TableType.New(element, in limits);
            tableType.Should().NotBeNull();
            tableType.Element.Kind.Should().Be(kind);
            tableType.Limits.Should().Be(limits);
            
            GC.Collect();
        }
    }
}