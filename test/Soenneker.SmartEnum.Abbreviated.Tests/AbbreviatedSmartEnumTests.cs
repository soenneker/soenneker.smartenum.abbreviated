using Soenneker.Tests.HostedUnit;

namespace Soenneker.SmartEnum.Abbreviated.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class AbbreviatedSmartEnumTests : HostedUnitTest
{
    public AbbreviatedSmartEnumTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
