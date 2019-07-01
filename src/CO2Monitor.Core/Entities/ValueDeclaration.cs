using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CO2Monitor.Core.Entities
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ValueTypes
    {
        Void,
        Float,
        Enum,
        String,
        Time,
    }

    public class ValueDeclaration
    {
        public ValueDeclaration(ValueTypes type, IReadOnlyList<string> enumValues = null)
        {
            Type = type;

            if (type == ValueTypes.Enum)
            {
                if (enumValues == null || enumValues.Count == 0)
                    throw new ArgumentException("enumValues must be not null and has at least one element!");
            }
            else if ( enumValues != null)
                throw new ArgumentException("Can not set enumValues for non enum ValueDescription");

            EnumValues = enumValues;
        }

        public ValueTypes Type { get; }

        public IReadOnlyList<string> EnumValues { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var other = (ValueDeclaration)obj;
            if (this.Type != other.Type)
                return false;

            if (this.Type == ValueTypes.Enum && this.ToString() != other.ToString())
                return false;
            return true;
        }

        public override string ToString()
        {
            switch (this.Type)
            {
                case ValueTypes.Enum:
                    return ValueTypes.Enum.ToString().ToLower() + ": " + EnumValues.Aggregate((acc, x) => acc + ", " + x);
                default:
                    return Type.ToString().ToLower();
            }
        }

        public static ValueDeclaration Parse(string valueDeclaration)
        {
            if (string.IsNullOrEmpty(valueDeclaration))
                throw new ArgumentException("Declaration can not be null or whitespace");

            var splits = valueDeclaration.Split(':');

            var type = Enum.Parse<ValueTypes>(splits[0].Trim(),  true);
            IReadOnlyList<string> enumValues = null;
            if (type == ValueTypes.Enum)
            {
                if (splits.Length != 2)
                    throw new ArgumentException("Invalid format: " + valueDeclaration);
                enumValues = splits[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
            }
            else
            {
                if(splits.Length != 1)
                    throw new ArgumentException("Invalid format: " + valueDeclaration);
            }

            return new ValueDeclaration(type, enumValues);
        }
    }
}
