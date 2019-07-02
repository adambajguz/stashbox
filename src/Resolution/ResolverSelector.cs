﻿using Stashbox.Entity;
using Stashbox.Resolution.Resolvers;
using Stashbox.Utils;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolverSelector : IResolverSelector
    {
        private ArrayStore<IMultiServiceResolver> multiServiceResolverRepository;
        private ArrayStore<IResolver> resolverRepository;
        private readonly UnknownTypeResolver unknownTypeResolver;
        private readonly ParentContainerResolver parentContainerResolver;

        public ResolverSelector()
        {
            this.resolverRepository = ArrayStore<IResolver>.Empty;
            this.multiServiceResolverRepository = ArrayStore<IMultiServiceResolver>.Empty;
            this.unknownTypeResolver = new UnknownTypeResolver();
            this.parentContainerResolver = new ParentContainerResolver();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.resolverRepository.Length; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return true;

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ||
                this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext);
        }

        public Expression GetResolverExpression(IContainerContext containerContext,
            TypeInformation typeInfo,
            ResolutionContext resolutionContext,
            bool forceSkipUnknownTypeCheck = false)
        {
            for (var i = 0; i < this.resolverRepository.Length; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetExpression(containerContext, typeInfo, resolutionContext);
            }

            if (this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                return this.parentContainerResolver.GetExpression(containerContext, typeInfo, resolutionContext);

            return !forceSkipUnknownTypeCheck && this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext)
                ? this.unknownTypeResolver.GetExpression(containerContext, typeInfo, resolutionContext)
                : null;
        }

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.multiServiceResolverRepository.Length; i++)
            {
                var item = this.multiServiceResolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetExpressions(containerContext, typeInfo, resolutionContext);
            }

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ? this.parentContainerResolver.GetExpressions(containerContext, typeInfo, resolutionContext) : null;
        }

        public void AddResolver(IResolver resolver)
        {
            Swap.SwapValue(ref this.resolverRepository, (t1, t2, t3, t4, repo) =>
                repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            if (resolver is IMultiServiceResolver multiServiceResolver)
                Swap.SwapValue(ref this.multiServiceResolverRepository, (t1, t2, t3, t4, repo) =>
                    repo.Add(t1), multiServiceResolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }
    }
}
