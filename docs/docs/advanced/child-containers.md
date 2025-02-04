import CodeDescPanel from '@site/src/components/CodeDescPanel';

# Child containers

With child containers, you can build up parent-child relationships between containers. This means you can have a different subset of services present in a child than in the parent container. 

When a dependency is missing from the child container during a resolution request, the parent will be asked to resolve the missing service. If it's found there, the parent will return only the service's registration, and the resolution request will continue with the child. Also, child registrations with the same [service type](/docs/getting-started/glossary#service-type--implementation-type) will override the parent's services.

:::info
Child containers are the foundation of the [ASP.NET Core multi-tenant extension](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#multitenant).
:::

## Example

Here is an example case:
```cs
interface IDependency {}

class B : IDependency {}
class C : IDependency {}

class A 
{
    public A(IDependency dependency)
    { }
}

using (var container = new StashboxContainer())
{
    // register 'A' into the parent container.
    container.Register<A>();

    // register 'B' as a dependency into the parent container.
    container.Register<IDependency, B>();

    using (var child = container.CreateChildContainer())
    {
        // register 'C' as a dependency into the child container.
        child.Register<IDependency, C>();

        // 'A' is resolved from the parent and gets
        // 'C' as IDependency because the resolution
        // request was initiated on the child.
        A fromChild = child.Resolve<A>();
    }

    // 'A' gets 'B' as IDependency because the 
    // resolution request was initiated on the parent.
    A fromParent = container.Resolve<A>();
}
```
Let's see what's happening when we request `A` from the *child*:
1. `A` not found in the *child*, go up to the *parent* and check there.
2. `A` found in the *parent*, resolve.
3. `A` depends on `IDependency`, go back to the *child* and search `IDependency` implementations.
4. `C` found in the *child*, it does not have any dependencies, instantiate.
5. Inject the new `C` instance into `A`.
5. All dependencies are resolved; return `A`.

When we make the same request on the parent, everything will go as usual because we have all dependencies in place. `B` will be injected into `A`.

:::info
You can [re-configure](/docs/configuration/container-configuration) child containers with the `.Configure()` method. It doesn't affect the parent container's configuration.
:::

## Re-building singletons
By default, singletons are instantiated and stored only in those containers that registered them. However, you can enable the re-instantiation of singletons in child containers with the `.WithReBuildSingletonsInChildContainer()` [container configuration option](/docs/configuration/container-configuration#re-build-singletons-in-child-containers). 

If it's enabled, all singletons will be re-created in those containers that initiated the resolution request. By this, re-built singletons can use overridden dependencies from child containers. 

Re-building in child containers does not affect the singletons instantiated in the parent container.

```cs
interface IDependency {}

class B : IDependency {}
class C : IDependency {}

class A 
{
    public A(IDependency dependency)
    { }
}

using (var container = new StashboxContainer(options => options.WithReBuildSingletonsInChildContainer()))
{
    // register 'A' as a singleton into the parent container.
    container.RegisterSingleton<A>();

    // register 'B' as a dependency into the parent container.
    container.Register<IDependency, B>();

    // 'A' gets 'B' as IDependency and will be stored
    // in the parent container as a singleton.
    A fromParent = container.Resolve<A>();

    using (var child = container.CreateChildContainer())
    {
        // register 'C' as a dependency into the child container.
        child.Register<IDependency, C>();

        // a new 'A' singleton will be created in 
        // the child container with 'C' as IDependency
        A fromChild = child.Resolve<A>();
    } 
    // using will dispose the child and the 
    // newly created singleton instance of 'A'
}
// using will dispose the parent and the 
// original singleton instance of 'A'
```

## Nested child containers

<CodeDescPanel>
<div>

You can build up a hierarchical tree structure from containers by creating more child containers with the `.CreateChildContainer()` method.

</div>
<div>

```cs
using var child1 = container.CreateChildContainer();
using var child2 = child1.CreateChildContainer();
```

</div>
</CodeDescPanel>