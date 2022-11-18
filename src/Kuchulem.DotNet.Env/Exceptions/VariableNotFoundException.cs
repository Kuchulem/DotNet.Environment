using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Exceptions
{
    /// <summary>
    /// Exception thrown when an environment variable was not found.
    /// </summary>
    public class VariableNotFoundException : Exception
    {
        private const string MESSAGE_FORMAT = "Could not get varialbe \"{0}\" from environment";

        internal VariableNotFoundException(string variable)
            : base(string.Format(MESSAGE_FORMAT, variable))
        {
        }

        internal VariableNotFoundException(string variable, Exception innerException)
            : base(string.Format(MESSAGE_FORMAT, variable), innerException)
        {
        }
    }
}
