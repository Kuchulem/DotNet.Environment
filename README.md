[![NuGet Version](https://img.shields.io/nuget/v/Kuchulem.DotNet.Env?label=Nuget%20version&logo=nuget)](https://www.nuget.org/packages/Kuchulem.DotNet.Env/)
[![NuGet Preversion](https://img.shields.io/nuget/vpre/Kuchulem.DotNet.Env?label=Nuget%20prerelease&logo=nuget)](https://www.nuget.org/packages/Kuchulem.DotNet.Env/)


[![Unit tests](https://github.com/Kuchulem/DotNet.Environment/actions/workflows/tests.yml/badge.svg?branch=main)](https://github.com/Kuchulem/DotNet.Environment/actions/workflows/tests.yml) [![CodeQL](https://github.com/Kuchulem/DotNet.Environment/actions/workflows/codeql.yml/badge.svg?branch=main)](https://github.com/Kuchulem/DotNet.Environment/actions/workflows/codeql.yml)

# Introduction 
The `Kuchulem.DotNet.Env` library provides the `EnvironmentParser` class to manage environment variables.

This class provides advanced mechanics to get environment variables values and map them to an object or class.

# Getting Started

## Add the nuget package to your project

Connect to the feed :

Install the library in your project :

From the CLI (powershell) :

```powershell
Install-Package Kuchulem.DotNet.Env
```

or from the package manager in Visual Studio :

seach `Kuchulem.DotNet.Env` and click on `install`

## Include the package

include the `Kuchulem.DotNet.Env` namespace when you need to use the `EnvironmentParser`.

```csharp
using Kuchulem.DotNet.Env;
```

# Usage

To get environment variable value you have to instanciate the parser :

```csharp
var parser = new EvironmentParser();
```

## Get values

To retrieve the value of `MY_ENV_VAR` environment variable as string :

```csharp
string myEnvVar = parser.GetVariable("MY_ENV_VAR");
```

You can also get a converted value :

```csharp
object dbPort = parser.GetVariable("BD_PORT", typeof(int));

DateTime releaseDate = parser.GetVariable<DateTime>("RELEASE_DATE");
```

## Map environment variables to an object

Usefull when creating configuration objects from environment variables, the
library provides the `MapEnvVar` attribute to configure mapping between environment
variables and object.

Considering the following environement variables :

```
DB_NAME='my_db'
DB_USER='application_user'
DB_PASSWORD='the_password_is_in_keypass'
DB_ADDRESS='127.0.0.1'
DB_PORT='3333'
```

Define a configuration class with the `MapEnvVar` attribute on each property :

```csharp
using Kuchulem.DotNet.Env.Attributes;

namespace MyApp.Configurations
{
    class DbConfig
    {
        // The property DbName will be mapped with the
        // DB_NAME environement variable
        [MapEnvVar("DB_NAME")]
        public string? DbName { get; set; }
        
        // The property DbUser will be mapped with the
        // DB_USER environement variable
        [MapEnvVar("DB_USER")]
        public string? DbUser { get; set; }
        
        // The property DbPassword will be mapped with the
        // DB_PASSWORD environement variable
        [MapEnvVar("DB_PASSWORD")]
        public string? DbPassword { get; set; }
        
        // The property DbAddress will be mapped with the
        // DB_ADDRESS environement variable
        [MapEnvVar("DB_ADDRESS")]
        public string? DbAddress { get; set; }
        
        // The property DbPort will be mapped with the
        // DB_PORT environement variable
        [MapEnvVar("DB_PORT")]
        public int DbPort { get; set; }
    }
}
```

Then invoke the `MapVariables<T>` method of the parser :

```csharp
var dbConfig = parser.MapVariables<DbConfig>();

Console.WriteLine(dbConfig.DbName); // outputs "my_db"
```

You can also populate an existing object :

```csharp
var dbConfig = new DbConfig();

parser.MapVariables(dbConfig);

Console.WriteLine(dbConfig.DbName); // outputs "my_db"
```

## Secrets in files

If you wish to keep some secrets in files, this is possible by adding
the `_FILE` suffix to you environement variable.

Consider the follwing environment variable :

```
DB_PASSWORD_FILE='/var/secrets/db_password'
```

and the file `/var/secrets/db_password` content :

```
the_password_is_in_keypass
```

To get the password value keep using the `DB_PASSWORD` variable name in your code :

```csharp
var password = parser.GetVariable("DB_PASSWORD");

Console.WriteLine(password); // outputs "the_password_is_in_keypass" (never do that !)
```

The `_FILE` suffix can be customized by using the `fileEnvSuffix` optional
argument in the `EnvironmentParser` constructor.

consider the env var :

```
DB_PASSWORD_SECRET='/var/secrets/db_password'
```

```csharp
var parser = new EnvironmentParser("_SECRET"); // change the suffix to _SECRET

var password = parser.GetVariable("DB_PASSWORD");

Console.WriteLine(password); // outputs "the_password_is_in_keypass" (never do that !)
```