﻿using Stashbox.BuildUp.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public FactoryObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            MethodCallExpression expr;
            if (serviceRegistration.RegistrationContext.ContainerFactory != null)
            {
                var resolverParam = resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType);
                var method = serviceRegistration.RegistrationContext.ContainerFactory.GetMethod();
                expr = method.IsStatic
                        ? method.InvokeMethod(resolverParam)
                        : method.CallMethod(serviceRegistration.RegistrationContext.ContainerFactory.Target.AsConstant(), resolverParam);

            }
            else
            {
                var method = serviceRegistration.RegistrationContext.SingleFactory.GetMethod();
                expr = method.IsStatic
                        ? method.InvokeMethod()
                        : method.CallMethod(serviceRegistration.RegistrationContext.SingleFactory.Target.AsConstant());
            }

            return this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, expr, resolutionContext, resolveType);
        }
    }
}