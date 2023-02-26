using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ValueInstanceTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(int.MaxValue)]
        [TestCase(int.MinValue)]
        [RequiresPlayMode(false)]
        public void CreateInt32Test(int value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewInt32(value);
            valueInstance.Kind.Should().Be(ValueKind.Int32);
            valueInstance.OfInt32.Should().Be(value);

            GC.Collect();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(long.MaxValue)]
        [TestCase(long.MinValue)]
        [RequiresPlayMode(false)]
        public void CreateInt64Test(long value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewInt64(value);
            valueInstance.Kind.Should().Be(ValueKind.Int64);
            valueInstance.OfInt64.Should().Be(value);

            GC.Collect();
        }

        [TestCase(0f)]
        [TestCase(1.1f)]
        [TestCase(-1.1f)]
        [TestCase(float.MaxValue)]
        [TestCase(float.MinValue)]
        [RequiresPlayMode(false)]
        public void CreateFloat32Test(float value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewFloat32(value);
            valueInstance.Kind.Should().Be(ValueKind.Float32);
            valueInstance.OfFloat32.Should().Be(value);

            GC.Collect();
        }

        [TestCase(0d)]
        [TestCase(1.1d)]
        [TestCase(-1.1d)]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        [RequiresPlayMode(false)]
        public void CreateFloat64Test(double value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewFloat64(value);
            valueInstance.Kind.Should().Be(ValueKind.Float64);
            valueInstance.OfFloat64.Should().Be(value);

            GC.Collect();
        }

        [Test]
        [RequiresPlayMode(false)]
        public void CreateAnyReferenceTestSuite()
        {
            CreateAnyReferenceTest(IntPtr.Zero);
            CreateAnyReferenceTest(IntPtr.Zero + 1);
            CreateAnyReferenceTest(IntPtr.Zero - 1);
            CreateAnyReferenceTest(IntPtr.Zero + int.MaxValue);
            CreateAnyReferenceTest(IntPtr.Zero + int.MinValue);
        }

        private void CreateAnyReferenceTest(IntPtr value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewAnyReference(value);
            valueInstance.Kind.Should().Be(ValueKind.AnyRef);
            valueInstance.OfReference.Should().Be(value);

            GC.Collect();
        }

        [Test]
        [RequiresPlayMode(false)]
        public void CreateFunctionReferenceTestSuite()
        {
            CreateFunctionReferenceTest(IntPtr.Zero);
            CreateFunctionReferenceTest(IntPtr.Zero + 1);
            CreateFunctionReferenceTest(IntPtr.Zero - 1);
            CreateFunctionReferenceTest(IntPtr.Zero + int.MaxValue);
            CreateFunctionReferenceTest(IntPtr.Zero + int.MinValue);
        }

        private void CreateFunctionReferenceTest(IntPtr value)
        {
            // Do not dispose because ValueInstance is created at C#, not native.
            var valueInstance = ValueInstance.NewFunctionReference(value);
            valueInstance.Kind.Should().Be(ValueKind.FuncRef);
            valueInstance.OfReference.Should().Be(value);

            GC.Collect();
        }
    }
}