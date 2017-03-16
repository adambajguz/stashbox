﻿using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal abstract class ObjectBuilderBase : IObjectBuilder
    {
        private readonly IContainerContext containerContext;

        protected ObjectBuilderBase(IContainerContext containerContext)
        {
            this.containerContext = containerContext;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var decorators = this.containerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null) return this.GetExpressionInternal(resolutionInfo, resolveType);

            var expression = this.GetExpressionInternal(resolutionInfo, resolveType);

            foreach (var decoratorRegistration in decorators)
            {
                resolutionInfo.ExpressionOverrides.AddOrUpdate(resolveType, expression, (oldValue,newValue) => newValue);
                expression = decoratorRegistration.GetExpression(resolutionInfo, resolveType);
            }

            return expression;
        }

        protected abstract Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType);

        public virtual bool HandlesObjectDisposal => false;

        public virtual void CleanUp()
        { }
    }
}
