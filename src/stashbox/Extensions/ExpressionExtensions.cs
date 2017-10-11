﻿using Stashbox;
using Stashbox.Infrastructure;

#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.BuildUp.Expressions.Compile;
#endif

namespace System.Linq.Expressions
{
    /// <summary>
    /// Holds the <see cref="Expression"/> extension methods.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="object"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, object> CompileDelegate(this Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                var instance = ((ConstantExpression)expression).Value;
                return scope => instance;
            }

#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, object>), typeof(object), Constants.ResolutionScopeParameter))
                factory = Expression.Lambda(expression, Constants.ResolutionScopeParameter).Compile();

            return (Func<IResolutionScope, object>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, object>>(expression, Constants.ResolutionScopeParameter).Compile();
#endif
        }

        /// <summary>
        /// Compiles an <see cref="Expression"/> to a <see cref="Func{T,R}"/> of <see cref="IResolutionScope"/>, <see cref="Delegate"/>.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>The compiled delegate.</returns>
        public static Func<IResolutionScope, Delegate> CompileDynamicDelegate(this Expression expression)
        {
#if NET45 || NET40 || NETSTANDARD1_3
            if (!expression.TryEmit(out Delegate factory, typeof(Func<IResolutionScope, Delegate>), typeof(Delegate), Constants.ResolutionScopeParameter))
                factory = Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, Constants.ResolutionScopeParameter).Compile();

            return (Func<IResolutionScope, Delegate>)factory;
#else
            return Expression.Lambda<Func<IResolutionScope, Delegate>>(expression, Constants.ResolutionScopeParameter).Compile();
#endif
        }
    }
}
