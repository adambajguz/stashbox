﻿using System;
using System.Collections.Generic;
using Stashbox.Entity;
using Stashbox.Configuration;
using Stashbox.Entity.Resolution;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a registration context. Allows a fluent registration configuration.
    /// </summary>
    public interface IRegistrationContext
    {
        /// <summary>
        /// The type that will be requested.		
        /// </summary>		
        Type TypeFrom { get; }

        /// <summary>		
        /// The type that will be returned.		
        /// </summary>		
        Type TypeTo { get; }

        /// <summary>
        /// The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/>
        /// </summary>
        IContainerContext ContainerContext { get; }

        /// <summary>
        /// Sets the lifetime of the registration.
        /// </summary>
        /// <param name="lifetime">An <see cref="ILifetime"/> implementation.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithLifetime(ILifetime lifetime);

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithName(string name);

        /// <summary>
        /// Sets injection parameters for the registration.
        /// </summary>
        /// <param name="injectionParameters">The injection parameters.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithInjectionParameters(params InjectionParameter[] injectionParameters);

        /// <summary>
        /// Sets a parameterless factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactory(Func<object> singleFactory);

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithFactory(Func<IStashboxContainer, object> containerFactory);

        /// <summary>
        /// Enables auto member injection on the registration.
        /// </summary>
        /// <param name="rule">The auto member injection rule.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithAutoMemberInjection(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        /// <param name="rule">The constructor selection rule.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithConstructorSelectionRule(Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> rule);

        /// <summary>
        /// Sets an instance as the resolution target of the registration.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithInstance(object instance);

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <typeparam name="TTarget">The type of the dependant.</typeparam>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenDependantIs<TTarget>(string dependencyName = null) where TTarget : class;

        /// <summary>
        /// Sets a dependant target condition for the registration.
        /// </summary>
        /// <param name="targetType">The type of the dependant.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenDependantIs(Type targetType, string dependencyName = null);

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenHas<TAttribute>() where TAttribute : Attribute;

        /// <summary>
        /// Sets an attribute condition for the registration.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WhenHas(Type attributeType);

        /// <summary>
        /// Sets a generic condition for the registration.
        /// </summary>
        /// <param name="resolutionCondition">The predicate.</param>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext When(Func<TypeInformation, bool> resolutionCondition);

        /// <summary>
        /// Tells the container that it shouldn't track the resolved transient object for disposal.
        /// </summary>
        /// <returns>The <see cref="IRegistrationContext"/> which on this method was called.</returns>
        IRegistrationContext WithoutDisposalTracking();

        /// <summary>
        /// Creates a service registration from the registration context.
        /// </summary>
        /// <param name="isDecorator">True if we are requesting a decorator registration, otherwise false.</param>
        /// <returns>The created service registration.</returns>
        IServiceRegistration CreateServiceRegistration(bool isDecorator = false);

        /// <summary>
        /// Registers the registration into the container.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer Register();

        /// <summary>
        /// Replaces an already registered service.
        /// </summary>
        /// <returns>The container.</returns>
        IStashboxContainer ReMap();
    }
}
