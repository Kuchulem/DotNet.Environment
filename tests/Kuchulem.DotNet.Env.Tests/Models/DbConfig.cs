using Kuchulem.DotNet.Env.Attributes;

namespace Kuchulem.DotNet.Env.Tests.Models
{
    internal class DbConfig
    {
        [MapEnvVar("DB_NAME")]
        public string? DbName { get; set; }

        [MapEnvVar("DB_USER")]
        public string? DbUser { get; set; }

        [MapEnvVar("DB_PASSWORD")]
        public string? DbPassword { get; set; }

        [MapEnvVar("DB_ADDRESS")]
        public string? DbAddress { get; set; }

        [MapEnvVar("DB_PORT")]
        public int DbPort { get; set; }

        [MapEnvVar("DB_Secret")]
        public string? DbSecret { get; set; }

        [MapEnvVar("DB_UPDATE_DATE")]
        public DateTime DbUpdateDate { get; set; }

        [MapEnvVar("DB_TRESHOLD")]
        public double DbTreshold { get; set; }
    }
}
