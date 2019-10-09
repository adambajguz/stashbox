﻿using Stashbox.Configuration;
using System;
using System.Collections.Generic;

namespace Stashbox
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public interface IResolutionScope : IDisposable
    {
        /// <summary>
        /// The parent scope.
        /// </summary>
        IResolutionScope ParentScope { get; }

        /// <summary>
        /// True if the scope contains scoped instances, otherwise false.
        /// </summary>
        bool HasScopedInstances { get; }

        /// <summary>
        /// The name of the scope, if it's null then it's a regular nameless scope.
        /// </summary>
        object Name { get; }

        /// <summary>
        /// Adds or updates an instance in the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="name">The identifier.</param>
        void AddScopedInstance(Type key, object value, object name = null);

        /// <summary>
        /// Gets an instance from the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="name">The identifier.</param>
        /// <returns>The item or null if it doesn't exists.</returns>
        object GetScopedInstanceOrDefault(Type key, object name = null);

        /// <summary>
        /// Adds a service for further disposable tracking.
        /// </summary>
        /// <typeparam name="TDisposable">The type parameter.</typeparam>
        /// <param name="disposable">The <see cref="IDisposable"/> object.</param>
        /// <returns>The <see cref="IDisposable"/> object.</returns>
        TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable;

        /// <summary>
        /// Adds a service with a cleanup delegate.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="finalizable">The object to cleanup.</param>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The object to cleanup.</returns>
        TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer);

        /// <summary>
        /// Gets or adds an item to the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="sync">The object use for synchronization.</param>
        /// <param name="factory">The value factory used if the item doesn't exist yet.</param>
        /// <returns>The scoped item.</returns>
        object GetOrAddScopedItem(int key, object sync, Func<IResolutionScope, object> factory);

        /// <summary>
        /// Invalidates the delegate cache.
        /// </summary>
        void InvalidateDelegateCache();

        /// <summary>
        /// Gets the names of the already opened scopes.
        /// </summary>
        /// <returns>The scope names.</returns>
        ISet<object> GetActiveScopeNames();

        /// <summary>
        /// Called by every node of the internal graph when the <see cref="ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled"/> is true.
        /// Checks for runtime circular dependencies in the compiled delegates.
        /// </summary>
        /// <param name="key">The key of the dependency.</param>
        /// <param name="type">The type of the dependency.</param>
        void CheckRuntimeCircularDependencyBarrier(int key, Type type);

        /// <summary>
        /// Called by every node of the internal graph when the <see cref="ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled"/> is true.
        /// Resets the runtime circular dependency checks state for a node.
        /// </summary>
        /// <param name="key"></param>
        void ResetRuntimetCircularDependencyBarrier(int key);
    }
}
