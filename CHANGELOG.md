# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v5.8.2] - 2023-03-29
### Fixed
- [#133](https://github.com/z4kn4fein/stashbox/issues/133): In some cases, open generic constraint validation rejected resolution requests for generic arguments with `struct` constraint.

## [v5.8.1] - 2023-03-29
### Fixed
- [#132](https://github.com/z4kn4fein/stashbox/issues/132): Open generic constraint validation rejected resolution requests for interface type generic arguments with `class` constraint. 

## [v5.8.0] - 2023-02-28
### Fixed
- Batch registration (like `.RegisterAssembly()` and `.RegisterTypes()`) produced individual registrations for each interface/base type and implementation type pairs.

  For an example class like `class Sample : ISample1, ISample2 { }` the registration mapping looked like this:

  ```
  ISample1 => NewRegistrationOf(Sample)
  ISample2 => NewRegistrationOf(Sample)
  ```
  
  Now each interface/base type is mapped to the same registration:
  
  ```
   registration = NewRegistrationOf(Sample)
   ISample1 => registration
   ISample2 => registration
  ```

### Changed
- There are cases where the above fix for batch registration indirectly breaks the following service type filter format:
  ```cs
  container.RegisterAssemblyContaining<ISample>(configurator: options =>
      {
          if (options.ServiceType == typeof(IService))
              context.WithScopedLifetime();
      });
  ```
  
  This worked before (and still works if the related service implements only a single type) because for each implemented type there was an individual registration configuration object passed to the `configurator` delegate. 
  
  Now it will not work properly if the bound type implements more than one type, as only one object containing each implemented type is passed to the delegate.  
  
  Therefore, to still support this case, a new service type checker method was introduced:
  ```cs
  container.RegisterAssemblyContaining<ISample>(configurator: options =>
      {
          if (options.HasServiceType<IService>()) // or .HasServiceType(typeof(IService))
              context.WithScopedLifetime();
      });
  ```

## [v5.7.1] - 2023-01-20
### Added
- `net7.0` target framework.

### Changed
- Replaced many `typeof()` calls with static type cache.

## [v5.7.0] - 2022-12-19
### Changed
- `ITenantDistributor` now extends `IStashboxContainer` for easier integration.

## [v5.6.0] - 2022-12-06
### Added
- `WhenResolutionPathHas()` & `WhenInResolutionPathOf()` registration options for handling more conditional resolution cases. They extend the original *parent type* and *attribute* conditions with inheritance.

### Fixed
- Name comparison during named scope resolution.

## [v5.5.3] - 2022-11-29
### Fixed
- `IsRegistered()` produced falsy results on requests with dynamically constructed string service names.

## [v5.5.2] - 2022-10-14
### Fixed
- [#119](https://github.com/z4kn4fein/stashbox/issues/119)

## [v5.5.1] - 2022-10-13
### Fixed
- During the resolution of an open generic service, the actual closed generic registration didn't inherit the registration options from the open generic registration.

## [v5.5.0] - 2022-10-12
### Fixed
- Minor registration improvements.

## [v5.4.3] - 2022-09-09
### Fixed
- Named resolution using ResolveAll returns all named and unnamed instances disregarding the WithNamedDependencyResolutionForUnNamedRequests flag. [#118](https://github.com/z4kn4fein/stashbox/issues/118)

## [v5.4.2] - 2022-06-02
### Fixed
- Name conflict on the service resolution delegates.

### Changed
- Make the `name` parameter of the `Resolve()` functions nullable.

## [v5.4.1] - 2022-05-16
### Fixed
- Type load exception when the library was trimmed.

## [v5.4.0] - 2022-05-03
### Changed
- `Resolve<IEnumerable<>>(name)` now returns each service that has the same name.
### Added
- `ResolveAll(name)` that returns each service that has the same name.
### Removed
- Obsolete `Resolve()` method with the `nullResultAllowed` parameter, it was replaced by `ResolveOrDefault()`.
- Each obsolete `ResolveFactory<>()` method as their functionality is equivalent to `Resolve<Func<>>()`.
- Obsolete `.WithRuntimeCircularDependencyTracking()` container configuration option in favor of [parameterized factory delegates](https://z4kn4fein.github.io/stashbox/docs/guides/advanced-registration#consider-this-before-using-the-resolver-parameter-inside-a-factory).

## [v5.3.0] - 2022-04-10
### Added
- `WithDynamicResolution()` registration option to indicate that the service's resolution should be handled by a dynamic `Resolve()` call on the current `IDependencyResolver` instead of a pre-built instantiation expression.
- Support for resolving custom [Delegate](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers#delegate) types alongside `Func<>`.

## [v5.2.1] - 2022-03-12
### Fixed
- Consolidate `Resolve()` API, using method overloads instead of optional parameters.

## [v5.2.0] - 2022-03-07
### Fixed
- Unable to resolve IHubContext. [#114](https://github.com/z4kn4fein/stashbox/issues/114)

### Added
- Null-safety by enabling null-state analysis.
- Option to exclude a factory's result from dispose tracking, even if it would be tracked by default. This gives the ability to decide within the factory delegate that the result should be tracked or not.
  ```cs
  .Register<Service>(options => options
      .WithFactory<IRequestContext>(requestContext => 
          requestContext.ExcludeFromTracking(/* get an existing or instantiate a new service */)
      )
  );
  ```
- A new `ResolveFactoryOrDefault()` method that allows `null` results.
- A new `ResolveOrDefault()` method that allows `null` results.
- `ValueTuple<,>` [metadata](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers#metadata--tuple) support.

### Changed
- `Resolve()` with the `nullResultAllowed` parameter became obsolete, it was replaced by `ResolveOrDefault()`.
- Each `ResolveFactory<>()` method became obsolete as their functionality is equivalent to `Resolve<Func<>>()`.

### Removed
- `nullResultAllowed` parameter of `ResolveFactory()`.

## [v5.1.0] - 2022-02-27
### Changed
- Marked the `.WithRuntimeCircularDependencyTracking()` container configuration option as **Obsolete** in favor of [parameterized factory delegates](https://z4kn4fein.github.io/stashbox/docs/guides/advanced-registration#consider-this-before-using-the-resolver-parameter-inside-a-factory).

## [v5.0.1] - 2022-02-10
### Changed
- Converted the `ServiceContext` to a read-only struct.
- Made the `.AsServiceContext()` extension method of `Expression` public.

## [v5.0.0] - 2022-02-09
### Added
- Additional [metadata registration](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers#metadata--tuple) option.
- Support for requesting a [service along with its identifier](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers#keyvaluepair--readonlykeyvalue).
- Support for [per-request lifetime](https://z4kn4fein.github.io/stashbox/docs/guides/lifetimes#per-request-lifetime).
- New, clearer API for wrapper extensions.

### Fixed
- There was a bug in the expression compiler that resulted in wrong IL generation in case of value types inside `IEnumerable<>`.

### Changed
- `Tuple<>` requests are not resolved with services in all its items anymore. It's became part of the newly introduced [resolution with metadata](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers#metadata--tuple) feature.
- The `IResolver` interface became the base for the newly introduced `IServiceWrapper` and `IServiceResolver` interfaces. These became the main entrypoints for [container extensions](https://z4kn4fein.github.io/stashbox/docs/advanced/wrappers-resolvers).
- To make the dependency overrides available in factories the `IResolutionContext` was bound to the generated expression tree and the compiled delegate. ([#105](https://github.com/z4kn4fein/stashbox/issues/105)) This temporary solution could lead issues as the resolution context is static between the compiled delegates, however the dependency overrides are not.

  To resolve this, a new `IRequestContext` parameter is introduced for each compiled factory delegate that can be used to access overrides. (The same context object is used to produce and track per-request services)
  ```cs
  container.Register<Service>(options => options.
      WithFactory<IRequestContext>(requestContext => 
  {
     // access the overrides through: requestContext.GetOverrides()
     // or: requestContext.GetDependencyOverrideOrDefault<OverrideType>()
  }))
  ```

### Removed
- Support of circular dependencies with `Lazy<>` along with the `.WithCircularDependencyWithLazy()` container configuration option.

## [v4.1.0] - 2021-11-21
### Fixed
- `IsRegistered()` returns `true` only when the container has a registration with the given type (and name).
- `CanResolve()` returns `true` only when at least one of the following is true:
   - The given type is registered in the current or one of the parent containers.
   - The given type is a closed generic type and its open generic definition is registered.
   - The given type is a wrapper (`IEnumerable<>`, `Lazy<>`, `Func<>`, or `Tuple<>`) and the underlying type is registered.
   - The given type is not registered but it's resolvable and the [unknown type resolution](https://z4kn4fein.github.io/stashbox/docs/advanced/special-resolution-cases#unknown-type-resolution) is enabled.

## [v4.0.0] - 2021-11-18
### Removed
- .NET 4.0 support.

## [v3.6.4] - 2021-08-31
### Added
- `Skip()` method for `UnknownRegistrationConfigurator` used to prevent specific types from auto injection. [#105](https://github.com/z4kn4fein/stashbox/issues/105)
- Parameterized `WithFactory()` option for runtime type based registrations and decorators. [#105](https://github.com/z4kn4fein/stashbox/issues/105)

## [v3.6.3] - 2021-05-26
### Fixed
- Resolving Func uses the wrong constructor. [#102](https://github.com/z4kn4fein/stashbox/issues/102)
- Base class InjectionMethod not populated. [#103](https://github.com/z4kn4fein/stashbox/issues/103)

## [v3.6.2] - 2021-04-23
### Fixed
- Rare NullReferenceException on Resolve. [#101](https://github.com/z4kn4fein/stashbox/issues/101)
- Decorators having `IEnumerable<TDecoratee>` dependency were not handled correctly.


## [v3.6.1] - 2021-03-16
### Fixed
- **Lifetime validation for scoped services requested from root scope.**
The validation was executed only at the expression tree building phase, so an already built scoped factory invoked on the root scope was able to bypass the lifetime validation and store the instance as a singleton. Now the validation runs at every request.

## [v3.6.0] - 2021-02-25

[API changes](https://www.fuget.org/packages/Stashbox/3.6.0/lib/netstandard2.1/diff/3.5.1/)

### Added
- Parameterized factory delegates. [Read more](https://z4kn4fein.github.io/stashbox/docs/guides/advanced-registration#factory-registration). Also, [here](https://z4kn4fein.github.io/stashbox/docs/configuration/registration-configuration#factory) is the list of the new factory configuration methods.
- Multiple conditions from the same type are now combined with **OR** logical operator. [Read more](https://z4kn4fein.github.io/stashbox/docs/guides/service-resolution#conditional-resolution).
- Named version of the `.WhenDecoratedServiceIs()` decorator condition. [Read more](https://z4kn4fein.github.io/stashbox/docs/advanced/decorators#conditional-decoration).

### Deprecated
- `.InjectMember()` registration configuration option. `.WithDependencyBindig()` should be used instead. [Read more](https://z4kn4fein.github.io/stashbox/docs/configuration/registration-configuration#dependency-configuration).

### Removed
- The `GetRegistrationOrDefault(type, resolutionContext, name)` method of the `IRegistrationRepository` interface.
- Some properties of the `RegistrationContext` class were moved to internal visibility.

## [v3.5.1] - 2021-02-19
### Fixed
- When a singleton registration was replaced with `.ReplaceExisting()`, the container still used the old instance. [#98](https://github.com/z4kn4fein/stashbox/issues/98)

## [v3.5.0] - 2021-01-31
### Added
- Assembly scanning:
   - Added option to filter service types and disable self-registration.
   - Recognize generic definitions.
- Support to covariant/contravariant generic type resolution.

### Fixed
- Services with named scope lifetime were not chosen right from the registration repo.

## [v3.4.0] - 2020-11-15
### Added
- The core components of multitenant functionality.
- Throw `ObjectDisposedException` when the container or scope is used after their disposal.

## [v3.3.0] - 2020-11-05
### Added
- Option to rebuild singletons in child container with dependencies overridden in it.

### Fixed
- Singleton instances were built when the Validate() was called, now just the expression is generated for them.

## [v3.2.9] - 2020-11-02
### Added
- Option to replace a registration only if an existing one is registered with the same type or name.

## [v3.2.8] - 2020-10-17
### Changed
- Switch to license expression in nuget package. [#95](https://github.com/z4kn4fein/stashbox/issues/95)

## [v3.2.7] - 2020-10-16
### Changed
- Minor bugfixes.

## [v3.2.6] - 2020-10-16
### Added
- The Validate() method now throws an AggregateException containing all the underlying exceptions.

### Changed
- Minor bugfixes.

## [v3.2.5] - 2020-10-12

### Changed
- Minor bugfixes.

## [v3.2.4] - 2020-07-22
### Added
- The `.WhenDecoratedServiceHas()` and `.WhenDecoratedServiceIs()` decorator configuration options.

## [v3.2.2] - 2020-07-21
### Added
- Support of conditional and lifetime managed decorators [#93](https://github.com/z4kn4fein/stashbox/issues/93)      

## [v3.2.1] - 2020-07-09
### Fixed
- Factory resolution didn't use the built-in expression compiler.

## [v3.2.0] - 2020-06-29
### Added
- IAsyncDisposable support [#90](https://github.com/z4kn4fein/stashbox/issues/90)
 - It works on >=net461, >=netstandard2.0 frameworks.
 - On net461 and netstandard2.0 the usage of IAsyncDisposable interface requires the
   Microsoft.Bcl.AsyncInterfaces package, on netstandard2.1 it's part of the framework.

### Fixed
- Resolution with custom parameter values [#91](https://github.com/z4kn4fein/stashbox/issues/91)

## [v3.1.2] - 2020-06-22
### Fixed
- IdentityServer not compatible [#88](https://github.com/z4kn4fein/stashbox/issues/88)
- Call interception [#89](https://github.com/z4kn4fein/stashbox/issues/89)

## [v3.1.1] - 2020-06-11
### Fixed
- String constant is not handled well by the built-in compiler [#86](https://github.com/z4kn4fein/stashbox/issues/86)
- Registration behaviour doesn't respect replacing [#87](https://github.com/z4kn4fein/stashbox/issues/87)

## [v3.1.0] - 2020-06-08
### Fixed
- Nested named resolution could cause stack overflow [#74](https://github.com/z4kn4fein/stashbox/issues/74)
- Improve support for Assemblies loaded into Collectible AssemblyLoadContexts [#73](https://github.com/z4kn4fein/stashbox/issues/73)
- Unknown type resolution does not work recursively [#77](https://github.com/z4kn4fein/stashbox/issues/77)
- Exception when building expressions [#76](https://github.com/z4kn4fein/stashbox/issues/76)
- Bad performance [#79](https://github.com/z4kn4fein/stashbox/issues/79)
- Expected override behaviour not working with scopes [#80](https://github.com/z4kn4fein/stashbox/issues/80)

### Breaking changes:
- `WithUniqueRegistrationIdentifiers()` option has been removed, `WithRegistrationBehavior()` has been added instead.
- Circular dependency tracking is enabled now by default, for runtime tracking the renamed `WithRuntimeCircularDependencyTracking()` option can be used.
- `WithMemberInjectionWithoutAnnotation()` container configuration option has been renamed to `WithAutoMemberInjection()`.
- `SetImplementationType()` option has been added to the registration configuration used when unknown type detected.
- Removed the `GetScopedInstace()` method from the `IResolutionScope`, they are treated as expression overrides now and consumed automatically by the container.
- Lifetimes became stateless and their API has been changed, read [this](https://z4kn4fein.github.io/stashbox/docs/guides/lifetimes) for more info.
- Lifetime validation has been added:
 - Tracking dependencies that has shorter life-span than their direct or indirect parent's.
 - Tracking scoped services resolved from root.
 - The container throws a LifetimeValidationFailedException when the validation fails.
- `PerRequestLifetime` has been renamed to `PerScopedRequestLifetime`.
- `RegisterInstanceAs()` has been removed, every functionality is available on the `RegisterInstance()` methods.
- Service/Implementation type map validation has been added to the non-generic registration methods.
- `InjectionParameter` has been replaced with `KeyValuePair<string, object>`.
- `IserviceRegistration` interface has been removed, only its implementation remained.
- Removed the legacy container extension functionality.
- Removed the support of PCL v259.

[v5.8.2]: https://github.com/z4kn4fein/stashbox/compare/5.8.1...5.8.2
[v5.8.1]: https://github.com/z4kn4fein/stashbox/compare/5.8.0...5.8.1
[v5.8.0]: https://github.com/z4kn4fein/stashbox/compare/5.7.1...5.8.0
[v5.7.1]: https://github.com/z4kn4fein/stashbox/compare/5.7.0...5.7.1
[v5.7.0]: https://github.com/z4kn4fein/stashbox/compare/5.6.0...5.7.0
[v5.6.0]: https://github.com/z4kn4fein/stashbox/compare/5.5.3...5.6.0
[v5.5.3]: https://github.com/z4kn4fein/stashbox/compare/5.5.2...5.5.3
[v5.5.2]: https://github.com/z4kn4fein/stashbox/compare/5.5.1...5.5.2
[v5.5.1]: https://github.com/z4kn4fein/stashbox/compare/5.5.0...5.5.1
[v5.5.0]: https://github.com/z4kn4fein/stashbox/compare/5.4.3...5.5.0
[v5.4.3]: https://github.com/z4kn4fein/stashbox/compare/5.4.2...5.4.3
[v5.4.2]: https://github.com/z4kn4fein/stashbox/compare/5.4.1...5.4.2
[v5.4.1]: https://github.com/z4kn4fein/stashbox/compare/5.4.0...5.4.1
[v5.4.0]: https://github.com/z4kn4fein/stashbox/compare/5.3.0...5.4.0
[v5.3.0]: https://github.com/z4kn4fein/stashbox/compare/5.2.1...5.3.0
[v5.2.1]: https://github.com/z4kn4fein/stashbox/compare/5.2.0...5.2.1
[v5.2.0]: https://github.com/z4kn4fein/stashbox/compare/5.1.0...5.2.0
[v5.1.0]: https://github.com/z4kn4fein/stashbox/compare/5.0.1...5.1.0
[v5.0.1]: https://github.com/z4kn4fein/stashbox/compare/5.0.0...5.0.1
[v5.0.0]: https://github.com/z4kn4fein/stashbox/compare/4.1.0...5.0.0
[v4.1.0]: https://github.com/z4kn4fein/stashbox/compare/4.0.0...4.1.0
[v4.0.0]: https://github.com/z4kn4fein/stashbox/compare/3.6.4...4.0.0
[v3.6.4]: https://github.com/z4kn4fein/stashbox/compare/3.6.3...3.6.4
[v3.6.3]: https://github.com/z4kn4fein/stashbox/compare/3.6.2...3.6.3
[v3.6.2]: https://github.com/z4kn4fein/stashbox/compare/3.6.1...3.6.2
[v3.6.1]: https://github.com/z4kn4fein/stashbox/compare/3.6.0...3.6.1
[v3.6.0]: https://github.com/z4kn4fein/stashbox/compare/3.5.1...3.6.0
[v3.5.1]: https://github.com/z4kn4fein/stashbox/compare/3.5.0...3.5.1
[v3.5.0]: https://github.com/z4kn4fein/stashbox/compare/3.4.0...3.5.0
[v3.4.0]: https://github.com/z4kn4fein/stashbox/compare/3.3.0...3.4.0
[v3.3.0]: https://github.com/z4kn4fein/stashbox/compare/3.2.9...3.3.0
[v3.2.9]: https://github.com/z4kn4fein/stashbox/compare/3.2.8...3.2.9
[v3.2.8]: https://github.com/z4kn4fein/stashbox/compare/3.2.7...3.2.8
[v3.2.7]: https://github.com/z4kn4fein/stashbox/compare/3.2.6...3.2.7
[v3.2.6]: https://github.com/z4kn4fein/stashbox/compare/3.2.5...3.2.6
[v3.2.5]: https://github.com/z4kn4fein/stashbox/compare/3.2.4...3.2.5
[v3.2.4]: https://github.com/z4kn4fein/stashbox/compare/3.2.3...3.2.4
[v3.2.3]: https://github.com/z4kn4fein/stashbox/compare/3.2.2...3.2.3
[v3.2.2]: https://github.com/z4kn4fein/stashbox/compare/3.2.1...3.2.2
[v3.2.1]: https://github.com/z4kn4fein/stashbox/compare/3.2.0...3.2.1
[v3.2.0]: https://github.com/z4kn4fein/stashbox/compare/3.1.2...3.2.0
[v3.1.2]: https://github.com/z4kn4fein/stashbox/compare/3.1.1...3.1.2
[v3.1.1]: https://github.com/z4kn4fein/stashbox/compare/3.1.0...3.1.1
[v3.1.0]: https://github.com/z4kn4fein/stashbox/compare/2.8.9...3.1.0
