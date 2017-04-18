﻿using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Registration
{
    internal class DecoratorRegistrationContext : IDecoratorRegistrationContext
    {
        private readonly RegistrationContext registrationContext;
        private readonly IServiceRegistrator serviceRegistrator;
        private bool replaceExistingRegistration;

        public DecoratorRegistrationContext(RegistrationContext registrationContext, IServiceRegistrator serviceRegistrator)
        {
            this.registrationContext = registrationContext;
            this.serviceRegistrator = serviceRegistrator;
        }

        public IFluentDecoratorRegistrator WithInjectionParameters(params InjectionParameter[] injectionParameters)
        {
            this.registrationContext.WithInjectionParameters(injectionParameters);
            return this;
        }

        public IFluentDecoratorRegistrator WithAutoMemberInjection(
            Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            this.registrationContext.WithAutoMemberInjection(rule);
            return this;
        }

        public IFluentDecoratorRegistrator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule)
        {
            this.registrationContext.WithConstructorSelectionRule(rule);
            return this;
        }

        public IFluentDecoratorRegistrator WithoutDisposalTracking()
        {
            this.registrationContext.WithoutDisposalTracking();
            return this;
        }

        public IStashboxContainer Register() => this.serviceRegistrator.Register(this.registrationContext, true, this.replaceExistingRegistration);

        public IStashboxContainer ReMap() => this.serviceRegistrator.ReMap(this.registrationContext, true);

        public IFluentDecoratorRegistrator ReplaceExisting()
        {
            this.replaceExistingRegistration = true;
            return this;
        }
    }
}
