[![](https://img.shields.io/nuget/v/soenneker.smartenum.abbreviated.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.smartenum.abbreviated/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.smartenum.abbreviated/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.smartenum.abbreviated/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.smartenum.abbreviated.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.smartenum.abbreviated/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.smartenum.abbreviated/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.smartenum.abbreviated/actions/workflows/codeql.yml)

# Soenneker.SmartEnum.Abbreviated

An Ardalis SmartEnum base class that adds an abbreviation and abbreviation-based lookup.

## Installation

```bash
dotnet add package Soenneker.SmartEnum.Abbreviated
```

## Defining an enum

```csharp
using Soenneker.SmartEnum.Abbreviated;

public sealed class Language : AbbreviatedSmartEnum<Language>
{
    public static readonly Language English = new(nameof(English), 1, "EN");
    public static readonly Language Spanish = new(nameof(Spanish), 2, "ES");

    private Language(string name, int value, string abbreviation)
        : base(name, value, abbreviation)
    {
    }
}
```

Members must be exposed as static fields so the SmartEnum discovery process can find them. Abbreviations must be unique; duplicate abbreviations fail when the lookup is initialized.

## Looking up abbreviations

```csharp
Language english = Language.FromAbbreviation("EN");

if (Language.TryFromAbbreviation("es", ignoreCase: true, out Language? spanish))
{
    Console.WriteLine(spanish.Name); // Spanish
}
```

`FromAbbreviation` throws when no match exists and uses the type's `StaticIgnoreCase` setting. `TryFromAbbreviation` does not throw for a missing or empty abbreviation and takes case sensitivity explicitly for that call.

```csharp
// Initialize the enum lookup, then change the default used by FromAbbreviation.
_ = Language.FromAbbreviation("EN");
Language.StaticIgnoreCase = true;

Language sameValue = Language.FromAbbreviation("en");
```

`Abbreviation` is mutable for compatibility, but lookup dictionaries are created once. Treat abbreviations as immutable after members are declared; changing one later does not rebuild the lookup.
