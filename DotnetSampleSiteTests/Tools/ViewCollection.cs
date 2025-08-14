using Xunit;

namespace SampleSiteTests.Tools;

[CollectionDefinition(nameof(ViewCollection))]
public class ViewCollection : ICollectionFixture<SampleSiteTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}