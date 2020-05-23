﻿using Stashbox.Entity;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Resolution.Resolvers
{
    internal class DefaultValueResolver : IResolver
    {
        public Expression GetExpression(IContainerContext containerContext,
            IResolutionStrategy resolutionStrategy,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            typeInfo.Type.AsDefault();

        public bool CanUseForResolution(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            containerContext.ContainerConfiguration.DefaultValueInjectionEnabled &&
                 (typeInfo.Type.GetTypeInfo().IsValueType
                    || typeInfo.Type == typeof(string));
    }
}
