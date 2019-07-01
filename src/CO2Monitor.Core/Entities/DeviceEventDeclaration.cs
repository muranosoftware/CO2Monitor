using System;
using System.Collections.Generic;
using System.Text;

namespace CO2Monitor.Core.Entities
{
    public class DeviceEventDeclaration
    {
        public DeviceEventDeclaration(string name, ValueDeclaration dataDeclaration)
        {
            Name = name;
            DataType = dataDeclaration;
        }

        public string Name { get; private set; }
        public ValueDeclaration DataType {get; private set;}


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var other = (DeviceEventDeclaration)obj;
            return (this.DataType == other.DataType) && (this.Name == other.Name);
        }

        public override int GetHashCode()
        {
            return DataType.GetHashCode() + 431 * (Name == null ? 0 : Name.GetHashCode());
        }
    }
}
