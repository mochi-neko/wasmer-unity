using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class ValueInstanceVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ValueInstanceVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.Should().NotBeNull();
                vector.size.Should().Be((nuint)0);
            }

            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedArrayTest()
        {
            var array = new ValueInstance[]
            {
                ValueInstance.NewInt32(1),
                ValueInstance.NewInt64(2),
                ValueInstance.NewFloat32(1.1f),
                ValueInstance.NewFloat64(-2.2d),
                ValueInstance.NewAnyReference(IntPtr.Zero + 3),
                ValueInstance.NewFunctionReference(IntPtr.Zero + 4),
            };

            ValueInstanceVector.New(array, out var vector);
            using (vector)
            {
                vector.Should().NotBeNull();
                vector.size.Should().Be((nuint)array.Length);

                vector.ToManaged(out var managed);
                for (int i = 0; i < array.Length; i++)
                {
                    managed[i].Should().Be(array[i]);
                }
            }

            GC.Collect();
        }
    }
}