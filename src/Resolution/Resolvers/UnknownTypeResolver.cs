﻿using Stashbox.Registration;
using Stashbox.Registration.Fluent;
using System;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Resolvers
{
    internal class UnknownTypeResolver : IResolver, ILookup
    {
        public bool CanLookupService(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.CanUseForResolution(typeInfo, resolutionContext);

        public bool CanUseForResolution(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled &&
            !resolutionContext.UnknownTypeCheckDisabled &&
            typeInfo.Type.IsResolvableType() ||
            resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator != null;

        public Expression GetExpression(
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext)
        {
            var name = typeInfo.DependencyName;
            var configurator = name != null
                ? context =>
                {
                    context.WithName(name);
                    resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator?.Invoke(context);
                }
            : resolutionContext.RequestInitiatorContainerContext.ContainerConfiguration.UnknownTypeConfigurator;

            var registrationConfigurator = new UnknownRegistrationConfigurator(typeInfo.Type, typeInfo.Type);
            configurator?.Invoke(registrationConfigurator);

            if (!registrationConfigurator.TypeMapIsValid(out _) || registrationConfigurator.RegistrationShouldBeSkipped)
                return null;

            var registration = RegistrationBuilder.BuildServiceRegistration(resolutionContext.RequestInitiatorContainerContext,
                registrationConfigurator, false);
            ServiceRegistrator.Register(resolutionContext.RequestInitiatorContainerContext, registration, typeInfo.Type);

            return resolutionStrategy.BuildExpressionForRegistration(registration, resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext,
                typeInfo);
        }
    }
}
