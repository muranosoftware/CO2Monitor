using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CO2Monitor.Infrastructure.Helpers.Dapper {
	internal class ExpressionToSql {
		private int _parametersCount;
		private readonly Dictionary<ParameterExpression, string> _relations = new Dictionary<ParameterExpression, string>();

		private static readonly IReadOnlyDictionary<ExpressionType, string> SqlOpDict = new Dictionary<ExpressionType, string> {
			{ ExpressionType.Equal, " = " },
			{ ExpressionType.NotEqual, " <> " },
			{ ExpressionType.LessThan, " < " },
			{ ExpressionType.GreaterThan, " > " },
			{ ExpressionType.LessThanOrEqual, " <= " },
			{ ExpressionType.GreaterThanOrEqual, " >= " },
			{ ExpressionType.OrElse, " OR " },
			{ ExpressionType.AndAlso, " AND " },
		};

		internal ExpressionToSql(LambdaExpression exp, DynamicParameters parameters, string[] relations) {
			Parameters = parameters;
			_parametersCount = Parameters.ParameterNames.Count();
			int i = 0;
			foreach (ParameterExpression p in exp.Parameters) {
				_relations.Add(p, relations[i++]);
			}

			Sql = PredicateBodyParse(exp.Body);
		}

		private string Sql { get; }
		private DynamicParameters Parameters { get; }

		private string PredicateBodyParse(Expression body) {
			if (!(body is BinaryExpression)) {
				throw new NotSupportedException($"Expression ({body}) must be binary");
			}

			var binExp = (BinaryExpression)body;
			switch (binExp.NodeType) {
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.LessThan:
				case ExpressionType.GreaterThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThanOrEqual: {
					string left = GetSqlFromMemberAcessOrConstant(binExp.Left);
					string right = GetSqlFromMemberAcessOrConstant(binExp.Right);
					if (left == null) {
						throw new ArgumentException("Expression can not contains compration with left side null object:" + binExp);
					}

					if (right == null) {
						if (binExp.NodeType == ExpressionType.Equal) {
							return left + " IS NULL";
						}

						if (binExp.NodeType == ExpressionType.NotEqual) {
							return left + " IS NOT NULL";
						}
					}

					return left + SqlOpDict[body.NodeType] + right;
				}
				case ExpressionType.AndAlso:
				case ExpressionType.OrElse: {
					var left = PredicateBodyParse(binExp.Left);
					var right = PredicateBodyParse(binExp.Right);

					return left + SqlOpDict[body.NodeType] + right;
				}
				default:
					throw new NotSupportedException($"Expression ({binExp}) has unsupported type: [{binExp.NodeType}]");
			}
		}

		private string GetSqlFromMemberAcessOrConstant(Expression exp) {
			switch (exp) {
				case MemberExpression memberExp:
					if (memberExp.Expression == null) { // static 
						return AddParameter(GetPropertyOrFieldValue(memberExp, null));
					} else {
						switch (memberExp.Expression) {
							case ParameterExpression paramExp:
								return _relations[paramExp] + "." + memberExp.Member.Name;
							case ConstantExpression _: //may be closure 
							case MemberExpression _: //may be local variable access 
								return AddParameter(GetNestedMemberConstantValue(memberExp));
							default:
								throw new NotSupportedException($"Expression({memberExp.Expression}) has unsupported type: [{memberExp.Expression.NodeType}]");
						}
					}
				case ConstantExpression constExp:
					return AddParameter(constExp.Value);
				default:
					throw new NotSupportedException($"Expression({exp}) has unsupported type: [{exp.NodeType}]");
			}
		}

		private object GetPropertyOrFieldValue(MemberExpression memberExp, object obj) {
			MemberInfo memberInfo = memberExp.Member;

			switch (memberInfo) // static properties 
			{
				case FieldInfo fieldInfo:
					return fieldInfo.GetValue(obj);
				case PropertyInfo propertyInfo:
					return propertyInfo.GetValue(obj);
				default:
					throw new NotSupportedException($"Member expression ({memberExp}) has unsupported member access: [{memberInfo.MemberType}]");
			}
		}

		/// <summary>
		/// Return true if expression is nested member access of constant. 
		/// Return false if expression contains expression parameter access. 
		/// Otherwise raise exception 
		/// </summary>
		private object GetNestedMemberConstantValue(MemberExpression memberExp) {
			MemberExpression exp = memberExp;
			var memberStack = new Stack<MemberExpression>();

			do {
				memberStack.Push(exp);
			} while ((exp = exp.Expression as MemberExpression) != null);

			MemberExpression top = memberStack.Peek();

			if (top.Expression is ParameterExpression) {
				throw new NotSupportedException($"Nested parameter member access expression ({memberExp}) is not supported");
			}

			if (!(top.Expression is ConstantExpression)) {
				throw new NotSupportedException($"Can not process expression({memberExp})");
			}

			object obj = ((ConstantExpression)top.Expression).Value;

			do {
				obj = GetPropertyOrFieldValue(memberStack.Pop(), obj);
			} while (memberStack.Count != 0);

			return obj;
		}

		private static string GetColumnFromPropertySelector<T, TSelector>(Expression<Func<T, TSelector>> selector) {
			var lambda = (LambdaExpression)selector;

			switch (lambda.Body) {
				case MemberExpression memberExp:
					if (memberExp.Expression == null) {
						MemberInfo memberInfo = memberExp.Member;
						throw new NotSupportedException($"Member expression ({memberExp}) has unsupported member access: [{memberInfo.MemberType}]");
					} else {
						switch (memberExp.Expression) {
							case ParameterExpression _:
								return memberExp.Member.Name;
							default:
								if (memberExp.NodeType == ExpressionType.MemberAccess) {
									throw new NotSupportedException($"Member expression ({memberExp}) has unsupported nested member access. Use simple selectors: x => x.Prop");
								} else {
									throw new NotSupportedException($"Selector body expression ({memberExp}) has unsupported type: [{memberExp.NodeType}]. Use simple selectors: x => x.Prop");
								}
						}
					}
				default:
					throw new NotSupportedException($"Selector expression body ({lambda.Body}) has unsupported type: [{lambda.Body.NodeType}]. Use simple selectors: x => x.Prop");
			}
		}

		private string AddParameter(object obj) {
			if (obj == null) {
				return null;
			}

			var param = "Param" + _parametersCount++;

			Parameters.Add(param, obj);

			return "@" + param;
		}

		public static string Select<T, TOrderBy>(Expression<Func<T, bool>> where,
			                                     DynamicParameters parameters,
			                                     string relation,
			                                     Expression<Func<T, TOrderBy>> orderBySelector = null,
			                                     bool orderByDesc = false,
			                                     uint? limit = null) {
			var sql = $"SELECT * FROM {relation} ";
			if (where != null) {
				var ptsc = new ExpressionToSql(where, parameters, new[] { relation });
				sql += (string.IsNullOrWhiteSpace(ptsc.Sql) ? "" : $"WHERE {ptsc.Sql}");
			}

			if (orderBySelector != null) {
				string col = GetColumnFromPropertySelector(orderBySelector);
				sql += " ORDER BY " + col + (orderByDesc ? " DESC " : "");
			}

			if (limit.HasValue) {
				sql += $" LIMIT {limit} ";
			}

			return sql;
		}

		public static string CreateIndexOnProperty<T, TProperty>(Expression<Func<T, TProperty>> selector, string relation) {
			string col = GetColumnFromPropertySelector(selector);
			return $"CREATE INDEX IF NOT EXISTS idx_{relation}_{col} ON {relation} ({col});";
		}
	}
}