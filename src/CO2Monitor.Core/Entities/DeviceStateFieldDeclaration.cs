using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Entities
{
    public class DeviceStateFieldDeclaration
    {
        public DeviceStateFieldDeclaration(string name, ValueDeclaration fieldType)
        {
            Name = name;
            Type = fieldType;
        }

        public string Name { get; private set; }
        public ValueDeclaration Type { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var other = (DeviceStateFieldDeclaration)obj;
            return (this.Type == other.Type) && (this.Name == other.Name);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + 431 * (Name == null ? 0 : Name.GetHashCode());
        }
    }
}
