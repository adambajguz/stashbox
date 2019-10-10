﻿using System;
using System.Linq;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class AspNetCoreFailingSpecTestsForConstrainedGenerics
    {
        [Fact]
        public void PublicNoArgCtorConstrainedOpenGenericServicesCanBeResolved()
        {
            var container = new StashboxContainer();
            container.Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNoConstraints<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNewConstraint<>));

            var allServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<ClassWithPrivateCtor>>().ToList();

            Assert.Equal(2, allServices.Count);
            Assert.Single(constrainedServices);
        }

        [Fact]
        public void SelfReferencingConstrainedOpenGenericServicesCanBeResolved()
        {
            var container = new StashboxContainer();
            var poco = new PocoClass();
            var comparable = new ClassImplementingIComparable();
            container.Register(typeof(IFakeOpenGenericService<>), typeof(FakeOpenGenericService<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithSelfReferencingConstraint<>))
                .RegisterInstance(poco)
                .RegisterInstance(comparable);

            var allServices = container.ResolveAll<IFakeOpenGenericService<ClassImplementingIComparable>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();

            Assert.Equal(2, allServices.Count);
            Assert.Same(comparable, allServices[0].Value);
            Assert.Same(comparable, allServices[1].Value);
            Assert.Single(constrainedServices);
            Assert.Same(poco, constrainedServices[0].Value);
        }

        [Fact]
        public void ClassConstrainedOpenGenericServicesCanBeResolved()
        {
            var container = new StashboxContainer();
            container.Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNoConstraints<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithClassConstraint<>));

            var allServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<int>>().ToList();

            Assert.Equal(2, allServices.Count);
            Assert.Single(constrainedServices);
        }

        [Fact]
        public void StructConstrainedOpenGenericServicesCanBeResolved()
        {
            var container = new StashboxContainer();
            container.Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithNoConstraints<>))
                .Register(typeof(IFakeOpenGenericService<>), typeof(ClassWithStructConstraint<>));

            var allServices = container.ResolveAll<IFakeOpenGenericService<int>>().ToList();
            var constrainedServices = container.ResolveAll<IFakeOpenGenericService<PocoClass>>().ToList();

            Assert.Equal(2, allServices.Count);
            Assert.Single(constrainedServices);
        }
    }

    interface IFakeOpenGenericService<T>
    {
        T Value { get; }
    }

    class ClassWithNoConstraints<T> : IFakeOpenGenericService<T>
    {
        public T Value { get; } = default;
    }

    class ClassWithNewConstraint<T> : IFakeOpenGenericService<T>
        where T : new()
    {
        public T Value { get; } = new T();
    }

    class ClassWithSelfReferencingConstraint<T> : IFakeOpenGenericService<T>
        where T : IComparable<T>
    {
        public ClassWithSelfReferencingConstraint(T value) => Value = value;
        public T Value { get; }
    }

    class FakeOpenGenericService<TVal> : IFakeOpenGenericService<TVal>
    {
        public FakeOpenGenericService(TVal value)
        {
            Value = value;
        }

        public TVal Value { get; }
    }

    class ClassWithClassConstraint<T> : IFakeOpenGenericService<T>
        where T : class
    {
        public T Value { get; } = default;
    }

    class ClassWithStructConstraint<T> : IFakeOpenGenericService<T>
        where T : struct
    {
        public T Value { get; } = default;
    }

    class PocoClass
    {
    }

    class ClassWithPrivateCtor
    {
        private ClassWithPrivateCtor()
        {
        }
    }

    class ClassImplementingIComparable : IComparable<ClassImplementingIComparable>
    {
        public int CompareTo(ClassImplementingIComparable other) => 0;
    }
}
