using System;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Mochineko.WasmerBridge.Tests
{
    [TestFixture]
    internal sealed class ExternalInstanceVectorTest
    {
        [Test, RequiresPlayMode(false)]
        public void CreateEmptyTest()
        {
            ExternalInstanceVector.NewEmpty(out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)0);
            }

            ExternalInstanceVector.New(ArraySegment<ExternalInstance>.Empty, out var emptyVector);
            using (emptyVector)
            {
                emptyVector.size.Should().Be((nuint)0);
            }

            GC.Collect();
        }

        [Test, RequiresPlayMode(false)]
        public unsafe void CreateFromManagedArrayTest()
        {
            using var engine = Engine.New();
            using var store = Store.New(engine);
            using var functionType = FunctionType.New(
                Array.Empty<ValueKind>(),
                Array.Empty<ValueKind>());
            bool callbackCalled = false;
            using var functionInstance = FunctionInstance.New(
                store,
                functionType,
                callback: (_, _) =>
                {
                    callbackCalled = true;
                    return IntPtr.Zero;
                });
            using var externalInstance = ExternalInstance.FromFunctionWithOwnership(functionInstance);
            var externalInstances = new[] { externalInstance };

            ExternalInstanceVector.New(externalInstances, out var vector);
            using (vector)
            {
                vector.size.Should().Be((nuint)externalInstances.Length);
                vector.ToManaged(out var managed);
                managed.Length.Should().Be(externalInstances.Length);

                using var excludedFunctionInstance = managed[0].ToFunction();
                ValueInstanceVector.NewEmpty(out var arguments);
                ValueInstanceVector.NewEmpty(out var results);
                using (arguments)
                using (results)
                {
                    using var trap = excludedFunctionInstance.Call(in arguments, ref results);
                    trap.Should().BeNull();
                    callbackCalled.Should().BeTrue();
                }
            }

            GC.Collect();
        }
    }
}