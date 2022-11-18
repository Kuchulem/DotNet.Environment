using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Attributes
{
    /// <summary>
    /// Attribute used to map a property with an environement variable.<br/>
    /// The <see cref="VariableName"/> property defines the name of the
    /// environment variable to map to the property.<br/>
    /// Mapping is ensured by the static class <see cref="EnvironmentParser"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MapEnvVarAttribute : Attribute
    {
        public string VariableName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variableName">
        /// The name of the environment variable to map
        /// </param>
        public MapEnvVarAttribute(string variableName)
        {
            VariableName = variableName;
        }
    }
}
