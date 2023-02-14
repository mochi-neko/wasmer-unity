using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ByteArrayTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            using var emptyArray = NativeByteArray.New();
            emptyArray.Should().NotBeNull();
            emptyArray.size.Should().Be((nuint)0);
        }
        
        [Test, RequiresPlayMode(false)]
        public unsafe void CrateFromByteArrayUnsafeTest()
        {
            var binary = MockModule.EmptyWasmBinary;
            fixed (byte* ptr = MockModule.EmptyWasmBinary)
            {
                using var array = NativeByteArray.New((nuint)binary.Length, ptr);
                array.Should().NotBeNull();
                array.size.Should().Be((nuint)binary.Length);
            }
        }
        
        [Test, RequiresPlayMode(false)]
        public void CreateFromManagedByteArrayTest()
        {
            var binary = MockModule.EmptyWasmBinary;
            using var converted = NativeByteArray.CreateFromManaged(binary);
            converted.Should().NotBeNull();
            converted.size.Should().Be((nuint)binary.Length);
        }
    }
}