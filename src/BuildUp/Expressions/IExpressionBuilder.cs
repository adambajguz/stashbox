﻿using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal interface IExpressionBuilder
    {
        Expression ConstructBuildUpExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType);

        Expression ConstructBuildUpExpression(
            IContainerContext containerContext,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType);

        Expression ConstructExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Type serviceType);

        Expression ConstructExpression(
            IContainerContext containerContext,
            ResolutionContext resolutionContext,
            Type serviceType);
    }
}
