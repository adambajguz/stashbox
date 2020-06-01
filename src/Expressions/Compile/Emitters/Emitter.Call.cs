﻿#if IL_EMIT
using System.Linq.Expressions;
using System.Reflection.Emit;
using Stashbox.Expressions.Compile.Extensions;

namespace Stashbox.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this MethodCallExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            if (expression.Object != null && !expression.Object.TryEmit(generator, context, parameters))
                return false;

            return expression.Arguments.TryEmit(generator, context, parameters) &&
                generator.EmitMethod(expression.Method);
        }
    }
}
#endif
