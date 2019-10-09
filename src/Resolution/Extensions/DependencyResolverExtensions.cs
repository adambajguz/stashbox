﻿using Stashbox.Exceptions;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the extensions of the <see cref="IDependencyResolver"/>.
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        public static TKey Resolve<TKey>(this IDependencyResolver resolver, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            (TKey)resolver.Resolve(typeof(TKey), nullResultAllowed, dependencyOverrides);


        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="dependencyOverrides">A collection of objects which are used to override certain dependencies of the requested service.</param>
        /// <returns>The resolved object.</returns>
        public static TKey Resolve<TKey>(this IDependencyResolver resolver, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            (TKey)resolver.Resolve(typeof(TKey), name, nullResultAllowed, dependencyOverrides);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        public static Func<TService> ResolveFactory<TService>(this IDependencyResolver resolver, object name = null, bool nullResultAllowed = false) =>
            resolver.ResolveFactory(typeof(TService), name, nullResultAllowed) as Func<TService>;

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        public static Func<T1, TService> ResolveFactory<T1, TService>(this IDependencyResolver resolver, object name = null, bool nullResultAllowed = false) =>
            resolver.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1)) as Func<T1, TService>;

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        public static Func<T1, T2, TService> ResolveFactory<T1, T2, TService>(this IDependencyResolver resolver, object name = null, bool nullResultAllowed = false) =>
            resolver.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2)) as Func<T1, T2, TService>;

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        public static Func<T1, T2, T3, TService> ResolveFactory<T1, T2, T3, TService>(this IDependencyResolver resolver, object name = null, bool nullResultAllowed = false) =>
            resolver.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3)) as Func<T1, T2, T3, TService>;

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <typeparam name="T4">The fourth parameter of the factory.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        public static Func<T1, T2, T3, T4, TService> ResolveFactory<T1, T2, T3, T4, TService>(this IDependencyResolver resolver, object name = null, bool nullResultAllowed = false) =>
            resolver.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3), typeof(T4)) as Func<T1, T2, T3, T4, TService>;

        /// <summary>
        /// Puts an instance into the scope which will be dropped when the scope is being disposed.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="name">The identifier.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The scope.</returns>
        public static IDependencyResolver PutInstanceInScope<TFrom>(this IDependencyResolver resolver, TFrom instance, object name = null, bool withoutDisposalTracking = false) =>
            resolver.PutInstanceInScope(typeof(TFrom), instance, name, withoutDisposalTracking);

        /// <summary>
        /// On the fly activates an object without registering it into the container. If you want to resolve a
        /// registered service use the <see cref="IDependencyResolver.Resolve(Type, bool, object[])" /> instead.
        /// </summary>
        /// <typeparam name="TTo">The service type.</typeparam>
        /// <param name="resolver">The dependency resolver.</param>
        /// <param name="arguments">Optional dependency overrides.</param>
        /// <returns>The built object.</returns>
        public static TTo Activate<TTo>(this IDependencyResolver resolver, params object[] arguments) =>
            (TTo)resolver.Activate(typeof(TTo), arguments);
    }
}
