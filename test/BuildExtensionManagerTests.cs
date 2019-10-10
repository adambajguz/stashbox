﻿using Moq;
using Stashbox.ContainerExtension;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using Xunit;

namespace Stashbox.Tests
{

    public class BuildExtensionManagerTests
    {
        [Fact]
        public void BuildExtensionManagerTests_AddPostBuildExtension()
        {
            var post = new Mock<IPostBuildExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);
                var obj = new object();
                container.Register<object>(context => context.WithFactory(() => obj));

                post.Setup(p => p.PostBuild(obj, container.ContainerContext, It.IsAny<ResolutionContext>(),
                    It.IsAny<IServiceRegistration>(), It.IsAny<Type>())).Returns(obj).Verifiable();

                var inst = container.Resolve(typeof(object));
                post.Verify(p => p.Initialize(container.ContainerContext));
            }

            post.Verify(p => p.CleanUp());
        }

        [Fact]
        public void BuildExtensionManagerTests_AddPostBuildExtension_WithDefaultReg()
        {
            var post = new Mock<IPostBuildExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);
                container.Register<ITest, Test>();

                bool called = false;
                post.Setup(p => p.PostBuild(It.IsAny<object>(), container.ContainerContext, It.IsAny<ResolutionContext>(),
                    It.IsAny<IServiceRegistration>(), It.IsAny<Type>())).Returns(It.IsAny<object>()).Callback(() => called = true).Verifiable();

                var inst = container.Resolve<ITest>();
                Assert.True(called);

                post.Verify(p => p.Initialize(container.ContainerContext));
            }

            post.Verify(p => p.CleanUp());
        }

        [Fact]
        public void BuildExtensionManagerTests_AddRegistrationBuildExtension()
        {
            var post = new Mock<IRegistrationExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);
                container.RegisterInstanceAs(new object());
                post.Verify(p => p.Initialize(container.ContainerContext));
                post.Verify(p => p.OnRegistration(container.ContainerContext, It.IsAny<IServiceRegistration>()));
            }

            post.Verify(p => p.CleanUp());
        }

        [Fact]
        public void BuildExtensionManagerTests_CreateCopy()
        {
            var post = new Mock<IRegistrationExtension>();
            var post2 = new Mock<IRegistrationExtension>();
            using (var container = new StashboxContainer())
            {
                container.RegisterExtension(post.Object);

                post.Setup(p => p.CreateCopy()).Returns(post2.Object).Verifiable();

                using (var child = container.CreateChildContainer())
                {
                    child.RegisterInstanceAs(new object());

                    post2.Verify(p => p.Initialize(child.ContainerContext));
                    post2.Verify(p => p.OnRegistration(child.ContainerContext, It.IsAny<IServiceRegistration>()));
                }

                post2.Verify(p => p.CleanUp());
            }

            post.Verify(p => p.CleanUp());
        }

        interface ITest { }

        class Test : ITest { }
    }
}
