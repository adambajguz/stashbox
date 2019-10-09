﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Lifetime;
using System;

namespace Stashbox.Tests
{
    [TestClass]
    public class DisposeTests
    {
        [TestMethod]
        public void DisposeTests_Singleton()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.RegisterSingleton<ITest1, Test1>();
                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Singleton_WithoutDisposal()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>(context => context.WithLifetime(new SingletonLifetime()).WithoutDisposalTracking());
                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.RegisterInstanceAs(test);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance_WithoutDisposal()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.RegisterInstanceAs(test, withoutDisposalTracking: true);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance_AsObject_WithoutDisposal()
        {
            object test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.RegisterInstance(typeof(ITest1), test, withoutDisposalTracking: true);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsFalse(((ITest1)test).Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance_WithoutDisposal_Fluent()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1>(context => context.WithInstance(test).WithoutDisposalTracking());
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_WireUp()
        {
            ITest1 test = new Test1();
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.WireUpAs(test);
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>();
                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Scoped()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.RegisterScoped<ITest2, Test2>();
                container.RegisterScoped<Test3>();
                container.RegisterScoped<ITest1, Test1>();
                container.RegisterScoped<ITest11, Test4>();
                container.RegisterScoped<ITest1, Test1>("test");
                container.RegisterScoped<ITest11, Test4>("test2");

                test = container.Resolve<ITest1>();
                var a = container.Resolve<ITest11>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = (ITest1)child.Resolve(typeof(ITest1), "test");
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_PutInScope_RootScope()
        {
            var test = new Test1();
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();

                container.PutInstanceInScope<ITest1>(test);

                var test1 = container.Resolve<ITest1>();
                var test2 = container.Resolve(typeof(ITest2));
                var test3 = container.Resolve<Test3>();

                Assert.AreSame(test, test1);
                Assert.AreSame(test, ((ITest2)test2).Test1);
                Assert.AreSame(test, test3.Test1);
            }

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_PutInScope_RootScope_WithoutDispose()
        {
            var test = new Test1();
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();

                container.PutInstanceInScope<ITest1>(test, withoutDisposalTracking: true);

                var test1 = container.Resolve<ITest1>();
                var test2 = container.Resolve(typeof(ITest2));
                var test3 = container.Resolve<Test3>();

                Assert.AreSame(test, test1);
                Assert.AreSame(test, ((ITest2)test2).Test1);
                Assert.AreSame(test, test3.Test1);
            }

            Assert.IsFalse(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_PutInScope_Scoped()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.RegisterScoped<ITest2, Test2>();
                container.RegisterScoped<Test3>();

                var test = new Test1();

                using (var child = container.BeginScope())
                {
                    child.PutInstanceInScope<ITest1>(test);

                    var test1 = child.Resolve<ITest1>();
                    var test2 = child.Resolve<ITest2>();
                    var test3 = child.Resolve<Test3>();

                    Assert.AreSame(test, test1);
                    Assert.AreSame(test, test2.Test1);
                    Assert.AreSame(test, test3.Test1);
                }

                Assert.IsTrue(test.Disposed);

                var test4 = new Test1();

                using (var child = container.BeginScope())
                {
                    child.PutInstanceInScope<ITest1>(test4);

                    var test1 = child.Resolve<ITest1>();
                    var test2 = child.Resolve<ITest2>();
                    var test3 = child.Resolve<Test3>();

                    Assert.AreSame(test4, test1);
                    Assert.AreSame(test4, test2.Test1);
                    Assert.AreSame(test4, test3.Test1);

                    Assert.AreNotSame(test, test1);
                    Assert.AreNotSame(test, test2.Test1);
                    Assert.AreNotSame(test, test3.Test1);
                }

                Assert.IsTrue(test4.Disposed);
            }
        }

        [TestMethod]
        public void DisposeTests_PutInScope_WithoutDispose()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();

                var test = new Test1();

                using (var child = container.BeginScope())
                {
                    child.PutInstanceInScope<ITest1>(test, withoutDisposalTracking: true);

                    var test1 = child.Resolve<ITest1>();
                    var test2 = child.Resolve<ITest2>();
                    var test3 = child.Resolve<Test3>();

                    Assert.AreSame(test, test1);
                    Assert.AreSame(test, test2.Test1);
                    Assert.AreSame(test, test3.Test1);
                }

                Assert.IsFalse(test.Disposed);
            }
        }

        [TestMethod]
        public void DisposeTests_PutInScope_Named()
        {
            using (IStashboxContainer container = new StashboxContainer())
            {
                container.Register<Test5>();

                var dummy1 = new Test1();
                var dummy2 = new Test1();

                using (var child = container.BeginScope())
                {
                    child.PutInstanceInScope<ITest1>(dummy1, "d1", withoutDisposalTracking: true);
                    child.PutInstanceInScope<ITest1>(dummy2, "d2", withoutDisposalTracking: true);

                    var test1 = child.Resolve<ITest1>("d2");
                    var test2 = child.Resolve<Test5>();

                    Assert.AreSame(dummy2, test1);
                    Assert.AreSame(dummy2, test2.Test1);
                }

                Assert.IsFalse(dummy1.Disposed);
                Assert.IsFalse(dummy2.Disposed);
            }
        }

