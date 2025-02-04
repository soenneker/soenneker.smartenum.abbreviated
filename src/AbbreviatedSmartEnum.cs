using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System;
using Soenneker.Extensions.Type;
using Soenneker.Extensions.String;
using Soenneker.SmartEnum.Named;

namespace Soenneker.SmartEnum.Abbreviated;

/// <summary>
/// Represents an abstract base class for abbreviated smart enums.
/// </summary>
/// <typeparam name="TEnum">The type of the enum.</typeparam>
public abstract class AbbreviatedSmartEnum<TEnum> : NamedSmartEnum<TEnum> where TEnum : AbbreviatedSmartEnum<TEnum>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbbreviatedSmartEnum{TEnum}"/> class.
    /// </summary>
    /// <param name="name">The name of the enum value.</param>
    /// <param name="value">The value of the enum.</param>
    /// <param name="abbreviation">The abbreviation of the enum value.</param>
    /// <param name="ignoreCase">A value indicating whether to ignore case when comparing abbreviations.</param>
    protected AbbreviatedSmartEnum(string name, int value, string abbreviation, bool ignoreCase = false) : base(name, value)
    {
        Abbreviation = abbreviation;
        IgnoreCase = ignoreCase;
    }

    /// <summary>
    /// Gets a value indicating whether to ignore case when comparing abbreviations for the current instance.
    /// </summary>
    protected bool IgnoreCase { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to ignore case when comparing abbreviations across all instances.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static bool StaticIgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets the abbreviation of the enum value.
    /// </summary>
    public string Abbreviation { get; set; }

    private static List<TEnum> GetAllOptions()
    {
        Type baseType = typeof(TEnum);

        List<TEnum> enums = Assembly.GetAssembly(baseType)!
            .GetTypes()
            .Where(baseType.IsAssignableFrom)
            .SelectMany(t => t.GetFieldsOfType<TEnum>())
            .OrderBy(t => t.Name)
            .ToList();

        StaticIgnoreCase = enums.First().IgnoreCase;

        return enums;
    }

    private static readonly Lazy<List<TEnum>> _enumOptions = new(GetAllOptions, LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly Lazy<Dictionary<string, TEnum>> _fromAbbreviation = new(() => _enumOptions.Value.ToDictionary(item => item.Abbreviation));

    private static readonly Lazy<Dictionary<string, TEnum>> _fromAbbreviationIgnoreCase =
        new(() => _enumOptions.Value.ToDictionary(item => item.Abbreviation, StringComparer.OrdinalIgnoreCase));

    /// <summary>
    /// Gets the enum value corresponding to the specified abbreviation.
    /// </summary>
    /// <param name="abbreviation">The abbreviation of the enum value to retrieve.</param>
    /// <returns>The enum value corresponding to the specified abbreviation.</returns>
    /// <exception cref="Exception">Thrown when the specified abbreviation is not found.</exception>
    public static TEnum FromAbbreviation(string abbreviation)
    {
        _ = _enumOptions.Value;

        if (StaticIgnoreCase)
            return GetAbbreviation(abbreviation, _fromAbbreviationIgnoreCase.Value);

        return GetAbbreviation(abbreviation, _fromAbbreviation.Value);
    }

    /// <summary>
    /// Tries to get the enum value corresponding to the specified abbreviation.
    /// </summary>
    /// <param name="abbreviation">The abbreviation of the enum value to retrieve.</param>
    /// <param name="ignoreCase">A value indicating whether to ignore case when comparing abbreviations.</param>
    /// <param name="result">The enum value corresponding to the specified abbreviation, if found.</param>
    /// <returns><c>true</c> if the specified abbreviation was found; otherwise, <c>false</c>.</returns>
    public static bool TryFromAbbreviation(string abbreviation, bool ignoreCase, out TEnum? result)
    {
        if (abbreviation.IsNullOrEmpty())
        {
            result = null;
            return false;
        }

        if (ignoreCase)
            return _fromAbbreviationIgnoreCase.Value.TryGetValue(abbreviation, out result);

        return _fromAbbreviation.Value.TryGetValue(abbreviation, out result);
    }

    private static TEnum GetAbbreviation(string abbreviation, Dictionary<string, TEnum> dictionary)
    {
        if (!dictionary.TryGetValue(abbreviation, out TEnum? result))
        {
            throw new Exception($"Abbreviation {abbreviation} not found in {nameof(AbbreviatedSmartEnum<TEnum>)}");
        }

        return result;
    }
}