using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Exceptions
{
    /// <summary>
    /// Exception thrown when an environment variable could not be converted
    /// to the desired type.
    /// </summary>
    public class VariableConversionException : Exception
    {
        private const string MESSAGE_FORMAT = "Could not convert \"{0}\" value to \"{1}\" for environment variable \"{2}\".";

        internal VariableConversionException(string variable, string value, Type expetedType)
            : base(string.Format(MESSAGE_FORMAT, value, expetedType.Name, variable))
        {
        }

        internal VariableConversionException(string variable, string value, Type expetedType, Exception innerException)
            : base(string.Format(MESSAGE_FORMAT, value, expetedType.Name, variable), innerException)
        {
        }
    }
}
