using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using CO2Monitor.Infrastructure.Devices;

namespace CO2Monitor.Infrastructure.Helpers {
	public class PropRenAndIgnDepInjSerializerContractResolver : DefaultContractResolver {
		private readonly Dictionary<Type, HashSet<string>> _ignores;
		private readonly Dictionary<Type, Dictionary<string, string>> _renames;
		private readonly Dictionary<Type, IDeviceBuilder> _deviceBuilders;

		public PropRenAndIgnDepInjSerializerContractResolver(IServiceProvider serviceProvider) {
			_ignores = new Dictionary<Type, HashSet<string>>();
			_renames = new Dictionary<Type, Dictionary<string, string>>();
			_deviceBuilders = serviceProvider.GetServices<IDeviceBuilder>().ToDictionary(x => x.DeviceType);
		}

		public void IgnoreProperty(Type type, params string[] jsonPropertyNames) {
			if (!_ignores.ContainsKey(type))
				_ignores[type] = new HashSet<string>();

			foreach (var prop in jsonPropertyNames)
				_ignores[type].Add(prop);
		}

		public void RenameProperty(Type type, string propertyName, string newJsonPropertyName) {
			if (!_renames.ContainsKey(type))
				_renames[type] = new Dictionary<string, string>();

			_renames[type][propertyName] = newJsonPropertyName;
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			if (IsIgnored(property.DeclaringType, property.PropertyName)) {
				property.ShouldSerialize = i => false;
				property.Ignored = true;
			}

			if (IsRenamed(property.DeclaringType, property.PropertyName, out var newJsonPropertyName))
				property.PropertyName = newJsonPropertyName;

			return property;
		}

		protected override JsonObjectContract CreateObjectContract(Type objectType) {
			if (_deviceBuilders.ContainsKey(objectType)) {
				IDeviceBuilder builder = _deviceBuilders[objectType];
				JsonObjectContract contract = base.CreateObjectContract(objectType);
				contract.DefaultCreator = () => builder.CreateDevice();
				return contract;
			}

			return base.CreateObjectContract(objectType);
		}

		private bool IsIgnored(Type type, string jsonPropertyName) {
			if (!_ignores.ContainsKey(type))
				return false;

			return _ignores[type].Contains(jsonPropertyName);
		}

		private bool IsRenamed(Type type, string jsonPropertyName, out string newJsonPropertyName) {
			if (_renames.TryGetValue(type, out Dictionary<string, string> renames) && renames.TryGetValue(jsonPropertyName, out newJsonPropertyName)) {
				return true;
			}
			newJsonPropertyName = null;
			return false;
		}
	}
}
