using Kuchulem.DotNet.Env.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuchulem.DotNet.Env.Tests.Models
{
    public class ObjectWithWronglyMappedProperty
    {
        [MapEnvVar("DB_NAME")]
        public bool UseSsl { get; set; }
    }
}
