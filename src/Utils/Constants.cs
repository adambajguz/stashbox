﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal static class Constants
    {
        public static readonly Type ResolutionScopeType = typeof(IResolutionScope);

#if HAS_SERVICEPROVIDER
        public static readonly Type ServiceProviderType = typeof(IServiceProvider);
#endif

        public static readonly Type ResolverType = typeof(IDependencyResolver);

        public static readonly ParameterExpression ResolutionScopeParameter = ResolutionScopeType.AsParameter("scope");

        public static readonly MethodInfo AddDisposalMethod = ResolutionScopeType.GetSingleMethod("AddDisposableTracking");

        public static readonly MethodInfo GetOrAddScopedObjectMethod = ResolutionScopeType.GetSingleMethod("GetOrAddScopedObject");

        public static readonly MethodInfo AddWithFinalizerMethod = ResolutionScopeType.GetSingleMethod("AddWithFinalizer");

        public static readonly MethodInfo CheckRuntimeCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod("CheckRuntimeCircularDependencyBarrier");

        public static readonly MethodInfo ResetRuntimeCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod("ResetRuntimeCircularDependencyBarrier");

        public static readonly MethodInfo BeginScopeMethod = ResolverType.GetSingleMethod("BeginScope");

        public static readonly Type DisposableType = typeof(IDisposable);

        public static readonly Type FuncType = typeof(Func<>);

        public static readonly Type[] EmptyTypes = EmptyArray<Type>();

        public static readonly Type ObjectType = typeof(object);

        public static readonly Type CompositionRootType = typeof(ICompositionRoot);

        public static readonly Type DependencyAttributeType = typeof(Attributes.DependencyAttribute);

        public static readonly Type InjectionAttributeType = typeof(Attributes.InjectionMethodAttribute);

        public const MethodImplOptions Inline = (MethodImplOptions)256;

        public const byte DelegatePlaceholder = 0;

        public static T[] EmptyArray<T>() => InternalArrayHelper<T>.Empty;
    }
}
