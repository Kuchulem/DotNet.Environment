using Kuchulem.DotNet.Env.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Tests.Models
{
    public class ObjectWithUnMappedProperties
    {
        [MapEnvVar("DB_TRESHOLD")]
        public float Treshold { get; set; }
        [MapEnvVar("DB_USE_SSL")]
        public bool UseSsl { get; set; }    

        public string? UnmappedProperty { get; set; }
    }
}
