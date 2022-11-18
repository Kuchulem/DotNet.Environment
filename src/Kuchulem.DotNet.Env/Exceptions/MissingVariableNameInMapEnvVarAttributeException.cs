using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Exceptions
{
    /// <summary>
    /// Exception throw when a <see cref="Attributes.MapEnvVarAttribute"/>
    /// is set on a class property without any <see cref="Attributes.MapEnvVarAttribute.VariableName"/>
    /// defined.
    /// </summary>
    public class MissingVariableNameInMapEnvVarAttributeException : Exception
    {
        private const string MESSAGE_FORMAT = "No variable name defined for property {0}.{1}.";

        internal MissingVariableNameInMapEnvVarAttributeException(MemberInfo memberInfo)
            : base(string.Format(MESSAGE_FORMAT, memberInfo.DeclaringType?.Name ?? "object", memberInfo.Name))
        {
        }
    }
}
