﻿using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the base of the fluent registration api.
    /// </summary>
    /// <typeparam name="TConfigurator"></typeparam>
    public class BaseFluentConfigurator<TConfigurator> : RegistrationConfiguration, IBaseFluentConfigurator<TConfigurator>
        where TConfigurator : BaseFluentConfigurator<TConfigurator>
    {
        internal BaseFluentConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        /// <inheritdoc />
        public TConfigurator WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            var length = injectionParameters.Length;
            for (int i = 0; i < length; i++)
                this.AddInjectionParameter(injectionParameters[i]);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInjectionParameter(string name, object value)
        {
            this.AddInjectionParameter(new InjectionParameter { Name = name, Value = value });
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithAutoMemberInjection(Rules.AutoMemberInjectionRules rule = Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter, Func<MemberInfo, bool> filter = null)
        {
            this.Context.AutoMemberInjectionEnabled = true;
            this.Context.AutoMemberInjectionRule = rule;
            this.Context.MemberInjectionFilter = filter;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> rule)
        {
            this.Context.ConstructorSelectionRule = rule;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorByArgumentTypes(params Type[] argumentTypes)
        {
            var constructor = this.ImplementationType.GetConstructorByTypes(argumentTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argumentTypes);

            this.Context.SelectedConstructor = constructor;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithConstructorByArguments(params object[] arguments)
        {
            var argTypes = arguments.Select(arg => arg.GetType()).ToArray();
            var constructor = this.ImplementationType.GetConstructorByTypes(argTypes);
            if (constructor == null)
                this.ThrowConstructorNotFoundException(this.ImplementationType, argTypes);

            this.Context.SelectedConstructor = constructor;
            this.Context.ConstructorArguments = arguments;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator InjectMember(string memberName, object dependencyName = null)
        {
            this.Context.InjectionMemberNames.Add(memberName, dependencyName);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithDependencyBinding(Type dependencyType, object dependencyName)
        {
            Shield.EnsureNotNull(dependencyType, nameof(dependencyType));
            Shield.EnsureNotNull(dependencyName, nameof(dependencyName));

            this.Context.DependencyBindings.Add(dependencyType, dependencyName);

            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithDependencyBinding(string parameterName, object dependencyName)
        {
            Shield.EnsureNotNull(parameterName, nameof(parameterName));
            Shield.EnsureNotNull(dependencyName, nameof(dependencyName));

            this.Context.DependencyBindings.Add(parameterName, dependencyName);

            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithoutDisposalTracking()
        {
            this.Context.IsLifetimeExternallyOwned = true;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator ReplaceExisting()
        {
            this.Context.ReplaceExistingRegistration = true;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithoutFactoryCache()
        {
            this.Context.FactoryCacheDisabled = true;
            return (TConfigurator)this;
        }

        private void AddInjectionParameter(InjectionParameter injectionParameter)
        {
            var store = (List<InjectionParameter>)this.Context.InjectionParameters;
            store.Add(injectionParameter);
        }

        private void ThrowConstructorNotFoundException(Type type, params Type[] argTypes)
        {
            if (argTypes.Length == 0)
                throw new ConstructorNotFoundException(type);

            if (argTypes.Length == 1)
                throw new ConstructorNotFoundException(type, argTypes[0]);

            throw new ConstructorNotFoundException(type, argTypes);
        }
    }
}
