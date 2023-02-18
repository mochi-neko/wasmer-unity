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
            ByteVector.NewEmpty(out var emptyArray);
            using (emptyArray)
            {
                emptyArray.Should().NotBeNull();
                emptyArray.size.Should().Be((nuint)0);
            }
        }

        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedByteArrayTest()
        {
            var binary = MockResource.EmptyWasmBinary;
            
            ByteVector.New(binary, out var nativeBinary);
            using (nativeBinary)
            {
                nativeBinary.Should().NotBeNull();
                nativeBinary.size.Should().Be((nuint)binary.Length);

                nativeBinary.ToManagedSpan(out var managedBinary);
                for (int i = 0; i < binary.Length; i++)
                {
                    managedBinary[i].Should().Be(binary[i]);
                }
            }
        }
    }
}