﻿using Stashbox.ContainerExtension;
using System;
using System.Linq;

namespace Stashbox.Registration
{
    internal class ServiceRegistrator : IServiceRegistrator
    {
        private readonly IContainerContext containerContext;
        private readonly IContainerExtensionManager containerExtensionManager;

        internal ServiceRegistrator(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
        }

        public IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext)
        {
            if (serviceRegistration.IsDecorator)
                return this.Register(serviceRegistration, serviceType, registrationContext.ReplaceExistingRegistration);

            if (registrationContext.AdditionalServiceTypes.Any())
                foreach (var additionalServiceType in registrationContext.AdditionalServiceTypes)
                    this.Register(serviceRegistration, additionalServiceType, registrationContext.ReplaceExistingRegistration);

            return this.Register(serviceRegistration, serviceType, registrationContext.ReplaceExistingRegistration);
        }

        public IStashboxContainer Register(IServiceRegistration serviceRegistration, Type serviceType, bool replace)
        {
            if (serviceRegistration.IsDecorator)
            {
                this.containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, false, replace);
                this.containerContext.Container.RootScope.InvalidateDelegateCache();
            }
            else
                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, false, replace);

            if (replace)
                this.containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, serviceRegistration);

            return this.containerContext.Container;
        }

        public IStashboxContainer ReMap(IServiceRegistration serviceRegistration, Type serviceType, RegistrationContext registrationContext)
        {
            if (serviceRegistration.IsDecorator)
                this.containerContext.DecoratorRepository.AddDecorator(serviceType, serviceRegistration, true, false);
            else
            {
                if (registrationContext.AdditionalServiceTypes.Any())
                    foreach (var additionalServiceType in registrationContext.AdditionalServiceTypes)
                        this.containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, additionalServiceType, true, false);

                this.containerContext.RegistrationRepository.AddOrUpdateRegistration(serviceRegistration, serviceType, true, false);
            }

            this.containerContext.Container.RootScope.InvalidateDelegateCache();

            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.containerContext, serviceRegistration);

            return this.containerContext.Container;
        }
    }
}
