using System;
using System.Linq.Expressions;

namespace CO2Monitor.Infrastructure.Helpers {
	public class PredicateBuilder<T> {
		private readonly ParameterExpression _parameter;
		private Expression _body;

		public PredicateBuilder() {
			_parameter = Expression.Parameter(typeof(T));
		}

		public Expression<Func<T, bool>> Predicate => (Expression<Func<T, bool>>)Expression.Lambda(_body ?? Expression.Constant(true), _parameter);

		public bool IsEmpty => _body == null;

		public void AndAlso(Expression<Func<T, bool>> predicate) {
			var lambda = predicate as LambdaExpression;

			_body = _body == null ? lambda.Body : Expression.AndAlso(_body, lambda.Body);
		}
	}
}
