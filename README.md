[![](https://img.shields.io/nuget/v/soenneker.smartenum.abbreviated.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.smartenum.abbreviated/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.smartenum.abbreviated/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.smartenum.abbreviated/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.smartenum.abbreviated.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.smartenum.abbreviated/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.SmartEnum.Abbreviated
### A derivative of [Ardalis'](https://github.com/ardalis) [SmartEnum](https://github.com/ardalis/SmartEnum), adding support for abbreviations

## Installation

```
dotnet add package Soenneker.SmartEnum.Abbreviated
```

## Usage

The `AbbreviatedSmartEnum` class is an abstract base class that extends the `SmartEnum` class from Ardalis' library. It provides additional functionality for working with abbreviated enum values.

To create an abbreviated SmartEnum, you need to derive a new class from `AbbreviatedSmartEnum<TEnum>`.

```csharp
public class LanguageType : AbbreviatedSmartEnum<LanguageType>
{
    public static readonly LanguageType English = new(nameof(English), 1, "EN");
    public static readonly LanguageType Spanish = new(nameof(Spanish), 2, "ES");
    public static readonly LanguageType French = new(nameof(French), 3, "FR");

    private LanguageType(string name, int value, string abbreviation)
        : base(name, value, abbreviation)
    {
    }
}
```

and how you use your new SmartEnum:

```csharp
string abbreviated = LanguageType.English.Abbreviation; // "EN"

// Get the enum value for the "EN" abbreviation
LanguageType english = LanguageType.FromAbbreviation("EN");

// Try to get the enum value for the "ES" abbreviation (case-insensitive)
if (LanguageType.TryFromAbbreviation("es", ignoreCase: true, out LanguageType spanish))
{
    // spanish will be the LanguageType.Spanish value
}
```

The IgnoreCase and StaticIgnoreCase properties allow you to control whether the abbreviation matching is case-sensitive or case-insensitive, either for a specific instance or globally across all instances of the derived enum class.