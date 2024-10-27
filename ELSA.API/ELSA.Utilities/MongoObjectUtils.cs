using System.Linq.Expressions;

namespace ELSA.CodeChallenge.Utilities
{
    public static class NameCollector
    {
        private static readonly string expressionCannotBeNullMessage = "The expression cannot be null.";
        public static string Get<T>(Expression<Func<T, object>> expression)
        {
            return GetMemberName(expression.Body);
        }
        private static string GetMemberName(Expression expression, string path = "")
        {
            if (expression == null)
            {
                throw new ArgumentException(expressionCannotBeNullMessage);
            }

            var memberName = string.Empty;
            Expression nextExpression = null;

            if (expression is MemberExpression memberExpression)
            {
                memberName = memberExpression.Member.Name;
                nextExpression = memberExpression.Expression;
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                if (methodCallExpression.Method.Name == "SetFirstMatch")
                {
                    memberName = "$";
                }
                else if (methodCallExpression.Method.Name == "SetAllMatch")
                {
                    var constExpression = methodCallExpression.Arguments.LastOrDefault() as ConstantExpression;
                    memberName = "$[" + constExpression.Value + "]";
                }

                nextExpression = methodCallExpression.Arguments.FirstOrDefault();
            }

            if (expression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is MethodCallExpression unaryMethodExpression)
                {
                    memberName = unaryMethodExpression.Method.Name;
                    nextExpression = unaryMethodExpression.Arguments.FirstOrDefault();
                }
                else if (unaryExpression.Operand is MemberExpression unaryMemberExpression)
                {
                    memberName = unaryMemberExpression.Member.Name;
                    nextExpression = unaryMemberExpression.Expression;
                }
            }

            if (!string.IsNullOrEmpty(memberName))
            {
                path = memberName + (string.IsNullOrEmpty(path) ? string.Empty : "." + path);
            }

            if (nextExpression != null)
            {
                return GetMemberName(nextExpression, path);
            }

            return path;
        }
    }
}
