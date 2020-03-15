using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression)
            where T : class
        {
            return self == null ? expression : self.CombineBinary(expression, Expression.OrElse);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> expression)
            where T : class
        {
            return self == null ? expression : self.CombineBinary(expression, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Not<T>(Expression<Func<T, bool>> expression)
            where T : class
        {
            return expression == null ? null : expression.ApplyUnary(Expression.Not);
        }

        public static Expression<T> ApplyUnary<T>(this Expression<T> self, Func<Expression, Expression> unaryExpr)
            where T : class
        {
            if (self == null) return null;
            if (unaryExpr == null)
                throw new ArgumentNullException(nameof(unaryExpr));

            var parameters = self.CloneParameters();
            self = self.Update(self.RebindBodyParametersWith(parameters), parameters);

            return Expression.Lambda<T>(unaryExpr(self.Body), parameters);
        }

        public static Expression<Func<T, bool>> CombineWithConst<T, R>(this Expression<Func<T, R>> self, R value, Func<Expression, Expression, BinaryExpression> combinator, ParameterExpression[] parameters = null)
            where T : class
        {
            return self.CombineBinary(p => value, combinator);
        }

        public static Expression<Func<T, bool>> CombineBinary<T, R>(this Expression<Func<T, R>> self, Expression<Func<T, R>> expression, Func<Expression, Expression, BinaryExpression> combinator, ParameterExpression[] parameters = null)
            where T : class
        {
            if (self == null)
                throw new ArgumentNullException(nameof(self));
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));
            if (combinator == null)
                throw new ArgumentNullException(nameof(combinator));

            if (parameters == null)
            {
                // parameters can be on different levels of hierarchy so we should choose higher level parameter as a target for cloning
                parameters = self.Parameters.First().Type.IsAssignableFrom(expression.Parameters.First().Type) ? expression.CloneParameters() : self.CloneParameters();
            }

            self = self.Update(self.RebindBodyParametersWith(parameters), parameters);
            expression = expression.Update(expression.RebindBodyParametersWith(parameters), parameters);
            var combined = combinator(self.Body, expression.Body);

            return Expression.Lambda<Func<T, bool>>(combined, parameters);
        }

        public static Expression RebindBodyParametersWith(this LambdaExpression expression, IEnumerable<ParameterExpression> parameters)
        {
            return new ParameterRebinder(expression.Parameters, parameters).Visit(expression.Body);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly IDictionary<ParameterExpression, ParameterExpression> _parameterMapping;

            public ParameterRebinder(IEnumerable<ParameterExpression> candidates, IEnumerable<ParameterExpression> replacements)
            {
                _parameterMapping = candidates.Zip(replacements, (c, r) => new { Candidate = c, Replacement = r }).ToDictionary(t => t.Candidate, t => t.Replacement);
            }

            protected override Expression VisitParameter(ParameterExpression expression)
            {
                ParameterExpression replacement;
                return _parameterMapping.TryGetValue(expression, out replacement) ? replacement : expression;
            }
        }
    }
}