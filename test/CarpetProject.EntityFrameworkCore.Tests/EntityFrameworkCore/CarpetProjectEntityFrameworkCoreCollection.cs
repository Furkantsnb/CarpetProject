using Xunit;

namespace CarpetProject.EntityFrameworkCore;

[CollectionDefinition(CarpetProjectTestConsts.CollectionDefinitionName)]
public class CarpetProjectEntityFrameworkCoreCollection : ICollectionFixture<CarpetProjectEntityFrameworkCoreFixture>
{

}
