using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CO2Monitor.Core.Entities
{
  
    public class DeviceActionDeclaration
    {
        public DeviceActionDeclaration(string path, ValueDeclaration argumentDeclaration)
        {
            Path = path;
            Argument = argumentDeclaration; 
        }

        public string Path { get; }

        public ValueDeclaration Argument { get; set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var other = (DeviceActionDeclaration)obj;
            return (this.Argument.Equals(other.Argument)) && (this.Path == other.Path);
        }

        public override int GetHashCode()
        {
            return Argument.GetHashCode() + 431 * (Path == null ? 0 : Path.GetHashCode());
        }
    }
}
