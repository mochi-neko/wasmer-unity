using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ByteVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            using var emptyArray = ByteVector.NewEmpty();
            emptyArray.Should().NotBeNull();
            emptyArray.size.Should().Be((nuint)0);
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedByteArrayTest()
        {
            var binary = MockResource.EmptyWasmBinary;
            
            using var nativeBinary = ByteVector.New(binary);
            nativeBinary.Should().NotBeNull();
            nativeBinary.size.Should().Be((nuint)binary.Length);

            ByteVector.ToManagedSpan(in nativeBinary, out var managedBinary);
            for (int i = 0; i < binary.Length; i++)
            {
                managedBinary[i].Should().Be(binary[i]);
            }
        }
    }
}