        [TestMethod]
        public void DisposeTests_Scoped_Factory()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.RegisterScoped<ITest2, Test2>();
                container.RegisterScoped<Test3>();
                container.RegisterScoped<ITest1, Test1>();

                test = container.ResolveFactory<ITest1>()();
                test2 = container.ResolveFactory<ITest2>()();
                test3 = container.ResolveFactory<Test3>()();

                using (var child = container.BeginScope())
                {
                    test4 = child.ResolveFactory<ITest1>()();
                    test5 = child.ResolveFactory<ITest2>()();
                    test6 = child.ResolveFactory<Test3>()();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Scoped_WithoutDisposal()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer())
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.Register<ITest2, Test2>(context => context.WithLifetime(new ScopedLifetime()).WithoutDisposalTracking());
                container.Register<Test3>(context => context.WithLifetime(new ScopedLifetime()).WithoutDisposalTracking());
                container.Register<ITest1, Test1>(context => context.WithLifetime(new ScopedLifetime()).WithoutDisposalTracking());

                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = child.Resolve<ITest1>();
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsFalse(test4.Disposed);
                Assert.IsFalse(test5.Test1.Disposed);
                Assert.IsFalse(test6.Test1.Disposed);
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>();

                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = child.Resolve<ITest1>();
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_Factory()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>();

                test = container.ResolveFactory<ITest1>()();
                test2 = container.ResolveFactory<ITest2>()();
                test3 = container.ResolveFactory<Test3>()();

                using (var child = container.BeginScope())
                {
                    test4 = child.ResolveFactory<ITest1>()();
                    test5 = child.ResolveFactory<ITest2>()();
                    test6 = child.ResolveFactory<Test3>()();
                }

                Assert.IsTrue(test4.Disposed);
                Assert.IsTrue(test5.Test1.Disposed);
                Assert.IsTrue(test6.Test1.Disposed);
            }

