using System;
using System.Linq;
using System.Linq.Expressions;

namespace Common
{
    public static class ExpressionExtensions
    {
        public static ParameterExpression[] CloneParameters(this LambdaExpression self)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return self.Parameters.Select(p => Expression.Parameter(p.Type, p.Name)).ToArray();
        }

        public static Expression<Func<TDerived, bool>> Cast<TBase, TDerived>(this Expression<Func<TBase, bool>> self)
            where TDerived : TBase
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            return Expression.Lambda<Func<TDerived, bool>>(self.Body, self.Parameters);
        }

        #region [ReplaceTargetWith]

        public static Expression ReplaceTargetWith(this Expression self, Expression target, Expression with)
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (with == null)
                throw new ArgumentNullException(nameof(with));

            var visitor = new ReplaceExpressionVisitor(target, with);
            return visitor.Visit(self);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _replaceTarget;
            private readonly Expression _replaceWith;

            public ReplaceExpressionVisitor(Expression replaceTarget, Expression replaceWith)
            {
                _replaceTarget = replaceTarget;
                _replaceWith = replaceWith;
            }

            public override Expression Visit(Expression node)
            {
                return node == _replaceTarget ? _replaceWith : base.Visit(node);
            }
        }

        #endregion

        #region [GetMethodNameFromExpression]

        public static string GetMethodNameFromExpression<T>(Expression<Action<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMethodNameFromExpression(expression.Body);
        }

        public static string GetMethodNameFromExpression<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return GetMethodNameFromExpression(expression.Body);
        }

        public static string GetMethodNameFromExpression(Expression expression)
        {
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null)
                throw new InvalidOperationException("expression is not MethodCallExpression.");

            return methodCallExpression.Method.Name;
        }

        #endregion
    }
}