using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soenneker.SmartEnum.Abbreviated.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.SmartEnum.Abbreviated.Tests;

[Collection("Collection")]
public class AbbreviatedSmartEnumTests : FixturedUnitTest
{
    private readonly IAbbreviatedSmartEnum _util;

    public AbbreviatedSmartEnumTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IAbbreviatedSmartEnum>(true);
    }
}
