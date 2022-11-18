namespace Kuchulem.DotNet.Env.Tests
{
    public class EnvironmentParser_Should
    {
        private const string ErrorMessageFormat = "Property {0}.{1} : {4}\nGot: {2}\nExpected: {3}\n";

        private EnvironmentParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new EnvironmentParser();
            Environment.SetEnvironmentVariable("DB_NAME", "tests_db");
            Environment.SetEnvironmentVariable("DB_USER", "tests_user");
            Environment.SetEnvironmentVariable("DB_PASSWORD", "tests_password");
            Environment.SetEnvironmentVariable("DB_ADDRESS", "tests_address");
            Environment.SetEnvironmentVariable("DB_PORT", "7431");
            Environment.SetEnvironmentVariable("DB_SECRET_FILE", "../../../Data/Secret.txt");
            Environment.SetEnvironmentVariable("DB_UPDATE_DATE", "2022-02-01 00:00:00");
            Environment.SetEnvironmentVariable("DB_TRESHOLD", "6.458");
            Environment.SetEnvironmentVariable("DB_USE_SSL", "true");
            Environment.SetEnvironmentVariable("DB_NO_SSL", "false");
            Environment.SetEnvironmentVariable("DB_MAX_CONNECTIONS_FILE", "../../../Data/Int.txt");
        }

        [Test]
        public void ReturnVariableValueAsString()
        {
            var value = parser.GetVariable("DB_NAME");

            var expected = "tests_db";

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsInt()
        {
            var value = parser.GetVariable<int>("DB_PORT");

            int expected = 7431;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsLong()
        {
            var value = parser.GetVariable<long>("DB_PORT");

            long expected = 7431;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsFloat()
        {
            var value = parser.GetVariable<float>("DB_TRESHOLD");

            float expected = 6.458f;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsDouble()
        {
            var value = parser.GetVariable<double>("DB_TRESHOLD");

            double expected = 6.458;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsDateTime()
        {
            var value = parser.GetVariable<DateTime>("DB_UPDATE_DATE");

            DateTime expected = DateTime.Parse("2022-02-01 00:00:00");

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsBoolTrue()
        {
            var value = parser.GetVariable<bool>("DB_USE_SSL");

            bool expected = true;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsBoolFalse()
        {
            var value = parser.GetVariable<bool>("DB_NO_SSL");

            bool expected = false;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnVariableValueAsIntFromFile()
        {
            var value = parser.GetVariable<int>("DB_MAX_CONNECTIONS");

            int expected = 452;

            Assert.That(value, Is.EqualTo(expected));
        }

        [Test]
        public void ThrowExceptionOnUnkonwnVariable()
        {
            Assert.Throws<Exceptions.VariableNotFoundException>(() => parser.GetVariable("WRONG_VAR_NAME"));
        }

        [Test]
        public void NotThrowExceptionOnUnkonwnVariable()
        {
            Assert.DoesNotThrow(() => parser.GetVariable("WRONG_VAR_NAME", false));
        }

        [Test]
        public void ReturnObjectMappedFromVariables()
        {
            var config = parser.MapVariables<Models.DbConfig>();

            var expected = new Models.DbConfig
            {
                DbAddress = "tests_address",
                DbName = "tests_db",
                DbPassword = "tests_password",
                DbPort = 7431,
                DbUser = "tests_user",
                DbSecret = "this is a secret",
                DbUpdateDate = DateTime.Parse("2022-02-01 00:00:00"),
                DbTreshold = 6.458,
            };

            Assert.That(AreObjectsEqual(config, expected, out string message), message);
        }

        [Test]
        public void PopulateObjectMappedFromVariables()
        {
            var config = new Models.DbConfig();
            parser.MapVariables(config);

            var expected = new Models.DbConfig
            {
                DbAddress = "tests_address",
                DbName = "tests_db",
                DbPassword = "tests_password",
                DbPort = 7431,
                DbUser = "tests_user",
                DbSecret = "this is a secret",
                DbUpdateDate = DateTime.Parse("2022-02-01 00:00:00"),
                DbTreshold = 6.458,
            };

            Assert.That(AreObjectsEqual(config, expected, out string message), message);
        }

        [Test]
        public void ReturnObjectMappedFromVariablesWithUnmappedProperty()
        {
            var config = parser.MapVariables<Models.ObjectWithUnMappedProperties>();

            var expected = new Models.ObjectWithUnMappedProperties
            {
                Treshold = 6.458f,
                UseSsl = true,
                UnmappedProperty = null
            };

            Assert.That(AreObjectsEqual(config, expected, out string message), message);
        }

        [Test]
        public void ThrowExceptionForObjectMappedFromVariablesWithUnmappedProperty()
        {
            Assert.Throws<Exceptions.VariableNotFoundException>(() => parser.MapVariables<Models.ObjectWithUnMappedProperties>(true));
        }

        [Test]
        public void ThrowExceptionForObjectWithWronglyMappedProperty()
        {
            Assert.Throws<Exceptions.VariableConversionException>(() => parser.MapVariables<Models.ObjectWithWronglyMappedProperty>());
        }

        protected bool AreObjectsEqual<T>(T mapped, T expected, out string message)
        {
            message = "";
            var messages = new List<string>();
            bool isValid = true;

            foreach (var property in typeof(T).GetProperties())
            {
                var mappedValue = property.GetValue(mapped);
                var expectedValue = property.GetValue(expected);

                if (!AreEqual(mappedValue, expectedValue, out string error))
                {
                    messages.Add(string.Format(ErrorMessageFormat, typeof(T).Name, property.Name, mappedValue ?? "null", expectedValue ?? "null", error));
                    isValid = false;
                }
            }

            message = string.Join('\n', messages);

            return isValid;
        }

        private bool AreEqual(object? a, object? b, out string message)
        {

            if (a is null && b is null)
            {
                message = "";
                return true;
            }

            if (a is null || b is null)
            {
                message = "One of the values is null";
                return false;
            }

            if (
                a is IEnumerable<object> enumerationA
                && b is IEnumerable<object> enumerationB)
            {
                var result = CompareEnumerations(enumerationA, enumerationB, out message);

                return result;
            }

            if (a is IEnumerable<object> || b is IEnumerable<object>)
            {
                message = "Only one of the values is an enumeration";
                return false;
            }

            if (a.ToString() != b.ToString())
            {
                message = "Values are different";
                return false;
            }

            message = "";
            return true;
        }

        private bool CompareEnumerations(IEnumerable<object> enumerationA, IEnumerable<object> enumerationB, out string message)
        {

            if (enumerationA.Count() != enumerationB.Count())
            {
                message = "Enumerations have different counts";
                return false;
            }

            message = "";

            return true;
        }
    }
}