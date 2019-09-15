﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using Stashbox.Utils;

namespace Stashbox.Tests
{
    [TestClass]
    public class WireUpTests
    {
        [TestMethod]
        public void WireUp_Multiple()
        {
            using (var container = new StashboxContainer())
            {
                var test1 = new Test1();
                container.WireUpAs<ITest1>(test1);

                var test2 = new Test();
                container.WireUpAs<ITest>(test2);

                var inst = container.Resolve<ITest1>();
                var inst2 = container.Resolve<ITest>();

                Assert.AreSame(test1, inst);
                Assert.AreSame(test2, inst2);
            }
        }

        [TestMethod]
        public void WireUp_Multiple_Named()
        {
            using (var container = new StashboxContainer())
            {
                var test1 = new Test();
                container.WireUpAs<ITest>(test1, "test1");

                var test2 = new Test();
                container.WireUpAs<ITest>(test2, "test2");

                var inst = container.Resolve<ITest>("test1");
                var inst2 = container.Resolve<ITest>("test2");

                Assert.AreSame(test1, inst);
                Assert.AreSame(test2, inst2);
            }
        }

        [TestMethod]
        public void WireUpTests_InjectionMember()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();

                var test1 = new Test1();
                container.WireUpAs<ITest1>(test1);

                container.Register<Test2>();

                var inst = container.Resolve<Test2>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test1);
                Assert.IsInstanceOfType(inst, typeof(Test2));
                Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test1.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void WireUpTests_InjectionMember_ServiceUpdated()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();

                var test1 = new Test1();
                container.WireUpAs<ITest1>(test1);

                container.ReMap<ITest, Test>();

                container.Register<Test2>();

                var inst = container.Resolve<Test2>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test1);
                Assert.IsInstanceOfType(inst, typeof(Test2));
                Assert.IsInstanceOfType(inst.Test1, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test1.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void WireUpTests_InjectionMember_WithoutService()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                var test1 = new Test1();
                container.WireUp(test1);
                var inst = container.Resolve<Test1>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test);
                Assert.IsInstanceOfType(inst, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test, typeof(Test));
            }
        }

        [TestMethod]
        public void WireUpTests_WithoutService_NonGeneric()
        {
            using (var container = new StashboxContainer())
            {
                container.Register<ITest, Test>();
                object test1 = new Test1();
                container.WireUp(typeof(Test1), test1);
                var inst = container.Resolve<Test1>();

                Assert.IsNotNull(inst);
                Assert.IsNotNull(inst.Test);
                Assert.IsNotNull(inst.test);
                Assert.IsInstanceOfType(inst, typeof(Test1));
                Assert.IsInstanceOfType(inst.Test, typeof(Test));
                Assert.IsInstanceOfType(inst.test, typeof(Test));
            }
        }

        interface ITest { }

        interface ITest1 { ITest Test { get; } }

        class Test : ITest { }

        class Test1 : ITest1
        {
            [Dependency]
            public ITest test;

            [Dependency]
            public ITest Test { get; set; }

            [InjectionMethod]
            public void Init()
            {
                Shield.EnsureNotNull(Test, nameof(Test));
            }
        }

        class Test2
        {
            public ITest1 Test1 { get; set; }

            public Test2(ITest1 test1)
            {
                this.Test1 = test1;
            }
        }
    }
}
