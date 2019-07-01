using System;
using System.Collections.Generic;
using System.Linq;

using CO2Monitor.Core.Shared;

namespace CO2Monitor.Core.Entities
{
    public class Value
    {
        private readonly string _string;
        private readonly double _float;
        private readonly TimeSpan _time; 

        public Value()
        {
            Declaration = new ValueDeclaration(ValueTypes.Void);
        }

        public Value(ValueDeclaration declaration, string val)
        {
            Declaration = declaration;
            try
            {
                switch (Declaration.Type)
                {
                    case ValueTypes.Enum:
                        if (!Declaration.EnumValues.Contains(val))
                            throw new CO2MonitorArgumentException(nameof(val), $"Enum [{Declaration}] does not contain value [{val}]");
                        _string = val;
                        break;
                    case ValueTypes.Float:
                        _float = double.Parse(val);
                        break;
                    case ValueTypes.String:
                        _string = val;
                        break;
                    case ValueTypes.Time:
                        _time = TimeSpan.Parse(val);
                        break;
                    case ValueTypes.Void:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                throw new CO2MonitorArgumentException(nameof(val), $"Can not init Value with type [{Declaration}] and string value [{val}]", ex);
            }
        }

        public Value(float value)
        {
            Declaration = new ValueDeclaration(ValueTypes.Float);
            _float = value;
        }

        public Value(TimeSpan time)
        {
            Declaration = new ValueDeclaration(ValueTypes.Time);
            _time = time;
        }

        public Value(string value)
        {
            Declaration = new ValueDeclaration(ValueTypes.String);
            _string = value;
        }

        public Value(string value, IReadOnlyList<string> enumValues)
        {
            Declaration = new ValueDeclaration(ValueTypes.Enum, enumValues);

            if (!enumValues.Contains(value))
                throw new ArgumentException($"value \"{value}\" is not in enumValues [{enumValues.Aggregate((acc, x) => acc + $"\"{x}\" ") }]");
            
            _string = value;
        }

        public ValueDeclaration Declaration { get; }

        public string Enum
        {
            get
            {
                if (Declaration.Type != ValueTypes.Enum)
                    throw new InvalidOperationException($"Value type is {Declaration.Type} not Enum");

                return _string;
            }
        }

        public string String
        {
            get
            {
                switch (Declaration.Type)
                {
                    case ValueTypes.Float:
                        return _float.ToString();
                    case ValueTypes.Time:
                        return _time.ToString();
                    default:
                        return _string ?? "";
                }
            }
        }

        public double Float
        {
            get
            {
                if (Declaration.Type != ValueTypes.Float)
                    throw new InvalidOperationException($"Value type is {Declaration.Type } not Float" );

                return _float;
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (Declaration.Type != ValueTypes.Time)
                    throw new InvalidOperationException($"Value type is {Declaration.Type } not Time");

                return _time;
            }
        }


        public override string ToString()
        {
            switch(Declaration.Type)
            {
                case ValueTypes.Enum:
                case ValueTypes.String:
                    return _string;
                case ValueTypes.Time:
                    return _time.ToString();
                case ValueTypes.Float:
                    return _float.ToString();
                case ValueTypes.Void:
                    return string.Empty;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
