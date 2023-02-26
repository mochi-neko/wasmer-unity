using System;
using Mochineko.WasmerUnity.Wasm;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerUnity.Wasm.Tests
{
    [TestFixture]
    internal sealed class ByteVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ByteVector.NewEmpty(out var vector);
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
            var array = MockResource.EmptyWasmBinary;
            
            ByteVector.New(array, out var vector);
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
        
        [Test, RequiresPlayMode(false)]
        public void StringEncodingTest()
        {
            var message = "message";
            
            ByteVector.FromText(message, out var encoded);
            using (encoded)
            {
                encoded.size.Should().Be((nuint)7);
                var decoded = encoded.ToText();
                decoded.Should().Be(message);
            }
            
            GC.Collect();
        }
    }
}