            Assert.IsTrue(test.Disposed);
            Assert.IsTrue(test2.Test1.Disposed);
            Assert.IsTrue(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_TrackingDisabled()
        {
            ITest1 test;
            ITest2 test2;
            Test3 test3;
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                ITest1 test4;
                ITest2 test5;
                Test3 test6;

                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>(context => context.WithoutDisposalTracking());

                test = container.Resolve<ITest1>();
                test2 = container.Resolve<ITest2>();
                test3 = container.Resolve<Test3>();

                using (var child = container.BeginScope())
                {
                    test4 = child.Resolve<ITest1>();
                    test5 = child.Resolve<ITest2>();
                    test6 = child.Resolve<Test3>();
                }

                Assert.IsFalse(test4.Disposed);
                Assert.IsFalse(test5.Test1.Disposed);
                Assert.IsFalse(test6.Test1.Disposed);
            }

            Assert.IsFalse(test.Disposed);
            Assert.IsFalse(test2.Test1.Disposed);
            Assert.IsFalse(test3.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_ScopeOfScope_Transient()
        {
            using (IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<ITest2, Test2>();
                container.Register<Test3>();
                container.Register<ITest1, Test1>();

                ITest1 test;
                ITest2 test2;
                Test3 test3;

                using (var scope = container.BeginScope())
                {
                    test = scope.Resolve<ITest1>();
                    test2 = scope.Resolve<ITest2>();
                    test3 = scope.Resolve<Test3>();

                    ITest1 test4;
                    ITest2 test5;
                    Test3 test6;

                    using (var scope2 = scope.BeginScope())
                    {
                        test4 = scope2.Resolve<ITest1>();
                        test5 = scope2.Resolve<ITest2>();
                        test6 = scope2.Resolve<Test3>();
                    }

                    Assert.IsTrue(test4.Disposed);
                    Assert.IsTrue(test5.Test1.Disposed);
                    Assert.IsTrue(test6.Test1.Disposed);

                    Assert.IsFalse(test.Disposed);
                    Assert.IsFalse(test2.Test1.Disposed);
                    Assert.IsFalse(test3.Test1.Disposed);
                }

                Assert.IsTrue(test.Disposed);
                Assert.IsTrue(test2.Test1.Disposed);
                Assert.IsTrue(test3.Test1.Disposed);
            }
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_ChildContainer()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            ITest1 test4;
            ITest2 test5;
            Test3 test6;

            container.Register<ITest2, Test2>();
            container.Register<Test3>();
            container.Register<ITest1, Test1>();

            using (var child = container.CreateChildContainer())
            using (var scope = child.BeginScope())
            {
                test4 = scope.Resolve<ITest1>();
                test5 = scope.Resolve<ITest2>();
                test6 = scope.Resolve<Test3>();
            }

            Assert.IsTrue(test4.Disposed);
            Assert.IsTrue(test5.Test1.Disposed);
            Assert.IsTrue(test6.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Scoped_Transient_Singleton()
        {
            var container = new StashboxContainer(config => config.WithDisposableTransientTracking());


            container.Register<ITest2, Test2>();
            container.Register<Test3>();
            container.RegisterSingleton<ITest1, Test1>();

            ITest1 test4;
            ITest2 test5;
            Test3 test6;

            using (var child = container.BeginScope())
            {
                test4 = child.Resolve<ITest1>();
                test5 = child.Resolve<ITest2>();
                test6 = child.Resolve<Test3>();

                Assert.IsFalse(test4.Disposed);
                Assert.IsFalse(test5.Test1.Disposed);
                Assert.IsFalse(test6.Test1.Disposed);
            }

            Assert.IsFalse(test4.Disposed);
            Assert.IsFalse(test5.Test1.Disposed);
            Assert.IsFalse(test6.Test1.Disposed);

            container.Dispose();

            Assert.IsTrue(test4.Disposed);
            Assert.IsTrue(test5.Test1.Disposed);
            Assert.IsTrue(test6.Test1.Disposed);
        }

        [TestMethod]
        public void DisposeTests_TrackTransientDisposal_Implementation_Has_Disposable()
        {
            IStashboxContainer container = new StashboxContainer(config => config.WithDisposableTransientTracking());
            ITest11 test1;

            container.Register<ITest11, Test4>();

            using (var child = container.BeginScope())
            {
                test1 = (ITest11)child.Resolve(typeof(ITest11));
            }

            Assert.IsTrue(((Test4)test1).Disposed);
        }

        [TestMethod]
        public void DisposeTests_Instance_TrackTransient()
        {
            var test = new Test1();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.RegisterInstanceAs<ITest1>(test);

                Assert.AreSame(test, container.Resolve<ITest1>());
            }

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_WireUp_TrackTransient()
        {
            ITest1 test = new Test1();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.WireUpAs(test);

                Assert.AreSame(test, container.Resolve<ITest1>());
            }

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_WireUp_TrackTransient_WithoutDisposal()
        {
            ITest1 test = new Test1();
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.WireUpAs(test, withoutDisposalTracking: true);

                Assert.AreSame(test, container.Resolve<ITest1>());
            }

            Assert.IsFalse(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Factory()
        {
            ITest1 test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest1, Test1>(context => context.WithFactory(() => new Test1()));
                test = container.Resolve<ITest1>();
            }

            Assert.IsFalse(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Factory_TrackTransient()
        {
            ITest1 test;
            using (var container = new StashboxContainer(config => config.WithDisposableTransientTracking()))
            {
                container.Register<ITest1, Test1>(context => context.WithFactory(() => new Test1()));
                test = container.Resolve<ITest1>();
            }

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Factory_Scoped()
        {
            ITest1 test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest1, Test1>(context => context.WithLifetime(new ScopedLifetime()).WithFactory(() => new Test1()));
                test = container.Resolve<ITest1>();
            }

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Factory_Scoped_WithoutTracking()
        {
            ITest1 test;
            using (var container = new StashboxContainer())
            {
                container.Register<ITest1, Test1>(context => context.WithLifetime(new ScopedLifetime()).WithFactory(() => new Test1()).WithoutDisposalTracking());
                test = container.Resolve<ITest1>();
            }

            Assert.IsFalse(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Multiple_Dispose_Call()
        {
            var container = new StashboxContainer();
            container.RegisterSingleton<ITest1, Test1>();
            var test = container.Resolve<ITest1>();

            container.Dispose();
            container.Dispose();

            Assert.IsTrue(test.Disposed);
        }

        [TestMethod]
        public void DisposeTests_Scoped_Multiple_Dispose_Call()
        {
            var container = new StashboxContainer();
            container.RegisterScoped<ITest1, Test1>();
            var scope = container.BeginScope();
            var test = scope.Resolve<ITest1>();

            scope.Dispose();
            scope.Dispose();

            Assert.IsTrue(test.Disposed);
        }

        interface ITest11 { }

        interface ITest12 { }

        interface ITest1 : ITest11, ITest12, IDisposable { bool Disposed { get; } }

        interface ITest2 { ITest1 Test1 { get; } }

        class Test1 : ITest1
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                {
                    throw new ObjectDisposedException(nameof(Test1));
                }

                this.Disposed = true;
            }
        }

        class Test2 : ITest2
        {
            public ITest1 Test1 { get; private set; }

            public Test2(ITest1 test1)
            {
                this.Test1 = test1;
            }
        }

        class Test3
        {
            [Dependency]
            public ITest1 Test1 { get; set; }
        }

        class Test4 : ITest11, IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                if (this.Disposed)
                {
                    throw new ObjectDisposedException(nameof(Test4));
                }

                this.Disposed = true;
            }
        }
        class Test5
        {
            [Dependency("d2")]
            public ITest1 Test1 { get; set; }
        }
    }
}