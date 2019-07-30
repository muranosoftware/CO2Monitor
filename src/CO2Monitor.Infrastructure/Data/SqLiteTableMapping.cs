using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CO2Monitor.Infrastructure.Data {
	public class SqLiteTableMapping<TEntity> where TEntity : new() {
		private static readonly Dictionary<Type, string> SqLiteTypeMapping = new Dictionary<Type, string> {
			{ typeof(int), "INTEGER" },
			{ typeof(short), "INTEGER" },
			{ typeof(long), "INTEGER" },
			{ typeof(uint), "INTEGER" },
			{ typeof(ushort), "INTEGER" },
			{ typeof(ulong), "INTEGER" },
			{ typeof(int?), "INTEGER" },
			{ typeof(short?), "INTEGER" },
			{ typeof(long?), "INTEGER" },
			{ typeof(uint?), "INTEGER" },
			{ typeof(ushort?), "INTEGER" },
			{ typeof(ulong?), "INTEGER" },

			{ typeof(string), "TEXT" },
			{ typeof(Enum), "TEXT" },
			{ typeof(DateTime), "TEXT" },
			{ typeof(DateTime?), "TEXT" },

			{ typeof(float), "REAL" },
			{ typeof(double), "REAL" },
			{ typeof(float?), "REAL" },
			{ typeof(double?), "REAL" },
		};

		private static readonly string _createSqlFormat;
		private static readonly string _updateSqlFormat;
		private static readonly string _deleteSqlFormat;
		private static readonly string _createTableSqLiteFormat;

		public SqLiteTableMapping(string table) {
			SelectFromRelation = table;
			CreateSql = string.Format(_createSqlFormat, table);
			UpdateSql = string.Format(_updateSqlFormat, table);
			DeleteSql = string.Format(_deleteSqlFormat, table);
			CreateTableSql = string.Format(_createTableSqLiteFormat, table);
		}

		static SqLiteTableMapping() {
			var sbTableCreate = new StringBuilder();
			sbTableCreate.Append("CREATE TABLE IF NOT EXISTS {0} ( ");

			PropertyInfo[] props = typeof(TEntity).GetProperties(BindingFlags.Instance | 
																 BindingFlags.Public | 
																 BindingFlags.SetProperty | 
																 BindingFlags.GetProperty);

			var sbCreate = new StringBuilder();
			sbCreate.Append("INSERT INTO {0} (");

			var sbUpdate = new StringBuilder();
			sbUpdate.Append("UPDATE {0} SET ");

			foreach (PropertyInfo pi in props) {
				if (!SqLiteTypeMapping.ContainsKey(pi.PropertyType)) {
					throw new NotSupportedException($"Can not map type [{pi.PropertyType.FullName}] to SqlLite type");
				}

				sbTableCreate.Append(pi.Name);
				sbTableCreate.Append(" ");
				sbTableCreate.Append(SqLiteTypeMapping[pi.PropertyType]);

				if (pi.Name == "Id") {
					sbTableCreate.Append(" PRIMARY KEY AUTOINCREMENT,\r\n");
					continue;
				} else {
					sbTableCreate.Append(",\r\n");
				}

				sbCreate.Append(pi.Name);
				sbCreate.Append(",");

				sbUpdate.Append(pi.Name);
				sbUpdate.Append(" = @");
				sbUpdate.Append(pi.Name);
				sbUpdate.Append(",");
			}
			
			sbCreate[sbCreate.Length - 1] = ')'; // remove last ,
			sbCreate.Append(" VALUES (");

			sbUpdate[sbUpdate.Length - 1] = ' '; // remove last ,

			foreach (PropertyInfo pi in props) {
				if (pi.Name == "Id") {
                    continue;
                }

                sbCreate.Append("@");
				sbCreate.Append(pi.Name);
				sbCreate.Append(",");
			}
			sbCreate[sbCreate.Length - 1] = ')';

			_createSqlFormat = sbCreate.ToString();

			sbUpdate.Append("WHERE Id = @Id");

			_updateSqlFormat = sbUpdate.ToString();

			_deleteSqlFormat = "DELETE FROM {0} WHERE Id = @Id";

			sbTableCreate[sbTableCreate.Length - 3] = ')'; // remove last ,

			_createTableSqLiteFormat = sbTableCreate.ToString();
		}

		public string SelectFromRelation { get; }

		public string CreateSql { get; }

		public string UpdateSql { get; }

		public string DeleteSql { get; }

		public string CreateTableSql { get; }
	}
}
