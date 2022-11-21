using Kuchulem.DotNet.Env.Attributes;
using Kuchulem.DotNet.Env.Exceptions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Kuchulem.DotNet.Env
{
    /// <summary>
    /// An <see cref="EnvironmentParser"/> instance provides mechanics to read and convert
    /// environment variables.<br/>
    /// The <see cref="EnvironmentParser"/> should be instantiated for usage.<br/>
    /// With the parser you can :
    /// <list type="bullet">
    ///     <item>Read environment variable</item>
    ///     <item>Read and cast environment variable</item>
    ///     <item>Read environment variable from a file</item>
    ///     <item>Map environment variables to an item properties</item>
    /// </list>
    /// </summary>
    public class EnvironmentParser
    {
        private const string FILE_ENV_SUFFIX = "_FILE";
        private readonly string fileEnvSuffix;

        public EnvironmentParser(string fileEnvSuffix = FILE_ENV_SUFFIX)
        {
            this.fileEnvSuffix = fileEnvSuffix;
        }

        /// <summary>
        /// Populated an object with environment variables.<br/>
        /// The mapping from variables names to properties in case insensitive and does not
        /// takes account of underscores ("_") in varialbe names.<br/>
        /// Values are cast to the object properties types.<br/>
        /// </summary>
        /// <typeparam name="T">The type of the object to populate</typeparam>
        /// <param name="destination">The object to populate</param>
        /// <param name="throwIfNullOrEmpty">
        ///     If true, any unset environment variable will throw an exception. If false, the
        ///     default value of the mapped property is used Defaut to false.
        /// </param>
        /// <exception cref="MissingVariableNameInMapEnvVarAttributeException"></exception>
        /// <exception cref="VariableConversionException"></exception>
        /// <exception cref="VariableNotFoundException"></exception>
        public void MapVariables<T>(T destination, bool throwIfNullOrEmpty = false)
        {
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                string variable = property.Name;

                var attribute = property.GetCustomAttributes(typeof(MapEnvVarAttribute), true).FirstOrDefault();
                if (attribute is MapEnvVarAttribute mapAttribute)
                    variable = mapAttribute.VariableName
                        ?? throw new MissingVariableNameInMapEnvVarAttributeException(property);
                var value = GetVariable(variable, throwIfNullOrEmpty);

                if(value != null)
                {
                    try
                    {
                        var converted = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture);
                        property.SetValue(destination, converted);
                    }
                    catch (FormatException e)
                    {
                        throw new VariableConversionException(variable, value, property.PropertyType, e);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new instance of type <em>T</em> populated with environment variables.<br/>
        /// The mapping from variables names to properties in case insensitive and does not
        /// takes account of underscores ("_") in variable names.<br/>
        /// Values are cast to the object properties types.<br/>
        /// </summary>
        /// <typeparam name="T">The type of the object to populate and return</typeparam>
        /// <param name="throwIfNullOrEmpty">
        ///     If true, any unset environment variable will throw an exception. If false, the
        ///     default value of the mapped property is used Defaut to false.
        /// </param>
        /// <returns>An new instance of T populated with properties mapped from environment variables</returns>
        /// <exception cref="MissingVariableNameInMapEnvVarAttributeException"></exception>
        /// <exception cref="VariableConversionException"></exception>
        /// <exception cref="VariableNotFoundException"></exception>
        public T MapVariables<T>(bool throwIfNullOrEmpty = false)
            where T : class, new()
        {
            var destination = new T();

            MapVariables(destination, throwIfNullOrEmpty);

            return destination;
        }

        /// <summary>
        /// Gets a variable from evironment variables and returns it value.<br/>
        /// If the mapper fiends a variable with <em>_FILE</em> suffix then reads the
        /// first line of the file and use it as the value.<br/>
        /// Exemple :
        /// <code>
        /// // Get a value from variable MY_ENV_VAR="some string"
        /// var value = parser.GetVariable("MY_ENV_VAR"); // value = "some string"
        /// 
        /// // Get a value from a file in an env var : MY_SECRET_FILE=/var/secret.txt
        /// var secret = parser.GetVariable("MY_SECRET"); // value = `file content`
        /// </code>
        /// </summary>
        /// <param name="variable">The variable name to read the data from</param>
        /// <param name="throwIfNullOrEmpty">
        ///     If true will throw an exception if variable is not set. Default
        ///     to true
        /// </param>
        /// <returns></returns>
        /// <exception cref="VariableNotFoundException"></exception>
        public string GetVariable(string variable, bool throwIfNullOrEmpty = true)
        {
            if (Environment.GetEnvironmentVariable(variable) is string value && !string.IsNullOrEmpty(value))
                return value;

            if (Environment.GetEnvironmentVariable($"{variable}{fileEnvSuffix}") is string valueFile && !string.IsNullOrEmpty(valueFile))
            {
                value = File.ReadLines(valueFile).FirstOrDefault()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            if (throwIfNullOrEmpty)
                throw new VariableNotFoundException(variable);

            return null;
        }

        /// <summary>
        /// Gets an environement variable value and converts it to the specified type.
        /// </summary>
        /// <param name="variable">The variable name to read the data from</param>
        /// <param name="convertTo">The type used when converting the value</param>
        /// <param name="throwIfNullOrEmpty">
        ///     If true will throw an exception if variable is not set. Default
        ///     to true
        /// </param>
        /// <returns></returns>
        /// <exception cref="VariableConversionException"></exception>
        /// <exception cref="VariableNotFoundException"></exception>
        public object GetVariable(string variable, Type convertTo, bool throwIfNullOrEmpty = true)
        {
            object converted = null;
            var value = GetVariable(variable, throwIfNullOrEmpty);

            if (value != null)
            {
                try
                {
                    converted = Convert.ChangeType(value, convertTo, CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {
                    throw new VariableConversionException(variable, value, convertTo, e);
                }
            }

            if (converted != null)
                return converted;

            if (throwIfNullOrEmpty)
                throw new VariableNotFoundException(variable);

            return default;
        }

        /// <summary>
        /// Gets an environement variable value and converts it to the specified type.
        /// </summary>
        /// <typeparam name="T">The type used when converting the value</typeparam>
        /// <param name="variable">The variable name to read the data from</param>
        /// <param name="throwIfNullOrEmpty">
        ///     If true will throw an exception if variable is not set. Default
        ///     to true
        /// </param>
        /// <returns></returns>
        /// <exception cref="VariableConversionException"></exception>
        /// <exception cref="VariableNotFoundException"></exception>
        public T GetVariable<T>(string variable, bool throwIfNullOrEmpty = true)
        {
            var converted = GetVariable(variable, typeof(T), throwIfNullOrEmpty);

            if (converted != null)
               return (T)converted;

            if (throwIfNullOrEmpty)
                throw new VariableNotFoundException(variable);

            return default;
        }
    }
}