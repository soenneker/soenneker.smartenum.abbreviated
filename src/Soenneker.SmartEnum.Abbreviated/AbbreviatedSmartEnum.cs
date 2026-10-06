using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
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
public abstract class AbbreviatedSmartEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum> : NamedSmartEnum<TEnum> where TEnum : AbbreviatedSmartEnum<TEnum>
{
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
    /// Gets or sets a value indicating whether <see cref="FromAbbreviation"/> ignores abbreviation casing.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static bool StaticIgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets the abbreviation of the enum value.
    /// </summary>
    public string Abbreviation { get; set; }

    private static readonly object _optionsLock = new();
    private static List<TEnum>? _registeredOptions;
    private static bool _optionsRead;

    /// <summary>Registers all lookup values, including values declared on derived types, before the first lookup.</summary>
    public static void RegisterAbbreviationOptions(IEnumerable<TEnum> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        List<TEnum> values = options.ToList();
        lock (_optionsLock)
        {
            if (_optionsRead) throw new InvalidOperationException("Register enum options before the first lookup.");
            _registeredOptions = values;
        }
    }

    private static List<TEnum> GetAllOptions()
    {
        lock (_optionsLock)
        {
            _optionsRead = true;
            List<TEnum> enums = (_registeredOptions ?? typeof(TEnum).GetFieldsOfType<TEnum>()).OrderBy(t => t.Name).ToList();
            if (enums.Count != 0)
                StaticIgnoreCase = enums[0].IgnoreCase;
            return enums;
        }
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
