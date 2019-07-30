using System;
using Newtonsoft.Json.Serialization;

namespace CO2Monitor.Infrastructure.Helpers {
	public class TypeBinder : ISerializationBinder {
		public Type BindToType(string assemblyName, string typeName) =>
			Type.GetType($"{typeName}, {assemblyName}", true);

		public void BindToName(Type serializedType, out string assemblyName, out string typeName) {
			assemblyName = serializedType.Assembly.FullName;
			typeName = serializedType.FullName;
		}
	}
}
