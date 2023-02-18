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

                nativeBinary.ToManaged(out var managedBinary);
                for (int i = 0; i < binary.Length; i++)
                {
                    managedBinary[i].Should().Be(binary[i]);
                }
            }
        }
        
        [Test, RequiresPlayMode(false)]
        public void StringEncodingTest()
        {
            var message = "message";
            
            ByteVector.FromString(message, out var encoded);
            encoded.size.Should().Be((nuint)7);
            using (encoded)
            {
                var decoded = encoded.ToString();
                decoded.Should().Be(message);
            }
        }
    }
}